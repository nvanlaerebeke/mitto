ToDo's before v1:


- Fix rabbitmq + write tests for it

- Add logging
- Add bandwidth limiting & monitoring to WebSocketSharp
- Make max frame size configurable, default 1kb, should be able to grow to 500kb
- add timetstamps to requests & responses

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