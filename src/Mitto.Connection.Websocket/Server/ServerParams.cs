using System.Net;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Mitto.Messaging.Tests.Action.Request")]
[assembly: InternalsVisibleTo("Mitto.Connection.Websocket.Tests.Server")]

namespace Mitto.Connection.Websocket {

    public class ServerParams {

        public ServerParams() {
        }

        /// <summary>
        /// IP Address to listen on
        ///
        /// default: IPAddress.Any
        /// </summary>
        public IPAddress IP { get; set; } = IPAddress.Any;

        /// <summary>
        /// Port to listen on
        ///
        /// default: 80
        /// </summary>
        public int Port { get; set; } = 80;

        /// <summary>
        /// Is this a secure connection? (https)
        ///
        /// default: false
        /// </summary>
        public bool Secure { get; set; } = false;

        /// <summary>
        /// Path to listen on (http(s)://<IP>:<Port>/<Path>)
        /// </summary>
        public string Path { get; set; } = "/";

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

        public int FragmentSize { get; set; } = 1024;
    }
}