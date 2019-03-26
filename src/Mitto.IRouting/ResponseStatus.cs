/// <summary>
/// ToDo: move back to IMessaging
/// </summary>
namespace Mitto.IRouting {
	public class ResponseStatus {
		public ResponseState State { get; set; } = ResponseState.Success;
		public int ErrorCode { get; set; } = 0;
		public string Message { get; set; } = "";

		public ResponseStatus() { }

		public ResponseStatus(ResponseState pState) {
			State = pState;
		}

		public ResponseStatus(ResponseState pState, int pErrorCode) {
			State = pState;
			ErrorCode = pErrorCode;
		}

		public ResponseStatus(ResponseState pState, int pErrorCode, string pMessage) {
			State = pState;
			ErrorCode = pErrorCode;
			Message = pMessage;
		}

		public ResponseStatus(ResponseState pState, string pMessage) {
			State = pState;
			Message = pMessage;
		}
	}
}