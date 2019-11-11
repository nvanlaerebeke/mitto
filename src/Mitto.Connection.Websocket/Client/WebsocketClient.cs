using Mitto.Connection.Websocket.Wrapper;
using Mitto.IConnection;
using Mitto.Logging;
using Mitto.Utilities;
using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]

/// <summary>
/// Class that represents the WebSocket Client in Mitto
/// Provides functionality to communicate with a WebSocket server
/// </summary>
namespace Mitto.Connection.Websocket.Client {

    public class WebsocketClient : IClient {
        private readonly ILog Log = LoggingFactory.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly IWebSocketClientWrapper WebSocket;
        private readonly int BufferSize = 0;

        public event EventHandler<IClient> Connected;

        public event EventHandler<IConnection.IConnection> Disconnected;

        public event EventHandler<byte[]> Rx;

        /// <summary>
        /// Used to stop the Receive loop
        /// </summary>
        private CancellationTokenSource _objCancelationSource;

        private CancellationToken _objCancelationToken;

        public string ID { get; private set; } = Guid.NewGuid().ToString();

        public long CurrentBytesPerSecond => 0;

        internal WebsocketClient(IWebSocketClientWrapper pWebSocket, int pFragmentLength) {
            WebSocket = pWebSocket;
            BufferSize = pFragmentLength;
        }

        public void ConnectAsync(IClientParams pParams) {
            try {
                if (_objCancelationSource != null) {
                    _objCancelationSource.Dispose();
                }
                _objCancelationSource = new CancellationTokenSource();
                _objCancelationToken = _objCancelationSource.Token;

                _ = WebSocket.ConnectAsync(
                    new Uri(
                        string.Format(
                            (pParams.Secure ? "wss" : "ws") + "://{0}:{1}/",
                            pParams.HostName,
                            pParams.Port
                        )
                    ),
                    _objCancelationToken
                ).ContinueWith((t) => {
                    _ = Receive();
                    Connected?.Invoke(this, this);
                });
            } catch (TaskCanceledException) {
            } catch (OperationCanceledException) {
                //ignore
            } catch (Exception ex) {
                Log.Info($"Error connecting client: {ex.Message}, closing connection...");
                Disconnect();
            }
        }

        private async Task Receive() {
            try {
                while (WebSocket.State == WebSocketState.Open) {
                    var buffer = new byte[BufferSize];
                    var result = await WebSocket.ReceiveAsync(new ArraySegment<byte>(buffer), _objCancelationToken);

                    if (result.MessageType == WebSocketMessageType.Close) {
                        Disconnect();
                    } else {
                        var lstMessage = new List<byte>(buffer);
                        while (!result.EndOfMessage && result.CloseStatus == WebSocketCloseStatus.Empty) {
                            result = await WebSocket.ReceiveAsync(new ArraySegment<byte>(buffer), _objCancelationToken);
                            lstMessage.AddRange(buffer);
                        }
                        Rx?.Invoke(this, lstMessage.ToArray());
                    }
                }
            } catch (TaskCanceledException) {
            } catch(OperationCanceledException) { 
                //ignore
            } catch (Exception ex) {
                Log.Info($"Error receiving data: {ex.Message}, closing connection");
                Disconnect();
            }
        }

        public void Transmit(byte[] pData) {
            try {
                if (WebSocket.State == WebSocketState.Open) {
                    var EOD = false;
                    var intPage = 1;
                    while (!EOD) {
                        var buffer = new byte[BufferSize];
                        if (pData.Length <= BufferSize) {
                            EOD = true;
                            buffer = pData;
                        } else {
                            var Start = intPage * BufferSize;
                            var End = (intPage + 1) * BufferSize;
                            var intChunkSize = BufferSize;
                            if (End > pData.Length) {
                                EOD = true;
                                intChunkSize = pData.Length - Start;
                            }
                            Array.Copy(pData, Start, buffer, 0, intChunkSize);
                        }
                        var objTask = WebSocket.SendAsync(
                            new ArraySegment<byte>(buffer),
                            WebSocketMessageType.Binary,
                            EOD,
                            _objCancelationToken
                        );
                        objTask.Wait();
                        intPage++;
                    }
                }
            } catch (TaskCanceledException) {
            } catch (OperationCanceledException) {
                //ignore
            } catch (Exception ex) {
                Log.Info($"Error sending data: {ex.Message}, closing connection...");
                Disconnect();
            }
        }

        public void Disconnect() {
            //Prevent multiple disconnect calls
            if (!_objCancelationToken.IsCancellationRequested) {
                _objCancelationSource.Cancel();
                _ = WebSocket.CloseAsync(
                    WebSocketCloseStatus.NormalClosure,
                    string.Empty,
                    CancellationToken.None
                ).ContinueWith(s =>
                    Disconnected?.Invoke(this, this)
                );
            }
        }

        ~WebsocketClient() => _objCancelationSource.Dispose();
    }
}