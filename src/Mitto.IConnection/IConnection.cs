using System;

namespace Mitto.IConnection {
	public delegate void DataHandler(IConnection pConnection, byte[] pData);

	/// <summary>
	/// ToDo: 
	/// - EventHandler<IConnection> => EventHandler
	/// - DataHandler Rx => EventHandler<byte[]>
	/// 
	/// </summary>
	public interface IConnection {
		event EventHandler<IConnection> Disconnected;
		event DataHandler Rx;
		string ID { get; }

		void Disconnect();
	}
}
