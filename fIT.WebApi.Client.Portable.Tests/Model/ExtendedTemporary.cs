using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fIT.WebApi.Client.Portable.Tests.Model
{
    public class ExtendedTemporary<TCreate, T> : IDisposable
    {
        #region Field
        private readonly Action<T> disposeAction;

        #endregion

        #region Ctor

        public ExtendedTemporary(TCreate createModel, T model, Action<T> disposeAction)
        {
            this.CreateModel = createModel;
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
        public TCreate CreateModel { get; private set; }
        public T Model { get; private set; }

        #endregion
    }
}
