using System;

namespace fIT.WebApi.Client.Portable.Tests.Model
{
  public class Temporary<TCreate, T> : IDisposable
  {
    private readonly Action<T> disposeAction;
    public TCreate CreateModel { get; private set; }
    public T Model { get; private set; }

    public Temporary(TCreate createModel, T model, Action<T> disposeAction)
    {
      this.CreateModel = createModel;
      this.Model = model;
      this.disposeAction = disposeAction;
    }

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
  }

}
