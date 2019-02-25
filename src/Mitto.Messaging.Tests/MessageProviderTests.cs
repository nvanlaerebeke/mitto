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
			var objClient = Substitute.For<IQueue.IQueue>();
			var objMessage = Substitute.For<IMessage>();

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
			var objClient = Substitute.For<IQueue.IQueue>();
			var objMessage = Substitute.For<Request.Echo>();
			objMessage.Name.Returns("Echo");

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
			var strName = "Echo";
			var arrName = Encoding.UTF32.GetBytes(strName);
			var arrData = new byte[] { 1, 2, 3, 4, 5 };

			var lstBytes = new List<byte>();
			lstBytes.Add((byte)MessageType.Request);
			lstBytes.Add((byte)arrName.Length);
			lstBytes.AddRange(arrName);
			lstBytes.AddRange(arrData);

			var objMessage = Substitute.For<IMessage>();
			objConverter.GetMessage(Arg.Is(typeof(Request.Echo)), Arg.Is<byte[]>(b => b.SequenceEqual(arrData))).Returns(objMessage);

			Config.Initialize(new Config.ConfigParams() {
				MessageConverter = objConverter
			});

			//Act
			var objReturnMessage = new MessageProvider().GetMessage(lstBytes.ToArray());

			//Assert
			objConverter.Received(1).GetMessage(typeof(Request.Echo), Arg.Is<byte[]>(b => b.SequenceEqual(arrData)));
			Assert.NotNull(objMessage);
			Assert.AreEqual(objMessage, objReturnMessage);
		}

		/// <summary>
		/// Tests getting an IResponseMessage for the given IMessage with the provided code
		/// </summary>
		[Test]
		public void GetResponseMessageTest() {
			//Arrange
			var objMessage = new Request.Echo("MyMessage"); // Substitute.For<Request.Echo>();
			//objMessage.ID.Returns("MyID");
			//objMessage.Name.Returns("Echo");
			
			//Act
			var objReturnMessage = new MessageProvider().GetResponseMessage(objMessage, ResponseCode.TimeOut);

			//Assert
			Assert.NotNull(objReturnMessage);
			Assert.AreEqual("Echo", objReturnMessage.Name);
			Assert.AreEqual(MessageType.Response, objReturnMessage.Type);
			Assert.AreEqual(ResponseCode.TimeOut, objReturnMessage.Status);
		}

		/// <summary>
		/// Tests the initialization of the MessgeProvider
		/// This means the types in Mitto.Messaging are expected in the
		/// Types and Actions properties present in the MessageProvider class
		/// </summary>
		[Test]
		public void TestLoadTypes() {
			//Arrange & Act
			var objProvider = new MessageProvider();

			//Assert
			Assert.IsTrue(objProvider.Types.ContainsKey(MessageType.Notification));
			Assert.IsTrue(objProvider.Types.ContainsKey(MessageType.Request));
			Assert.IsTrue(objProvider.Types.ContainsKey(MessageType.Response));

			Assert.IsTrue(objProvider.Types[MessageType.Notification].Count.Equals(2));
			Assert.IsTrue(objProvider.Types[MessageType.Request].Count.Equals(2));
			Assert.IsTrue(objProvider.Types[MessageType.Response].Count.Equals(3));

			Assert.IsTrue(objProvider.Actions.ContainsKey(MessageType.Notification));
			Assert.IsTrue(objProvider.Actions.ContainsKey(MessageType.Request));

			Assert.IsTrue(objProvider.Actions[MessageType.Notification].Count.Equals(2));
			Assert.IsTrue(objProvider.Actions[MessageType.Request].Count.Equals(2));
		}

		/// <summary>
		/// Tests if a correct response message is returned
		/// This means that when an IMessage is passed, an IResponseMessage for that
		/// type with the given response code is expected
		/// </summary>
		//[Test]
		/*public void GetResponseMessageTest() {
			//Arrange
			var objRequest = Substitute.For<IMessage>();
			var objProvider = Substitute.For<IMessageProvider>();
			objRequest.Name.Returns("TestMessageResponse");
			objProvider.GetResponseType("TestMessageResponse").Returns(typeof(Libs.TestMessageResponse));
			Mitto.Initialize(new Mitto.Config() {
				MessageProvider = objProvider
			});

			//Act
			var objResponse = new MessageConverter().GetResponseMessage(objRequest, ResponseCode.Cancelled);

			//Assert
			Assert.AreEqual(objResponse.Name, "TestMessageResponse");
			Assert.IsTrue(objResponse.Request.Equals(objRequest));
			Assert.AreEqual(objResponse.Type, MessageType.Response);
			Assert.AreEqual(objResponse.Status, ResponseCode.Cancelled);
			Assert.AreEqual(objResponse.GetCode(), 0x66);
		}*/

	}
}
