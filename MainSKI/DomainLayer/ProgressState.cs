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
        #endregion

        #region Constructors

        public ProgressState(string parentID, string comment, bool begun, bool done)
        {
            Id = id;
            ParentID = parentID;
            Comment = comment;
            Begun = begun;
            Done = done;
        }

        #endregion
    }
}
