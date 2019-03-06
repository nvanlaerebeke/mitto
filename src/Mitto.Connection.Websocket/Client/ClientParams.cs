﻿namespace Mitto.Connection.Websocket {
	public class ClientParams : Params, IConnection.IClientParams {
		/// <summary>
		/// Hostname to connect to 
		///
		/// default: localhost
		/// </summary>
		public string Hostname { get; set; } = "localhost";

		/// <summary>
		/// Port the websocket client will connect to 
		/// 
		/// default: 80
		/// </summary>
		public int Port { get; set; } = 80;

		/// <summary>
		/// Create a secure connection (ws:// vs wss://)
		/// 
		/// default: false
		/// </summary>
		public bool Secure { get; set; } = false;

        public ProxySettings Proxy { get; set; } = new ProxySettings();

        public class ProxySettings {
            public string URL { get; set; } = "";
            public string UserName { get; set; } = "";
            public string Password { get; set; } = "";
            public ProxySettings() { }
        }
	}
}