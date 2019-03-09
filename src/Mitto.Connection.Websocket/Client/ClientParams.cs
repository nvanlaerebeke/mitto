namespace Mitto.Connection.Websocket {
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

		/// <summary>
		/// The maximum number of bytes allowed for this connection
		/// 0 or lower is infinite 
		/// </summary>
		public long MaxBytePerSecond { get; set; } = 0;

		public ProxySettings Proxy { get; set; } = new ProxySettings();
		public class ProxySettings {
            public string URL { get; set; } = "";
            public string UserName { get; set; } = "";
            public string Password { get; set; } = "";
            public ProxySettings() { }
        }
	}
}