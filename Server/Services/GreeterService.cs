using Grpc.Core;

namespace Server.Services
{
    public class GreeterService : Greeter.GreeterBase
    {
        private readonly ILogger<GreeterService> _logger;

        public GreeterService(ILogger<GreeterService> logger)
        {
            _logger = logger;
        }

        public override async Task SayHello(IAsyncStreamReader<HelloRequest> requestStream, IServerStreamWriter<HelloReply> responseStream, ServerCallContext context)
        {
            try
            {
                _logger.LogInformation($"Connected {GetHashCode()}");

                await foreach (var item in requestStream.ReadAllAsync(context.CancellationToken))
                {
                    await responseStream.WriteAsync(new HelloReply { Message = "Hello, " + item.Name });
                }
            }
            finally
            {
                _logger.LogInformation($"Disconnected {GetHashCode()}");
            }
        }
    }
}