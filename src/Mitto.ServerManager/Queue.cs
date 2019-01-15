using IQueue;

namespace Queue {
	public delegate void DataHandler(Queue pQueue, byte[] pBytes);
	public interface Queue : IQueue.IQueue {
		event DataHandler Rx;
		event DataHandler Tx;
		void Transmit(byte[] pData);
		void Start();
		void Stop();
	}
}
