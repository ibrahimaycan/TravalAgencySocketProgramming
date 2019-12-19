using SocketProgramming.Airline;
using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using SocketProgramming.Client;

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
                Customer_Info customer = ParseRequest(strData);
                //string[] parameters = strData.Split(' ');
                string airlineName = customer.preferedAirline;
                string date = customer.Date;
                string customerNumber = customer.peopleCount;

                ////id yi çekme ve kontrol
                using(airlineEntities airlinedb = new airlineEntities())
                {
                    int plane_id = airlinedb.airline_table.Where(x => x.airline_Name == airlineName && x.trip_date == date).FirstOrDefault().airline_ID;
                    airline_table plane = airlinedb.airline_table.Find(plane_id);
                    int availableSeats = plane.available_Seats;
                    if (int.Parse(customerNumber) > availableSeats)
                    {
                        accepted.Send(GetResponse(customer, "404", "No available Place"));
                    }
                    else
                    {
                        accepted.Send(GetResponse(customer, "200", "Place is available"));

                    }

                }

                Console.WriteLine(strData);
                j++;
            }



            socket.Close();
            accepted.Close();



        }
        private static Customer_Info ParseRequest(string request)
        {
            Console.WriteLine(request);
            string[] splittedRequest;
            splittedRequest = request.Split('\n');
            string[] userInfo = splittedRequest[splittedRequest.Length - 1].Split('+');
            Customer_Info customer = new Customer_Info();
            customer.preferedHotel = userInfo[0].Split(':')[1];
            customer.preferedAirline = userInfo[1].Split(':')[1];
            customer.Date=userInfo[2].Split(':')[1];
            customer.peopleCount = userInfo[3].Split(':')[1];
            return customer;

        }

        private static byte[] GetResponse(Customer_Info customer_Info,string responseCode,string data)
        {

            string header = "GET "+responseCode+" HTTP/1.1\r\n" +
                "Host: 127.0.0.1\r\n" +
                "Date=" + DateTime.Now + "\r\n" +
                "Connection: keep-alive\r\n" +
                "\r\n";
            string Entitybody = data;
            return Encoding.ASCII.GetBytes(header + Entitybody);
        }


    }
}
