using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using TodoMvvm.Models;

namespace TodoMvvm.ViewModels
{
    public class MainPageViewModel : BindableBase, INavigationAware
    {
        private readonly TodoItemRepository _todoItemRepository = new TodoItemRepository();

        private string _title;
        public string Title {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        public ReactiveProperty<string> NewItemsText { get; } = new ReactiveProperty<string>();

        public ReactiveCommand AddNewItem { get; private set; }

        public ReactiveCollection<TodoItem> TodoItems { get; } = new ReactiveCollection<TodoItem>();

        public ReactiveProperty<TodoItem> SelectedItem { get; } = new ReactiveProperty<TodoItem>();

        public MainPageViewModel(IPageDialogService pageDialogService)
        {
            this.AddNewItem = this.NewItemsText
                .Select(x => !string.IsNullOrWhiteSpace(x)) // NewItemsText の状況から bool に変換
                .ToReactiveCommand();

            this.AddNewItem
                .Subscribe(async _ => {
                    var item = new TodoItem { Text = this.NewItemsText.Value, CreatedAt = DateTime.Now, Delete = false };
                    this.TodoItems.AddOnScheduler(item);
                    this.NewItemsText.Value = "";
                    await this._todoItemRepository.SaveItemAsync(item);
                });

            // アイテムタップ時の動作を定義
            this.SelectedItem
                .Where(item => item != null)
                .Subscribe(async item => {
                    if (await pageDialogService.DisplayAlert("削除します。よろしいですか？", item.Text, "はい", "いいえ")) {
                        item.Delete = true;
                        this.TodoItems.RemoveOnScheduler(item);
                        await _todoItemRepository.SaveItemAsync(item);
                    }
                    this.SelectedItem.Value = null;
                });
        }

        public void OnNavigatedFrom(NavigationParameters parameters)
        {

        }

        public async void OnNavigatedTo(NavigationParameters parameters)
        {
            if (parameters.ContainsKey("title"))
                Title = (string)parameters["title"] + " and Prism";
            // SQLite からデータを取得
            this.TodoItems.AddRangeOnScheduler(await this._todoItemRepository.GetItemsAsync());
        }
    }
}
