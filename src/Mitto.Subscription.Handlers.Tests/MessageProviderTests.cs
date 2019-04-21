using Mitto.Messaging;
using Mitto.Subscription.Messaging.Tests.TestData.Action.SubscriptionHandler;
using NUnit.Framework;

namespace Mitto.Subscription.Messaging.Tests {

    [TestFixture]
    public class MessageProviderTests {

        [Test]
        public void GetSubscriptionManagerTest() {
            //Arrange
            var objProvider = new MessageProvider();
            //Act
            var obj = objProvider.GetSubscriptionHandler<SubscriptionHandlerTestClass>();
            //Assert
            Assert.NotNull(obj);
        }
    }
}