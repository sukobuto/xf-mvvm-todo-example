using SQLite.Net.Attributes;
using System;

namespace TodoMvvm.Models
{
    public class TodoItem
    {

        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        public string Text { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool Delete { get; set; }

    }
}
