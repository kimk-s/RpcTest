using Grpc.Core;
using Server;
using System;
using System.Threading;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Program : MonoBehaviour
{
    public Logger logger;
    public TMP_InputField hostText;
    public TMP_InputField portText;
    public Button startButton;

    private readonly CancellationTokenSource _ctsOnDestroy = new();
    private bool _started;

    private CancellationToken CancellationTokenOnDestroy => _ctsOnDestroy.Token;

    private void Start()
    {
        startButton.onClick.AddListener(async () =>
        {
            if (_started)
            {
                return;
            }

            _started = true;
            startButton.interactable = false;

            try
            {
                string host = hostText.text;
                int port = int.Parse(portText.text);

                await DnsService.InitializeAsync();

                var channel = new Channel(host, port, ChannelCredentials.Insecure);
                var client = new Greeter.GreeterClient(channel);

                logger.Log("Client created");

                using (var call = client.SayHello(cancellationToken: CancellationTokenOnDestroy))
                {
                    var responseTask = Task.Run(async () =>
                    {
                        await foreach (var item in call.ResponseStream.ReadAllAsync(CancellationTokenOnDestroy))
                        {
                            logger.Log(item.Message);
                        }
                    }, CancellationTokenOnDestroy);

                    var requestTask = Task.Run(async () =>
                    {
                        while (!CancellationTokenOnDestroy.IsCancellationRequested)
                        {
                            await call.RequestStream.WriteAsync(new HelloRequest { Name = "Bob" });

                            try
                            {
                                await Task.Delay(2000, CancellationTokenOnDestroy);
                            }
                            catch (OperationCanceledException)
                            {
                                break;
                            }
                        }

                        logger.Log("Disconnecting");
                        await call.RequestStream.CompleteAsync();
                    }, CancellationTokenOnDestroy);

                    await Task.WhenAll(requestTask, responseTask);
                }

                logger.Log("Disconnected");
            }
            catch (Exception e)
            {
                logger.Log(e.ToString());
            }
        });
    }

    private void OnDestroy()
    {
        _ctsOnDestroy.Cancel();
    }
}
