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
            RunTest();
        }

        public static void RunTest()
        {
            string OrderID = "TEstOrder";
            List<Link> ll = new List<Link>();
            Link l = new Link("TEST", "TESTLINK");
            ll.Add(l);

            ProgressState[] EPSArray = new ProgressState[4];
            ProgressState EPS = null;
            for (int i = 0; i < 4; i++)
            {
                EPS = new ProgressState("TESTELEMENT1", "comemnt", false, false, i + 4);
                EPSArray[i] = EPS;
            }

            List<Element> el = new List<Element>();
            el.Add(new Element("TESTELEMENT1", OrderID, "TEST", "TEST", "TEST", "TEST", 0.1, "TEST", "TEST", EPSArray));


            List<ProductionData> Prods = new List<ProductionData>();
            List<string> testData = new List<string>();
            testData.Add("This is TestData");
            testData.Add("This is more TestData");
            ProductionData pd = new ProductionData("PD1", testData);
            Prods.Add(pd);


            ProgressState[] OPSArray = new ProgressState[4];
            ProgressState OPS = null;
            for (int i = 0; i < 4; i++)
            {
                OPS = new ProgressState(OrderID, "comment", false, false, i + 4);
                OPSArray[i] = OPS;
            }


            Order o = Order.CreateOrder(OrderID,
                new CustomerData("CustIDTEST", "CustNameTEST", "CustAddressTEST", "CustDeliveryAddressTEST", "CustMailTEST", "CustPhoneTEST", "MorePhoneTEST", "moremorePhoneTEST", "faxTEST"),
                1, DateTime.Now, DateTime.Now, 0.0, 1, ll, "TEST", new List<Order>(), el, EPSArray, Prods);

            Console.WriteLine("TEST: Order created in memory");
            if (DBFacade.Instance.CreateOrder(o))
            {
                Console.WriteLine("TEST: DBFacade returned true on OrderCreation.");
                List<Order> orders = DBFacade.Instance.RetrieveAllOrders();
                if (orders != null)
                {
                    Console.WriteLine("TEST: orders succefully retrieved from DB...");
                }
            }



        }
    }
}
