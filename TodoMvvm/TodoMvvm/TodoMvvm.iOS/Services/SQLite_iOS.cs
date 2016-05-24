using SQLite.Net;
using SQLite.Net.Platform.XamarinIOS;
using System;
using System.IO;
using TodoMvvm.iOS;
using TodoMvvm.Services;
using Xamarin.Forms;

[assembly: Dependency(typeof(SQLite_iOS))]
namespace TodoMvvm.iOS
{
    public class SQLite_iOS : ISQLite
    {
        public SQLiteConnection GetConnection()
        {
            const string sqliteFilename = "TodoSQLite.db3";
            var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);

            var libraryPath = Path.Combine(documentsPath, "..", "Library");
            var path = Path.Combine(libraryPath, sqliteFilename);
            var plat = new SQLitePlatformIOS();
            var conn = new SQLiteConnection(plat, path);
            return conn;
        }
    }
}
