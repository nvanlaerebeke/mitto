namespace Mitto.Messaging.Notification {
    public class Info : NotificationMessage {
		public Info() : base() { }
        public Info(string pMessage) : base() {
            Message = pMessage;
        }

        public string Message { get; set; }
    }
}