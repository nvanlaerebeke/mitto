using System;

namespace Mitto.IRouting {
	/// <summary>
	/// 
	/// The IQueue interface represents the boundery between the receiving/sending messages
	/// and what it means on what is done with the message(s)
	/// 
	/// An example would be an IConnection that receives/sends messages to an IQueue
	/// implementation, that IQueue class then implements what it means to receive or send 
	/// a message over the IConnection.
	/// In Mitto we can have Websockets(IConnection) then sends/receives some binary data.
	/// That data must then be processed by 'someone', this is what the IQueue is for.
	/// 
	/// A good implemented example is the Mitto.Queue.RabbitMQ implementation, in there
	/// receiving a message from IConnection means putting it on a queue where the workers
	/// are reading from, and sending a message is done by reading from that IQueue and 
	/// sending it back over the IConnection (Websocket) in the correct format
	/// 
	/// </summary>
	public interface IRouter {
		string ConnectionID { get; }
		void Transmit(byte[] pData);
		void Receive(byte[] pData);
		void Close();
		bool IsAlive(string pRequestID);
	}
}