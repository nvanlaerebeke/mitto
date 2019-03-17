using NUnit.Framework;
using NSubstitute;
using System.Linq;
using System;
using Mitto.IRouting;
using System.Text;
using System.Collections.Generic;

namespace Mitto.Routing.Tests {
	[TestFixture()]
	public class ControlProviderTests {

		/// <summary>
		/// Tests the creation of an IFrame by passing a byte array
		/// This means that the MessageType, name and data is returned successfully
		/// </summary>
		[Test]
		public void CreateTest() {
			//Arrange
			Config.Initialize();

			//Act
			var objProvider = new ControlProvider();

			//Assert
			Assert.IsTrue(objProvider.Requests.Count.Equals(1));
			Assert.IsTrue(objProvider.Responses.Count.Equals(1));
			Assert.IsTrue(objProvider.Actions.Count.Equals(1));
		}
	}
}