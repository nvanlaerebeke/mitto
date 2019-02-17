namespace Mitto.Messaging.Request {
    public class Echo: RequestMessage {
        public Echo() : base() { }
		public Echo(string pMessage) : base() {
			Message = pMessage;
		}

		public string Message { get; set; }

        public override byte GetCode() {
            return 0x53;
        }
    }
}