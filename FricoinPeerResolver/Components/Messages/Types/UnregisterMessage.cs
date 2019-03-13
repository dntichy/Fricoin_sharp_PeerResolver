using System;
using System.Text;
using System.Xml;
using FricoinPeerResolver.Components.Interfaces;

namespace FricoinPeerResolver.Components.Messages.Types
{
    public class UnregisterMessage : IRegisterMessage
    {
        private CollaborativeClientDetails mClient;
        public CollaborativeClientDetails Client
        {
            get { return mClient; }
            set { mClient = value; }
        }

        private String mGroup;
        public String Group
        {
            get { return mGroup; }
            set { mGroup = value; }
        }

        public /*MessageType*/int Type
        {
            get { return (int)FricoinPeerResolver.Components.Enums.MessageType.UnregisterMessage; }
        }

        public Byte[] GetMessagePacket()
        {
            String textResult = "";
            textResult += "<message>";

            textResult += "<type>" + Type + "</type>";
            textResult += "<group>" + Group + "</group>";
            textResult += "<clientdetails><name>" + mClient.ClientName + "</name><ipaddress>" + mClient.ClientIPAddress + "</ipaddress><listenport>" + mClient.ClientListenPort + "</listenport></clientdetails>";

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

            FricoinPeerResolver.Components.Enums.MessageType type = FricoinPeerResolver.Components.Enums.MessageType.EmptyMessage;

            XmlElement messageElement = xmlDoc.DocumentElement;
            if (messageElement.Name == "message")
            {
                foreach (XmlNode node in messageElement.ChildNodes)
                {
                    if (node.Name == "type")
                    {
                        type = (FricoinPeerResolver.Components.Enums.MessageType)Enum.Parse(typeof(FricoinPeerResolver.Components.Enums.MessageType), node.InnerText);
                        break;
                    }
                }
            }

            if (type != FricoinPeerResolver.Components.Enums.MessageType.UnregisterMessage)
            {
                System.Diagnostics.Debug.WriteLine("The supplied data was the wrong message type!");
                return false;
            }

            //////////////////////////////////////////////////////////////////////////
            // The real data parsing
            this.mGroup = "";
            this.mClient = null;
            this.mClient = new CollaborativeClientDetails();

            foreach (XmlNode node in messageElement.ChildNodes)
            {
                if (node.Name == "group")
                {
                    this.mGroup = node.InnerText;
                }
                else if (node.Name == "clientdetails")
                {
                    foreach (XmlNode detailsNode in node.ChildNodes)
                    {
                        if (detailsNode.Name == "name")
                        {
                            this.mClient.ClientName = detailsNode.InnerText;
                        }
                        else if (detailsNode.Name == "ipaddress")
                        {
                            this.mClient.ClientIPAddress = detailsNode.InnerText;
                        }
                        else if (detailsNode.Name == "listenport")
                        {
                            this.mClient.ClientListenPort = int.Parse(detailsNode.InnerText);
                        }
                    }
                }
            }

            //////////////////////////////////////////////////////////////////////////
            return true;
        }

        public IMessage Clone()
        {
            var clone = new UnregisterMessage();
            clone.Group = Group;
            clone.Client = Client;

            return clone;
        }
    }
}