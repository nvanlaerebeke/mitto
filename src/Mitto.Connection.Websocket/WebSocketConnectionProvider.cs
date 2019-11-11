﻿using Mitto.IConnection;
using Mitto.Utilities;

namespace Mitto.Connection.Websocket {

    public class WebSocketConnectionProvider : IConnectionProvider {

        /// <summary>
        /// Represents the length used to determine whether the data should be fragmented in sending.
        /// </summary>
        /// <remarks>
        ///   <para>
        ///   The data will be fragmented if that length is greater than the value of this field.
        ///   </para>
        ///   <para>
        ///   If you would like to change the value, you must set it to a value between <c>125</c> and
        ///   <c>Int32.MaxValue - 14</c> inclusive.
        ///   </para>
        /// </remarks>
        public int FragmentLength { get; set; } = 1024;

        public IClient CreateClient() {
            return new Client.WebsocketClient(new Wrapper.WebSocketClientWrapper(), FragmentLength);
        }

        public IServer CreateServer() {
            return new Server.WebSocketServer(new Wrapper.WebSocketServerWrapper(), FragmentLength);
        }
    }
}