using CommandLineMenu;
using Mitto.IMessaging;
using Mitto.Messaging.Response;
using Mitto.Subscription.Messaging.Request;
using System;

namespace Channel.Client.Actions.Message {

    internal class SendToChannel : BaseAction {
        private readonly string Channel;

        public SendToChannel(State pState, string pChannel) : base(pState) {
            Channel = pChannel;
        }

        public override void Run() {
            State.Client.Request<ACKResponse>(new SendToChannelRequest(Channel, new RequestInput().Start()), (r) => {
                if (r.Status.State == ResponseState.Success) {
                    Console.WriteLine($"Sent message to channel {Channel}");
                } else {
                    Console.WriteLine($"FAILED sending message to channel");
                }
            });
        }
    }
}