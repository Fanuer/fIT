using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using fIT.App.iOS.Specific;
using fIT.App.Interfaces;
using SQLite.Net;
using SQLite.Net.Async;
using SQLite.Net.Platform.XamarinIOS;
using Xamarin.Forms;

[assembly: Dependency(typeof(fIT.App.iOS.Specific.SQLite))]
namespace fIT.App.iOS.Specific
{
  public class SQLite:ISqlLite
  {


    public SQLiteConnection GetConnection()
    {
      var path = this.GetDBPath();
      // Create the connection
      var conn = new SQLiteConnection(new SQLitePlatformIOS(), path);
      // Return the database connection
      return conn;
    }

    public SQLiteAsyncConnection GetAsyncConnection()
    {
      var path = this.GetDBPath();
      var connectionFactory = new Func<SQLiteConnectionWithLock>(() => new SQLiteConnectionWithLock(new SQLitePlatformIOS(), new SQLiteConnectionString(path, storeDateTimeAsTicks: false)));
      return  new SQLiteAsyncConnection(connectionFactory);
    }

    private string GetDBPath()
    {
      var sqliteFilename = "fIt-Db.db3";
      string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal); // Documents folder
      string libraryPath = Path.Combine(documentsPath, "..", "Library"); // Library folder
      var path = Path.Combine(libraryPath, sqliteFilename);
      return path;
    }
  }
}
