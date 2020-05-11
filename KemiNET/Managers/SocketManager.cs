using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using KemiNETShared;
using Newtonsoft.Json;
using Serilog;

namespace KemiNET.Managers
{
    public class SocketManager
    {
        private Socket handlerSocket;
        private Action<string> actionCallback;
        // Thread signal.  
        public ManualResetEvent processSignalState = new ManualResetEvent(false);

        public void StartListening(Action<string> callback)
        {
            actionCallback = callback;
            // Establish the local endpoint for the socket.  
            IPHostEntry ipHostInfo = Dns.GetHostEntry(Constants.DefaultHostname);
            IPAddress ipAddress = ipHostInfo.AddressList[0];
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, Constants.DefaultPort);

            // Create a TCP/IP socket.  
            Socket listenerSocket = new Socket(ipAddress.AddressFamily,
                SocketType.Stream, ProtocolType.Tcp);

            // Bind the socket to the local endpoint and listen for incoming connections.  
            try
            {
                listenerSocket.Bind(localEndPoint);
                listenerSocket.Listen(100);

                while (true)
                {
                    // Set the event to non-signaled state.  
                    processSignalState.Reset();

                    // Start an asynchronous socket to listen for connections.  
                    Log.Information("Waiting for a connection...");
                    listenerSocket.BeginAccept(
                        AcceptCallback,
                        listenerSocket);

                    // Wait until a connection is made before continuing.  
                    processSignalState.WaitOne();
                }

            }
            catch (SocketException e)
            {
                if (e.SocketErrorCode != SocketError.AddressAlreadyInUse) throw;
                
                Log.Error(e.Message);
                Log.Error($"Shutting down server instance with PID {Process.GetCurrentProcess().Id}");
                Environment.Exit(65);
            }
            catch (Exception e)
            {
                Log.Information(e.ToString());
            }
        }

        public void StopListening()
        {
            //
        }

        public void AcceptCallback(IAsyncResult ar)
        {
            // Signal the main thread to continue.  
            processSignalState.Set();

            // Get the socket that handles the client request.  
            Socket listener = (Socket) ar.AsyncState;
            Socket handler = listener.EndAccept(ar);

            // Create the state object.  
            StateObject state = new StateObject();
            state.openSocket = handler;
            handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                ReadCallback, state);
        }

        public void ReadCallback(IAsyncResult ar)
        {
            String content = String.Empty;

            // Retrieve the state object and the handler socket  
            // from the asynchronous state object.  
            StateObject socketState = (StateObject) ar.AsyncState; 
            handlerSocket = socketState.openSocket;

            // Read data from the client socket.   
            int bytesRead = handlerSocket.EndReceive(ar);

            if (bytesRead > 0)
            {
                // There  might be more data, so store the data received so far.  
                socketState.builtString.Append(Encoding.ASCII.GetString(
                    socketState.buffer, 0, bytesRead));

                // Check for end-of-file tag. If it is not there, read   
                // more data.  
                content = socketState.builtString.ToString();
                if (handlerSocket.Available < 1)
                {
                    // All the data has been read from the   
                    // client. Display it on the console.  
                    Log.Information("Read {0} bytes from socket.",
                        content.Length);
                    // Echo the data back to the client.
                    actionCallback(content);
                    //SendResponse(handlerSocket, content);
                    /*handlerSocket.Shutdown(SocketShutdown.Both);
                    handlerSocket.Close();*/
                }
                else
                {
                    // Not all data received. Get more.  
                    handlerSocket.BeginReceive(socketState.buffer, 0, StateObject.BufferSize, 0,
                        ReadCallback, socketState);
                }
            }
        }

        public void SendResponse(object data)
        {
            // Convert the string data to byte data using ASCII encoding.  
            byte[] byteData = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(data));

            // Begin sending the data to the remote device.  
            handlerSocket.BeginSend(byteData, 0, byteData.Length, 0,
                SendCallback, handlerSocket);
        }

        private static void SendCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.  
                Socket handler = (Socket) ar.AsyncState;

                // Complete sending the data to the remote device.  
                int bytesSent = handler.EndSend(ar);
                Log.Information("Sent {0} bytes to client.", bytesSent);

                handler.Shutdown(SocketShutdown.Both);
                handler.Close();

            }
            catch (Exception e)
            {
                Log.Information(e.ToString());
            }
        }
    }




    // State object for reading client data asynchronously  
    public class StateObject {  
        // Client  socket.  
        public Socket openSocket;  
        // Size of receive buffer.  
        public const int BufferSize = 1024;  
        // Receive buffer.  
        public byte[] buffer = new byte[BufferSize];  
        // Received data string.  
        public StringBuilder builtString = new StringBuilder();    
    }  
}