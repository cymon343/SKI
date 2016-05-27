using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Persistence;


namespace DomainLayer
{
    public class OrderController
    {
        #region Fields
        private static OrderController _instance;
        private List<Order> _orders;
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
            _orders = DBFacade.Instance.RetrieveAllOrders();
            SortOrders();
        }

        #endregion

        #region Methods

        public bool AddOrder(string e02)
        {
            Order order = Order.CreateOrder(e02);
            if (order != null)
            {
                if (DBFacade.Instance.CreateOrder(order))
                {
                    _orders.Add(order);
                    return true;
                }
            }
            return false;
        }

        public bool AddSubOrderToOrder(string e02, string mainOrderID)
        {
            Order mainOrder = FindOrderByID(mainOrderID);
            if (mainOrder != null)
            {
                Order subOrder = Order.CreateOrder(e02);
                subOrder.MainOrderID = mainOrderID;

                if (DBFacade.Instance.CreateOrder(subOrder))
                {
                    _orders.Add(subOrder);
                    mainOrder.SubOrders.Add(subOrder);
                    return true;
                }
            }
            return false;
        }

        public bool AddSubOrderToOrder(string e02, Order mainOrder)
        {
            if (mainOrder != null)
            {
                Order subOrder = Order.CreateOrder(e02);
                subOrder.MainOrderID = mainOrder.ID;

                if (DBFacade.Instance.CreateOrder(subOrder))
                {
                    _orders.Add(subOrder);
                    mainOrder.SubOrders.Add(subOrder);
                    return true;
                }
            }
            return false;
        }

        public bool AddSubOrderToOrder(Order subOrder, string mainOrderID)
        {
            Order mainOrder = FindOrderByID(mainOrderID);
            if (mainOrder != null)
            {
                subOrder.MainOrderID = mainOrderID;

                if (DBFacade.Instance.CreateOrder(subOrder))
                {
                    _orders.Add(subOrder);
                    mainOrder.SubOrders.Add(subOrder);
                    return true;
                }
            }
            return false;
        }

        public bool AddSubOrderToOrder(Order subOrder, Order mainOrder)
        {
            if (mainOrder != null)
            {
                subOrder.MainOrderID = mainOrder.ID;

                if (DBFacade.Instance.CreateOrder(subOrder))
                {
                    _orders.Add(subOrder);
                    mainOrder.SubOrders.Add(subOrder);
                    return true;
                }
            }
            return false;
        }

        public void AddPictureLinkToOrder(string link, string orderID)
        {
            Link l = new Link(orderID, link); //Missing ID
            Order order = FindOrderByID(orderID);
            if (order != null)
                order.AppendixLinks.Add(l);
        }

        public void AddPictureLinksToOrder(string orderID, params string[] links)
        {
            List<Link> tmpLinkList = new List<Link>();

            foreach (string l in links)
            {
                tmpLinkList.Add(new Link(orderID, l)); //Missing ID
            }

            Order order = FindOrderByID(orderID);
            if (order != null)
            {
                order.AppendixLinks.AddRange(tmpLinkList);
            }
        }

        public void FlipElementBegun(int station, string orderID, string elementID)
        {

            Order order = FindOrderByID(orderID);
            if (order != null)
            {
                Element element = FindElementOnOrderByID(order, elementID);
                if (element != null)
                {
                    if (DBFacade.Instance.UpdateElementProgressStateBegun(elementID, station, !element.ProgressInfo[station].Begun))
                    {
                        element.ProgressInfo[station].Begun = !element.ProgressInfo[station].Begun;
                    }
                }
            }
        }

        public void FlipElementDone(int station, string orderID, string elementID)
        {
            Order order = FindOrderByID(orderID);
            if (order != null)
            {
                Element element = FindElementOnOrderByID(order, elementID);
                if (element != null)
                {
                    if (DBFacade.Instance.UpdateElementProgressStateDone(elementID, station, !element.ProgressInfo[station].Done))
                    {
                        element.ProgressInfo[station].Done = !element.ProgressInfo[station].Done;
                    }
                }
            }
        }

        public void SetElementComment(int station, string orderID, string elementID, string comment)
        {
            Order order = FindOrderByID(orderID);
            if (order != null)
            {
                Element element = FindElementOnOrderByID(order, elementID);
                if (element != null)
                {
                    if (DBFacade.Instance.UpdateElementProgressStateComment(elementID, station, comment))
                    {
                        element.ProgressInfo[station].Comment = comment;
                    }
                }
            }
        }

        public void FlipOrderBegun(int station, string orderID)
        {
            Order order = FindOrderByID(orderID);
            if (order != null)
            {
                if (DBFacade.Instance.UpdateOrderProgressStateBegun(orderID, station, !order.ProgressInfo[station].Begun))
                {
                    order.ProgressInfo[station].Begun = !order.ProgressInfo[station].Begun;
                }
            }

        }

        public void FlipOrderDone(int station, string orderID)
        {
            Order order = FindOrderByID(orderID);
            if (order != null)
            {
                if (DBFacade.Instance.UpdateOrderProgressStateDone(orderID, station, !order.ProgressInfo[station].Done))
                {
                    order.ProgressInfo[station].Done = !order.ProgressInfo[station].Done;
                }
            }
        }

        public void SetOrderComment(int station, string orderID, string comment)
        {
            Order order = FindOrderByID(orderID);
            if (order != null)
            {
                if (DBFacade.Instance.UpdateOrderProgressStateComment(orderID, station, comment))
                {
                    order.ProgressInfo[station].Comment = comment;
                }
            }
        }

        public List<Order> GetOrdersByDeliveryDate(DateTime fromDate)
        {
            List<Order> orders = new List<Order>();

            foreach (Order o in _orders)
            {
                if (o.DeliveryDate > fromDate)
                {
                    orders.Add(o);
                }
            }
            return orders;
        }

        private Order FindOrderByID(string orderID)
        {
            foreach (Order o in _orders)
            {
                if (o.ID == orderID)
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
            if (_orders != null)
                _orders = _orders.OrderBy(o => o.DeliveryDate).ToList<Order>();
        }

        #endregion

    }
}
