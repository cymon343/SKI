using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using DomainLayer;

namespace ControllerLayer
{
    [ServiceContract]
    public interface IProductionOrderService
    {
        [OperationContract]
        List<Order> GetOrders();

        [OperationContract]
        void FlipElementBegun(ElementProgressFlipRequest data);

        [OperationContract]
        void FlipElementDone(ElementProgressFlipRequest data);

        [OperationContract]
        void SetElementComment(ElementCommentRequest data);
    }
}

