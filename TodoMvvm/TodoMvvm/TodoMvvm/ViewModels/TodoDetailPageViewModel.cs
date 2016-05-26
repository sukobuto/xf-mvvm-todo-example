using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
        }

        public void OnNavigatedFrom(NavigationParameters parameters)
        {

        }

        public async void OnNavigatedTo(NavigationParameters parameters)
        {
            Debug.WriteLine("[OnNavigateTo] TodoDetailPageViewModel");
            if (!parameters.ContainsKey("id")) {
                throw new InvalidOperationException("lost parameter 'id' for TodoDatailPage");
            }
            int id = int.Parse(parameters["id"] as string);
            this.TodoItem.Value = await this._todoItemRepository.FindFirstAsync(id);
        }
    }
}
