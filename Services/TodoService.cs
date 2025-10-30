using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using TodoMauiApp.Models;
using Microsoft.Maui.Storage;
using System.Threading.Tasks;

namespace TodoMauiApp.Services
{
    public class TodoService : ITodoService
    {
        private const string PreferencesKey = "TodoItems";

        public List<TodoItem> GetItems()
        {
            var json = Preferences.Get(PreferencesKey, "[]");
            return JsonSerializer.Deserialize<List<TodoItem>>(json) ?? new List<TodoItem>();
        }

        public void SaveItems(List<TodoItem> items)
        {
            var json = JsonSerializer.Serialize(items);
            Preferences.Set(PreferencesKey, json);
        }

        public void AddItem(TodoItem item)
        {
            var items = GetItems();
            items.Add(item);
            SaveItems(items);
        }

        public void UpdateItem(TodoItem item)
        {
            var items = GetItems();
            var index = items.FindIndex(x => x.Id == item.Id);
            if (index != -1)
            {
                items[index] = item;
                SaveItems(items);
            }
        }

        public void DeleteItem(string id)
        {
            var items = GetItems();
            items.RemoveAll(x => x.Id == id);
            SaveItems(items);
        }
    }
}
