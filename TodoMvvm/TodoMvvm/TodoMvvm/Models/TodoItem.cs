using Prism.Mvvm;
using SQLite.Net.Attributes;
using System;

namespace TodoMvvm.Models
{
    public class TodoItem : BindableBase
    {

        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }

        private string text;
        public string Text {
            get { return text; }
            set { SetProperty(ref text, value); }
        }

        private string note;
        public string Note {
            get { return note; }
            set { SetProperty(ref note, value); }
        }

        private DateTime createdAt;
        public DateTime CreatedAt {
            get { return createdAt; }
            set { SetProperty(ref createdAt, value); }
        }

        private bool delete;
        public bool Delete {
            get { return delete; }
            set { SetProperty(ref delete, value); }
        }

    }
}
