using AutoMapper;
using eCommerceApp.Application.DTOs;
using eCommerceApp.Application.DTOs.Identity;
using eCommerceApp.Application.Services.Interfaces.Authentication;
using eCommerceApp.Application.Services.Interfaces.Logging;
using eCommerceApp.Application.Validations;
using eCommerceApp.Domain.Entities.Identity;
using eCommerceApp.Domain.Interfaces.Authentication;
using FluentValidation;

namespace eCommerceApp.Application.Services.Implementations.Authentication
{
    public class AuthenticationService
        (ITokenManagement tokenManagement, 
        IUserManagement userManagement, 
        IRoleManagement roleManagement, 
        IAppLogger<AuthenticationService> logger, 
        IMapper mapper,
        IValidator<CreateUser> createUserValidator,
        IValidator<LoginUser> loginUserValidator,
        IValidationService validationService) : IAuthenticationService
    {
        public async Task<ServiceResponse> CreateUser(CreateUser user)
        {
            //Validate first
            var _validationResult = await validationService.ValidateAsync(user, createUserValidator); //validate the user object that is being passed into our Task
            if (!_validationResult.Success) return _validationResult; //if not successful return the result

            var mappedModel = mapper.Map<AppUser>(user); //try and map to our AppUser object
            mappedModel.UserName = user.Email; //need to add these to the mappedModel as these do not exist in our CreateUser object, and or not named the same
            mappedModel.PasswordHash = user.Password;

            //Sending the mapped model for a AppUser to the Interface in Domain to the Repository Management in the Infrastructure
            var result = await userManagement.CreateUser(mappedModel);
            if (!result)
                return new ServiceResponse { Message = "Email Address might be already in use or unknown error occurred" };

            //Role Assignment + Validations
            var _user = await userManagement.GetUserByEmail(user.Email);
            var users = await userManagement.GetAllUsers();
            bool assignedResult = await roleManagement.AddUserToRole(_user!, users!.Count() > 1 ? "User" : "Admin"); //we assign a user to Admin if no users exist yet.

            if (!assignedResult)
            {
                //remove user
                int removeUserResult = await userManagement.RemoveUserByEmail(_user!.Email!);
                if (removeUserResult <= 0)
                {
                    // error occurred while rolling back changes
                    // then log the error
                    logger.LogError(new Exception($"User with Email as {_user.Email} failed to be reomved as a result of role assigning issue"), "User could not be assigned Role");
                    return new ServiceResponse { Message = "Error occurred in creating account" };
                }
            }

            return new ServiceResponse { Success = true, Message = "Account created!" };

            // verify Email

        }

        public async Task<LoginResponse> LoginUser(LoginUser user)
        {
            //validate first
            var _validationResult = await validationService.ValidateAsync(user, loginUserValidator);
            if (!_validationResult.Success)
                return new LoginResponse(Message: _validationResult.Message); //since this may have lots of different messages we just send the entire result.message

            var mappedModel = mapper.Map<AppUser>(user);
            mappedModel.PasswordHash = user.Password;

            bool loginResult = await userManagement.LoginUser(mappedModel);
            if (!loginResult)
                return new LoginResponse(Message: "Email not found or invalid credentials");

            var _user = await userManagement.GetUserByEmail(user.Email!);
            var claims = await userManagement.GetUserClaims(_user!.Email!); // grab user claims from the Email account

            string jwtToken = tokenManagement.GenerateToken(claims); //generate a token from those claims
            string refreshToken = tokenManagement.GetRefreshToken();

            int saveTokenResult = 0;
            bool userTokenCheck = await tokenManagement.ValidateRefreshToken(refreshToken);
            if(userTokenCheck)
                saveTokenResult = await tokenManagement.UpdateRefreshToken(_user.Id, refreshToken);
            else
            saveTokenResult = await tokenManagement.AddRefreshToken(_user.Id, refreshToken);


            return saveTokenResult <= 0 ? new LoginResponse(Message: "Internal error occurred while authenticating") :
                new LoginResponse(Success: true, Token: jwtToken, RefreshToken: refreshToken);
        }

        public async Task<LoginResponse> ReviveToken(string refreshToken)
        {
            bool validateTokenResult = await tokenManagement.ValidateRefreshToken(refreshToken);
            if (!validateTokenResult)
                return new LoginResponse(Message: "Invalid Token");

            string userId = await tokenManagement.GetUserIdByRefreshToken(refreshToken);
            AppUser? user = await userManagement.GetUserById(userId);
            var claims = await userManagement.GetUserClaims(user!.Email!);
            string newJwtToken = tokenManagement.GenerateToken(claims);
            string newRefreshToken = tokenManagement.GetRefreshToken();
            await tokenManagement.UpdateRefreshToken(userId, newRefreshToken);
            return new LoginResponse(Success: true, Token:newJwtToken, RefreshToken: newRefreshToken);
        }
    }
}
