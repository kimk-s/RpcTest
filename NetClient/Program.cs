// See https://aka.ms/new-console-template for more information
using Grpc.Core;
using Grpc.Net.Client;
using Server;

CancellationTokenSource cts = new();

Console.CancelKeyPress += delegate (object? _, ConsoleCancelEventArgs e) {
    e.Cancel = true;
    cts.Cancel();
};

Log("Started");

try
{
    using var channel = GrpcChannel.ForAddress("http://3.36.61.80:5000");
    var client = new Greeter.GreeterClient(channel);

    Log("Client created");

    using (var call = client.SayHello())
    {
        var responseTask = Task.Run(async () =>
        {
            await foreach (var item in call.ResponseStream.ReadAllAsync())
            {
                Log(item.Message);
            }
        });

        while (!cts.IsCancellationRequested)
        {
            await call.RequestStream.WriteAsync(new HelloRequest { Name = "Kim" });

            try
            {
                await Task.Delay(1000, cts.Token);
            }
            catch (OperationCanceledException)
            {
                break;
            }
        }

        Log("Disconnecting");
        await call.RequestStream.CompleteAsync();
        await responseTask;
    }

    Log("Disconnected");
}
finally
{
    Log("Stopped");
}

static void Log(string message)
{
    Console.WriteLine($"[{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC] {message}");
}