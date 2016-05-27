using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer
{
    [DataContract]
    public class ProductionData
    {
        #region Fields
        private string _id;
        private List<string> _data;
        private string _orderID;
        #endregion

        #region Properties
        [DataMember]
        public List<string> Data
        {
            get
            {
                return _data;
            }

            set
            {
                _data = value;
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
        public ProductionData(string ID, string orderID, List<string> data)
        {
            this.ID = ID;
            Data = data;
            OrderID = orderID;
        }
        #endregion
    }
}
