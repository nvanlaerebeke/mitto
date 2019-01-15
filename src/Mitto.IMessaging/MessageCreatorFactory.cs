using Unity;

namespace Mitto.IMessaging {
	public static class MessagingFactory {
		public static UnityContainer UnityContainer = new UnityContainer();

		private static IMessageCreator _objMessageCreator;
		private static IMessageProvider _objMessageProvider;

		private static IMessageProvider MessageProvider {
			get {
				if(_objMessageProvider == null) {
					_objMessageProvider = UnityContainer.Resolve<IMessageProvider>();
				}
				return _objMessageProvider;
			}
		}
		private static IMessageCreator MessageCreator {
			get {
				if(_objMessageCreator == null) {
					_objMessageCreator = UnityContainer.Resolve<IMessageCreator>();
				}
				return _objMessageCreator;
			}
		}

		public static IMessageProvider GetMessageProvider() {
			return MessageProvider;
		}

		public static IMessageCreator GetMessageCreator() {
			return MessageCreator;
		}
	}
}
