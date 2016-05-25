using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer
{
    public class Link
    {
        #region Fields
        string _orderID;
        string _theLink;
        #endregion

        #region Properties
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

        public string TheLink
        {
            get
            {
                return _theLink;
            }

            set
            {
                _theLink = value;
            }
        }
        #endregion

        #region Constructors
        public Link(string orderID, string theLink)
        {
            OrderID = _orderID;
            TheLink = _theLink;
        }

        #endregion

    }
}
