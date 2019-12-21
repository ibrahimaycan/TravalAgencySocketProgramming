using SocketProgramming.Client;
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




            Customer_Info customer_Info = null;
            int j = 0;
            while (j < 3) {

 
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
                Console.WriteLine(GetRequest(customer_Info, "GET", "CHECK"));
                Airplane_socket.Send(GetRequest(customer_Info,"GET","CHECK"));//HTTP tipinde gönderilecek mesajı generate ediyo
                byte[] ReceivedMessage=new byte[2048];
                Airplane_socket.Receive(ReceivedMessage);//HTTP tipinde Response kodu
                string AirplaneResponseCode;
                ParseResponse(Encoding.ASCII.GetString(ReceivedMessage),out AirplaneResponseCode);//Response kodu çekiyo

                Hotel_socket.Send(GetRequest(customer_Info,"GET","CHECK"));
                ReceivedMessage = new byte[2048];
                Hotel_socket.Receive(ReceivedMessage);
                string HotelResponseCode;
                ParseResponse(Encoding.ASCII.GetString(ReceivedMessage), out HotelResponseCode);//Response kodu çekiyo
                                                                                                    // Console.WriteLine("HotelResponseCode"+HotelResponseCode);
                while (true)
                {
                    if (AirplaneResponseCode == "200" && HotelResponseCode == "200")
                    {
                        Airplane_socket.Send(GetRequest(customer_Info, "POST","UPDATE"));
                        Hotel_socket.Send(GetRequest(customer_Info, "POST","UPDATE"));
                        break;
                    }

                }
                
                j++;
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
