using Grpc.Core;
using Grpc.Core.Interceptors;

namespace GrpcServiceWithSupportHttp1.Interceptors
{
    public class AuthorizationInterceptor : Interceptor
    {
        private readonly ILogger<AuthorizationInterceptor> _logger;

        public AuthorizationInterceptor(ILogger<AuthorizationInterceptor> logger)
        {
            _logger = logger;
        }

        public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(TRequest request, ServerCallContext context, UnaryServerMethod<TRequest, TResponse> continuation)
        {
            _logger.LogInformation("Interceptor start");
            var accessKey = context.RequestHeaders.Get("Access-Key");
            if (accessKey?.Value != "Access-Key")
            {
                throw new RpcException(new Status(StatusCode.Unauthenticated, "Unauthenticated"), "Unauthenticated");
            }
            TResponse? response = await base.UnaryServerHandler(request, context, continuation);
            _logger.LogInformation("Interceptor finish");
            return response;
        }
    }
}
