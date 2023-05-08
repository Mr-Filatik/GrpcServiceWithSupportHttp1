using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using GrpcServiceWithSupportHttp1;
using GrpcServiceWithSupportHttp1.Database.Entities;
using GrpcServiceWithSupportHttp1.Database.Services;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using System;
using System.Security.Cryptography;

namespace GrpcServiceWithSupportHttp1.Services
{
    public class AuthGrpcService : AuthGrpc.AuthGrpcBase
    {
        private readonly ILogger<AuthGrpcService> _logger;
        private readonly IUserService _userService;

        public AuthGrpcService(ILogger<AuthGrpcService> logger, IUserService userService)
        {
            _logger = logger;
            _userService = userService;
        }

        public override Task<LoginResponse> Login(LoginRequest request, ServerCallContext context)
        {
            try
            {
                _logger.LogInformation($"AuthGrpcService: Login ({request.Data.Login})");
                string pass = request.Data.Password;/*Convert.ToBase64String(KeyDerivation.Pbkdf2(password: request.Data.Password,
                  salt: RandomNumberGenerator.GetBytes(128 / 8),
                  prf: KeyDerivationPrf.HMACSHA256,
                  iterationCount: 100000,
                  numBytesRequested: 256 / 8));*/
                string key = _userService.Login(request.Data.Login, pass).Result;
                return Task.FromResult(new LoginResponse() { Key = key});
            }
            catch (Exception e)
            {
                throw new RpcException(new Status(StatusCode.Aborted, $"Error on server: {e.Message}"));
            }
        }

        public override Task<RegisterResponse> Register(RegisterRequest request, ServerCallContext context)
        {
            _logger.LogInformation("Register");
            return base.Register(request, context);
        }

        public override Task<LogoutResponse> Logout(LogoutRequest request, ServerCallContext context)
        {
            try
            {
                _logger.LogInformation($"AuthGrpcService: Logout ({request.Data.Key})");
                bool flag = _userService.Logout(request.Data.Key).Result;
                return Task.FromResult(new LogoutResponse() { Value = flag });
            }
            catch (Exception e)
            {
                throw new RpcException(new Status(StatusCode.Aborted, $"Error on server: {e.Message}"));
            }
        }

        //private User ConvertToUser(UserRequest user)
        //{
        //    return new User
        //    {
        //        Id = 0,
        //        Surname = user.Surname,
        //        Name = user.Name,
        //        Patronymic = user.Patronymic ?? string.Empty,
        //        Email = user.Email,
        //        PasswordHash = user.Password,
        //        AccessKey = null,
        //        Birthday = user.Birthday.ToDateTime()
        //    };
        //}

        //private UserResponse ConvertFromUser(User user)
        //{
        //    return new UserResponse
        //    {
        //        Id = user.Id,
        //        Email = user.Email,
        //        Surname = user.Surname,
        //        Name = user.Name,
        //        Patronymic = user.Patronymic ?? string.Empty,
        //        Birthday = Timestamp.FromDateTime(DateTime.SpecifyKind(user.Birthday, DateTimeKind.Utc))
        //    };
        //}

        //private List<UserResponse> ConvertFromUsers(List<User> users)
        //{
        //    List<UserResponse> response = new();
        //    foreach (User user in users)
        //    {
        //        response.Add(ConvertFromUser(user));
        //    }
        //    return response;
        //}
    }
}