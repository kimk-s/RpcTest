// See https://aka.ms/new-console-template for more information
using Grpc.Core;
using Grpc.Net.Client;
using Server;

Console.WriteLine("Hello, World!");

var channel = GrpcChannel.ForAddress("https://localhost:7166");
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

    for (int i = 0; i < 5; i++)
    {
        await call.RequestStream.WriteAsync(new HelloRequest { Name = "Kim" });

        await Task.Delay(2000);
    }

    Console.WriteLine("Disconnecting");
    await call.RequestStream.CompleteAsync();
    await responseTask;
}

Console.WriteLine("Disconnected");
