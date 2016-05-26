using SQLite.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TodoMvvm.Services;
using Xamarin.Forms;

namespace TodoMvvm.Models
{
    public class TodoItemRepository
    {
        static readonly object Locker = new object();
        readonly SQLiteConnection _db;

        public TodoItemRepository()
        {
            _db = DependencyService.Get<ISQLite>().GetConnection();
            _db.CreateTable<TodoItem>();
        }

        public async Task<IEnumerable<TodoItem>> GetItemsAsync(int parentID = 0)
        {
            return await Task.Run<IEnumerable<TodoItem>>(() => {
                lock (Locker) {
                    return _db.Table<TodoItem>()
                        .Where(m => m.Delete == false && m.ParentID == parentID);
                }
            });
        }

        public async Task<TodoItem> FindFirstAsync(int id)
        {
            return await Task.Run<TodoItem>(() => {
                lock (Locker) {
                    return _db.Table<TodoItem>()
                        .First(m => m.ID == id && m.Delete == false);
                }
            });
        }

        public async Task<int> SaveItemAsync(TodoItem item)
        {
            return await Task.Run<int>(() => {
                lock (Locker) {
                    if (item.ID != 0) {
                        _db.Update(item);
                        return item.ID;
                    }
                    return _db.Insert(item);
                }
            });
        }

        public async Task<int> DeleteItemAsync(TodoItem item)
        {
            return await Task.Run<int>(() => {
                lock (Locker) {
                    return _db.Delete(item);
                }
            });
        }
    }
}
