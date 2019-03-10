using Mitto.IMessaging;
using Mitto.IRouting;
using System.Linq;

namespace Mitto.Routing.RabbitMQ.Publisher {
	/// <summary>
	/// RabbitMQ Router
	/// Used for publishing byte[] on a queue so a publisher can handle the processing of the data
	/// 
	/// Ideal for deployments where scalability and high availability is key
	/// 
	/// Data received from the IConnection is put on the main queue while
	/// the Queue will listen on the IConnection.ID queue for any data 
	/// that needs to be forwarded to the IConnection
	/// </summary>
	public class RabbitMQ : IRouter {
		private RequestManager RequestManager;
		private IConnection.IClient Connection { get; set; }
		private Queue Queue { get; set; }

		public RabbitMQ(IConnection.IClient pConnection) {
			RequestManager = new RequestManager();

			Connection = pConnection;
			Connection.Rx += Connection_Rx;

			Queue = new Queue("MittoMain", Connection.ID);
			Queue.Rx += Queue_Rx;
		}

		private void Queue_Rx(object sender, Frame e) {
			Transmit(e.Data);
		}

		private void Connection_Rx(object sender, byte[] e) {
			if(((MessageType)e.ElementAt(0)) == MessageType.Response) {
				RequestManager.SetResponse(MessagingFactory.Provider.GetMessage(e) as IResponseMessage);
			} else {
				Queue.Transmit(new Frame(e));
			}
		}

		public void Transmit(byte[] pData) {
			Connection.Transmit(pData);
		}

		public void Close() {
			Queue.Close();
			Connection.Rx -= Connection_Rx;
		}
	}
}