using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer
{
    public class ProductionData
    {
        private List<string> _data;
        private string _orderID;

        #region Properties

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

        public ProductionData()
        {
            Data = new List<string>();
        }

        public ProductionData(string orderID, List<string> data)
        {
            Data = data;
            OrderID = orderID;
        }
    }
}
