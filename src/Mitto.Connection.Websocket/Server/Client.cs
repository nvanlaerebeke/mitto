using System;
using System.Runtime.CompilerServices;
using System.Threading;
using Mitto.IConnection;
using Mitto.Logging;
using System.Net.WebSockets;
using System.Collections.Generic;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("Mitto.Connection.Websocket.Tests")]

namespace Mitto.Connection.Websocket.Server {

    internal class Client : IClientConnection {
        private readonly ILog Log = LoggingFactory.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly int _intBufferSize = 1024;

        private readonly WebSocket _objClient;

        public event EventHandler<IConnection.IConnection> Disconnected;

        public event EventHandler<byte[]> Rx;

        /// <summary>
        /// Used to stop the Accept loop
        /// </summary>
        private CancellationTokenSource _objCancelationSource;

        private CancellationToken _objCancelationToken;

        public string ID { get; private set; }

        public Client(WebSocket pClient, int pBufferSize) {
            ID = Guid.NewGuid().ToString();
            _objClient = pClient;
            _intBufferSize = pBufferSize;

            Log.Info($"Client {ID} connected");
        }

        public async void Start() {
            try {
                if (_objCancelationSource != null) {
                    _objCancelationSource.Dispose();
                }
                _objCancelationSource = new CancellationTokenSource();
                _objCancelationToken = _objCancelationSource.Token;

                while (!_objCancelationToken.IsCancellationRequested) {
                    var buffer = new byte[_intBufferSize];
                    var result = await _objClient.ReceiveAsync(
                        new ArraySegment<byte>(buffer),
                        _objCancelationToken
                    );

                    if (result.MessageType == WebSocketMessageType.Close) {
                        Disconnect();
                        return;
                    }

                    var lstMessage = new List<byte>(buffer);

                    //ToDo: dynamic sizing of the buffer
                    while (!result.EndOfMessage) {
                        result = await _objClient.ReceiveAsync(
                            new ArraySegment<byte>(buffer),
                            _objCancelationToken
                        );
                        lstMessage.AddRange(buffer);
                    }
                    Rx(this, lstMessage.ToArray());
                }
            } catch (TaskCanceledException) {
                //ignore
            } catch (Exception ex) {
                Console.WriteLine($"Error: {ex.Message}, closing connection");
                Disconnect();
            }
        }

        public void Transmit(byte[] pData) {
            try {
                _objClient.SendAsync(
                    new ArraySegment<byte>(pData),
                    WebSocketMessageType.Binary,
                    true,
                    _objCancelationToken
                );
            } catch (TaskCanceledException) {
                //ignore
            } catch (Exception ex) {
                Log.Error($"Error sending data: {ex.Message}, closing connection...");
                Disconnect();
            }
        }

        public void Disconnect() {
            try {
                //Prevent disconnect loops
                if (!_objCancelationToken.IsCancellationRequested) {
                    Log.Info($"Closing {ID}");
                    _objCancelationSource.Cancel();
                    _ = _objClient.CloseAsync(
                        WebSocketCloseStatus.NormalClosure,
                        "",
                        CancellationToken.None // -- do not use existing CancellationToken, that one is already canceled
                        ).ContinueWith((t) => {
                            try {
                                _objClient.Dispose();
                            } catch { }
                        });
                } else {
                    Log.Debug("Already Disconnected");
                }
            } catch (Exception) { }

            Disconnected?.Invoke(this, this);
        }

        ~Client() => _objCancelationSource.Dispose();
    }
}