using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer
{
    public class Customer
    {
        #region Fields
        private string _id;
        private string _name;
        private string _address;
        private string _deliveryAddress;
        private string _email;
        private string _phonePrivate;
        private string _phoneWork;
        private string _phoneCell;
        private string _fax;
        List<Order> _orders;
        #endregion

        #region Properties
        public string Id
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

        public string Name
        {
            get
            {
                return _name;
            }

            set
            {
                _name = value;
            }
        }

        public string Address
        {
            get
            {
                return _address;
            }

            set
            {
                _address = value;
            }
        }

        public string DeliveryAddress
        {
            get
            {
                return _deliveryAddress;
            }

            set
            {
                _deliveryAddress = value;
            }
        }

        public string Email
        {
            get
            {
                return _email;
            }

            set
            {
                _email = value;
            }
        }

        public string PhonePrivate
        {
            get
            {
                return _phonePrivate;
            }

            set
            {
                _phonePrivate = value;
            }
        }

        public string PhoneWork
        {
            get
            {
                return _phoneWork;
            }

            set
            {
                _phoneWork = value;
            }
        }

        public string PhoneCell
        {
            get
            {
                return _phoneCell;
            }

            set
            {
                _phoneCell = value;
            }
        }

        public string Fax
        {
            get
            {
                return _fax;
            }

            set
            {
                _fax = value;
            }
        }
        #endregion

        public Customer(string id, string name, string address, string deliveryAddress, string email,
                        string phonePrivate, string phoneWork, string phoneCell, string fax)
        {
            Id = id;
            Name = name;
            Address = address;
            DeliveryAddress = address;
            Email = email;
            PhonePrivate = phonePrivate;
            PhoneWork = phoneWork;
            PhoneCell = phoneCell;
            Fax = fax;
        }
    }
}
