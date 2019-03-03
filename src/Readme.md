ToDo's before v1:

- Connection properties (secure/port/ip etc) to Config class
    Connection stuff should be generic, example IPC/WCF/Websocket, each has different parameters

- Fix assembly info's

- Add postfix to messages, example:
	Ping => PingRequest
	Pong => PongResponse
	ACK => ACKResponse

	This is to prevent namespace conflicts even if the namespace makes the name obsolete
	for developing it's much easier to have a postfixed

- MessageProvider.LoadTypes called twice

- Change namepace order for messages
  Now it's: <Project>.Message.Request.<Class>
  Sould be <Project>.Request, will change project structure a bit

- Keepalive for message processing between Client -> Server -> Queue -> Worker
    Make it so that ping has more then 1 seconds time to respond when using "bool WebSocket.Ping()"
	Might have to refactor a bit for passing properties to Mitto.Connection.Websockets, 

- Threading - only need ThreadPool.QueueWorkerItem when processing starts (example the Action<T> and callback methods)

- Custom error codes

- Make the Response messages type-safe, don't require IMessage but the actual Request type where needed
    Example:
	  - Response.Echo requires the interface IEcho? that implements the Message property

  Those that don't need the request can just accept IMessage, example ACK, Ping, ..

- Fix rabbitmq + write tests for it
- Add ability to close & clean up the server connection
- Go over the public interface and make as much as possible private/internal 
    This is to make it easier for the user - only leave assemblies available that are needed to create new plugins and 
	those that are needed to interact with Mitto - Also see if a few additional classes can't be made to make interaction
	easier, example Mitto.Client.Get() instead of Mitto.IConnection.ConnectionFactory().CreateClient();

- Refactor ResponseMessage
    - Remove objects that don't translate from object -> json -> byte{} -> json -> object
	  Example: IMessage in the constructor, it's supposed to be the request, but there should be no 
	  dependency on the request message, the constructor  should just only give the info it needs
	  and the properties should be filled in from that. 
	  
	  This is to prevent what I did for the test Echo message and use the "Request" in the property to return the message
	  This is obviously not possible, so we should prevent it so that a developer cannot make that mistake

After v1:

Improvements:
- Autoscale ThreadPool.MinThreads so the application ThreadPool autoscales in time
- WebSocketSharp - autoscale the message size within boundaries
- Create a pull request for the autoscale message sizes and bandwidth limiting/monitoring
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

  - Add bandwidth limiting to WebSocketSharp

  - add timetstamps to requests & responses

  - Add statistics in classes that do a lot of work or keep objects for a certain lifetime
    Example:
	  - Current open requests
	  - Current Actions being run
	  - Avarage time/action<T>

  - Create a message type that uses smaller byte type messages (not json)
    This is to compare what overhead json gives vs a simple byte API

Testing:
  - Unicode/UTF-32 tests for parts where were interact with text
   - Json messages
	  - Message names
	  - Verify input byte[], so that no crashes happen when bogus/invalid data is tranfered
	    There is no error handling on the Frame objects for example
	  - Think about how functional tests can be realized
    - Message names where the text in byte[] > 255, should cause an exception
      For UTF-32 this will be quite a bit faster than UTF-8 or even 16