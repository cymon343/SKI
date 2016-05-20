using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DomainLayer
{
    public class OrderController
    {
        #region Fields
        private static OrderController _instance;
        private List<Order> _orders;
        private const int FROM_DATE_MONTH_OFFSET = -2; //TODO: Save this in properties file.

        #endregion

        #region Properties
        public static OrderController Instance
        {
            get
            {
                if (_instance == null)
                    Instance = new OrderController(); //Using "setter" in case stuff needs to happen.
                return _instance;
            }
            private set { _instance = value; }
        }

        public List<Order> Orders { get { return _orders; } private set { _orders = value; } }

        #endregion

        #region Constructor
        private OrderController()
        {
            DateTime fromDate = DateTime.Now;
            fromDate.AddMonths(FROM_DATE_MONTH_OFFSET);
            _orders = GetOrdersByDeliveryDate(fromDate);
            SortOrders();
        }

        #endregion

        #region Methods

        public bool AddOrder(string e02)
        {
            Order order = Order.CreateOrder(e02);
            if (order != null)
            {
                //if(DB)
                _orders.Add(order);
                return true;
            }
            return false;
        }

        public bool AddSubOrderToOrder(string e02, string orderID)
        {
            Order subOrder = Order.CreateOrder(e02);
            //TODO: Consider/discuss if Suborders in orders should be accessed via a "Add" method instead.
            Order order = FindOrderByID(orderID);
            if(order != null)
            {
                order.SubOrders.Add(subOrder);
                return true;
            }
            return false;
        }

        public void AddPictureLinkToOrder(string link, string orderID)
        {
            Order order = FindOrderByID(orderID);
            if (order != null)
                order.AppendixLinks.Add(link);
        }

        public void AddPictureLinksToOrder(string orderID, params string[] links)
        {
            Order order = FindOrderByID(orderID);
            if (order != null)
            {
                order.AppendixLinks.AddRange(links);
            }
        }

        public void FlipElementBegun(int station, string orderID, string elementID)
        {
            if(true) //TODO: Insert call to DBHandler.
            {
                Order order = FindOrderByID(orderID);
                if(order != null)
                {
                    Element element = FindElementOnOrderByID(order, elementID);
                    if(element != null)
                    {
                        element.ProgressInfo[station].Begun = !element.ProgressInfo[station].Begun;
                    }
                }
            }
        }

        public void FlipElementDone(int station, string orderID, string elementID)
        {
            if (true) //TODO: Insert call to DBHandler.
            {
                Order order = FindOrderByID(orderID);
                if (order != null)
                {
                    Element element = FindElementOnOrderByID(order, elementID);
                    if (element != null)
                    {
                        element.ProgressInfo[station].Done = !element.ProgressInfo[station].Done;
                    }
                }
            }
        }

        public void SetElementComment(int station, string orderID, string elementID, string comment)
        {
            if (true) //TODO: Insert call to DBHandler.
            {
                Order order = FindOrderByID(orderID);
                if (order != null)
                {
                    Element element = FindElementOnOrderByID(order, elementID);
                    if (element != null)
                    {
                        element.ProgressInfo[station].Comment = comment;
                    }
                }
            }
        }

        public void FlipOrderBegun(int station, string orderID)
        {
            if (true) //TODO: Insert call to DBHandler.
            {
                Order order = FindOrderByID(orderID);
                if(order != null)
                {
                    order.ProgressInfo[station].Begun = !order.ProgressInfo[station].Begun;
                }
            }
        }

        public void FlipOrderDone(int station, string orderID)
        {
            if (true) //TODO: Insert call to DBHandler.
            {
                Order order = FindOrderByID(orderID);
                if (order != null)
                {
                    order.ProgressInfo[station].Done = !order.ProgressInfo[station].Done;
                }
            }
        }

        public void SetOrderComment(int station, string orderID, string comment)
        {
            if (true) //TODO: Insert call to DBHandler.
            {
                Order order = FindOrderByID(orderID);
                if (order != null)
                {
                    order.ProgressInfo[station].Comment = comment;
                }
            }
        }

        public List<Order> GetOrdersByDeliveryDate(DateTime fromDate)
        {
            List<Order> orders = new List<Order>();

            foreach(Order o in _orders)
            {
                if(o.DeliveryDate > fromDate)
                {
                    orders.Add(o);
                }
            }
            return orders;
        }

        private Order FindOrderByID(string orderID)
        {
            foreach(Order o in _orders)
            {
                if (o.Id == orderID)
                    return o;
            }
            return null;
        }

        private Element FindElementOnOrderByID(Order order, string elementID)
        {
            foreach (Element e in order.Elements)
            {
                if (e.Id == elementID)
                    return e;
            }
            return null;
        }

        private void SortOrders()
        {
            if(_orders != null)
                _orders = _orders.OrderBy(o => o.DeliveryDate).ToList<Order>();
        }

        #endregion

    }
}
