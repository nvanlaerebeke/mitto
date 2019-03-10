ToDo's before v1:

- Add the ability to call connectasync on a disconnected client, now a new client is required

- Improve IQueue name, rename it to 
	- Mitto.IRouting
	- Mitto.Routing.PassThrough
	- Mitto.Routing.RabbitMQ.Publisher
	- Mitto.Routing.RabbitMQ.Consumer
	
- go over tests and create missing tests

- Fix rabbitmq + write tests for it

- go over the comments in the code - add/improve/fix them where needed

- create basic documentation about how to use Mitto
	How to use each component and how to create a custom one
	
- create detailed internal documentation how the Mitto internals work

After v1:

Improvements:
- Autoscale ThreadPool.MinThreads so the application ThreadPool autoscales in time
- don't allow  names where the byte[] for the strings > 255, this will cause exceptions

Benchmarking:
    For each benchmark this means memory, cpu and time so it can be easily graphed
	
	Tests:
      - encoding/decoding utf32 vs utf16 vs utf8 - UTF32 takes up much more bytes
	  - test overhead for converting between json/objects (both directions)
	  - Websockets vs TcpSocket
	  - Websocket TransmitQueue/Thread vs SendAsync

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

Testing:
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