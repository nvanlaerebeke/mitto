using Mitto.IMessaging;
using Mitto.Messaging.Response;
using System;

namespace Channel.Client.Actions.Subscription {

    internal class SubscribeToChannel : BaseAction {
        private readonly string Channel;

        public SubscribeToChannel(State pState, string pChannel) : base(pState) {
            Channel = pChannel;
        }

        public override void Run() {
            State.Client.Request<ACKResponse>(new Mitto.Subscription.Messaging.Subscribe.ChannelSubscribe(Channel), (r) => {
                if (r.Status.State == ResponseState.Success) {
                    Console.WriteLine($"Subscribed to {Channel}");
                } else {
                    Console.WriteLine($"FAILED Subscribing to {Channel}");
                }
            });
        }
    }
}