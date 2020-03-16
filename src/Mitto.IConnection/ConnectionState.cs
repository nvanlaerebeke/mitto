namespace Mitto.IConnection {

    public enum ConnectionState {

        //Reserved for future use.
        None = 0,

        //The connection is negotiating the handshake with the remote endpoint.
        Connecting = 1,

        //The initial state after the HTTP handshake has been completed.
        Open = 2,

        //A close message was sent to the remote endpoint.
        CloseSent = 3,

        //A close message was received from the remote endpoint.
        CloseReceived = 4,

        //Indicates the WebSocket close handshake completed gracefully.
        Closed = 5,

        //Reserved for future use.
        Aborted = 6
    }
}