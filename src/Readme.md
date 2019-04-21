Minimum ToDo's:
  - Make router a generic class of IRouter<F> where F is the Frame type that implements IFrame
    This is for defining how the data is represented internally, example when using RabbitMQ vs PassThrough vs ...
    The frame type will be used in the Transmit method, where the parameter is the frame instead of the byte[]
    Reason for this is that sometimes more information is needed then just the byte[], example for RabbitMQ the 
    Queue to put the data on.

  - IConnection.Disconnected => needs to be EventHandler<IClientConnection> instead of just EventHandler

  - Hide more classes, make them only available from Mitto.xxx, example the factories like ConnectionFactory
    This is to prevent misuse
 
  - Router.IClient is used in Messaging, does it need to be an IClient?, does IClient then belong in IRouting?
    Isn't it a IMessaging interface?
   
 - Create ISubMessage/IUnSubMessage interfaces in Mitto.IMessaging so that PassThrough router can remove
   the dependency on the Messaging project

Improvements:
  - Review, refactor and merge all Requests, ControlRequest, RabittMQRequest, SubscriptionRequest, ...
    Functionallity like the KeepAlive should only be implemented once in the base class

  - Refactor RoutingFrame: add modifying of the properties instead of creating new frames
    Also check if it's possible to decrease the number of times a RoutingFrame needs modifying

  - Merge ActionManager and RequestManager, it's both a Request/Response model

  - Use arrData.CopyTo instead of Array.Copy and if possible use System.Buffer.BlockCopy (much faster):
    https://stackoverflow.com/a/415839/2106514
    https://stackoverflow.com/questions/1605090/what-is-the-difference-between-array-copy-and-array-copyto/1667856#1667856

  - Improve MessageProvider so that it takes in all the messages in the entire application based on the 
    interfaces/base classes it implements, like done for the ControlProvider
    This will prevent the need for passing the namespaces
    Still need to keep the option of the namespaces due to priorities etc
    By default overwrite the Mitto.* classes with the application it's classes

  - Move Control messages away from IMessaging and implement them outside of it
    Move to the Router, use ControlFrame just as in RabbitMQ implementation, implementation 
    should be something router specific as only it knows how to get the status or who or what
    to pass the request onto

  - implement compression for data that is transfered using IConnection or JSON
    Prevent double compression from JSON && IConnection
    On what layer should compression be implemented?, test difference between 
    compressing the JSON text vs compressing the UTF32.GetBytes byte[]


  - Add a version to the Frame's being sent - can be useful for backwards compatibility when making changes 
    to the way data is being encapsulated, can be taken from the AssemblyVersion

  - Auto scale ThreadPool.MinThreads so the application ThreadPool auto-scales in time

  - don't allow  names where the byte[] for the strings > 255, this will cause exceptions

    - Add a status page (WebPage) that can be viewed when enabled on port x 

    - Remove the Sub and UnSub message types, they're just Requests, no need to separate them

Documentation:
  - go over the comments in the code - add/improve/fix them where needed
  - create basic documentation about how to use Mitto, how to use each component and how to create a custom one
  - create detailed internal documentation how the Mitto internals work

Benchmarking:
    For each benchmark this means memory, CPU and time so it can be easily graphed
  
  Tests:
      - encoding/decoding utf32 vs utf16 vs utf8 - UTF32 takes up much more bytes
    - Message names in ANSII?, currently UTF-32, see what encoding class names can actually be, can't it
    be switched to ANSI?, it's much smaller in overhead
    - test overhead for converting between JSON/objects (both directions)
    - WebSockets vs TcpSocket
    - WebSocket TransmitQueue/Thread vs SendAsync
    - Locations where messages are kept/pasted add timing benchmarks, example:
          Ping: x times, average response time over 5 sec, 10 sec, 30 sec, last 10k messages
      Enable when in debug or m/b info mode?
    - Memory profiling, checks for memory leaks

Features:
  - Create a connection type for IPC (WCF) - what can be used on Linux & OSX?
  
  - Add statistics in classes that do a lot of work or keep objects for a certain lifetime
    Example:
    - Current open requests
    - Current Actions being run
    - Average time/action<T>

  - Create a message type that uses smaller byte type messages (not JSON)
    This is to compare what overhead JSON gives vs a simple byte API

  - Kafka?

  - Mitto Web API's:
    -> .NET Core
  -> GraphQL
  -> Request/Response translation stuff: https://www.youtube.com/watch?v=SUiWfhAhgQw

Testing:
  - go over tests and create missing tests
  - Unicode/UTF-32 tests for parts where were interact with text
  - JSON messages
    - Message names
    - Verify input byte[], so that no crashes happen when bogus/invalid data is transfered
      There is no error handling on the Frame objects for example
    - Think about how functional tests can be realized
    - Message names where the text in byte[] > 255, should cause an exception
      For UTF-32 this will be quite a bit faster than UTF-8 or even 16
  - Data transfer speeds:
    - with jumbo frames
    - with different communication media and messaging
  - Code coverage plugin for nunit/nsubstitute?
  - .NET Core instead of .NET Framework



********************
* RabbitMQ Router: *
********************

ToDo's:
  - RabbitMQ Sender/Reader queue recovery after connection lost

  - RabbitMQ: Add subscription service startup 
    At startup a request needs to be done on the Subscription main queue, the request will be to provide a list
    of all subscriptions. Once the list is filled in, the service may start listening on the exchange
    When a certain timeout expires assume it's the only running Subscription Service and start up with an empty list

  - Keep-Alive between IRouter on Consumer and IRouter on publisher
      these are the disconnected events that need implementing

  - Improve Queue names, allowing topic exchanges as the routing key can be used to pushto multiple queues:
      -> Mitto.Main
      -> Mitto.Publisher.<ID>
      -> Mitto.Consumer.<ID>

    - Add GetMessageStatus support so that "KeepAlives" for message actions work
      We can do this by switching GetMessageStatus to a message type like "Control"/"Management"
      When receiving a control request Mitto.RabbitMQ can convert it to a Control request message
      and use it's own control interface implementation to do w/e is required for that action

    - Each worker needs a queue that listens for broadcasts, can be the worker Queue already created
      just need to make sure Mitto can broadcast to for example Mitto.Consumer.*/Mitto.Publisher to 
      talk to the publishers or consumers.


    - Make the Queue prefix configurable, by default(currently hard coded) the prefix should be 'Mitto'
      Useful for when having multiple applications using Mitto, all parts can use the same RabbitMQ setup
      This way users do not have to set up a RabbitMQ instance/application that uses Mitto