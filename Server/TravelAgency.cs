using Client;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Server
{
    class TravelAgency
    {
        static byte[] buffer { get; set; }
        static Socket socket;
        static Socket Airplane_socket;
        static Socket Hotel_socket;

        static void Main(string[] args)
        {
            Console.WriteLine("This is Travel Agency Server");
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Bind(new IPEndPoint(0, 1234));
            socket.Listen(100);
            Socket accepted = socket.Accept();

            //Airplane Socket connection
            Airplane_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 3000);
            try
            {
                Airplane_socket.Connect(localEndPoint);

            }
            catch
            {
                Console.WriteLine("Unable to Connect");
                Main(args);
            }

            //Hotel socket connection
            Hotel_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            localEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 2000);
            try
            {
                Hotel_socket.Connect(localEndPoint);

            }
            catch
            {
                Console.WriteLine("Unable to Connect");
                Main(args);
            }




            Customer_Info customer_Info = null;
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
                customer_Info = new Customer_Info(strData);
                
                string[] parameters = strData.Split(' ');
                Airplane_socket.Send(Encoding.ASCII.GetBytes(customer_Info.preferedAirline+" "+
                    customer_Info.Date+" "+ customer_Info.peopleCount));
                Hotel_socket.Send(Encoding.ASCII.GetBytes(customer_Info.preferedHotel + " " +
                    customer_Info.Date + " " + customer_Info.peopleCount));
                Console.WriteLine(strData);
                
                j++;
            }
            socket.Close();
            accepted.Close();


        }

    }



}
