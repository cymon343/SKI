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
        string _id;
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
        [DataMember]
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
        #endregion

        #region Constructors
        public Link(string ID, string orderID, string theLink)
        {
            this.ID = ID;
            OrderID = orderID;
            TheLink = theLink;
        }

        #endregion

    }
}
