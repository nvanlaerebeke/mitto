namespace Messaging.Base.Action.Request {
    public class Ping : RequestAction<Messaging.Base.Request.Ping> {
        public Ping(Job pClient, Messaging.Base.Request.Ping pMessage) : base(pClient, pMessage) { }

        public override ResponseMessage Start() {
            return new Response.Pong(Request, ResponseCode.Success);
        }
    }
}