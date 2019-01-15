namespace Mitto.IConnection {
	public interface IClient: IClientConnection {
		event ConnectionHandler Connected;
		event ConnectionHandler Disconnected;

		void ConnectAsync(string pHostname, int pPort, bool pSecure);
	}
}