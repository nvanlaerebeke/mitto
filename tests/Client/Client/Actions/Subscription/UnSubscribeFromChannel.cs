using Mitto.IMessaging;
using Mitto.Messaging.Response;
using System;

namespace Channel.Client.Actions.Subscription {

    internal class UnSubscribeFromChannel : BaseAction {
        private readonly string Channel;

        public UnSubscribeFromChannel(State pState, string pChannel) : base(pState) {
            Channel = pChannel;
        }

        public override void Run() {
            State.Client.Request<ACKResponse>(new Mitto.Subscription.Messaging.UnSubscribe.ChannelUnSubscribe(Channel), (r) => {
                if (r.Status.State == ResponseState.Success) {
                    Console.WriteLine($"Removed subscription from {Channel}");
                } else {
                    Console.WriteLine($"FAILED removing subscription from {Channel}");
                }
            });
        }
    }
}