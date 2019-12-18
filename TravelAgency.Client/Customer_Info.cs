using System;
using System.Collections.Generic;
using System.Text;

namespace SocketProgramming.Client
{
    public class Customer_Info
    {
     
        public string preferedAirline { get; set; }
        public string preferedHotel { get; set; }
        public string Date { get; set; }
        public string peopleCount { get; set; }
        public Customer_Info(string preferedHotel, string preferedAirline, string Date, string peopleCount)
        {
            
            this.preferedHotel = preferedHotel;
            this.preferedAirline = preferedAirline;
            this.Date = Date;
            this.peopleCount = peopleCount;
        }
        public Customer_Info(string text)
        {
            string[] Cus_info = text.Split(' ');

            this.preferedHotel = Cus_info[0];
            this.preferedAirline = Cus_info[1];
            this.Date = Cus_info[2];
            this.peopleCount = Cus_info[3];

        }

    }
}
