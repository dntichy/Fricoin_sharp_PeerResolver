﻿using System;
using System.Collections.ObjectModel;
using System.Text;
using System.Xml;
using FricoinPeerResolver.Components.Interfaces;
using MessageType = FricoinPeerResolver.Components.Enums.MessageType;

namespace FricoinPeerResolver.Components.Messages.Types
{
    public class RegisteredClientsListMessage : IMessage
    {
        public /*MessageType*/int Type
        {
            get { return (int)MessageType.ResgisteredClientsListMessage; }
        }

        private Collection<ICollaborativeClientDetails> mClients;
        public Collection<ICollaborativeClientDetails> Clients
        {
            get { return mClients; }
            set { mClients = value; }
        }

        private String mGroup;
        public String Group
        {
            get { return mGroup; }
            set { mGroup = value; }
        }

        public Byte[] GetMessagePacket()
        {
            String textResult = "";
            textResult += "<message>";

            textResult += "<type>" + Type + "</type>";
            textResult += "<group>" + Group + "</group>";
            textResult += "<clientslist>";
            foreach (ICollaborativeClientDetails CurrentClient in mClients)
            {
                textResult += "<clientdetails><name>" + CurrentClient.ClientName + "</name><ipaddress>" + CurrentClient.ClientIPAddress + "</ipaddress><listenport>" + CurrentClient.ClientListenPort + "</listenport></clientdetails>";
            }
            textResult += "</clientslist>";

            textResult += "</message>";

            return ASCIIEncoding.UTF8.GetBytes(textResult);
        }

        public bool Parse(Byte[] data)
        {
            //parse the message from "incoming data packet"
            String messageContent = Encoding.UTF8.GetString(data);
            //////////////////////////////////////////////////////////////////////////
            //Data validation
            XmlDocument xmlDoc = new XmlDocument();
            try
            {
                xmlDoc.LoadXml(messageContent);
            }
            catch (XmlException ex)
            {
                System.Diagnostics.Debug.WriteLine("There was an xml parsing error in the received message : " + ex.Message);
                return false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("There was an error in the received message : " + ex.Message);
                return false;
            }

            MessageType type = MessageType.EmptyMessage;

            XmlElement messageElement = xmlDoc.DocumentElement;
            if (messageElement.Name == "message")
            {
                foreach (XmlNode node in messageElement.ChildNodes)
                {
                    if (node.Name == "type")
                    {
                        type = (MessageType)Enum.Parse(typeof(MessageType), node.InnerText);
                        break;
                    }
                }
            }

            if (type != MessageType.ResgisteredClientsListMessage)
            {
                System.Diagnostics.Debug.WriteLine("The supplied data was the wrong message type!");
                return false;
            }

            //////////////////////////////////////////////////////////////////////////
            // The real data parsing
            this.mGroup = "";
            this.mClients = null;

            foreach (XmlNode node in messageElement.ChildNodes)
            {
                if (node.Name == "group")
                {
                    this.mGroup = node.InnerText;
                }
                else if (node.Name == "clientslist")
                {
                    this.mClients = new Collection<ICollaborativeClientDetails>();

                    foreach (XmlNode clientNode in node.ChildNodes)
                    {
                        if (clientNode.Name == "clientdetails")
                        {
                            CollaborativeClientDetails currClient = new CollaborativeClientDetails();
                            foreach (XmlNode detailsNode in clientNode.ChildNodes)
                            {
                                if (detailsNode.Name == "name")
                                {
                                    currClient.ClientName = detailsNode.InnerText;
                                }
                                else if (detailsNode.Name == "ipaddress")
                                {
                                    currClient.ClientIPAddress = detailsNode.InnerText;
                                }
                                else if (detailsNode.Name == "listenport")
                                {
                                    currClient.ClientListenPort = int.Parse(detailsNode.InnerText);
                                }
                            }

                            if (currClient.ClientIPAddress.Length > 0 && currClient.ClientListenPort > -1)
                                this.mClients.Add(currClient);
                        }
                    }
                }
            }

            //////////////////////////////////////////////////////////////////////////
            return true;
        }

        public IMessage Clone()
        {
            RegisteredClientsListMessage result = new RegisteredClientsListMessage();
            result.Group = this.mGroup;
            result.Clients = new Collection<ICollaborativeClientDetails>();
            foreach (var collaborativeClientDetails in mClients)
            {
                var currClient = (CollaborativeClientDetails) collaborativeClientDetails;
                var newClient = new CollaborativeClientDetails();
                newClient.ClientIPAddress = currClient.ClientIPAddress;
                newClient.ClientListenPort = currClient.ClientListenPort;
                result.Clients.Add(newClient);
            }
            return result;
        }
    }
}