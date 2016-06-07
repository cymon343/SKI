using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using DomainLayer;

namespace ControllerLayer
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface IOfficeOrderService
    {
        [OperationContract]
        bool AddOrder(string path);

        [OperationContract]
        bool AddSubOrderToOrder(string path, string orderID);
    }
}
