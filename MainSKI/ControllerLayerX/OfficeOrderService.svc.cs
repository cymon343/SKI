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
    public class OfficeOrderService : IOfficeOrderService
    {
        [WebInvoke(Method = "GET",
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped,
            UriTemplate = "addOrder/{path}")]
        public bool AddOrder(string e02FileLocation)
        {
            //e02FileLocation = e02FileLocation.Replace("_", "\\"); //Workaround for testing purposes
            return OrderController.Instance.AddOrder(e02FileLocation);
        }

        [WebInvoke(Method = "GET",
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped,
            UriTemplate = "addSubOrderToOrder/{path}/{orderID}")]
        public bool AddSubOrderToOrder(string path, string orderID)
        {            
            string e02 = "";
            return OrderController.Instance.AddSubOrderToOrder(e02, orderID);
        }
    }
}
