using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using TodoMvvm.Models;

namespace TodoMvvm.ViewModels
{
    public class TodoDetailPageViewModel : BindableBase, INavigationAware
    {
        private INavigationService _navService;

        private readonly TodoItemRepository _todoItemRepository = new TodoItemRepository();

        public ReactiveProperty<TodoItem> TodoItem { get; } = new ReactiveProperty<TodoItem>();

        public ReactiveCommand Save { get; } = new ReactiveCommand();

        public ReactiveCommand Delete { get; } = new ReactiveCommand();

        public ReactiveProperty<string> NewItemsText { get; } = new ReactiveProperty<string>();

        public ReactiveCommand AddNewItem { get; private set; }
        
        public ReactiveCollection<TodoItem> TodoItems { get; } = new ReactiveCollection<TodoItem>();

        public ReactiveProperty<TodoItem> SelectedItem { get; } = new ReactiveProperty<TodoItem>();

        public TodoDetailPageViewModel(INavigationService navigationService, IPageDialogService pageDialogService)
        {
            this._navService = navigationService;

            this.Save
                .Subscribe(async _ => {
                    var item = this.TodoItem.Value;
                    await this._todoItemRepository.SaveItemAsync(item);
                    await navigationService.GoBack();
                });

            this.Delete
                .Subscribe(async _ => {
                    var item = this.TodoItem.Value;
                    if (await pageDialogService.DisplayAlert("削除しますか？", item.Text, "はい", "いいえ")) {
                        item.Delete = true;
                        await this._todoItemRepository.SaveItemAsync(item);
                        await navigationService.GoBack();
                    }
                });

            this.AddNewItem = this.NewItemsText
                .Select(x => !string.IsNullOrWhiteSpace(x)) // NewItemsText の状況から bool に変換
                .ToReactiveCommand();

            this.AddNewItem
                .Subscribe(async _ => {
                    var item = new TodoItem {
                        ParentID = this.TodoItem.Value.ID,
                        Text = this.NewItemsText.Value,
                        CreatedAt = DateTime.Now,
                        Delete = false
                    };
                    this.TodoItems.AddOnScheduler(item);
                    this.NewItemsText.Value = "";
                    await this._todoItemRepository.SaveItemAsync(item);
                });

            // アイテムタップ時の動作を定義
            this.SelectedItem
                .Where(item => item != null)
                .Subscribe(item => {
                    this.SelectedItem.Value = null;
                    navigationService.Navigate($"TodoDetailPage?id={item.ID}");
                });
        }

        public void OnNavigatedFrom(NavigationParameters parameters)
        {

        }

        public async void OnNavigatedTo(NavigationParameters parameters)
        {
            Debug.WriteLine("[OnNavigateTo] TodoDetailPageViewModel");
            int id;

            if (parameters.ContainsKey("id")) {
                id = int.Parse(parameters["id"] as string);
                this.TodoItem.Value = await this._todoItemRepository.FindFirstAsync(id);
            } else if (this.TodoItem.Value != null) {
                id = this.TodoItem.Value.ID;
            } else {
                throw new InvalidOperationException("lost parameter 'id' for TodoDatailPage");
            }
            // SQLite からデータを取得
            this.TodoItems.ClearOnScheduler();
            this.TodoItems.AddRangeOnScheduler(await this._todoItemRepository.GetItemsAsync(id));
        }
    }
}
