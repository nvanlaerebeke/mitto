using Mitto.IConnection;
using Mitto.IMessaging;
using Mitto.IRouting;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleToAttribute("Mitto.Routing.PassThrough.Tests")]
namespace Mitto.Routing.PassThrough {
	/// <summary>
	/// PassThrought data router responsible for routing data between
	/// the IConnection and IMessageProcessor
	/// 
	/// The PassThrough router is ideal for small applications 
	/// where scaling is less of an issue or applications where 
	/// the messages actions are light weight
	/// 
	/// As the name describes, this class takes in byte[] data from the 
	/// IConnection and IMessageProcessor and passes it to the 
	/// IConnection or IMessageProcessor depending on the context to be handled
	/// </summary>
	public class PassThroughRouter : IRouter {
		private IClientConnection Connection;

		/// <summary>
		/// Constructs the PassThrough router for the provided connection
		/// </summary>
		/// <param name="pConnection"></param>
		public PassThroughRouter(IClientConnection pConnection) {
			Connection = pConnection;
			Connection.Rx += Connection_Rx;
		}

		/// <summary>
		/// Receive data from the connection to be passed to the IMessageProcessor
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Connection_Rx(object sender, byte[] e) {
			MessagingFactory.Processor.Process(this, e);
		}

		/// <summary>
		/// Transmit data on the IConnection to the other side
		/// </summary>
		/// <param name="pMessage"></param>
		public void Transmit(byte[] pMessage) {
			Connection.Transmit(pMessage);
		}

		/// <summary>
		/// Shuts down the router
		/// </summary>
		public void Close() {
			Connection.Rx -= Connection_Rx;
		}
	}
}