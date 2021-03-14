using Mitto.IMessaging;
using Mitto.Messaging.Request;
using Mitto.Messaging.Response;
using System;

namespace Channel.Client.Actions.Message {

    internal class Ping : BaseAction {

        public Ping(State pState) : base(pState) {
        }

        public override void Run() {
            State.Client.Request<PongResponse>(new PingRequest(), (r) => {
                if (r.Status.State == ResponseState.Success) {
                    Console.WriteLine("Pong Received");
                } else {
                    Console.WriteLine("Error in Ping");
                    r.Status.ToString();
                }
            });
        }
    }
}