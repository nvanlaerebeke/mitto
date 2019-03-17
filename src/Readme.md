Minimum ToDo's:
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
	- Improve MessageProvider so that it takes in all the messages in the entire application based on the 
	  interfaces/base classes it implements, like done for the ControlProvider
	  This will prevent the need for passing the namespace
	  Still need to keep the option of the namespaces due to priorities etc
	  By default overwrite the Mitto.* classes with the application it's classes

	- Move Control messages away from IMessaging and implement them outside of it
	  Move to the Router, use ControlFrame just as in RabbitMQ implementation, implementation 
	  should be something router specific as only it knows how to get the status or who or what
	  to pass the request onto

    - implement compression for data that is transfered using IConnection or Json
	    Prevent double compression from Json && IConnection
		On what layer should compression be implemented?, test difference between 
		compressing the json text vs compressing the UTF32.GetBytes byte[]


	- Add a version to the Frame's being sent - can be useful for backwards compatibility when making changes 
	  to the way data is being encapsulated, can be taken from the AssemblyVersion

	- Autoscale ThreadPool.MinThreads so the application ThreadPool autoscales in time

	- don't allow  names where the byte[] for the strings > 255, this will cause exceptions

	- RabbitMQ: 
		- keepalives between IRouter on Consumer and IRouter on publisher
		  these are the disconnected events that need implementing

	    - Improve Queue names, allowing topic exchanges as the routing key can be used to pushto multiple queues:
		    - Mitto.Main
			- Mitto.Publisher.<ID>
			- Mitto.Consumer.<ID>
		
		- Add GetMessageStatus support so that "KeepAlives" for message actions work
		  We can do this by switching GetMessageStatus to a message type like "Control"/"Management"
		  When receiving a control request Mitto.RabbitMQ can convert it to a Control request message
		  and use it's own control interface implementation to do w/e is required for that action
		  

		- Each worker needs a queue that listens for broadcasts, can be the worker Queue already created
		  just need to make sure Mitto can broadcast to for example Mitto.Consumer.*/Mitto.Publisher to 
		  talk to the publishers or consumers.
		
		- Add cleanup service that makes sure all queues are still in use and cleans up 
		  those that are not relevant anymore
		  This can be done by running workers, @ startup the workers can do a broadcast to find out 
		  who the 'broker' is, when no one anwsers, a negotiation can be started between all 
		  workers to promote a single worker to a 'broker'. Also when nothing is received from a 
		  broker for x seconds a new negotiation can be started by any worker to promote one
		  

		- Support subscriptions, two idea's to get this working
		  
		  1. Subscribes are broadcasted, meaning each worker knows all subscriptions
		     This has the advantage that subscribes are never lost, the node can go down and nothing is missed
			 do need to a startup for the subscription stuff as when we go online and there are already workers running
			 than a list is needed with all current subscriptions

		  2. Notify events are broadcasted
		     This has the upside that not all workers need a list of all subscriptions, the downside is that
			 the list of subscribed connections (Connection + Consumer + Event etc) needs to be  stored somewhere (a DB?)
			 because then Mitto can't afford to go down without restoring it's state.

		  Think 1. wins out due to the subscriptions being lightweight and being much easier to implement, a simple
		  broadcast @ startup to fetch the subscription list is enough to be up and running again. 

		- Make the Queue prefix configurable, by default(currently hardcoded) the prefix should be 'Mitto'
		  Useful for when having multiple applications using Mitto, all apps can use the same RabbitMQ setup
		  This way users do not have to set up a RabbitMQ instance/application that uses Mitto

		- Add a status page (WebPage) that can be viewed when enabled on port x 

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