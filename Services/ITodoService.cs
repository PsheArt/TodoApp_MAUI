using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TodoMauiApp.Models;

namespace TodoMauiApp.Services
{
    public interface ITodoService
    {
        List<TodoItem> GetItems();
        void SaveItems(List<TodoItem> items);
        void AddItem(TodoItem item);
        void UpdateItem(TodoItem item);
        void DeleteItem(string id);
    }
}
