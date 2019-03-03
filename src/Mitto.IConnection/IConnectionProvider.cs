namespace Mitto.IConnection {
	public interface IConnectionProvider {
		IClient CreateClient();
		IServer CreateServer();
	}
}