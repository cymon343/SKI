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
    public class ProductionOrderService : IProductionOrderService
    {
        #region Fields
        private const int FROM_DATE_MONTH_OFFSET = -2; //TODO: Save this in properties file.
        #endregion

        [WebInvoke(Method = "GET", 
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped,
            UriTemplate = "flipElementBegun/{orderID}/{elementID}/{stationNumber}")]
        public void FlipElementBegun(string orderID, string elementID, string stationNumber)
        {
            OrderController.Instance.FlipElementBegun(int.Parse(stationNumber), orderID, elementID);
        }

        [WebInvoke(Method = "GET", 
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped,
            UriTemplate = "flipElementDone/{orderID}/{elementID}/{stationNumber}")]
        public void FlipElementDone(string orderID, string elementID, string stationNumber)
        {
            OrderController.Instance.FlipElementDone(int.Parse(stationNumber), orderID, elementID);
        }

        [WebInvoke(Method = "GET", 
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped,
            UriTemplate = "flipOrderBegun/{orderID}/{stationNumber}")]
        public void FlipOrderBegun(string orderID, string stationNumber)
        {
            OrderController.Instance.FlipOrderBegun(int.Parse(stationNumber), orderID);
        }

        [WebInvoke(Method = "GET", 
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped,
            UriTemplate = "flipOrderDone/{orderID}/{stationNumber}")]
        public void FlipOrderDone(string orderID, string stationNumber)
        {
            OrderController.Instance.FlipOrderDone(int.Parse(stationNumber), orderID);
        }

        [WebInvoke(Method = "GET", 
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped,
            UriTemplate = "getOrders")]
        public List<Order> GetOrders()
        {
            DateTime fromDate = DateTime.Now;
            fromDate = fromDate.AddMonths(FROM_DATE_MONTH_OFFSET);
            return OrderController.Instance.GetOrdersByDeliveryDate(fromDate);
        }
        
        [WebInvoke(Method = "GET", 
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped,
            UriTemplate = "setElementComment/{orderID}/{elementID}/{stationNumber}/{comment}")]
        public void SetElementComment(string orderID, string elementID, string stationNumber, string comment)
        {
            OrderController.Instance.SetElementComment(int.Parse(stationNumber), orderID, elementID, comment);
        }

        [WebInvoke(Method = "GET", 
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped,
            UriTemplate = "setOrderComment/{orderID}/{stationNumber}/{comment}")]
        public void SetOrderComment(string orderID, string stationNumber, string comment)
        {
            OrderController.Instance.SetOrderComment(int.Parse(stationNumber), orderID, comment);
        }

        [WebInvoke(Method = "GET", 
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped,
            UriTemplate = "serviceTest/{test}")]
        public string ServiceTest(string test)
        {
            return "Test Text: " + test;
        }
    }
}
