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
    public class UserGrpcService : UserGrpc.UserGrpcBase
    {
        private readonly ILogger<UserGrpcService> _logger;
        private readonly IUserService _userService;

        public UserGrpcService(ILogger<UserGrpcService> logger, IUserService userService)
        {
            _logger = logger;
            _userService = userService;
        }

        public override Task<CountResponse> GetCountAll(Empty request, ServerCallContext context)
        {
            try
            {
                _logger.LogInformation($"UserGrpcService: GetCountAll (...)");
                int count = _userService.GetCount().Result;
                return Task.FromResult(new CountResponse
                {
                    Count = count
                });
            }
            catch (Exception e)
            {
                throw new RpcException(new Status(StatusCode.Aborted, $"Error on server: {e.Message}"));
                //throw new RpcException(new Status(StatusCode.Aborted, "Error on server"), $"Error on server: {e.Message}");
            }
        }

        public override Task<CountResponse> GetCount(PredicateRequest request, ServerCallContext context)
        {
            try
            {
                _logger.LogInformation($"UserGrpcService: GetCount ({request.Predicate})");
                //string jsonString = "x => x.Id % 2 == 0 && x.Id != 2 && x.Id != 6";
                var options = ScriptOptions.Default.AddReferences(typeof(User).Assembly);
                var predicate = CSharpScript.EvaluateAsync<Func<User, bool>>(request.Predicate, options).Result;
                int count = _userService.GetCount(predicate).Result;
                return Task.FromResult(new CountResponse
                {
                    Count = count
                });
            }
            catch (Exception e)
            {
                throw new RpcException(new Status(StatusCode.Aborted, $"Error on server: {e.Message}"));
                //throw new RpcException(new Status(StatusCode.Aborted, "Error on server"), $"Error on server: {e.Message}");
            }
        }

        public override Task<UserResponse> GetUser(IdRequest request, ServerCallContext context)
        {
            try
            {
                _logger.LogInformation($"UserGrpcService: GetUser ({request.Id})");
                User user = _userService.GetUser(request.Id).Result;
                return Task.FromResult(ConvertFromUser(user));
            }
            catch (Exception e)
            {
                throw new RpcException(new Status(StatusCode.Aborted, $"Error on server: {e.Message}"));
                //throw new RpcException(new Status(StatusCode.Aborted, "Error on server"), $"Error on server: {e.Message}");
            }
        }

        public override Task<UserListResponse> GetUsersAll(PageRequest request, ServerCallContext context)
        {
            try
            {
                _logger.LogInformation($"UserGrpcService: GetUsersAll ({request.Page})");
                (List<User> users, int currPage, int lastPage) = _userService.GetUsers(page: request.Page).Result;
                UserListResponse response = new UserListResponse
                {
                    Users = { ConvertFromUsers(users) },
                    CurrentPage = currPage,
                    LastPage = lastPage
                };
                return Task.FromResult(response);
            }
            catch (Exception e)
            {
                throw new RpcException(new Status(StatusCode.Aborted, $"Error on server: {e.Message}"));
                //throw new RpcException(new Status(StatusCode.Aborted, "Error on server"), $"Error on server: {e.Message}");
            }
        }

        public override Task<UserListResponse> GetUsers(PredicatePageRequest request, ServerCallContext context)
        {
            try
            {
                _logger.LogInformation($"UserGrpcService: GetUsers ({request.Predicate})");
                var options = ScriptOptions.Default.AddReferences(typeof(User).Assembly);
                var predicate = CSharpScript.EvaluateAsync<Func<User, bool>>(request.Predicate, options).Result;
                (List<User> users, int currPage, int lastPage) = _userService.GetUsers(predicate).Result;
                UserListResponse response = new UserListResponse
                {
                    Users = { ConvertFromUsers(users) },
                    CurrentPage = currPage,
                    LastPage = lastPage
                };
                return Task.FromResult(response);
            }
            catch (Exception e)
            {
                throw new RpcException(new Status(StatusCode.Aborted, $"Error on server: {e.Message}"));
                //throw new RpcException(new Status(StatusCode.Aborted, "Error on server"), $"Error on server: {e.Message}");
            }
        }

        public override Task<UserResponse> CreateUser(UserCreateRequest request, ServerCallContext context)
        {
            try
            {
                _logger.LogInformation($"UserGrpcService: CreateUser ({request.Message.ToString()})");
                User user = ConvertToUser(request.Message);
                User? index = _userService.CreateUser(user).Result;
                if (index != null)
                {
                    UserResponse response = ConvertFromUser(user);
                    return Task.FromResult(response);
                }
                else
                {
                    return Task.FromResult(new UserResponse());
                }
            }
            catch (Exception e)
            {
                throw new RpcException(new Status(StatusCode.Aborted, $"Error on server: {e.Message}"));
                //throw new RpcException(new Status(StatusCode.Aborted, "Error on server"), $"Error on server: {e.Message}");
            }
        }

        public override Task<BoolResponse> DeleteUser(IdRequest request, ServerCallContext context)
        {
            try
            {
                _logger.LogInformation($"UserGrpcService: DeleteUser ({request.Id})");
                bool index = _userService.DeleteUser(request.Id).Result;
                return Task.FromResult(new BoolResponse() { Value = index });
            }
            catch (Exception e)
            {
                throw new RpcException(new Status(StatusCode.Aborted, $"Error on server: {e.Message}"));
                //throw new RpcException(new Status(StatusCode.Aborted, "Error on server"), $"Error on server: {e.Message}");
            }
        }

        public override Task<UserResponse> UpdateUser(UserUpdateRequest request, ServerCallContext context)
        {
            try
            {
                _logger.LogInformation($"UserGrpcService: UpdateUser ({request.Message.Id})");
                User user = _userService.GetUser(request.Message.Id).Result;
                if (user != null)
                {
                    if (request.Message.SurnameUpdate) user.Surname = request.Message.Surname;
                    if (request.Message.NameUpdate) user.Name = request.Message.Name;
                    if (request.Message.PatronymicUpdate) user.Patronymic = request.Message.Patronymic;
                    if (request.Message.BirthdayUpdate) user.Birthday = request.Message.Birthday.ToDateTime();
                    if (request.Message.PasswordUpdate) user.PasswordHash = Convert.ToBase64String(KeyDerivation.Pbkdf2(password: request.Message.Password,
                                                                                                                        salt: RandomNumberGenerator.GetBytes(128 / 8),
                                                                                                                        prf: KeyDerivationPrf.HMACSHA256,
                                                                                                                        iterationCount: 100000,
                                                                                                                        numBytesRequested: 256 / 8));
                    user = _userService.UpdateUser(user).Result;
                    return Task.FromResult(ConvertFromUser(user));
                }
                throw new Exception("User not found");
            }
            catch (Exception e)
            {
                throw new RpcException(new Status(StatusCode.Aborted, $"Error on server: {e.Message}"));
                //throw new RpcException(new Status(StatusCode.Aborted, "Error on server"), $"Error on server: {e.Message}");
            }
        }

        private User ConvertToUser(UserRequest user)
        {
            return new User
            {
                Id = 0,
                Surname = user.Surname,
                Name = user.Name,
                Patronymic = user.Patronymic ?? string.Empty,
                Email = user.Email,
                PasswordHash = user.Password,
                AccessKey = null,
                Birthday = user.Birthday.ToDateTime()
            };
        }

        private UserResponse ConvertFromUser(User user)
        {
            return new UserResponse
            {
                Id = user.Id,
                Email = user.Email,
                Surname = user.Surname,
                Name = user.Name,
                Patronymic = user.Patronymic ?? string.Empty,
                Birthday = Timestamp.FromDateTime(DateTime.SpecifyKind(user.Birthday, DateTimeKind.Utc))
            };
        }

        private List<UserResponse> ConvertFromUsers(List<User> users)
        {
            List<UserResponse> response = new();
            foreach (User user in users)
            {
                response.Add(ConvertFromUser(user));
            }
            return response;
        }
    }
}