using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Linq;
namespace SocketProgramming.Hotel
{
    class Hotel
    {
        
        static Socket socket;
        static byte[]  buffer { get; set;}
        
        static void Main(string[] args)
        {
            Console.WriteLine("This is Hotel Server");
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Bind(new IPEndPoint(0, 2000));
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
                string hotelName = parameters[0];
                string date = parameters[1];

                using (HotelEntities hoteldb = new HotelEntities())
                {
                    int hotel_id = hoteldb.Hotel_table.Where(x => x.Hotel_Name == hotelName && x.Hotel_trip_date == date).FirstOrDefault().Hotel_ID;
                    Hotel_table hotel = hoteldb.Hotel_table.Find(hotel_id);
                    int availableRooms = hotel.available_Room;
                    if (int.Parse(parameters[2]) > availableRooms)
                    {
                        Console.WriteLine("Not available");
                    }
                    else
                    {
                        Console.WriteLine("Available");

                    }

                }
                //parameters[0] company,parameters[1] date, parameters[0] count


                Console.WriteLine(strData);
                j++;
            }
            socket.Close();
            accepted.Close();



        }
    }
}
