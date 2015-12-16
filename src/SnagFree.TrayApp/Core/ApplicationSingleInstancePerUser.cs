using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SnagFree.TrayApp.Core
{
    class ApplicationSingleInstancePerUser: IDisposable
    {
        #region Internal Fields

        private readonly string _name = ApplicationInformation.Name + Environment.UserDomainName;
        private readonly bool _firstInstance;
        private readonly Mutex _mutex;

        #endregion

        #region Public Properties

        /// <summary>
        /// Shows if the current instance of ghost is the first
        /// </summary>
        public bool FirstInstance
        {
            get { return _firstInstance; }
        }

        #endregion

        public ApplicationSingleInstancePerUser()
        {
            try
            {
                //Grab mutex if it exists
                _mutex = Mutex.OpenExisting(_name);
            }
            catch (WaitHandleCannotBeOpenedException)
            {
                _firstInstance = true;
            }

            if (_mutex == null)
            {
                _mutex = new Mutex(false, _name);
            }
        }

        public void Dispose()
        {
            _mutex.Dispose();
        }
    }
}
