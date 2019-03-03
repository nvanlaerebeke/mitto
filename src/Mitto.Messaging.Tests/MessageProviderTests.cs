using Mitto.IMessaging;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mitto.Messaging.Tests {
	[TestFixture]
	public class MessageProviderTests {
		/// <summary>
		/// Tests getting an action for the provided IMessage where the action does not exist
		/// This means the expected return value should be null
		/// </summary>
		[Test]
		public void GetActionForUnknownMessageTest() {
			//Arrange
			var objClient = Substitute.For<IClient>();
			var objMessage = Substitute.For<IRequestMessage>();

			objMessage.Type.Returns(MessageType.Request);
			objMessage.Name.Returns("NoneExistingMessage");

			//Act
			var objProvider = new MessageProvider();
			var objAction = objProvider.GetAction(objClient, objMessage);

			//Assert
			Assert.IsNull(objAction);
		}

		/// <summary>
		/// Tests getting an action for the provided IMessage
		/// This means that the expected return value should be an IAction
		/// </summary>
		[Test]
		public void GetActionTest() {
			//Arrange
			var objClient = Substitute.For<IClient>();
			var objMessage = Substitute.For<Request.EchoRequest>();
			objMessage.Name.Returns("EchoRequest");

			//Act
			var objProvider = new MessageProvider();
			var objAction = objProvider.GetAction(objClient, objMessage);

			//Assert
			Assert.NotNull(objAction);
		}

		/// <summary>
		/// Tests getting a message from the provider based on a byte array
		/// This means that the IMessageConverter expects the byte[] that represent 
		/// the actual message without the MessageType and message name
		/// 
		/// See Frame for more info about how the byte[] is build
		/// </summary>
		[Test]
		public void GetMessageTest() {
			//Arrange
			var objConverter = Substitute.For<IMessageConverter>();
			var strName = "EchoRequest";
			var arrName = Encoding.UTF32.GetBytes(strName);
			var arrData = new byte[] { 1, 2, 3, 4, 5 };

			var lstBytes = new List<byte>();
			lstBytes.Add((byte)MessageType.Request);
			lstBytes.Add((byte)arrName.Length);
			lstBytes.AddRange(arrName);
			lstBytes.AddRange(arrData);

			var objMessage = Substitute.For<IMessage>();
			objConverter.GetMessage(Arg.Is(typeof(Request.EchoRequest)), Arg.Is<byte[]>(b => b.SequenceEqual(arrData))).Returns(objMessage);

			Config.Initialize(new Config.ConfigParams() {
				MessageConverter = objConverter
			});

			//Act
			var objReturnMessage = new MessageProvider().GetMessage(lstBytes.ToArray());

			//Assert
			objConverter.Received(1).GetMessage(typeof(Request.EchoRequest), Arg.Is<byte[]>(b => b.SequenceEqual(arrData)));
			Assert.NotNull(objMessage);
			Assert.AreEqual(objMessage, objReturnMessage);
		}

		/// <summary>
		/// Tests getting an IResponseMessage for the given IMessage with the provided code
		/// </summary>
		[Test]
		public void GetResponseMessageTest() {
			//Arrange
			var objMessage = new Request.EchoRequest("MyMessage"); // Substitute.For<Request.Echo>();
			//objMessage.ID.Returns("MyID");
			//objMessage.Name.Returns("Echo");
			
			//Act
			var objReturnMessage = new MessageProvider().GetResponseMessage(objMessage, ResponseState.TimeOut);

			//Assert
			Assert.NotNull(objReturnMessage);
			Assert.AreEqual("EchoResponse", objReturnMessage.Name);
			Assert.AreEqual(MessageType.Response, objReturnMessage.Type);
			Assert.AreEqual(ResponseState.TimeOut, objReturnMessage.Status.State);
		}

		/// <summary>
		/// Tests the initialization of the MessgeProvider
		/// This means the types in Mitto.Messaging are expected in the
		/// Types and Actions properties present in the MessageProvider class
		/// </summary>
		[Test]
		public void TestLoadTypesDefault() {
			//Arrange & Act
			var objProvider = new MessageProvider();

			//Assert
			Assert.IsTrue(objProvider.Types.ContainsKey(MessageType.Notification));
			Assert.IsTrue(objProvider.Types.ContainsKey(MessageType.Request));
			Assert.IsTrue(objProvider.Types.ContainsKey(MessageType.Response));
			Assert.IsTrue(objProvider.Types.ContainsKey(MessageType.Sub));
			Assert.IsTrue(objProvider.Types.ContainsKey(MessageType.UnSub));

			Assert.IsTrue(objProvider.Types[MessageType.Notification].Count.Equals(2));
			Assert.IsTrue(objProvider.Types[MessageType.Request].Count.Equals(5));
			Assert.IsTrue(objProvider.Types[MessageType.Response].Count.Equals(4));
			Assert.IsTrue(objProvider.Types[MessageType.Sub].Count.Equals(1));
			Assert.IsTrue(objProvider.Types[MessageType.UnSub].Count.Equals(1));

			Assert.IsTrue(objProvider.Actions.ContainsKey(MessageType.Notification));
			Assert.IsTrue(objProvider.Actions.ContainsKey(MessageType.Request));
			Assert.IsTrue(objProvider.Actions.ContainsKey(MessageType.Sub));
			Assert.IsTrue(objProvider.Actions.ContainsKey(MessageType.UnSub));
			Assert.IsTrue(!objProvider.Actions.ContainsKey(MessageType.Response));

			Assert.IsTrue(objProvider.Actions[MessageType.Notification].Count.Equals(2));
			Assert.IsTrue(objProvider.Actions[MessageType.Request].Count.Equals(5));
			Assert.IsTrue(objProvider.Actions[MessageType.Sub].Count.Equals(1));
			Assert.IsTrue(objProvider.Actions[MessageType.UnSub].Count.Equals(1));

			Assert.IsTrue(objProvider.SubscriptionHandlers.Count.Equals(2));
		}


		/// <summary>
		/// Tests the initialization of the MessgeProvider with a custom namespace
		/// This means the types in Mitto.Messaging are expected together with the types in 
		/// in the custom namespace given to the constructor
		/// </summary>
		[Test]
		public void TestLoadTypesCustom() {
			//Arrange & Act
			var objProvider = new MessageProvider("Mitto.Messaging.Tests.TestData");

			//Assert
			Assert.IsTrue(objProvider.Types.ContainsKey(MessageType.Notification));
			Assert.IsTrue(objProvider.Types.ContainsKey(MessageType.Request));
			Assert.IsTrue(objProvider.Types.ContainsKey(MessageType.Response));
			Assert.IsTrue(objProvider.Types.ContainsKey(MessageType.Sub));
			Assert.IsTrue(objProvider.Types.ContainsKey(MessageType.UnSub));

			Assert.IsTrue(objProvider.Types[MessageType.Notification].Count.Equals(3));
			Assert.IsTrue(objProvider.Types[MessageType.Request].Count.Equals(6));
			Assert.IsTrue(objProvider.Types[MessageType.Response].Count.Equals(5));
			Assert.IsTrue(objProvider.Types[MessageType.Sub].Count.Equals(2));
			Assert.IsTrue(objProvider.Types[MessageType.UnSub].Count.Equals(2));

			Assert.IsTrue(objProvider.Actions.ContainsKey(MessageType.Notification));
			Assert.IsTrue(objProvider.Actions.ContainsKey(MessageType.Request));
			Assert.IsTrue(objProvider.Actions.ContainsKey(MessageType.UnSub));
			Assert.IsTrue(!objProvider.Actions.ContainsKey(MessageType.Response));

			Assert.IsTrue(objProvider.Actions[MessageType.Notification].Count.Equals(3));
			Assert.IsTrue(objProvider.Actions[MessageType.Request].Count.Equals(6));
			Assert.IsTrue(objProvider.Actions[MessageType.Sub].Count.Equals(2));
			Assert.IsTrue(objProvider.Actions[MessageType.UnSub].Count.Equals(2));

			Assert.IsTrue(objProvider.SubscriptionHandlers.Count.Equals(3));
		}

		[Test]
		public void GetSubscriptionManagerTest() {
			//Arrange 
			var objProvider = new MessageProvider("Mitto.Messaging.Tests.TestData");
			//Act
			var obj = objProvider.GetSubscriptionHandler<TestData.Action.SubscriptionHandler.SubscriptionHandlerTestClass>();
			//Assert
			Assert.NotNull(obj);
		}
	}
}