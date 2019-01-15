namespace Mitto.IQueue {
	public delegate void DataHandler(Message pMessage);
	/// <summary>
	/// ToDo: better name for IQueue
	/// Is it really a queue? - for using rabbit we'll be using queues 
	/// but that's an implementation detail and not a  description of how messages are passed
	/// from byte arrays on the transport layer to the 'processor' that knows the binary data represents
	/// 
	/// Funtionarlly this is middleware that represents the boundery between the connection that represents communication
	/// and the meaning of what is communicated
	/// 
	/// The reason for it's existence is more than creating the programatical boundary (else we could just call the interface directly)
	/// It allows the workers (message handlers) to scale infinitely and so that the communication cluster can be run/scale differently from 
	/// the communication(connections)
	/// 
	/// This middleware is build on the concept of event driver design and this allowed the connections and message handlers to run as microservices
	/// 
	/// 
	/// 
	/// ToDo:
	/// We should be able to remove Rx event, respond method thould shows the Rx events
	/// </summary>
	public interface IQueue {
		event DataHandler Rx;
		void Transmit(Message pMessage);
		void Receive(Message pMessage);
	}
}