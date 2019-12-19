using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace test
{
    class Program
    {
        static void Main(string[] args)
        {
            string str = "GET /index.html HTTP/1.1\r\n" + 
            "Host: 127.0.0.1\r\n" +
            "Date = 19.12.2019 20:27:01\r\n" +
            "Connection:keep - alive\r\n\r\n" +
            "preferedHotel:HILTON+preferedAirline:THY+Date:29/12/2019+peopleCount:10";
            ParseRequest(str);

        }
        private static void ParseRequest(string request)
        {
            

            string[] splittedRequest;
            splittedRequest = request.Split('\n');
            string[] userInfo = splittedRequest[splittedRequest.Length - 1].Split('+');
            string preferedHotel = userInfo[0].Split(':')[1];




        }
    }
}
