using CommandLineMenu;
using Mitto.IMessaging;
using Mitto.IRouting;
using Mitto.Messaging.Request;
using Mitto.Messaging.Response;
using System;

namespace Channel.Client.Actions.Message {

    internal class SendEcho : BaseAction {

        public SendEcho(State pState) : base(pState) {
        }

        public override void Run() {
            State.Client.Request<EchoResponse>(new EchoRequest(new RequestInput().Start()), (r) => {
                if (r.Status.State == ResponseState.Success) {
                    Console.WriteLine(r.Message);
                } else {
                    r.Status.ToString();
                }
            });
        }
    }
}