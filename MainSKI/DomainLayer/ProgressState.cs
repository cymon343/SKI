using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer
{
    public class ProgressState
    {
        private string _comment;
        private bool _begun;
        private bool _done;

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
        #endregion

        public ProgressState()
        {
            _comment = "";
            _begun = false;
            _done = false;
        }

        public ProgressState(string comment, bool begun, bool done)
        {
            Comment = comment;
            Begun = begun;
            Done = done; 
        }
    }
}
