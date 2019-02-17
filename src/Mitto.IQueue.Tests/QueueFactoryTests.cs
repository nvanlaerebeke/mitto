using NSubstitute;
using NUnit.Framework;

namespace Mitto.IQueue.Tests {
	[TestFixture]
	public class QueueFactoryTests {
		/// <summary>
		/// Tests the QueueFactory initialization
		/// This means that when the Create method is called on the QueueFactory
		/// the methods will be call on the mocked provider that was passed in 
		/// the configuration
		/// 
		/// Said create method should also return an IQueue object
		/// </summary>
		[Test]
		public void ConfigQueueTest() {
			var objProvider = Substitute.For<IQueueProvider>();
			QueueFactory.Initialize(objProvider);
			
			var objQueue = QueueFactory.Create();

			Assert.IsInstanceOf<IQueue>(objQueue);
			objProvider.Received(1).Create();
		}
	}
}
