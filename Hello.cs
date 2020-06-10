using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using RtspClientSharp;
using RtspClientSharp.Rtsp;

namespace MyApp
{
    public class Hello
    {
        public void test1()
        {
            Console.WriteLine("Hello test1");
        }

        public void test2()
        {
            var serverUri = new Uri("rtsp://192.168.0.5:554/h264/ch1/main/av_stream");
            var credentials = new NetworkCredential("admin", "12345qwert");

            var connectionParameters = new ConnectionParameters(serverUri, credentials);
            var cancellationTokenSource = new CancellationTokenSource();

            Task connectTask = ConnectAsync(connectionParameters, cancellationTokenSource.Token);

            Console.WriteLine("Press any key to cancel");
            Console.ReadLine();

            cancellationTokenSource.Cancel();

            Console.WriteLine("Canceling");
            connectTask.Wait(CancellationToken.None);
        }

        public async Task<bool> test3()
        {
            var serverUri = new Uri("rtsp://192.168.0.5:554/h264/ch1/main/av_stream");
            var credentials = new NetworkCredential("admin", "12345qwert");

            var connectionParameters = new ConnectionParameters(serverUri, credentials);
            var cancellationTokenSource = new CancellationTokenSource();

            return await ConnectAsync1(connectionParameters, cancellationTokenSource.Token);
        }

        private static async Task ConnectAsync(ConnectionParameters connectionParameters, CancellationToken token)
        {
            try
            {
                TimeSpan delay = TimeSpan.FromSeconds(5);

                using (var rtspClient = new RtspClient(connectionParameters))
                {
                    rtspClient.FrameReceived +=
                        (sender, frame) => Console.WriteLine($"New frame {frame.Timestamp}: {frame.GetType().Name}");

                    while (true)
                    {
                        Console.WriteLine("Connecting...");

                        try
                        {
                            await rtspClient.ConnectAsync(token);
                        }
                        catch (OperationCanceledException)
                        {
                            return;
                        }
                        catch (RtspClientException e)
                        {
                            Console.WriteLine(e.ToString());
                            await Task.Delay(delay, token);
                            continue;
                        }

                        Console.WriteLine("Connected.");

                        try
                        {
                            await rtspClient.ReceiveAsync(token);
                        }
                        catch (OperationCanceledException)
                        {
                            return;
                        }
                        catch (RtspClientException e)
                        {
                            Console.WriteLine(e.ToString());
                            await Task.Delay(delay, token);
                        }
                    }
                }
            }
            catch (OperationCanceledException)
            {
            }
        }

        private async Task<bool> ConnectAsync1(ConnectionParameters connectionParameters, CancellationToken token)
        {
            var result = false;
            try
            {
                TimeSpan delay = TimeSpan.FromSeconds(5);

                using (var rtspClient = new RtspClient(connectionParameters))
                {
                    rtspClient.FrameReceived +=
                        (sender, frame) => Console.WriteLine($"New frame {frame.Timestamp}: {frame.GetType().Name}");

                    Console.WriteLine("Connecting...");

                    try
                    {
                        await rtspClient.ConnectAsync(token);
                        result = true;
                    }
                    catch (OperationCanceledException)
                    {
                        return false;
                    }
                    catch (RtspClientException e)
                    {
                        Console.WriteLine(e.ToString());
                        await Task.Delay(delay, token);
                        result = false;
                    }
                }
            }
            catch (OperationCanceledException)
            {
            }

            return result;
        }
    }
}