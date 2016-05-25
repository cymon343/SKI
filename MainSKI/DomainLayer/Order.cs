﻿using System;
using System.Collections.Generic;

namespace DomainLayer
{
    public class Order
    {
        #region Fields
        private string _id; 
        private Customer _customer; 
        private int _orderNumber; 
        private DateTime _deliveryDate; 
        private DateTime _productionDate; 
        private double _cubicMeters; 
        private double _numOfElements; 
        private List<string> _appendixLinks; 
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

        public Customer Customer
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
        private Order()
        {
           //TODO: Determine Constructor.
        }

        private Order (string id, Customer customer, int orderNumber, DateTime deliveryDate, DateTime productionDate, double cubicMeters, double numberOfElements)
        {
            ID = id;
            Customer = customer;
            OrderNumber = orderNumber;
            DeliveryDate = deliveryDate;
            ProductionDate = productionDate;
            CubicMeters = cubicMeters;
            NumOfElements = numberOfElements;
            AppendixLinks = new List<string>();
            Elements = new List<Element>();
            ProdData = new List<ProductionData>();
        }
        #endregion

        #region Methods
        public static Order CreateOrder(string e02)
        {
            //TODO: Make magical code that creates awesome Order.
            return new Order();
        }

        public static Order CreateOrder(string id, Customer customer, int orderNumber, DateTime deliveryDate, DateTime productionDate, double cubicMeters, double numberOfElements)
        {
            return new Order(id, customer, orderNumber, deliveryDate, productionDate, cubicMeters, numberOfElements);
        }

        #endregion
    }
}
