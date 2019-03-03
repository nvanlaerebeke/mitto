using Mitto.IConnection;
using System;
using System.Net;

namespace Mitto.Connection.Websocket {
	public class ServerParams : IConnection.ServerParams {

		/// <summary>
		/// Parameters the Websocket server will use to start
		/// </summary>
		/// <param name="pIPAddress">IP Address to listen on</param>
		/// <param name="pPort">Port the websocket server will listen on </param>
		/// <param name="pCallback">Action to execute when a client connects</param>
		public ServerParams(IPAddress pIPAddress, int pPort) {
			IP = pIPAddress;
			Port = pPort;
		}

		/// <summary>
		/// Parameters the Websocket server will use to start
		/// </summary>
		/// <param name="pIPAddress">IP Address to listen on</param>
		/// <param name="pPort">Port the websocket server will listen on </param>
		/// <param name="pCallback">Action to execute when a client connects</param>
		/// <param name="pCertPath">Certificate to use for the websocket server</param>
		/// <param name="pCertPassword">Certificate password to use</param>
		public ServerParams(IPAddress pIPAddress, int pPort, string pCertPath, string pCertPassword) {
			IP = pIPAddress;
			Port = pPort;
			CertPath = pCertPath;
			CertPassword = pCertPassword;
		}
		/// <summary>
		/// IP Address to listen on 
		///
		/// default: IPAddress.Any
		/// </summary>
		public IPAddress IP { get; } = IPAddress.Any;

		/// <summary>
		/// Certificate password to use
		/// 
		/// default: none
		/// </summary>
		public string CertPassword { get; } = "";

		/// <summary>
		/// Port the websocket server will listen on 
		/// 
		/// default: 80
		/// </summary>
		public int Port { get; } = 80;

		/// <summary>
		/// Certificate to use for the websocket server
		/// 
		/// default: none
		/// </summary>
		public string CertPath { get; } = "";

		/// <summary>
		/// Action to execute when a client connects
		/// </summary>
		public new Action<IClientConnection> ClientConnected { get; set; }
	}
}