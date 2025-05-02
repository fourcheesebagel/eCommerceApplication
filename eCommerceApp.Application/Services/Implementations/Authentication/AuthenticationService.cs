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
            var _validationResult = await validationService.ValidateAsync(user, createUserValidator); //validate the user object that is being passed into our Task
            if (!_validationResult.Success) return _validationResult; //if not successful return the result

            var mappedModel = mapper.Map<AppUser>(user); //try and map to our AppUser object
            mappedModel.UserName = user.Email; //need to add these to the mappedModel as these do not exist in our CreateUser object, and or not named the same
            mappedModel.PasswordHash = user.Password;

            var result = await userManagement.CreateUser(mappedModel);
            if (!result)
                return new ServiceResponse { Message = "Email Address might be already in use or unknown error occurred" };

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

        }

        public Task<LoginResponse> LoginUser(LoginUser user)
        {
            throw new NotImplementedException();
        }

        public Task<LoginResponse> ReviveToken(string refreshToken)
        {
            throw new NotImplementedException();
        }
    }
}
