namespace Mitto.Messaging.Base.EventArgs {
    public class MessageEventArgs {
        private string _strKey;
        
        public string Key { 
            get { return _strKey; }
            set { _strKey = value; }
        }

        public MessageEventArgs(string pKey) {
            Key = pKey;
        }
    }
}
