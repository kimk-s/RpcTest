using Grpc.Core;

namespace Server.Services
{
    public class GreeterService : Greeter.GreeterBase
    {
        private static int s_id;

        private readonly ILogger<GreeterService> _logger;

        public GreeterService(ILogger<GreeterService> logger)
        {
            _logger = logger;
        }

        private static int GenerateId() => Interlocked.Increment(ref s_id);

        public override async Task SayHello(IAsyncStreamReader<HelloRequest> requestStream, IServerStreamWriter<HelloReply> responseStream, ServerCallContext context)
        {
            int id = GenerateId();
            _logger.LogInformation($"Connected Id: {id}");

            try
            {
                await foreach (var item in requestStream.ReadAllAsync(context.CancellationToken)) // <<-- not wake when client network is broken (ex. Wifi off)?
                {
                    await responseStream.WriteAsync(new HelloReply { Message = $"Hello, {item.Name}(ID: {id})" });
                }
            }
            finally
            {
                _logger.LogInformation($"Disconnected Id: {id}");
            }
        }
    }
}
