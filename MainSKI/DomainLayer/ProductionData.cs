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

        #endregion

        public ProductionData()
        {
            Data = new List<string>();
        }

        public ProductionData(List<string> data)
        {
            Data = data;
        }

        
    }
}
