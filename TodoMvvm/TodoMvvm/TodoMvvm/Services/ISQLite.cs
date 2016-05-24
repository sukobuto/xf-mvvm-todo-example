using SQLite.Net;

namespace TodoMvvm.Services
{
    public interface ISQLite
    {
        SQLiteConnection GetConnection();
    }
}
