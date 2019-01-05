using Messaging.Base;
using Messaging.Base.Action;

namespace Messaging.App.Action.Request {
    public class Echo : RequestAction<App.Request.Echo> {
        public Echo(Job pClient, App.Request.Echo pMessage) : base(pClient, pMessage) { }

        public override ResponseMessage Start() {
            return new Response.Echo(Request, ResponseCode.Success);
        }
    }
}