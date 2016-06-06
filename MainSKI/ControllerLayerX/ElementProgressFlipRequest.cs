using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace ControllerLayer
{
    [DataContract]
    public class ElementProgressFlipRequest
    {
        [DataMember]
        public string OrderID { get; set; }
        [DataMember]
        public string ElementID { get; set; }
        [DataMember]
        public int StationNumber { get; set; }
    }
}