namespace Mitto.Messaging.Base.Notification {
    public class LogStatus : NotificationMessage {
        public LogStatus() : base() { }

        public override byte GetCode() {
            return 0x54;
        }
    }
}