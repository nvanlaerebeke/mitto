namespace IConnection {
	public interface IClient: IClientConnection {
		event ConnectionHandler Connected;
		void ConnectAsync(string pHostname, int pPort, bool pSecure);
	}
}