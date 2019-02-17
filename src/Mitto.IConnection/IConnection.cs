namespace Mitto.IConnection {
	public delegate void ConnectionHandler(IConnection pConnection);
	public delegate void DataHandler(IConnection pConnection, byte[] pData);

	public interface IConnection {
		event ConnectionHandler Disconnected;
		event DataHandler Rx;
		string ID { get; }

		void Close();
	}
}
