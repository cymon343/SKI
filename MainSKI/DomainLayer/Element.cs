using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer
{
    [DataContract]
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
        [DataMember]
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
        [DataMember]
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
        [DataMember]
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
        [DataMember]
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
        [DataMember]
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
        [DataMember]
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
        [DataMember]
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
        [DataMember]
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
        [DataMember]
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
        [DataMember]
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

        #endregion

        #region Methods

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Element ID: " + Id + " Order ID: " + OrderID + " - Heading: " + Heading);
            sb.AppendLine("TEXT:\n" + Text);
            sb.AppendLine("Pos: " + Position + " - Hinge: " + Hinge + " - Finish: " + Fin + " - Amount: " + Amount + " " + Unit);
            return sb.ToString();
        }
        #endregion
    }
}
