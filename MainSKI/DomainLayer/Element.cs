using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer
{
    public class Element
    {
        #region Fields
        private string _id;
        private string _orderID;
        private string _position;
        private string _text;
        private string _hinge;
        private string _fin;
        private double _amount;
        private string _unit;
        private string _heading;
        private ProgressState[] _progressInfo;

        #endregion

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

        public string Position
        {
            get
            {
                return _position;
            }

            set
            {
                _position = value;
            }
        }

        public string Text
        {
            get
            {
                return _text;
            }

            set
            {
                _text = value;
            }
        }

        public string Hinge
        {
            get
            {
                return _hinge;
            }

            set
            {
                _hinge = value;
            }
        }

        public string Fin
        {
            get
            {
                return _fin;
            }

            set
            {
                _fin = value;
            }
        }

        public double Amount
        {
            get
            {
                return _amount;
            }

            set
            {
                _amount = value;
            }
        }

        public string Unit
        {
            get
            {
                return _unit;
            }

            set
            {
                _unit = value;
            }
        }

        public string Heading
        {
            get
            {
                return _heading;
            }

            set
            {
                _heading = value;
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

        public string OrderID
        {
            get
            {
                return _orderID;
            }

            set
            {
                _orderID = value;
            }
        }
        #endregion

        #region Constructors
        public Element(string id, string orderID, string position, string text, string hinge, string fin, double amount, string unit, string heading, ProgressState[] progressInfo)
        {
            Id = id;
            Position = position;
            Text = text;
            Hinge = hinge;
            Fin = fin;
            Amount = amount;
            Unit = unit;
            Heading = heading;
            OrderID = orderID;
            ProgressInfo = progressInfo;
        }

        public Element()
        {
            //TODO: Make magical code that creates awesome Element.
            //NB: Make sure that arraySize of _progressInfo is 4.
        }

        #endregion
    }
}
