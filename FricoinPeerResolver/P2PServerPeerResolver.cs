using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using System.Net.Sockets;
using FricoinPeerResolver.Components;
using FricoinPeerResolver.Components.Enums;
using FricoinPeerResolver.Components.Events;
using FricoinPeerResolver.Components.Interfaces;
using FricoinPeerResolver.Components.Messages;
using FricoinPeerResolver.Components.Messages.Types;


namespace FricoinPeerResolver
{
    public class P2PServerPeerResolver : IServer
    {
        public enum ServerStates
        {
            ServerStopped,
            ServerRunning,
            ServerPaused,
        }


        private Dictionary<String, Collection<ICollaborativeClientDetails>> mClientDetailsGroups;

        public Dictionary<String, Collection<ICollaborativeClientDetails>> ClientDetailsGroups
        {
            get { return mClientDetailsGroups; }
        }

        private Socket mListenerSocket;

        private ServerStates mServerState;
        public ServerStates ServerState
        {
            get { return mServerState; }
        }



        private event ServerRegisterEvent mOnRegisterClient;
        public event ServerRegisterEvent OnRegisterClient
        {
            add { mOnRegisterClient += value; }
            remove { mOnRegisterClient -= value; }
        }

        private int mListenPort;    
        public int ListenPort
        {
            get { return mListenPort; }
            set { mListenPort = value; }
        }

        private IMessageParserEngine mMessageParser;
        public IMessageParserEngine MessageParser
        {
            get { return mMessageParser; }
        }


        public P2PServerPeerResolver(int listenPort)
        {
            mListenPort = listenPort;
            mClientDetailsGroups = new Dictionary<string, Collection<ICollaborativeClientDetails>>();
            mMessageParser = new MessageParserEngineClass();
            Initialize();
        }

        public void StopServer()
        {
            if (mServerState == ServerStates.ServerRunning || mServerState == ServerStates.ServerPaused)
            {
                mServerState = ServerStates.ServerStopped;
                if (mListenerSocket != null)
                {
                    mListenerSocket.Close();
                    mListenerSocket = null;
                }

                mClientDetailsGroups = null;
                mClientDetailsGroups = new Dictionary<string, Collection<ICollaborativeClientDetails>>();
            }
        }

        public void PauseServer()
        {
            if (mServerState == ServerStates.ServerPaused)
            {
                mServerState = ServerStates.ServerRunning;
                Initialize();
            }
            else
            {
                mServerState = ServerStates.ServerPaused;
                if (mListenerSocket != null)
                {
                    mListenerSocket.Close();
                    mListenerSocket = null;
                }
            }
        }

        public void StartServer()
        {
            if (mServerState == ServerStates.ServerPaused || mServerState == ServerStates.ServerStopped)
            {
                mServerState = ServerStates.ServerRunning;
                Initialize();
            }
        }

        public InitState Initialize()
        {
            if (mListenPort <= 0 || mListenPort >= 65536) return InitState.InvalidListenPort;

            IPHostEntry hostEntry = Dns.GetHostEntry(Dns.GetHostName()); //Dns.Resolve("localhost").AddressList[0];

            if (hostEntry.AddressList.Length <= 0) return InitState.ErrorNoAvailableIPAddress;

            IPAddress localAddress = null;

            for (int i = 0; i < hostEntry.AddressList.Length; ++i)
            {
                if (hostEntry.AddressList[i].AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    localAddress = hostEntry.AddressList[i];
            }


            if (localAddress == null)
                return InitState.ErrorNoAvailableIPAddress;

            try
            {
                //create a listening socket
                if (mListenerSocket == null)
                    mListenerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPEndPoint localIP = new IPEndPoint(localAddress /*IPAddress.Any*/, mListenPort);
                mListenerSocket.Bind(localIP);
                mListenerSocket.Listen(50); //TODO -> ponder which is the best value to use here
                mListenerSocket.BeginAccept(new AsyncCallback(OnHandleClientConnection), null);
            }
            catch (SocketException ex)
            {
                System.Diagnostics.Debug.WriteLine("Unable to create the socket : " + ex.Message);
            }
            catch (ObjectDisposedException ex)
            {
                System.Diagnostics.Debug.WriteLine("The socket was forcefully closed : " + ex.Message);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }

            mServerState = ServerStates.ServerRunning;

            return InitState.InitOK;
        }

        //todo
        protected void RegisterClient(String group, ICollaborativeClientDetails client)
        {
            if (!mClientDetailsGroups.ContainsKey(group))
                mClientDetailsGroups[group] = new Collection<ICollaborativeClientDetails>();
            mClientDetailsGroups[group].Add(client);
        }

        protected void OnHandleClientConnection(IAsyncResult asyncResult)
        {
            try
            {
                Socket workerSocket = mListenerSocket.EndAccept(asyncResult);

                try
                {
                    TxRxPacket dataStatus = new TxRxPacket(workerSocket);
                    workerSocket.BeginReceive(dataStatus.mDataBuffer, 0, dataStatus.mDataBuffer.Length,
                        SocketFlags.None, new AsyncCallback(OnHandleClientData), dataStatus);
                }
                catch (SocketException ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                }

                mListenerSocket.BeginAccept(new AsyncCallback(OnHandleClientConnection), null);
            }
            catch (ObjectDisposedException ex)
            {
                System.Diagnostics.Debug.WriteLine("Socket has been closed : " + ex.Message);
            }
            catch (SocketException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }

        protected void OnHandleClientData(IAsyncResult asyncResult)
        {
            try
            {
                TxRxPacket dataStatus = (TxRxPacket) asyncResult.AsyncState;

                int countRx = dataStatus.mCurrentSocket.EndReceive(asyncResult);

                /*
                String rxMsgText = Encoding.UTF8.GetString(dataStatus.mDataBuffer, 0, countRx);

                //parse the message
                IMessage rxMessage = ParseMessage(rxMsgText);
                */

                IMessage rxMessage = mMessageParser.ParseMessage(dataStatus.mDataBuffer);

                //handle the message (which can either be register or unregister)
                //send response message if needed : workerSocket.Send(byData);
                switch (rxMessage.Type)
                {
                    case ((int) MessageType.RegisterMessage):
                    {
                        if (!mClientDetailsGroups.ContainsKey(((RegisterMessage) rxMessage).Group))
                            mClientDetailsGroups[((RegisterMessage) rxMessage).Group] = new Collection<ICollaborativeClientDetails>();


                        if (mClientDetailsGroups[((RegisterMessage) rxMessage).Group].IndexOf(((RegisterMessage) rxMessage).Client) >= 0)
                            mClientDetailsGroups[((RegisterMessage) rxMessage).Group].Remove(((RegisterMessage) rxMessage).Client);

                        //Socket workerSocket = (Socket)dataStatus.mCurrentSocket;
                        //respond with the current group in the message

                        RegisteredClientsListMessage response = new RegisteredClientsListMessage();
                        response.Clients = mClientDetailsGroups[((RegisterMessage) rxMessage).Group];
                        response.Group = ((RegisterMessage) rxMessage).Group;

                        //create a socket connection to the newly added client listener port
                        Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                        IPAddress remoteMachine = IPAddress.Parse(((RegisterMessage) rxMessage).Client.ClientIPAddress);
                        IPEndPoint remoteEndpoint = new IPEndPoint(remoteMachine,((RegisterMessage) rxMessage).Client.ClientListenPort);
                        clientSocket.Connect(remoteEndpoint);

                        if (clientSocket.Connected)
                        {
                            Console.WriteLine("Client connected " +
                                              ((RegisterMessage) rxMessage).Client.ClientIPAddress + ":" +
                                              ((RegisterMessage) rxMessage).Client.ClientListenPort);
                        }

                        clientSocket.Send(response.GetMessagePacket());
                        clientSocket.Close(1); //just a minor timeout to be sure the message got there
                        //the socket just lost the purpose of ever existing

                        mClientDetailsGroups[((RegisterMessage) rxMessage).Group].Add(((RegisterMessage) rxMessage).Client);

                        if (mOnRegisterClient != null)
                            mOnRegisterClient.Invoke(this, new ServerRegisterEventArgs(((RegisterMessage) rxMessage).Client));
                        break;
                    }
                    case ((int) MessageType.UnregisterMessage):
                    {
                            
                        mClientDetailsGroups[((UnregisterMessage) rxMessage).Group]
                            .Remove(((UnregisterMessage) rxMessage).Client);
                        //do not contact all others in the same group... have the client take care of that
                        if (mOnRegisterClient != null)
                            mOnRegisterClient.Invoke(this,
                                new ServerRegisterEventArgs(((UnregisterMessage) rxMessage).Client));


                        Console.WriteLine("Client "+ ((UnregisterMessage)rxMessage).Client.ClientIPAddress+ ((UnregisterMessage)rxMessage).Client.ClientListenPort + " unregistered");
                        break;
                    }
                }
            }
            catch (ObjectDisposedException ex)
            {
                System.Diagnostics.Debug.WriteLine("Socket has been closed : " + ex.Message);
            }
            catch (SocketException ex)
            {
                if (ex.ErrorCode == 10054) // Error code for Connection reset by peer
                {
                    System.Diagnostics.Debug.WriteLine("Connection reset by peer : " + ex.Message);
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                }
            }
        }
    }
}