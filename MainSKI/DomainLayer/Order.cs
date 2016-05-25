using System;
using System.Collections.Generic;

namespace DomainLayer
{
    public class Order
    {
        #region Fields
        private string _id;
        private CustomerData _customer;
        private int _orderNumber;
        private DateTime _deliveryDate;
        private DateTime _productionDate;
        private double _cubicMeters;
        private double _numOfElements;
        private List<Link> _appendixLinks;
        private string _mainOrderID;
        private List<Order> _subOrders;
        private List<Element> _elements;
        private ProgressState[] _progressInfo;
        private List<ProductionData> _prodData;


        #endregion

        #region Properties
        public string ID
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

        public List<Link> AppendixLinks
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

        public CustomerData Customer
        {
            get
            {
                return _customer;
            }

            set
            {
                _customer = value;
            }
        }

        public string MainOrderID
        {
            get
            {
                return _mainOrderID;
            }

            set
            {
                _mainOrderID = value;
            }
        }

        #endregion

        #region Constructors     
        private Order(string _id, CustomerData _customer, int _orderNumber, DateTime _deliveryDate, DateTime _productionDate, double _cubicMeters, double _numOfElements, List<Link> _appendixLinks, string _mainOrderID, List<Order> _subOrders, List<Element> _elements, ProgressState[] _progressInfo, List<ProductionData> _prodData)
        {
            this._id = _id;
            this._customer = _customer;
            this._orderNumber = _orderNumber;
            this._deliveryDate = _deliveryDate;
            this._productionDate = _productionDate;
            this._cubicMeters = _cubicMeters;
            this._numOfElements = _numOfElements;
            this._appendixLinks = _appendixLinks;
            this._mainOrderID = _mainOrderID;
            this._subOrders = _subOrders;
            this._elements = _elements;
            this._progressInfo = _progressInfo;
            this._prodData = _prodData;
        }
        #endregion

        #region Methods
        public static Order CreateOrder(string e02)
        {
            //TODO: Make magical code that creates awesome Order.
            //NB: Make sure that arraySize of _progressInfo is 4.
            return null;
        }

        public static Order CreateOrder(string _id, CustomerData _customer, int _orderNumber, DateTime _deliveryDate, DateTime _productionDate, double _cubicMeters, double _numOfElements, List<Link> _appendixLinks, string _mainOrderID, List<Order> _subOrders, List<Element> _elements, ProgressState[] _progressInfo, List<ProductionData> _prodData)
        {
            return new Order(_id, _customer, _orderNumber, _deliveryDate, _productionDate, _cubicMeters, _numOfElements, _appendixLinks, _mainOrderID, _subOrders, _elements, _progressInfo, _prodData);
        }

        #endregion
    }
}
