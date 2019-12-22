using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace SocketProgramming.Client
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
            byte[] selectChoice = new byte[100];
            while (i<100) {
                i++;
                //Console.WriteLine(i);
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
                
                while (true)
                {
                    selectChoice = new byte[150];
                    socket.Receive(selectChoice);
                    if (Encoding.ASCII.GetString(selectChoice).Split(' ')[0] == "SUCCESS")
                        break;
                    else if (Encoding.ASCII.GetString(selectChoice).Split(' ')[0] == "FAILED")
                    {
                        break;
                    }
                    else if (Encoding.ASCII.GetString(selectChoice).Split(' ')[0] == "++")
                    {
                        Console.WriteLine(Encoding.ASCII.GetString(selectChoice));
                    }
                    else if (Encoding.ASCII.GetString(selectChoice).Split(' ')[0] == "--")
                    {
                        Console.WriteLine(Encoding.ASCII.GetString(selectChoice));
                    }
                    else
                    {
                        Console.WriteLine(Encoding.ASCII.GetString(selectChoice));
                        socket.Send(Encoding.ASCII.GetBytes( Console.ReadLine()));
                    }
                }
                //Console.Write("data send\r\n");

            }
            Console.Read();
            socket.Close();

                
            
        }
    }
}
