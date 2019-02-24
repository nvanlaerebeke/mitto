using NSubstitute;
using NUnit.Framework;
using System.Linq;

namespace Mitto.IMessaging.Tests {
	[TestFixture]
	public class MessagingFactoryTests {
		/// <summary>
		/// Tests the MessagingFactory initialization
		/// This means that when the mocks passed in the initialization method are called
		/// the provider/convertor and processor are set to the provided values
		/// </summary>
		[Test]
		public void ConfigMessageConverterTest() {
			//Arrange
			var objConverter = Substitute.For<IMessageConverter>();
			var objProvider = Substitute.For<IMessageProvider>();
			var objProcessor = Substitute.For<IMessageProcessor>();

			//Act
			MessagingFactory.Initialize(objProvider, objConverter, objProcessor);

			//Assert
			MessagingFactory.Converter.Equals(objConverter);
			MessagingFactory.Processor.Equals(objProcessor);
			MessagingFactory.Provider.Equals(objProvider);
		}
	}
}