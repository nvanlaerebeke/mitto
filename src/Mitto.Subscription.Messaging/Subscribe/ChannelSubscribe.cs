namespace Mitto.Subscription.Messaging.Subscribe {

    public class ChannelSubscribe : SubMessage {
        public string ChannelName { get; set; }

        public ChannelSubscribe(string pName) {
            ChannelName = pName;
        }
    }
}