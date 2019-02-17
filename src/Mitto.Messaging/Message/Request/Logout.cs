namespace Mitto.Messaging.Request {
    public class Logout : RequestMessage {
        public string AccountName { get; set; }
        public Logout(string pAccountName) : base() {
            AccountName = pAccountName;
        }
        public Logout() { }
        public override byte GetCode() {
            return 0x52;
        }
    }
}