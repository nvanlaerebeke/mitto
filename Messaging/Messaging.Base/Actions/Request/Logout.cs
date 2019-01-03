namespace Messaging.Base.Action.Request {
    public class Logout : RequestAction<Messaging.Base.Request.Logout> {
        public Logout(Job pClient, Messaging.Base.Request.Logout pMessage) : base(pClient, pMessage) { }

        public override ResponseMessage Start() {
            return new Response.ACK(Request, ResponseCode.Success);
        }
    }
}