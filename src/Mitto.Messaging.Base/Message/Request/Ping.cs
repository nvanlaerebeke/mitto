namespace Mitto.Messaging.Base.Request {
    public class Ping: RequestMessage {
        public Ping() : base() { }

        public override byte GetCode() {
            return 0x50;
        }
    }
}