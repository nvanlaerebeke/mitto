namespace IConnection {
	public interface IClientConnection: IConnection {
		void Transmit(byte[] pData);
	}
}
