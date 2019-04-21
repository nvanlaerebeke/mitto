namespace Mitto.Subscription.Messaging {

    public static class SubscriptionFactory {

        public static void Initialize() {
            /*var tmp = typeof(Messaging.SubMessage);
            tmp = typeof(Messaging.UnSubMessage);*/
            var tmp = typeof(Mitto.Subscription.Messaging.UnSubscribe.ChannelUnSubscribe);
            System.Console.WriteLine(tmp.Name);
        }
    }
}