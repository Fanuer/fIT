using SQLite.Net;
using SQLite.Net.Async;

namespace fIT.App.Interfaces
{
  public interface ISqlLite
  {
    SQLiteConnection GetConnection();

    SQLiteAsyncConnection GetAsyncConnection();
  }
}