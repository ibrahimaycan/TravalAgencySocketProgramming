using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SocketProgramming.TravelAgency;
using SocketProgramming.Client;


namespace SocketProgramming.UnitTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            Customer_Info customer_Info = new Customer_Info();
            customer_Info.Date = "01/01/2020";
            customer_Info.peopleCount = "10";
            customer_Info.preferedAirline = "THY";
            customer_Info.preferedHotel = "HILTON";                  
        }
    }
}
