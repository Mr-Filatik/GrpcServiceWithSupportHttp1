using Grpc.Core;
using GrpcServiceWithSupportHttp1;
using GrpcServiceWithSupportHttp1.Database.Services;

namespace GrpcServiceWithSupportHttp1.Services
{
    public class TestService : Test.TestBase
    {
        private readonly ILogger<TestService> _logger;
        private readonly IUserService userService;
        public TestService(ILogger<TestService> logger, IUserService userService)
        {
            _logger = logger;
            this.userService = userService;
        }

        public override Task<HelloReplyTest> SayHelloTest(HelloRequestTest request, ServerCallContext context)
        {
            _logger.LogInformation($"Request - {request.Name}");
            int task = userService.GetCount().Result;
            return Task.FromResult(new HelloReplyTest
            {
                Message = task.ToString()
            });
        }
    }
}