using TodoMvvm.Services;
using SQLite.Net;
using System.IO;
using SQLite.Net.Platform.XamarinAndroid;
using Xamarin.Forms;
using TodoMvvm.Droid;

[assembly: Dependency(typeof(SQLite_Android))]
namespace TodoMvvm.Droid
{
    public class SQLite_Android : ISQLite
    {
        
        public SQLiteConnection GetConnection()
        {
            const string sqliteFilename = "TodoSQLite.db3";
            var documentsPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            var path = Path.Combine(documentsPath, sqliteFilename);
            var plat = new SQLitePlatformAndroid();
            var conn = new SQLiteConnection(plat, path);
            return conn;
        }

    }
}