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
            while (j < 100)
            {
                Console.WriteLine(j);
                buffer = new byte[accepted.SendBufferSize];
                int bytesRead = accepted.Receive(buffer);
                byte[] formatted = new byte[bytesRead];
                for (int i = 0; i < bytesRead; i++)
                {
                    formatted[i] = buffer[i];

                }
                string strData = Encoding.ASCII.GetString(formatted);
                Console.WriteLine(strData);
                Customer_Info customer;
                string Method_type;
                string transactionType;
                ParseRequest(strData,out customer,out Method_type,out transactionType);//Gelen requesti parse ediyoruz
                Console.WriteLine("Transaction type= " + transactionType);
                //string[] parameters = strData.Split(' ');
                string airlineName = customer.preferedAirline;
                string date = customer.Date;
                string customerNumber = customer.peopleCount;

                ////id yi çekme ve kontrol
                ///

                #region THY
                if (airlineName == "THY")
                {                   
                    int trip_id=0;                    
                    using (THYEntities airlineDatabase = new THYEntities())
                    {
                        try
                        {
                            trip_id = airlineDatabase.THY_table.Where(x => x.Airline_Name == airlineName && x.trip_date == date).FirstOrDefault().trip_ID;
                            THY_table plane = airlineDatabase.THY_table.Find(trip_id);
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
                        catch { }
                    }


                    buffer = new byte[2048];
                    accepted.Receive(buffer);
                    ParseRequest(Encoding.ASCII.GetString(buffer), out customer, out Method_type, out transactionType);
                    if (transactionType == "UPDATE")
                    {
                        using (THYEntities airlineDatabase = new THYEntities())
                        {
                            THY_table plane = airlineDatabase.THY_table.Find(trip_id);
                            plane.available_Seats = plane.available_Seats - int.Parse(customer.peopleCount);
                            airlineDatabase.SaveChanges();

                        }
                    }
                    if (transactionType == "CHECKOTHER")
                    {
                        using (PEGASUSEntities airlineDatabase = new PEGASUSEntities())
                        {
                            trip_id = airlineDatabase.PEGASUS_table.Where(x => x.trip_date == date).FirstOrDefault().trip_ID;
                            PEGASUS_table plane = airlineDatabase.PEGASUS_table.Find(trip_id);
                            int availableSeats = plane.available_Seats;
                            if (int.Parse(customerNumber) > availableSeats)
                            {
                                accepted.Send(GetResponse(customer, "404", "No available Place"));
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
                                plane = airlineDatabase.PEGASUS_table.Find(trip_id);
                                plane.available_Seats = plane.available_Seats - int.Parse(customer.peopleCount);
                                airlineDatabase.SaveChanges();
                            }                          
                        }
                    }                   
                }
                #endregion

                #region PEGASUS          
                if (airlineName == "PEGASUS")
                {
                    int trip_id=0;
                    using (PEGASUSEntities airlineDatabase = new PEGASUSEntities())
                    {
                        try
                        {
                            trip_id = airlineDatabase.PEGASUS_table.Where(x => x.Airline_Name == airlineName && x.trip_date == date).FirstOrDefault().trip_ID;
                            PEGASUS_table plane = airlineDatabase.PEGASUS_table.Find(trip_id);
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
                        catch { }
                    }

                    buffer = new byte[2048];
                    accepted.Receive(buffer);
                    ParseRequest(Encoding.ASCII.GetString(buffer), out customer, out Method_type, out transactionType);
                    if (transactionType == "UPDATE")
                    {
                        using (PEGASUSEntities airlineDatabase = new PEGASUSEntities())
                        {
                            PEGASUS_table plane = airlineDatabase.PEGASUS_table.Find(trip_id);
                            plane.available_Seats = plane.available_Seats - int.Parse(customer.peopleCount);
                            airlineDatabase.SaveChanges();
                        }

                    }
                    if (transactionType == "CHECKOTHER")
                    {
                        using(THYEntities airlineDatabase=new THYEntities())
                        {
                            trip_id = airlineDatabase.THY_table.Where(x => x.trip_date == date).FirstOrDefault().trip_ID;
                            THY_table plane = airlineDatabase.THY_table.Find(trip_id);
                            int availableSeats = plane.available_Seats;
                            if (int.Parse(customerNumber) > availableSeats)
                            {
                                accepted.Send(GetResponse(customer, "404", "No available Place"));
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
                                plane = airlineDatabase.THY_table.Find(trip_id);
                                plane.available_Seats = plane.available_Seats - int.Parse(customer.peopleCount);
                                airlineDatabase.SaveChanges();
                            }
                            
                        }
                    }
                }
                #endregion


                j++;
            }



            socket.Close();
            accepted.Close();



        }
        private static void ParseRequest(string request,out Customer_Info customer,out string method,out string transactionType)
        {
           // Console.WriteLine(request);
            string[] splittedRequest;
            splittedRequest = request.Split('\n');
            string[] userInfo = splittedRequest[splittedRequest.Length - 1].Split('+');
            customer = new Customer_Info();
            customer.preferedHotel = userInfo[0].Split(':')[1];
            customer.preferedAirline = userInfo[1].Split(':')[1];
            customer.Date=userInfo[2].Split(':')[1];
            customer.peopleCount = userInfo[3].Split(':')[1];
            method=splittedRequest[0].Split(' ')[0];
            transactionType = splittedRequest[4].Split(' ')[1];
        
            

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
