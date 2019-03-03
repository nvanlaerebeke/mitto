namespace Mitto.Messaging.Notification {
    public class InfoNotification : NotificationMessage {
		public InfoNotification() : base() { }
        public InfoNotification(string pMessage) : base() {
            Message = pMessage;
        }

        public string Message { get; set; }
    }
}