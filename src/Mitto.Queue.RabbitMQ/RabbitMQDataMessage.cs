namespace Mitto.Queue.RabbitMQ {
	public class RabbitMQDataMessage : RabbitMQMessage {
		public RabbitMQDataMessage(byte[] pData) : base(pData) { }
		public RabbitMQDataMessage(string pQueueID, string pClientID, byte[] pData) : base(pQueueID, pClientID, RabbitMQMessageFormat.data, pData) { }
    }
}