using NSubstitute;
using NUnit.Framework;
using System.Linq;

namespace Mitto.IMessaging.Tests {
	[TestFixture]
	public class MessagingFactoryTests {
		/// <summary>
		/// Tests the MessagingFactory initialization
		/// This means that when the mocks passed in the initialization method are called
		/// when doing calling the Creator/Provider and Processor
		/// the same methods will be called on the mocked IMessageCreator that was configured
		/// </summary>
		[Test]
		public void ConfigMessageCreatorTest() {
			//Arrange
			var objCreator = Substitute.For<IMessageConverter>();
			var objProvider = Substitute.For<IMessageProvider>();
			var objProcessor = Substitute.For<IMessageProcessor>();

			MessagingFactory.Initialize(objProvider, objCreator, objProcessor);

			//Act &Assert
			var objMessage = MessagingFactory.Converter.GetMessage(new byte[] { 1, 2, 3, 4 });
			Assert.IsInstanceOf<IMessage>(objMessage);

			var arrBytes = MessagingFactory.Converter.GetByteArray(objMessage);
			Assert.IsInstanceOf<byte[]>(arrBytes);

			var objResponse = MessagingFactory.Converter.GetResponseMessage(objMessage, ResponseCode.Success);
			Assert.IsInstanceOf<IMessage>(objResponse);

			objCreator.Received(1).GetMessage(Arg.Is<byte[]>(b => b.SequenceEqual(new byte[] { 1, 2, 3, 4 })));
			objCreator.Received(1).GetByteArray(Arg.Any<IMessage>());
			objCreator.Received(1).GetResponseMessage(Arg.Any<IMessage>(), ResponseCode.Success);

			MessagingFactory.Provider.GetTypes();
			MessagingFactory.Provider.GetResponseType("MyMessageType");
			MessagingFactory.Provider.GetType(MessageType.Request, 1);
			//MessagingFactory.Provider.GetActionType(Substitute.For<IMessage>());

			objProvider.Received(1).GetTypes();
			objProvider.Received(1).GetResponseType(Arg.Any<string>());
			objProvider.Received(1).GetType(MessageType.Request, 1);
			//objProvider.Received(1).GetActionType(Arg.Any<IMessage>());

			MessagingFactory.Processor.Process(Substitute.For<IQueue.IQueue>(), new byte[] { 1, 2, 3, 4 });
			objProcessor.Received(1).Process(Arg.Any<IQueue.IQueue>(), Arg.Is<byte[]>(b => b.SequenceEqual(new byte[] { 1, 2, 3, 4 })));


			Assert.Ignore("Test action stuff");
		}
	}
}