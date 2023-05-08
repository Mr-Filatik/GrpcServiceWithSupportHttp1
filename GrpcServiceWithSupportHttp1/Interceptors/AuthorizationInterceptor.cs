using Grpc.Core;
using Grpc.Core.Interceptors;
using GrpcServiceWithSupportHttp1.Database.Services;
using System.Runtime.CompilerServices;

namespace GrpcServiceWithSupportHttp1.Interceptors
{
    public class AuthorizationInterceptor : Interceptor
    {
        private readonly ILogger<AuthorizationInterceptor> _logger;
        private readonly IUserService _userService;

        public AuthorizationInterceptor(ILogger<AuthorizationInterceptor> logger, IUserService userService)
        {
            _logger = logger;
            _userService = userService;
        }

        public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(TRequest request, ServerCallContext context, UnaryServerMethod<TRequest, TResponse> continuation)
        {
            _logger.LogInformation("Interceptor start");
            Metadata.Entry? accessKey = context.RequestHeaders.Get("Access-Key");
            string key = accessKey != null ? accessKey.Value : "";
            bool flag = _userService.IsLogin(key).Result;
            if (!flag)
            {
                throw new RpcException(new Status(StatusCode.Unauthenticated, "Unauthenticated"), "Unauthenticated");
            }
            TResponse? response = await base.UnaryServerHandler(request, context, continuation);
            _logger.LogInformation("Interceptor finish");
            return response;
        }
    }
}
