using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer
{
    [DataContract]
    public class Link
    {
        #region Fields
        string _orderID;
        string _theLink;
        #endregion

        #region Properties
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
        [DataMember]
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
            OrderID = orderID;
            TheLink = theLink;
        }

        #endregion

    }
}
