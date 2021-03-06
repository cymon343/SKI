﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer
{
    [DataContract]
    public class CustomerData
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
        #endregion

        #region Properties
        [DataMember]
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
        [DataMember]
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
        [DataMember]
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
        [DataMember]
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
        [DataMember]
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
        [DataMember]
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
        [DataMember]
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
        [DataMember]
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
        [DataMember]
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

        #region Constructors
        public CustomerData(string id, string name, string address, string deliveryAddress, string email,
                        string phonePrivate, string phoneWork, string phoneCell, string fax)
        {
            Id = id;
            Name = name;
            Address = address;
            DeliveryAddress = deliveryAddress;
            Email = email;
            PhonePrivate = phonePrivate;
            PhoneWork = phoneWork;
            PhoneCell = phoneCell;
            Fax = fax;
        }
        #endregion

        #region Methods

        public override string ToString()
        {
            return "Name & Address: " + Name + " - " + Address + "\nDelivery Address: " + DeliveryAddress; 
        }
        #endregion
    }
}
