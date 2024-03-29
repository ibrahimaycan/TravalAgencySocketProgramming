﻿using SocketProgramming.Client;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace SocketProgramming.TravelAgency
{
    public class TravelAgency
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
            #region socketConnection
            //Airplane Socket connection
            Airplane_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 3000);
            try
            {
                Airplane_socket.Connect(localEndPoint);

            }
            catch
            {
                Console.WriteLine("Unable to Connect Airline Server");
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
                Console.WriteLine("Unable to Connect Hotel Server");
                Main(args);
            }
            #endregion
           
            Customer_Info customer_Info = null;
            int j = 0;
            while (j < 100) {
                j++;
                Console.WriteLine(j);
                buffer = new byte[accepted.SendBufferSize];
                int bytesRead = accepted.Receive(buffer);//Clienttan alıyo
                byte[] formatted = new byte[bytesRead];
                for (int i = 0; i < bytesRead; i++)
                {
                    formatted[i] = buffer[i];

                }

                string strData = Encoding.ASCII.GetString(formatted);///HILTON THY 15/01/2020
                customer_Info = new Customer_Info(strData);
                
                string[] parameters = strData.Split(' ');
                //Console.WriteLine(GetRequest(customer_Info, "GET", "CHECK"));
                Airplane_socket.Send(GetRequest(customer_Info,"GET","CHECK"));//HTTP tipinde gönderilecek mesajı generate ediyo
                byte[] ReceivedMessage=new byte[2048];
                Airplane_socket.Receive(ReceivedMessage);//HTTP tipinde Response kodu
                string AirplaneResponseCode;
                ParseResponse(Encoding.ASCII.GetString(ReceivedMessage),out AirplaneResponseCode);//Response kodu çekiyo

                Hotel_socket.Send(GetRequest(customer_Info,"GET","CHECK"));
                ReceivedMessage = new byte[2048];
                Hotel_socket.Receive(ReceivedMessage);
                string HotelResponseCode;
                string alternative,alternative_2;
                byte[] client_Answer = new byte[1];
                ParseResponse(Encoding.ASCII.GetString(ReceivedMessage), out HotelResponseCode);//Response kodu çekiyo
                                                                                                   // Console.WriteLine("HotelResponseCode"+HotelResponseCode);
                while (true)
                {
                    
                    if (AirplaneResponseCode == "200" && HotelResponseCode == "200")
                    {
                        Airplane_socket.Send(GetRequest(customer_Info, "POST","UPDATE"));
                        Hotel_socket.Send(GetRequest(customer_Info, "POST","UPDATE"));
                        accepted.Send(Encoding.ASCII.GetBytes("++ TICKET RECEIVED "));
                        accepted.Send(Encoding.ASCII.GetBytes("SUCCESS "));
                        break;
                    }
                    if (AirplaneResponseCode == "404" && HotelResponseCode == "200")
                    {
                        Airplane_socket.Send(GetRequest(customer_Info, "GET", "CHECKOTHER"));
                        ReceivedMessage = new byte[2048];
                        Airplane_socket.Receive(ReceivedMessage);//HTTP tipinde Response kodu
                        ParseResponse(Encoding.ASCII.GetString(ReceivedMessage), out AirplaneResponseCode);//Response kodu çekiyo
                        if (AirplaneResponseCode == "200")
                        {
                            
                            //Clienta sor
                            if (customer_Info.preferedAirline == "THY")
                                alternative = "PEGASUS";
                            else
                                alternative = "THY";
                            accepted.Send(Encoding.ASCII.GetBytes("There is no available seats on " + customer_Info.preferedAirline +
                                " Would you prefer " + alternative+" ? "));
                            accepted.Receive(client_Answer);
                            if (Encoding.ASCII.GetString(client_Answer) == "Y")
                            {
                                Airplane_socket.Send(GetRequest(customer_Info, "POST", "UPDATE"));
                                Hotel_socket.Send(GetRequest(customer_Info, "POST", "UPDATE"));
                                accepted.Send(Encoding.ASCII.GetBytes("++ TICKET RECEIVED "));
                                accepted.Send(Encoding.ASCII.GetBytes("SUCCESS "));
                            }
                            else
                            {
                                Airplane_socket.Send(GetRequest(customer_Info, "POST", "DONOTHING"));
                                Hotel_socket.Send(GetRequest(customer_Info, "POST", "DONOTHING"));
                                accepted.Send(Encoding.ASCII.GetBytes("-- TICKET NOT RECEIVED "));
                                accepted.Send(Encoding.ASCII.GetBytes("FAILED "));
                            }
                        }
                        else
                        {
                            Hotel_socket.Send(GetRequest(customer_Info, "GET", "DONOTHING"));
                            Console.WriteLine("Uygun uçak yok");
                           // accepted.Send(Encoding.ASCII.GetBytes("-- No Available Flight Try another day"));
                            accepted.Send(Encoding.ASCII.GetBytes("-- No Available Flight Try another day.\nTICKET NOT RECEIVED. "));
                            accepted.Send(Encoding.ASCII.GetBytes("FAILED "));
                            ///Clienta uygun yer yok mesajı gönder
                        }
                        break;

                    }
                    if (AirplaneResponseCode == "200" && HotelResponseCode == "404")
                    {
                        Hotel_socket.Send(GetRequest(customer_Info, "GET", "CHECKOTHER"));
                        ReceivedMessage = new byte[2048];
                        Hotel_socket.Receive(ReceivedMessage);//HTTP tipinde Response kodu
                        ParseResponse(Encoding.ASCII.GetString(ReceivedMessage), out HotelResponseCode);//Response kodu çekiyo
                        if (HotelResponseCode == "200")
                        {
                            //Clienta sor
                            if (customer_Info.preferedHotel == "HILTON")
                                alternative = "SWISS";
                            else
                                alternative = "HILTON";
                            accepted.Send(Encoding.ASCII.GetBytes("There is no available room on " + customer_Info.preferedHotel +
    " Would you prefer " + alternative + " ? "));
                            accepted.Receive(client_Answer);
                            if (Encoding.ASCII.GetString(client_Answer) == "Y")
                            {
                                Airplane_socket.Send(GetRequest(customer_Info, "POST", "UPDATE"));
                                Hotel_socket.Send(GetRequest(customer_Info, "POST", "UPDATE"));
                                accepted.Send(Encoding.ASCII.GetBytes("++ TICKET RECEIVED "));
                                accepted.Send(Encoding.ASCII.GetBytes("SUCCESS "));
                            }
                            else
                            {
                                Airplane_socket.Send(GetRequest(customer_Info, "POST", "DONOTHING"));
                                Hotel_socket.Send(GetRequest(customer_Info, "POST", "DONOTHING"));
                                accepted.Send(Encoding.ASCII.GetBytes("-- TICKET NOT RECEIVED "));
                                accepted.Send(Encoding.ASCII.GetBytes("FAILED "));
                            }

                        }
                        else
                        {
                            Airplane_socket.Send(GetRequest(customer_Info, "GET", "DONOTHING"));
                            Console.WriteLine("Uygun oda yok");
                            accepted.Send(Encoding.ASCII.GetBytes("-- No Available Room. Try another day\nTICKET  NOT RECEIVED "));
                            accepted.Send(Encoding.ASCII.GetBytes("FAILED "));
                            //Cliente uygun oda yok mesajı
                        }
                        break;

                    }
                    if (AirplaneResponseCode == "404" && HotelResponseCode == "404")
                    {
                        Airplane_socket.Send(GetRequest(customer_Info, "GET", "CHECKOTHER"));
                        ReceivedMessage = new byte[2048];
                        Airplane_socket.Receive(ReceivedMessage);//HTTP tipinde Response kodu
                        ParseResponse(Encoding.ASCII.GetString(ReceivedMessage), out AirplaneResponseCode);//Response kodu çekiyo
                        if (AirplaneResponseCode == "200")
                        {

                            Hotel_socket.Send(GetRequest(customer_Info, "GET", "CHECKOTHER"));
                            ReceivedMessage = new byte[2048];
                            Hotel_socket.Receive(ReceivedMessage);//HTTP tipinde Response kodu
                            ParseResponse(Encoding.ASCII.GetString(ReceivedMessage), out HotelResponseCode);//Response kodu çekiyo
                            
                            if (HotelResponseCode == "200")
                            {
                                if (customer_Info.preferedAirline == "THY")
                                    alternative = "PEGASUS";
                                else
                                    alternative = "THY";
                                if (customer_Info.preferedHotel == "HILTON")
                                    alternative_2 = "SWISS";
                                else
                                    alternative_2 = "HILTON";

                                accepted.Send(Encoding.ASCII.GetBytes("There is no available room on " 
                                    + customer_Info.preferedHotel 
                                    +" and there is no available seat on "+customer_Info.preferedAirline
                                    +" Would you prefer " + alternative_2+ " and "+alternative + " ? "));
                                accepted.Receive(client_Answer);

                                ///CUSTOMERA SOR
                                if (Encoding.ASCII.GetString(client_Answer) == "Y")
                                {
                                    Airplane_socket.Send(GetRequest(customer_Info, "POST", "UPDATE"));
                                    Hotel_socket.Send(GetRequest(customer_Info, "POST", "UPDATE"));
                                    accepted.Send(Encoding.ASCII.GetBytes("++ TICKET RECEIVED "));
                                    accepted.Send(Encoding.ASCII.GetBytes("SUCCESS "));
                                    
                                }
                                else
                                {
                                    Airplane_socket.Send(GetRequest(customer_Info, "POST", "DONOTHING"));
                                    Hotel_socket.Send(GetRequest(customer_Info, "POST", "DONOTHING"));
                                    accepted.Send(Encoding.ASCII.GetBytes("-- TICKET NOT RECEIVED "));
                                    accepted.Send(Encoding.ASCII.GetBytes("FAILED "));
                                }
                            }
                            else
                            {
                                Console.WriteLine("Uygun oda yok");
                                Airplane_socket.Send(GetRequest(customer_Info, "POST", "DONOTHING"));                   
                                accepted.Send(Encoding.ASCII.GetBytes("-- No Available Room .Try another day\nTICKET NOT RECEIVED "));
                                accepted.Send(Encoding.ASCII.GetBytes("FAILED "));
                                //Clienta uygun oda yok mesajı
                            }
                        }
                        else
                        {
                            Hotel_socket.Send(GetRequest(customer_Info, "GET", "DONOTHING"));            
                            accepted.Send(Encoding.ASCII.GetBytes("-- There is no flight and room.TRY another day. \nTICKET NOT RECEIVED "));
                            accepted.Send(Encoding.ASCII.GetBytes("FAILED "));
                            Console.WriteLine("Uygun uçuş yok");
                            //Clienta uygun uçuş yok mesajı
                        }
                        break;
                    }

                }
                
                
            }
            socket.Close();
            accepted.Close();


        }

        //TODO check port number.
        //Check header
        public static byte[] GetRequest(Customer_Info customer_Info,string method_Types,string transactionType)
        {

            string header = method_Types + " /index.html HTTP/1.1\r\n" +
                "Host: 127.0.0.1 \r\n" +
                "Date=" + DateTime.Now + " \r\n" +
                "Connection: keep-alive \r\n" +
                "TransactionType: " + transactionType + " \r\n"+
                "\r\n";
            string Entitybody = "preferedHotel:" + customer_Info.preferedHotel + "+preferedAirline:" + customer_Info.preferedAirline +
                "+Date:" + customer_Info.Date + "+peopleCount:" + customer_Info.peopleCount;
            return Encoding.ASCII.GetBytes(header+Entitybody);
        }

        private static void ParseResponse(string responseMessage,out string responseCode)
        {

            //"GET 404 HTTP/1.1\r\n" +
            //    "Host: 127.0.0.1\r\n" +
            //    "Date=" + DateTime.Now + "\r\n" +
            //    "Connection: keep-alive\r\n" +
            //    "\r\n";
            //"No available Place"

            string[] splittedMessage;
            splittedMessage = responseMessage.Split(' ');
            responseCode = splittedMessage[1];
        }

    }



}
