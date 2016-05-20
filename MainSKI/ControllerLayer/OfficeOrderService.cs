using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using DomainLayer;

namespace ControllerLayer
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in both code and config file together.
    public class OfficeOrderService : IOfficeOrderService
    {
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, UriTemplate = "addOrder/{path}")]
        public bool AddOrder(string path)
        {
            //TODO: Do path lookup stuff here.
            string e02 = "";
            return OrderController.Instance.AddOrder(e02);
        }

        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, UriTemplate = "addSubOrderToOrder/{path}/{orderID}")]
        public bool AddSubOrderToOrder(string path, string orderID)
        {
            //TODO: Do Path Lookup stuff here.
            string e02 = "";
            return OrderController.Instance.AddSubOrderToOrder(e02, orderID);
        }

        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, UriTemplate = "testService/{test}/{testy}")]
        public string TestService(string test, string testy)
        {
            Console.WriteLine("Test Text: " + test + " " + testy);
            return "Test Text: " + test + " " + testy;
        }
    }
}
