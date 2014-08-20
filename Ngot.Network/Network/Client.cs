using Common.IO;
using Ngot.Core;
using Ngot.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ngot.Common
{
    public abstract class Client : IDisposable
    {

        private uint _bytesReceived;

        private static readonly BufferManager Buffers = BufferManager.Default;

        private readonly BigEndianReader m_buffer = new BigEndianReader();


        private uint _bytesSent;

        private static long _totalBytesReceived;

        private static long _totalBytesSent;

        public static long TotalBytesSent
        {
            get { return _totalBytesSent; }
        }

        public static long TotalBytesReceived
        {
            get { return _totalBytesReceived; }
        }

        private Ngot.Network.Messages m_currentMessage;

        protected BufferSegment _bufferSegment;

        protected int _offset, _remainingLength;

        public abstract bool DataArriavls(BufferSegment data);

        public Server _server
        {
            get;
            set;
        }
        public int sizeBuffer = 1024;

        private object m_lock = new object();

        public Socket Sock
        {
            get;
            private set;
        }


        protected Client(Socket _sock, Server server)
        {
            this.Sock = _sock;
            _server = server;
            _bufferSegment = Buffers.CheckOut();

        }

        public void Receive()
        {

            if (Sock != null || Sock.Connected)
            {

                var args = SocketHelpers.AcquireSocketArg();
                var offset = _offset + _remainingLength;

                args.SetBuffer(_bufferSegment.Buffer.Array, _bufferSegment.Offset + offset, sizeBuffer - offset);

                args.UserToken = this;
                args.Completed += ReceiveAsyncComplete;

                var willRaiseEvent = Sock.ReceiveAsync(args);

                if (!willRaiseEvent)
                {
                    ProcessRecieve(args);
                }
            }
        }

        private void ProcessRecieve(SocketAsyncEventArgs args)
        {

            try
            {
                var bytesReceived = args.BytesTransferred;

                if (args.BytesTransferred == 0)
                {
                    Console.WriteLine("Deconnexion utilisateur");
                    _server.Disconnect(this);
                }
                else
                {

                    unchecked
                    {
                        _bytesReceived += (uint)bytesReceived;
                    }

                    Interlocked.Add(ref _totalBytesReceived, bytesReceived);

                    _remainingLength += bytesReceived;

                    m_buffer.Add(_bufferSegment.SegmentData, 0, _bufferSegment.SegmentData.Length);

                    if (m_currentMessage == null)
                        m_currentMessage = new Ngot.Network.Messages();

                    if (m_currentMessage.Build(m_buffer))
                    {
                        var messages = new BigEndianReader(_bufferSegment.SegmentData);

                        try
                        {
                            Console.WriteLine("id : " + m_currentMessage.MessageId.Value);
                            Console.WriteLine("Content : " + messages);

                        }
                        catch
                        {

                        }

                        if (this.DataArriavls(_bufferSegment))
                        {
                            _offset = 0;
                            _bufferSegment.DecrementUsage();
                            _bufferSegment = Buffers.CheckOut();
                        }
                        else
                        {
                            EnsureBuffer();
                        }

                    }
                    this.Receive();
                }
            }
            catch (ObjectDisposedException)
            {

                _server.Disconnect(this);
            }
            catch (Exception)
            {

                _server.Disconnect(this);
            }
            finally
            {
                args.Completed -= ReceiveAsyncComplete;
                SocketHelpers.ReleaseSocketArg(args);
            }
        }

        protected void EnsureBuffer() //(int size)
        {
            //if (size > BufferSize - _offset)
            {
                // not enough space left in buffer: Copy to new buffer
                var newSegment = Buffers.CheckOut();
                Array.Copy(_bufferSegment.Buffer.Array,
                    _bufferSegment.Offset + _offset,
                    newSegment.Buffer.Array,
                    newSegment.Offset,
                    _remainingLength);
                _bufferSegment.DecrementUsage();
                _bufferSegment = newSegment;
                _offset = 0;
            }
        }

        private void ReceiveAsyncComplete(object sender, SocketAsyncEventArgs args)
        {
            ProcessRecieve(args);
        }


        public virtual void send(Message Message)
        {
            if (Sock != null && Sock.Connected)
            {
                byte[] data;
                using (var writer = new BigEndianWriter())
                {
                    Message.Pack(writer);
                    data = writer.Data;
                }

                var args = SocketHelpers.AcquireSocketArg();
                if (args != null)
                {
                    args.Completed += SendAsyncComplete;
                    args.UserToken = this;
                    args.SetBuffer(data, 0, data.Length);

                    Sock.SendAsync(args);
                    unchecked
                    {
                        _bytesSent += (uint)data.Length;
                    }

                    Interlocked.Add(ref _totalBytesSent, data.Length);
                }
                else
                {
                }
            }
        }

        private void SendAsyncComplete(object sender, SocketAsyncEventArgs args)
        {
            args.Completed -= SendAsyncComplete;
            SocketHelpers.ReleaseSocketArg(args);
        }

        public void Connect(string host, int port)
        {
            Connect(IPAddress.Parse(host), port);
        }


        public void Connect(IPAddress addr, int port)
        {
            if (Sock != null)
            {
                if (Sock.Connected)
                {
                    Sock.Disconnect(true);
                }
                Sock.Connect(addr, port);

                Receive();
            }
        }

        ~Client()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public virtual void Dispose(bool disposing)
        {
            if (Sock != null && Sock.Connected)
            {
                try
                {
                    Sock.Shutdown(SocketShutdown.Both);
                    Sock.Close();
                    Sock = null;
                }
                catch
                {
                }
            }
        }

    }
}
