// See https://aka.ms/new-console-template for more information
using Grpc.Core;
using Grpc.Net.Client;
using Server;

CancellationTokenSource cts = new();

Console.CancelKeyPress += delegate (object? _, ConsoleCancelEventArgs e) {
    e.Cancel = true;
    cts.Cancel();
};

var channel = GrpcChannel.ForAddress("http://3.38.166.204:5000");
var client = new Greeter.GreeterClient(channel);

Console.WriteLine("Client created");

using (var call = client.SayHello())
{
    var responseTask = Task.Run(async () =>
    {
        await foreach (var item in call.ResponseStream.ReadAllAsync())
        {
            Console.WriteLine(item.Message);
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

    Console.WriteLine("Disconnecting");
    await call.RequestStream.CompleteAsync();
    await responseTask;
}

Console.WriteLine("Disconnected");
