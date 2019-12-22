using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Linq;
using SocketProgramming.Client;

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
            while (j < 100)
            {
                j++;
                Console.WriteLine(j);
                buffer = new byte[accepted.SendBufferSize];
                int bytesRead = accepted.Receive(buffer);
                byte[] formatted = new byte[bytesRead];
                for (int i = 0; i < bytesRead; i++)
                {
                    formatted[i] = buffer[i];

                }
                string strData = Encoding.ASCII.GetString(formatted);
                Customer_Info customer;
                string Method_type;
                string transactionType;
                ParseRequest(strData, out customer, out Method_type, out transactionType);//Gelen requesti parse ediyoruz
                string HotelName = customer.preferedHotel;
                string date = customer.Date;
                string customerNumber = customer.peopleCount;
                #region HILTON


                if (HotelName == "HILTON")
                {
                    int trip_id = 0;
                    using (HILTONEntities HotelDatabase = new HILTONEntities())
                    {
                        try
                        {
                            trip_id = HotelDatabase.HILTON_table.Where(x => x.Hotel_Name == HotelName && x.Hotel_trip_date == date).FirstOrDefault().Trip_ID;
                            HILTON_table hotel = HotelDatabase.HILTON_table.Find(trip_id);
                            int availableRooms = hotel.Available_Room;
                            if (int.Parse(customerNumber) > availableRooms)
                            {
                                accepted.Send(GetResponse(customer, "404", "No available Place"));
                            }
                            else
                            {
                                accepted.Send(GetResponse(customer, "200", "Place is available"));
                            }
                        }
                        catch { }
                    }
                    buffer = new byte[2048];
                    accepted.Receive(buffer);
                    ParseRequest(Encoding.ASCII.GetString(buffer), out customer, out Method_type, out transactionType);
                    if (transactionType == "UPDATE")
                    {
                        using (HILTONEntities HotelDatabase = new HILTONEntities())
                        {
                            HILTON_table hotel = HotelDatabase.HILTON_table.Find(trip_id);
                            hotel.Available_Room = hotel.Available_Room - int.Parse(customer.peopleCount);
                            HotelDatabase.SaveChanges();
                        }
                    }
                    if (transactionType == "CHECKOTHER")
                    {
                        using (SWISSEntities HotelDatabase = new SWISSEntities())
                        {
                            trip_id = HotelDatabase.SWISS_table.Where(x => x.Hotel_trip_date == date).FirstOrDefault().Trip_ID;
                            SWISS_table hotel = HotelDatabase.SWISS_table.Find(trip_id);
                            int availableRoom = hotel.Available_Room;
                            if (int.Parse(customerNumber) > availableRoom)
                            {
                                accepted.Send(GetResponse(customer, "404", "No available Placa"));
                                continue;
                            }
                            else
                            {
                                accepted.Send(GetResponse(customer, "200", "Place is available"));
                            }
                            buffer = new byte[2048];
                            accepted.Receive(buffer);
                            ParseRequest(Encoding.ASCII.GetString(buffer), out customer, out Method_type, out transactionType);
                            if (transactionType == "UPDATE")
                            {
                                hotel = HotelDatabase.SWISS_table.Find(trip_id);
                                hotel.Available_Room = hotel.Available_Room - int.Parse(customer.peopleCount);
                                HotelDatabase.SaveChanges();
                            }
                        }
                    }
                }
                #endregion
                #region SWISS


                if (HotelName == "SWISS")
                {
                    int trip_id = 0;
                    using (SWISSEntities HotelDatabase = new SWISSEntities())
                    {
                        try
                        {
                            trip_id = HotelDatabase.SWISS_table.Where(x => x.Hotel_Name == HotelName && x.Hotel_trip_date == date).FirstOrDefault().Trip_ID;
                            SWISS_table hotel = HotelDatabase.SWISS_table.Find(trip_id);
                            int availableRooms = hotel.Available_Room;
                            if (int.Parse(customerNumber) > availableRooms)
                            {
                                accepted.Send(GetResponse(customer, "404", "No available Place"));
                            }
                            else
                            {
                                accepted.Send(GetResponse(customer, "200", "Place is available"));
                            }


                        }
                        catch { }
                    }
                    buffer = new byte[2048];
                    accepted.Receive(buffer);
                    ParseRequest(Encoding.ASCII.GetString(buffer), out customer, out Method_type, out transactionType);
                    if (transactionType == "UPDATE")
                    {
                        using (SWISSEntities HotelDatabase = new SWISSEntities())
                        {
                            SWISS_table hotel = HotelDatabase.SWISS_table.Find(trip_id);
                            hotel.Available_Room = hotel.Available_Room - int.Parse(customer.peopleCount);
                            HotelDatabase.SaveChanges();

                        }
                    }
                    if (transactionType == "CHECKOTHER")
                    {
                        using (HILTONEntities HotelDatabase = new HILTONEntities())
                        {
                            trip_id = HotelDatabase.HILTON_table.Where(x => x.Hotel_trip_date == date).FirstOrDefault().Trip_ID;
                            HILTON_table hotel = HotelDatabase.HILTON_table.Find(trip_id);
                            int availableRoom = hotel.Available_Room;
                            if (int.Parse(customerNumber) > availableRoom)
                            {
                                accepted.Send(GetResponse(customer, "404", "No available Placa"));
                                continue;
                            }
                            else
                            {
                                accepted.Send(GetResponse(customer, "200", "Place is available"));
                            }
                            buffer = new byte[2048];
                            accepted.Receive(buffer);
                            ParseRequest(Encoding.ASCII.GetString(buffer), out customer, out Method_type, out transactionType);
                            if (transactionType == "UPDATE")
                            {
                                hotel = HotelDatabase.HILTON_table.Find(trip_id);
                                hotel.Available_Room = hotel.Available_Room - int.Parse(customer.peopleCount);
                                HotelDatabase.SaveChanges();
                            }
                        }
                    }
                }

                #endregion
                
            }   
            socket.Close();
            accepted.Close();
            
        }
        private static void ParseRequest(string request,out Customer_Info customer, out string method,out string transactionType)
        {
         //   Console.WriteLine(request);
            string[] splittedRequest;
            splittedRequest = request.Split('\n');
            string[] userInfo = splittedRequest[splittedRequest.Length - 1].Split('+');
            customer = new Customer_Info();
            customer.preferedHotel = userInfo[0].Split(':')[1];
            customer.preferedAirline = userInfo[1].Split(':')[1];
            customer.Date = userInfo[2].Split(':')[1];
            customer.peopleCount = userInfo[3].Split(':')[1];
            method = splittedRequest[0].Split(' ')[0];
            transactionType = splittedRequest[4].Split(' ')[1];

        }

        private static byte[] GetResponse(Customer_Info customer_Info, string responseCode, string data)
        {

            string header = "GET " + responseCode + " HTTP/1.1\r\n" +
                "Host: 127.0.0.1\r\n" +
                "Date=" + DateTime.Now + "\r\n" +
                "Connection: keep-alive\r\n" +
                "\r\n";
            string Entitybody = data;
            return Encoding.ASCII.GetBytes(header + Entitybody);
        }




    }
}
