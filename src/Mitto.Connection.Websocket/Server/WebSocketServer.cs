using System;
using System.Net;
using Mitto.Logging;
using System.Threading;
using Mitto.IConnection;
using Mitto.Connection.Websocket.Wrapper;
using System.Threading.Tasks;

namespace Mitto.Connection.Websocket.Server {

    internal class WebSocketServer : IServer {
        private readonly IWebSocketServerWrapper Listener;
        private ServerParams Params { get; set; }

        private ILog Log => LoggingFactory.GetLogger(GetType());

        public WebSocketServer(IWebSocketServerWrapper pListener, ServerParams pParams) {
            Listener = pListener;
            Params = pParams;
        }

        public void Start(IServerParams pParams, Action<IClientConnection> pClientConnectedAction) {
            Listener.Prefixes.Add($"http{(Params.Secure ? "s" : "")}://+:" + Params.Port + "/" + Params.Path.TrimStart('/'));
            new Thread(() => {
                Listener.Start();
                Log.Info("Started Listening");

                //ToDo: change while true to use a cancellationtoken that's set when disconnect/close is called
                try {
                    while (true) {
                        var objContext = Listener.GetContext();
                        if (objContext.Request.IsWebSocketRequest) {
                            ProcessConnection(objContext, pClientConnectedAction);
                        } else {
                            objContext.Response.StatusCode = 400;
                            objContext.Response.Close();
                        }
                    }
                } catch (TaskCanceledException) {
                } catch (OperationCanceledException) {
                    //ignore
                } finally {
                    Console.WriteLine("closing server...");
                    Stop();
                }
            }) {
                IsBackground = true,
                Name = "WebSocket Accept Thread"
            }.Start();
        }

        private async void ProcessConnection(HttpListenerContext pContext, Action<IClientConnection> pClientConnectedAction) {
            try {
                // When calling `AcceptWebSocketAsync` the negotiated subprotocol must be specified.
                // This assumes that no subprotocol was requested.
                //ToDo: cancellationtoken?
                var objClient = new Client(
                    (
                        await pContext.AcceptWebSocketAsync(
                            null,
                            new TimeSpan(0, 0, 30)
                        )
                    ).WebSocket,
                    Params.FragmentSize
                );
                objClient.Start();
                pClientConnectedAction.Invoke(objClient);
            } catch (TaskCanceledException) {
            } catch (OperationCanceledException) {
                //ignore
            } catch (Exception ex) {
                // The upgrade process failed somehow. For simplicity lets assume it was a failure on the part of the server and indicate this using 500.
                Log.Info($"Failed to accept connection: {ex.Message}, setting as closed with error 500");
                pContext.Response.StatusCode = 500;
                pContext.Response.Close();
                return;
            }
        }

        public void Stop() => Listener.Abort();
    }
}