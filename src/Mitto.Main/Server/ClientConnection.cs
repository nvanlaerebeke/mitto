using Mitto.IConnection;
using Mitto.IRouting;
using System;

namespace Mitto {
	/// <summary>
	/// Class that binds the IConnection and IRouter together for incomming connections
	/// 
	/// This is used when a client connects to the server and makes sure
	/// that any data arriving on the connection is routed to the correct
	/// message processor and any requests (byte[]) from the messageprovider
	/// is transfered over the IConnection
	/// </summary>
	public class ClientConnection {
		public event EventHandler<ClientConnection> Disconnected;
		private IClientConnection Connection { get; set; }
		private IRouter Router { get; set; }

		public ClientConnection(IClientConnection pConnection) {
			Connection = pConnection;
			Router = RouterFactory.Create(pConnection);
            Connection.Disconnected += Connection_Disconnected;

		}

		private void Connection_Disconnected(object sender, IConnection.IConnection e) {
            Connection.Disconnected -= Connection_Disconnected;
			Router.Close();
			Disconnected?.Invoke(this, this);
        }
    }
}