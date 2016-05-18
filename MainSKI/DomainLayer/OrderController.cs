using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer
{
    public class OrderController
    {
        private static OrderController _instance;
        private List<Order> _orders;

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

        private OrderController()
        {
            Orders = new List<Order>();
            //TODO: Maybe load list here from DB.
        }

        public Order CreateOrder(string e02)
        {
            return Order.CreateOrder(e02);
        }

    }
}
