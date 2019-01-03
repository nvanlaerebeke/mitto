using System;

namespace Messaging.Base.Request {
    public class Ident : RequestMessage {
        public string LocationID { get; set; }
        public Version LocationVersion { get; set; }
        public string LocationName { get; set; }
        public Ident() { }
        /*public Ident(ClientInfo pClientInfo) : base() {
            if (pClientInfo != null) {
                LocationID = pClientInfo.ID;
                LocationVersion = pClientInfo.Version;
                LocationName = pClientInfo.Name;
            }
        }*/

        public override byte GetCode() {
            return 0x51;
        }
    }
}