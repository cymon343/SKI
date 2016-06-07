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

        [WebInvoke(Method = "POST",
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped,
            UriTemplate = "flipElementBegun")]
        public void FlipElementBegun(ElementProgressFlipRequest data)
        {
            string orderID = data.OrderID;
            string elementID = data.ElementID;
            int stationNumber = data.StationNumber;
            OrderController.Instance.FlipElementBegun(stationNumber, orderID, elementID);
        }

        [WebInvoke(Method = "POST",
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped,
            UriTemplate = "flipElementDone")]
        public void FlipElementDone(ElementProgressFlipRequest data)
        {
            string orderID = data.OrderID;
            string elementID = data.ElementID;
            int stationNumber = data.StationNumber;
            OrderController.Instance.FlipElementDone(stationNumber, orderID, elementID);
        }

        [WebInvoke(Method = "GET",
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped,
            UriTemplate = "getOrders")]
        public List<Order> GetOrders()
        {
            DateTime fromDate = DateTime.Now;
            fromDate = fromDate.AddMonths(FROM_DATE_MONTH_OFFSET);
            fromDate = fromDate.AddYears(-5); //TODO: DELETE THIS LINE !! (TEST)            
            return OrderController.Instance.GetOrdersByDeliveryDate(fromDate);
        }        

        [WebInvoke(Method = "POST",
        ResponseFormat = WebMessageFormat.Json,
        RequestFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.Wrapped,
        UriTemplate = "setElementComment")]
        public void SetElementComment(ElementCommentRequest data)
        {            
            string orderID = data.OrderID;
            string elementID = data.ElementID;
            int stationNumber = data.StationNumber;
            string comment = data.Comment;

            OrderController.Instance.SetElementComment(stationNumber, orderID, elementID, comment);
        }
    }
}
