namespace Mitto.Connection.Websocket {
	public abstract class Params {
		/// <summary>
		/// The number of seconds a connection may be idle before a ping
		/// is send to see if the connection is still alive
		/// 
		/// default: 30
		/// </summary>
		public int ConnectionTimeoutSeconds { get; set; } = 30;
	}
}
