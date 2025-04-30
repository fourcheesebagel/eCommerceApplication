using AutoMapper;
using eCommerceApp.Application.DTOs;
using eCommerceApp.Application.DTOs.Identity;
using eCommerceApp.Application.Services.Interfaces.Authentication;
using eCommerceApp.Application.Services.Interfaces.Logging;
using eCommerceApp.Application.Validations;
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
