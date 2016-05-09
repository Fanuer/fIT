using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using fIT.App.Data;
using fIT.App.Helpers;
using fIT.App.Repositories;
using fIT.WebApi.Client.Portable.Implementation;

namespace fIT.App.Services
{
    internal class OnOffService
    {
        #region FIELDS

        private static OnOffService _current;
        private bool _stop;
        private bool _currentStatus = true;
        private readonly object _lock;
        private int _interval;
        #endregion

        #region Events
        public event EventHandler<ChangedOnlineStateEventArgs> OnStatusChanged;
        #endregion

        #region CTOR
        private OnOffService()
        {
            _lock = new object();
            this.Interval = 10000;
        }
        #endregion

        #region METHODS

        public void Start()
        {
            lock (_lock)
            {
                this._stop = false;
            }

            Task.Factory.StartNew(CheckConnection, TaskCreationOptions.LongRunning);
        }

        public void Stop()
        {
            lock (this._lock)
            {
                this._stop = true;
            }
        }

        private async Task CheckConnection()
        {
            while (!this._stop)
            {
                try
                {
                    var status = await ServerRespository.Current.Server.PingAsync();
                    if (status != _currentStatus)
                    {
                        lock (_lock)
                        {
                            this.OnStatusChanged?.Invoke(this, new ChangedOnlineStateEventArgs(status));
                            this._currentStatus = status;
                        }
                    }
                    Debug.WriteLine("Connection checked");
                    await Task.Delay(this.Interval);
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Ping-Error: " + e.Message);
                }

            }
        }
        #endregion

        #region PROPERTIES

        public static OnOffService Current
        {
            get
            {
                if (OnOffService._current == null)
                {
                    OnOffService._current = new OnOffService();
                }
                return OnOffService._current;
            }
        }

        /// <summary>
        /// Ping-Interval in ms
        /// </summary>
        public int Interval
        {
            get
            {
                return _interval;

            }
            set
            {
                if (value > 0)
                {
                    _interval = value;
                }
            }
        }
        #endregion
    }
}
