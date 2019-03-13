Minimum ToDo's:
	- Fix rabbitmq + write tests for it
	- Add the ability to call connectasync on a disconnected client, now a new client is required
	- Make router a generic class of IRouter<F> where F is the Frame type that implements IFrame
	  This is for defining how the data is represented internally, example when using RabbitMQ vs PassThrough vs ...
	  The frame type will be used in the Transmit method, where the parameter is the frame instead of the byte[]
	  Reason for this is that sometimes more information is needed then just the byte[], example for rabbitmq the 
	  Queue to put the data on.
	- IConnection.Disconnected => needs to be EventHandler<IClientConnection> instead of just EventHandler
	- Hide more classes, make them only available from Mitto.xxx, example the factories like ConnectionFactory
	  This is to prevent missuse

Improvements:
	- Autoscale ThreadPool.MinThreads so the application ThreadPool autoscales in time
	- don't allow  names where the byte[] for the strings > 255, this will cause exceptions
	- RabbitMQ: 
	    - Stop converting the message byte[] => Message, this means modifying our byte[] a bit 
		  and include the request id in the frame.
		- Add GetMessageStatus support so that "KeepAlives" for message actions work
		  We can do this by switching GetMessageStatus to a message type like "Control"/"Management"
		  The best way to do this is move it away from the general json message and just use
		  a constructed frame of <type><connectionid><requestid><control message frame?>
		  where the last part is custom, the first 3 parts should be part of every message(WIP)

		- Add cleanup service that makes sure all queues are still in use and cleans up 
		  those that are not relevant anymore

Documentation:
	- go over the comments in the code - add/improve/fix them where needed
	- create basic documentation about how to use Mitto, how to use each component and how to create a custom one
	- create detailed internal documentation how the Mitto internals work

Benchmarking:
    For each benchmark this means memory, cpu and time so it can be easily graphed
	
	Tests:
      - encoding/decoding utf32 vs utf16 vs utf8 - UTF32 takes up much more bytes
	  - test overhead for converting between json/objects (both directions)
	  - Websockets vs TcpSocket
	  - Websocket TransmitQueue/Thread vs SendAsync
	  - Message names in UTF-32, see what encoding class names can actually be, can't it
	    be switched to asni?, it's much smaller in overhead

Features:
  - Create a connection type for IPC (WCF) - what can can be used on linux & osx?
  
  - Add statistics in classes that do a lot of work or keep objects for a certain lifetime
    Example:
	  - Current open requests
	  - Current Actions being run
	  - Avarage time/action<T>

  - Create a message type that uses smaller byte type messages (not json)
    This is to compare what overhead json gives vs a simple byte API

  - Kafka?

  - Mitto as a REST API?

Testing:
	- go over tests and create missing tests
	- Unicode/UTF-32 tests for parts where were interact with text
	- Json messages
		- Message names
		- Verify input byte[], so that no crashes happen when bogus/invalid data is tranfered
		  There is no error handling on the Frame objects for example
		- Think about how functional tests can be realized
    - Message names where the text in byte[] > 255, should cause an exception
      For UTF-32 this will be quite a bit faster than UTF-8 or even 16
	- Data transfer speeds:
		- with jumbo frames
		- with different communication media and messaging
	- Code coverage plugin for nunit/nsubstitute?