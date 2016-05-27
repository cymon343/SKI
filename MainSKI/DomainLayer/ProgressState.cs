using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer
{
    [DataContract]
    public class ProgressState
    {
        #region fields
        private string _id;
        private string _comment;
        private bool _begun;
        private bool _done;
        private string _parentID;
        private int _stationNumber;

        #endregion

        #region properties
        [DataMember]
        public string Comment
        {
            get
            {
                return _comment;
            }

            set
            {
                _comment = value;
            }
        }
        [DataMember]
        public bool Begun
        {
            get
            {
                return _begun;
            }

            set
            {
                _begun = value;
            }
        }
        [DataMember]
        public bool Done
        {
            get
            {
                return _done;
            }

            set
            {
                _done = value;
            }
        }
        [DataMember]
        public string ParentID
        {
            get
            {
                return _parentID;
            }

            set
            {
                _parentID = value;
            }
        }
        [DataMember]
        public int StationNumber
        {
            get
            {
                return _stationNumber;
            }

            set
            {
                _stationNumber = value;
            }
        }
        [DataMember]
        public string ID
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
        #endregion

        #region Constructors

        public ProgressState(string ID, string parentID, string comment, bool begun, bool done, int stationNumber)
        {
            this.ID = ID;
            ParentID = parentID;
            Comment = comment;
            Begun = begun;
            Done = done;
            StationNumber = stationNumber;
        }

        #endregion
    }
}
