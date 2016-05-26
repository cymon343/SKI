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
        private List<string> _data;
        private string _orderID;

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

        #endregion

        #region Constructors
        public ProductionData()
        {
            Data = new List<string>();
        }

        public ProductionData(string orderID, List<string> data)
        {
            Data = data;
            OrderID = orderID;
        }
        #endregion
    }
}
