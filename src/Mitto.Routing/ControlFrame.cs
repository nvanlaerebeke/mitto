using Mitto.Routing;
using System;
using System.Linq;
using System.Text;

namespace Mitto.IRouting {

    public class ControlFrame {
        private byte[] _arrByteArray;

        public ControlFrame(byte[] data) {
            _arrByteArray = data;
        }

        public ControlFrame(MessageType pType, string pMessageName, string pRequestID, byte[] data) {
            var arrID = Encoding.ASCII.GetBytes(pRequestID);
            var arrMessageName = Encoding.UTF8.GetBytes(pMessageName);

            _arrByteArray = new byte[1 + 1 + arrID.Length + 1 + arrMessageName.Length + data.Length];
            _arrByteArray[0] = (byte)pType;
            _arrByteArray[1] = (byte)arrID.Length;
            Array.Copy(arrID, 0, _arrByteArray, 1 + 1, arrID.Length);
            _arrByteArray[1 + 1 + arrID.Length] = (byte)arrMessageName.Length;
            Array.Copy(arrMessageName, 0, _arrByteArray, (1 + 1 + arrID.Length + 1), arrMessageName.Length);
            Array.Copy(data, 0, _arrByteArray, 1 + 1 + arrID.Length + 1 + arrMessageName.Length, data.Length);
        }

        public MessageType FrameType {
            get {
                return (MessageType)_arrByteArray.ElementAt(0);
            }
        }

        public string RequestID {
            get {
                var newArray = new byte[_arrByteArray[1]];
                Array.Copy(_arrByteArray, 2, newArray, 0, newArray.Length);
                return Encoding.ASCII.GetString(newArray);
            }
        }

        public string MessageName {
            get {
                var newArray = new byte[_arrByteArray.ElementAt(1 + 1 + _arrByteArray.ElementAt(1))];
                Array.Copy(
                    _arrByteArray,
                    1 + 1 + _arrByteArray.ElementAt(1) + 1,
                    newArray,
                    0,
                    newArray.Length
                );
                return Encoding.UTF8.GetString(newArray);
            }
        }

        public byte[] Data {
            get {
                var newArray = new byte[(
                    _arrByteArray.Length -
                    (
                        1 +
                        1 +
                        _arrByteArray.ElementAt(1) +
                        1 +
                        _arrByteArray.ElementAt(1 + 1 + _arrByteArray.ElementAt(1))
                    )
                )];
                var startIndex = (
                        1 +
                        1 +
                        _arrByteArray.ElementAt(1) +
                        1 +
                        _arrByteArray.ElementAt(1 + 1 + _arrByteArray.ElementAt(1))
                    );
                Array.Copy(
                    _arrByteArray,
                    startIndex,
                    newArray,
                    0,
                    newArray.Length
                );
                return newArray;
            }
        }

        public byte[] GetBytes() {
            return _arrByteArray;
        }
    }
}