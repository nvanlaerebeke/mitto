ToDo's before v1:

- Make sure when clients disconnect everything is cleaned up correctly (leaks)
  Example:
    - Mitto.Connection.Websocket.Client.WebsocketClient.Close(): needs adittional Disconnect method(see class for more info)
	- Requests
	- ...

- Mitto.ClientManager references Metto.Messaging.Base - should be Mitto.IMessaging only

- For requets, add a IRequestMessage interface, it's the same as IMessage but it's 

  so we have a type difference between the base IMessage and derived IRequestMessage and IResponseMessage

- Subscription support, this should be fairly easy as it's not really related to Mitto, it's higher level 
  where the IClient is kept when a Action is run so something can be put on the IConnection
  Note that this does mean that cleanup needs to be implemented properly, so that's why it would come in 
  handy if it's already in Mitto as users can just inherit from a SubscriptionHandler or something
  and don't have to thing about cleanups/memory leaks

- Keepalive for message processing between Client -> Server -> Queue -> Worker

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

After v1:

Improvements:
- Autoscale ThreadPool.MinThreads so the application ThreadPool autoscales in time
- WebSocketSharp - autoscale the message size within boundaries
- Create a pull request for the autoscale message sizes and bandwidth limiting/monitoring
- don't allow  names where the byte[] for the strings > 255, this will cause exceptions

Benchmarking:
    For each benchmark this means memory, cpu and time so it can be easily graphed
	
	Tests:
      - encoding/decoding utf32 vs utf16 vs utf8
	  - test overhead for converting between json/objects (both directions)
	  - Websockets vs TcpSocket

Features:
  - Create a connection type for IPC

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
	  
  - Message names where the text in byte[] > 255, should cause an exception
