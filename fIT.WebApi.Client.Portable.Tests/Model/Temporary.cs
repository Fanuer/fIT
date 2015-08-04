using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fIT.WebApi.Client.Portable.Tests.Model
{
    public class Temporary<T> : IDisposable
    {
        #region Field
        private readonly Action<T> disposeAction;

        #endregion

        #region Ctor

        public Temporary(T model, Action<T> disposeAction)
        {
            this.Model = model;
            this.disposeAction = disposeAction;
        }

        #endregion

        #region Methods
        public virtual void Dispose()
        {
            try
            {
                disposeAction(Model);
            }
            catch (Exception e)
            {
                Console.WriteLine("Unable to dispose ... " + e);
            }
        }

        #endregion

        #region Properties
        public T Model { get; private set; }

        #endregion
    }
}
