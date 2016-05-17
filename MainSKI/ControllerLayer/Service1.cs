using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace ControllerLayer
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in both code and config file together.
    public class Service1 : IService1
    {
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, UriTemplate = "person/{id}")]
        public Person GetData(string id)
        {
            // lookup person with the requested id 
            return new Person()
            {
                Id = Convert.ToInt32(id),
                Name = "Paul Scholes"
            };
        }
    }


    //Move
    public class Person
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
