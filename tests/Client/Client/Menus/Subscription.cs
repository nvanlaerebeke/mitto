using System;
using CommandLineMenu;
using Mitto.Subscription.Messaging.Action.Request;

namespace Channel.Client.Menus {

    internal class Subscription : BaseMenu {

        public Subscription(State pState) : base(pState) {
        }

        public override Menu GetMenu() {
            Menu objMenu = new Menu();

            objMenu.Add(new MenuItem("s", "subscribe", "Subscribe to channel 'MyChannel'", () => {
                new Actions.Subscription.SubscribeToChannel(State, "MyChannel").Run();
                ReceiveOnChannelRequestAction.ChannelMessageReceived += ReceiveOnChannelRequestAction_ChannelMessageReceived;
            }));

            objMenu.Add(new MenuItem("u", "unsubscribe", "UnSubscribe from channel 'MyChannel'", () => {
                new Actions.Subscription.UnSubscribeFromChannel(State, "MyChannel").Run();
                ReceiveOnChannelRequestAction.ChannelMessageReceived += ReceiveOnChannelRequestAction_ChannelMessageReceived;
            }));

            objMenu.Add(new MenuItem("m", "message", "send message to channel 'MyChannel'", () => {
                new Actions.Message.SendToChannel(State, "MyChannel").Run();
            }));
            return objMenu;
        }

        private void ReceiveOnChannelRequestAction_ChannelMessageReceived(string pChannel, string pMessage) {
            Console.WriteLine($"{pChannel}: {pMessage}");
        }
    }
}