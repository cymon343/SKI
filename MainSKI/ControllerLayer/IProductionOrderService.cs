using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using DomainLayer;

namespace ControllerLayer
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IProductionOrderService" in both code and config file together.
    [ServiceContract]
    public interface IProductionOrderService
    {
        [OperationContract]
        List<Order> GetOrders();

        [OperationContract]
        void FlipOrderBegun(string orderID, string stationNumber);

        [OperationContract]
        void FlipOrderDone(string orderID, string stationNumber);

        [OperationContract]
        void SetOrderComment(string orderID, string stationNumber, string comment);

        [OperationContract]
        void FlipElementBegun(string orderID, string elementID, string stationNumber);

        [OperationContract]
        void FlipElementDone(string orderID, string elementID, string stationNumber);

        [OperationContract]
        void SetElementComment(string orderID, string elementID, string stationNumber, string comment);

        [OperationContract]
        string ServiceTest(string test);

    }
}
