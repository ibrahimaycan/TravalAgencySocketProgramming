using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Client
{
    class Client
    {
        static Socket socket;

        static void Main(string[] args)
        {
            Console.WriteLine("This is Client");
            socket = new Socket(AddressFamily.InterNetwork,SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"),1234);
            try
            {
                socket.Connect(localEndPoint);

            }
            catch
            {
                Console.WriteLine("Unable to Connect");
                Main(args);
            }
            int i = 0;
            while (i<3) {
                Console.WriteLine("Enter Preferred Hotel");
                string hotelName = Console.ReadLine();
                byte[] hotelNamedata = Encoding.ASCII.GetBytes(hotelName);

                Console.WriteLine("Enter Preferred Airline");
                string AirlineName = Console.ReadLine();
                byte[] airlineNameData = Encoding.ASCII.GetBytes(AirlineName);

                Console.WriteLine("Enter Date in format ddmmyyyy");
                string dateTime = Console.ReadLine();
                byte[] dateTimeData = Encoding.ASCII.GetBytes(dateTime);

                Console.WriteLine("Enter number of Customer");
                string customerNumber = Console.ReadLine();
                byte[] data = Encoding.ASCII.GetBytes(hotelName + " " + AirlineName +
                    " " + dateTime + " " + customerNumber);

                socket.Send(data);
                Console.Write("data send\r\n");
                i++;
            }
            Console.Read();
            socket.Close();

                
            
        }
    }
}
