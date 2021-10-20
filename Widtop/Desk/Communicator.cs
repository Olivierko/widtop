// ReSharper disable InconsistentNaming
using System;
using System.Collections.Generic;
using System.IO.Ports;

namespace Widtop.Desk
{
    public delegate void MessageReceived(MessageType type, byte value);

    public class Communicator
    {
        private const string PORT_NAME = "COM5";
        private const int BAUD_RATE = 9600;

        private const byte MSG_START_MARK = 0x11;
        private const byte MSG_END_MARK = 0x12;

        private readonly Queue<byte> _messageQueue = new Queue<byte>();

        public event MessageReceived OnMessageReceived;

        private SerialPort _serialPort;
        private MessagePosition _currentMessagePosition = MessagePosition.None;
        private MessageType _currentMessageType;
        private byte _currentMessageValue;

        private void OnDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            var serialPort = (SerialPort)sender;

            var data = new byte[serialPort.BytesToRead];
            serialPort.Read(data, 0, data.Length);

            lock (_messageQueue)
            {
                for (var index = 0; index < data.Length; index++)
                {
                    _messageQueue.Enqueue(data[index]);
                }
            }

            Process();
        }

        private static bool TryParseMessageType(byte value, out MessageType result)
        {
            if (!Enum.IsDefined(typeof(MessageType), value))
            {
                result = MessageType.NONE;
                return false;
            }

            result = (MessageType)value;
            return true;
        }

        private void DequeueOne()
        {
            byte message;
            lock (_messageQueue)
            {
                if (_messageQueue.Count == 0)
                {
                    return;
                }

                message = _messageQueue.Dequeue();
            }

            switch (_currentMessagePosition)
            {
                case MessagePosition.None:
                    _currentMessagePosition = message == MSG_START_MARK ? MessagePosition.Start : MessagePosition.None;
                    break;
                case MessagePosition.Start:
                    _currentMessagePosition = TryParseMessageType(message, out _currentMessageType) ? MessagePosition.Type : MessagePosition.None;
                    break;
                case MessagePosition.Type:
                    _currentMessageValue = message;
                    _currentMessagePosition = MessagePosition.Value;
                    break;
                case MessagePosition.Value:
                    _currentMessagePosition = message == MSG_END_MARK ? MessagePosition.End : MessagePosition.None;
                    break;
                case MessagePosition.End:
                    throw new Exception("End of message wasn't processed.");
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void CheckMessage()
        {
            if (_currentMessagePosition != MessagePosition.End)
            {
                return;
            }

            OnMessageReceived?.Invoke(_currentMessageType, _currentMessageValue);

            _currentMessagePosition = MessagePosition.None;
            _currentMessageType = MessageType.NONE;
            _currentMessageValue = 0x00;
        }

        private bool TryEnsureConnection()
        {
            if (_serialPort == null)
            {
                _serialPort = new SerialPort(PORT_NAME, BAUD_RATE);
            }

            if (_serialPort.IsOpen)
            {
                return true;
            }

            try
            {
                _serialPort.Open();
            }
            catch
            {
                // ignored
            }

            return _serialPort.IsOpen;
        }

        public void Start()
        {
            _serialPort = new SerialPort(PORT_NAME, BAUD_RATE);
            _serialPort.DataReceived += OnDataReceived;
            TryEnsureConnection();
        }

        public void Write(MessageType type)
        {
            if (!TryEnsureConnection())
            {
                return;
            }

            var bytes = new[] { (byte)type };
            _serialPort.Write(bytes, 0, 1);
        }

        public void Process()
        {
            lock (_messageQueue)
            {
                while (_messageQueue.Count > 0)
                {
                    DequeueOne();
                    CheckMessage();
                }
            }
        }

        public void Stop()
        {
            if (!_serialPort.IsOpen)
            {
                return;
            }

            _serialPort.Close();
        }
    }
}
