using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Airplane
{
    class Airplane
    {
        static Socket socket;
        static byte[] buffer { get; set; }
        static void Main(string[] args)
        {
            Console.WriteLine("This is Airplane Server");
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Bind(new IPEndPoint(0, 3000));
            socket.Listen(100);
            Socket accepted = socket.Accept();
            int j = 0;
            while (j < 3)
            {


                buffer = new byte[accepted.SendBufferSize];
                int bytesRead = accepted.Receive(buffer);
                byte[] formatted = new byte[bytesRead];
                for (int i = 0; i < bytesRead; i++)
                {
                    formatted[i] = buffer[i];

                }
                string strData = Encoding.ASCII.GetString(formatted);
                string[] parameters = strData.Split(' ');
                //parameters[0] company,parameters[1] date, parameters[0] count
                Console.WriteLine(strData);
                j++;
            }
            socket.Close();
            accepted.Close();



        }
    }
}
