using SocketProgramming.Airline;
using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace SocketProgramming.Airplane
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
                string airlineName = parameters[0];
                string date = parameters[1];
                string customerNumber = parameters[2];

                //id yi çekme ve kontrol
                using(airlineEntities airlinedb = new airlineEntities())
                {
                    int plane_id = airlinedb.airline_table.Where(x => x.airline_Name == airlineName && x.trip_date == date ).FirstOrDefault().airline_ID;
                    airline_table plane = airlinedb.airline_table.Find(plane_id);
                    int availableSeats = plane.available_Seats;
                    if (int.Parse(parameters[2]) > availableSeats)
                    {
                        Console.WriteLine("Not available");
                    }
                    else
                    {
                        Console.WriteLine("Available");

                    }

                }
       
                Console.WriteLine(strData);
                j++;
            }
            socket.Close();
            accepted.Close();



        }
    }
}
