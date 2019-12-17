using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Server
{
    class Server
    {
        static byte[] buffer { get; set; }
        static Socket socket;

        static void Main(string[] args)
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Bind(new IPEndPoint(0, 1234));
            socket.Listen(100);
            Socket accepted = socket.Accept();

            int j = 0;
            while (j < 3) {

 
                buffer = new byte[accepted.SendBufferSize];
                int bytesRead = accepted.Receive(buffer);
                byte[] formatted = new byte[bytesRead];
                for (int i = 0; i < bytesRead; i++)
                {
                    formatted[i] = buffer[i];

                }
                string strData = Encoding.ASCII.GetString(formatted);
                Console.WriteLine(strData);
                j++;
            }
            socket.Close();
            accepted.Close();


        }
    }
}
