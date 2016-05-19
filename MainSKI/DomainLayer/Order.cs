using System;
using System.Collections.Generic;

namespace DomainLayer
{
    public class Order
    {
        private string _id;
        private int _orderNumber;
        private DateTime _deliveryDate;
        private DateTime _productionDate;
        private double _cubicMeters;
        private double _numOfElements;
        private List<string> _appendixLinks;
        private List<Order> _subOrders;
        private List<Element> _elements;
        private ProgressState[] _progressInfo;
        private List<ProductionData> _prodData;

        #region Properties
        public string Id
        {
            get
            {
                return _id;
            }

            set
            {
                _id = value;
            }
        }

        public int OrderNumber
        {
            get
            {
                return _orderNumber;
            }

            set
            {
                _orderNumber = value;
            }
        }

        public DateTime DeliveryDate
        {
            get
            {
                return _deliveryDate;
            }

            set
            {
                _deliveryDate = value;
            }
        }

        public DateTime ProductionDate
        {
            get
            {
                return _productionDate;
            }

            set
            {
                _productionDate = value;
            }
        }

        public double CubicMeters
        {
            get
            {
                return _cubicMeters;
            }

            set
            {
                _cubicMeters = value;
            }
        }

        public double NumOfElements
        {
            get
            {
                return _numOfElements;
            }

            set
            {
                _numOfElements = value;
            }
        }

        public List<string> AppendixLinks
        {
            get
            {
                return _appendixLinks;
            }

            set
            {
                _appendixLinks = value;
            }
        }

        public List<Order> SubOrders
        {
            get
            {
                return _subOrders;
            }

            set
            {
                _subOrders = value;
            }
        }

        public List<Element> Elements
        {
            get
            {
                return _elements;
            }

            set
            {
                _elements = value;
            }
        }

        public ProgressState[] ProgressInfo
        {
            get
            {
                return _progressInfo;
            }

            set
            {
                _progressInfo = value;
            }
        }

        public List<ProductionData> ProdData
        {
            get
            {
                return _prodData;
            }

            set
            {
                _prodData = value;
            }
        }

        #endregion

        private Order()
        {
           //TODO: Determine Constructor.
        }

        public static Order CreateOrder(string e02)
        {
            //TODO: Make magical code that creates awesome Order.
            return new Order();
        }
        
    }
}
