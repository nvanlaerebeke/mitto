
using System;

namespace Mitto.Routing.RabbitMQ.Consumer {
	/// <summary>
	/// RabbitMQ Consumer Message Handler
	/// 
	/// This is a consumer that reads from the MittoMain queue and processes any requests
	/// 
	/// </summary>
	public class RabbitMQ : RabbitMQBase {
		public event EventHandler<byte[]> Rx;

		public RabbitMQ() : base("MittoMain") { }

		public override void Transmit(byte[] data) {
			RabbitMQDataMessage objMessage = new RabbitMQDataMessage("MainMitto", this.ID, data);
			//this.AddToTxQueue(objMessage.);
		}

		public override void Receive(byte[] data) {
			RabbitMQDataMessage objMessage = new RabbitMQDataMessage(data);
			Rx?.Invoke(this, objMessage.Data);
		}
	}
}