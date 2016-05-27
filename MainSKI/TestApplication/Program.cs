using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainLayer;
using Persistence;

namespace TestApplication
{
    class Program
    {
        public static void Main(string[] args)
        {
            //CreateOrderTest();
            //RetrieveordersTest();
            CreateOrderTeste02();

            Console.WriteLine("Press anything to continue");
            Console.ReadKey();
        }

        private static void RetrieveordersTest()
        {
            List<Order> orders = null;

            orders = DBFacade.Instance.RetrieveAllOrders();
            if (orders != null)
            {
                if (orders.Count > 0)
                {
                    Console.WriteLine("TEST: orders succefully retrieved from DB...");
                    Console.WriteLine("ID of First Order: " + orders[0].ID);
                }
            }
        }

        private static void CreateOrderTeste02()
        {
            string e02Path = @"C:\Users\tm_ma\Google Drive\Project SKI Kitchen\01 Assignment\Material From SKI\";
            string e02FileName = "w0000520.e02";
            string e02FileName2 = "w0000524.e02";
            string e02FileName3 = "w0000293 Ascii file with all info.e02";
            Order.CreateOrder(e02Path + e02FileName);
            Console.WriteLine("\n");
            Order.CreateOrder(e02Path + e02FileName2);
            Console.WriteLine("\n");
            Order.CreateOrder(e02Path + e02FileName3);
        }

        private static void CreateOrderTest()
        {

            int OrderNum = 1234;
            int OrderSubject = 1;
            int OrderAlt = 2;
            string OrderID = "" + OrderNum + "/" + OrderSubject + "/" + OrderAlt;


            List<Link> ll = new List<Link>();
            Link l1 = new Link("LinkID1", "TEST1", "TESTLINK");
            Link l2 = new Link("LinkID2", "TEST2", "TESTLINK");
            ll.Add(l1);
            ll.Add(l2);

            Console.WriteLine("Displaying Link Data: ID: " + l1.OrderID);

            ProgressState[] EPSArray = new ProgressState[4];
            ProgressState EPS = null;
            for (int i = 0; i < 4; i++)
            {
                EPS = new ProgressState("EPSID1", "TESTELEMENT" + (i + 1), "this is a comment", false, false, i + 4);
                EPSArray[i] = EPS;
            }

            List<Element> el = new List<Element>();
            el.Add(new Element("TESTELEMENT1", OrderID, "TEST", "TEST", "TEST", "TEST", 0.1, "TEST", "TEST", EPSArray));


            List<ProductionData> Prods = new List<ProductionData>();
            List<string> testData = new List<string>();
            testData.Add("This is TestData");
            testData.Add("This is more TestData");
            ProductionData pd = new ProductionData("prodID1", "PD1", testData);
            Prods.Add(pd);


            ProgressState[] OPSArray = new ProgressState[4];
            ProgressState OPS = null;
            for (int i = 0; i < 4; i++)
            {
                OPS = new ProgressState("OPSID1", OrderID + (i + 1), "comment", false, false, i + 4);
                OPSArray[i] = OPS;
            }


            Order o = Order.CreateOrder(OrderID,
                new CustomerData("CustIDTEST", "CustNameTEST", "CustAddressTEST", "CustDeliveryAddressTEST", "CustMailTEST", "CustPhoneTEST", "MorePhoneTEST", "moremorePhoneTEST", "faxTEST"),
                OrderNum, OrderSubject, OrderAlt, DateTime.Now, DateTime.Now, 0.0, 1, ll, "TEST", new List<Order>(), el, EPSArray, Prods);

            Console.WriteLine("TEST: Order created in memory...");


            if (DBFacade.Instance.CreateOrder(o))
                Console.WriteLine("TEST: DBFacade returned true on OrderCreation....");
        }
    }
}
