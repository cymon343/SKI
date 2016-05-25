using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer
{
    public class ProgressState
    {
        #region fields
        private string _comment;
        private bool _begun;
        private bool _done;
        private string _parentID;
        private int _stationNumber;

        #endregion

        #region properties
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
        #endregion

        #region Constructors

        public ProgressState(string parentID, string comment, bool begun, bool done, int stationNumber)
        {
            ParentID = parentID;
            Comment = comment;
            Begun = begun;
            Done = done;
            StationNumber = stationNumber;
        }

        #endregion
    }
}
