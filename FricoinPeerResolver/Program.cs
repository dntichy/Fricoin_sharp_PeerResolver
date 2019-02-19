using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FricoinPeerResolver
{
    class Program
    {

        private static P2PServerPeerResolver mRepositoryServer;
        static void Main(string[] args)
        {
            String hostName = System.Net.Dns.GetHostName();
            System.Net.IPHostEntry hostEntry = System.Net.Dns.GetHostEntry(hostName);
            for (int i = 0; i < hostEntry.AddressList.Length; ++i)
            {
                if (hostEntry.AddressList[i].AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    Console.WriteLine(hostEntry.AddressList[i].ToString());
            }


            try
            {
                mRepositoryServer = new P2PServerPeerResolver(46800);
      
            }
            catch (Exception ex)
            {
                
                mRepositoryServer = null;
                
            }


            Console.Read();
        }
    }
}