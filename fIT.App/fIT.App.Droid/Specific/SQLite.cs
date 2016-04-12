using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using fIT.App.Droid.Specific;
using fIT.App.Interfaces;
using SQLite.Net;
using SQLite.Net.Async;
using SQLite.Net.Platform.XamarinAndroid;
using Xamarin.Forms;

[assembly: Dependency(typeof(fIT.App.Droid.Specific.SQLite))]
namespace fIT.App.Droid.Specific
{
  public class SQLite:ISqlLite
  {
    public SQLiteConnection GetConnection()
    {
      var path = this.GetDBPath();
      // Create the connection
      var conn = new SQLiteConnection(new SQLitePlatformAndroid(), path);
      // Return the database connection
      return conn;
    }

    public SQLiteAsyncConnection GetAsyncConnection()
    {
      var path = this.GetDBPath();
      var connectionFactory = new Func<SQLiteConnectionWithLock>(() => new SQLiteConnectionWithLock(new SQLitePlatformAndroid(), new SQLiteConnectionString(path, storeDateTimeAsTicks: false)));
      return new SQLiteAsyncConnection(connectionFactory);
    }

    private string GetDBPath()
    {
      var sqliteFilename = "fIt-Db.db3";
      string documentsPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal); // Documents folder
      return Path.Combine(documentsPath, sqliteFilename);
    }
  }
}