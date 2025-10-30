using CommunityToolkit.Maui.Views;
using System.Collections.ObjectModel;
using TodoMauiApp.Helper;
using TodoMauiApp.Models;
using TodoMauiApp.Services;

namespace TodoMauiApp.Views
{
    public partial class MainPage : ContentPage
    {
        private readonly List<SubTask> _newSubTasks = new();
        private string _selectedFilter = "Все";
        private FilterOptions _currentFilters = new();
        private bool _isUpdatingCollection;
        public string SelectedFilter
        {
            get => _selectedFilter;
            set
            {
                _selectedFilter = value;
                FilterTodos();
            }
        }

        private readonly ITodoService _todoService;
        private readonly IThemeService? _themeService;
        public ObservableCollection<TodoItem> TodoItems { get; set; }

        public MainPage(ITodoService todoService, IThemeService themeService)
        {
            InitializeComponent();
            _todoService = todoService;
            _themeService = themeService;
            TodoItems = new ObservableCollection<TodoItem>();
            BindingContext = this;
            foreach (var cat in AppCategories.All)
            {
                CategoryPicker.Items.Add(cat.Name);
            }
            CategoryPicker.SelectedIndex = 0; 

            LoadTodos();
            ThemeSwitch.IsToggled = _themeService.IsDarkMode;
            _themeService.ThemeChanged += () =>
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    ThemeSwitch.IsToggled = _themeService.IsDarkMode;
                });
            };

            CategoryPicker.SelectedIndexChanged += (s, e) =>
            {
                if (CategoryPicker.SelectedIndex >= 0)
                {
                    SelectedFilter = CategoryPicker.Items[CategoryPicker.SelectedIndex];
                }
            };

            PriorityPicker.Items.Add("Низкий");
            PriorityPicker.Items.Add("Средний");
            PriorityPicker.Items.Add("Высокий");
            PriorityPicker.SelectedIndex = 1;
            RecurrencePicker.Items.Add("Не повторять");
            RecurrencePicker.Items.Add("Ежедневно");
            RecurrencePicker.Items.Add("Еженедельно");
            RecurrencePicker.Items.Add("Ежемесячно");
            RecurrencePicker.Items.Add("Каждые N дней");
            RecurrencePicker.SelectedIndex = 0;
            DueDatePicker.Date = DateTime.Today;
        }

        private void FilterTodos()
        {
            if (_isUpdatingCollection)
                return; 

            var allItems = _todoService.GetItems()
                .Where(item => _selectedFilter == "Все" || item.Category == _selectedFilter)
                .ToList();

            var now = DateTime.Now;
            var sorted = allItems
                .OrderBy(item => item.IsCompleted ? 1 : 0)
                .ThenBy(item =>
                {
                    if (item.DueDate == null) return 3;
                    if (item.DueDate.Value < now) return 0;
                    if (item.DueDate.Value.Date == now.Date) return 1;
                    return 2;
                })
                .ThenBy(item => item.DueDate ?? DateTime.MaxValue)
                .ThenByDescending(item => item.Priority)
                .ToList();

            MainThread.BeginInvokeOnMainThread(() =>
            {
                if (_isUpdatingCollection) return;

                _isUpdatingCollection = true;
                try
                {
                    TodoItems.Clear();
                    foreach (var item in sorted)
                    {
                        TodoItems.Add(item);
                    }
                }
                finally
                {
                    _isUpdatingCollection = false;
                }
            });
        }

        private async void OnMenuClicked(object sender, EventArgs e)
        {
            var popup = new FilterPopup(_currentFilters);

            popup.FiltersApplied += (options) =>
            {
                _currentFilters = options;
                OnFiltersApplied(options);
            };

            await this.ShowPopupAsync(popup);
        }

        private void OnFiltersApplied(FilterOptions options)
        {
            _currentFilters = options;
            ApplyFilters(options);
        }

        private void LoadTodos()
        {
            FilterTodos();
        }

        private void OnAddClicked(object sender, EventArgs e)
        {
            var title = NewItemEntry.Text?.Trim();
            if (!string.IsNullOrEmpty(title))
            {
                var category = CategoryPicker.SelectedItem?.ToString() ?? "Все";
                var priority = PriorityPicker.SelectedIndex switch
                {
                    0 => PriorityLevel.Low,
                    2 => PriorityLevel.High,
                    _ => PriorityLevel.Medium
                };

                DateTime? dueDate = null;
                if (!NoDeadlineCheckBox.IsChecked)
                {
                    dueDate = DueDatePicker.Date.Add(TimeSpan.Zero);
                }
                var recurrence = RecurrencePicker.SelectedIndex switch
                {
                    1 => RecurrenceType.Daily,
                    2 => RecurrenceType.Weekly,
                    3 => RecurrenceType.Monthly,
                    4 => RecurrenceType.CustomDays,
                    _ => RecurrenceType.None
                };

                int interval = 1;
                if (recurrence == RecurrenceType.CustomDays)
                {
                    _ = int.TryParse(RecurrenceIntervalEntry.Text, out interval);
                    if (interval < 1) interval = 1;
                }
                var newItem = new TodoItem
                {
                    Title = title,
                    Category = category,
                    Priority = priority,
                    DueDate = dueDate,
                    Recurrence = recurrence,
                    RecurrenceInterval = interval,
                    SubTasks = new List<SubTask>(_newSubTasks)
                };

                _todoService.AddItem(newItem);
                TodoItems.Add(newItem);

                NewItemEntry.Text = string.Empty;
                PriorityPicker.SelectedIndex = 1;
                DueDatePicker.Date = DateTime.Today;
                NoDeadlineCheckBox.IsChecked = false;
                RecurrencePicker.SelectedIndex = 0;
                RecurrenceIntervalEntry.Text = "1";
                _newSubTasks.Clear(); 
                SubTasksContainer.Children.Clear(); 
                NewItemEntry.Unfocus();
            }
        }

        private async void OnEditClicked(object sender, EventArgs e)
        {
            if (sender is Button btn && btn.CommandParameter is TodoItem item)
            {
                // Получаем список категорий (как в конструкторе)
                var categories = AppCategories.All.Select(c => c.Name).ToList();

                var popup = new EditTaskPopup(item, categories);
                popup.TaskUpdated += (updatedTask) =>
                {
                    // Обновляем задачу в сервисе
                    _todoService.UpdateItem(updatedTask);

                    // Находим и заменяем в коллекции
                    var index = TodoItems.IndexOf(item);
                    if (index >= 0)
                    {
                        TodoItems[index] = updatedTask;
                    }

                    // Перезапускаем фильтрацию (на случай, если категория изменилась)
                    FilterTodos();
                };

                await this.ShowPopupAsync(popup);
            }
        }

        private void OnTodoCheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            if (sender is CheckBox checkBox && checkBox.BindingContext is TodoItem item)
            {
                item.IsCompleted = e.Value;

                if (e.Value && item.Recurrence != RecurrenceType.None)
                {
                    var newTask = new TodoItem
                    {
                        Title = item.Title,
                        Category = item.Category,
                        Priority = item.Priority,
                        Recurrence = item.Recurrence,
                        RecurrenceInterval = item.RecurrenceInterval,
                        SubTasks = new List<SubTask>(item.SubTasks.Select(st => new SubTask
                        {
                            Title = st.Title,
                            IsCompleted = false
                        }))
                    };
                    var baseDate = item.DueDate ?? DateTime.Now;
                    newTask.DueDate = item.Recurrence switch
                    {
                        RecurrenceType.Daily => baseDate.AddDays(1),
                        RecurrenceType.Weekly => baseDate.AddDays(7),
                        RecurrenceType.Monthly => baseDate.AddMonths(1),
                        RecurrenceType.CustomDays => baseDate.AddDays(item.RecurrenceInterval),
                        _ => null
                    };

                    TodoItems.Remove(item);
                    TodoItems.Add(newTask);

                }
                _isUpdatingCollection = true;
                FilterTodos();
            }
        }

        private async void OnEditSwipeInvoked(object sender, EventArgs e)
        {
            if (sender is SwipeItem item && item.CommandParameter is TodoItem todoItem)
            {
                // Подтверждение редактирования
                var result = await DisplayAlert(
                    "Подтверждение",
                    "Открыть редактирование задачи?",
                    "Да", "Нет");

                if (result)
                {
                    // Получаем категории (как в OnEditClicked)
                    var categories = AppCategories.All.Select(c => c.Name).ToList();
                    var popup = new EditTaskPopup(todoItem, categories);
                    popup.TaskUpdated += (updatedTask) =>
                    {
                        _todoService.UpdateItem(updatedTask);
                        var index = TodoItems.IndexOf(todoItem);
                        if (index >= 0)
                            TodoItems[index] = updatedTask;
                        FilterTodos();
                    };
                    await this.ShowPopupAsync(popup);
                }
            }
        }

        private async void OnDeleteSwipeInvoked(object sender, EventArgs e)
        {
            if (sender is SwipeItem item && item.CommandParameter is TodoItem todoItem)
            {
                // Подтверждение удаления
                var result = await DisplayAlert(
                    "Подтверждение",
                    $"Удалить задачу \"{todoItem.Title}\"?",
                    "Да", "Нет");

                if (result)
                {
                    _todoService.DeleteItem(todoItem.Id);
                    TodoItems.Remove(todoItem);
                }
            }
        }

        private void OnDeleteClicked(object sender, EventArgs e)
        {
            if (sender is Button btn && btn.CommandParameter is TodoItem item)
            {
                _todoService.DeleteItem(item.Id);
                TodoItems.Remove(item);
            }
        }

        private void OnNoDeadlineCheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            DueDatePicker.IsEnabled = !e.Value;
        }

        private void OnSubTaskCheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            if (sender is CheckBox checkBox && checkBox.BindingContext is SubTask subTask)
            {
                subTask.IsCompleted = e.Value;

                var parentTask = TodoItems.FirstOrDefault(t => t.SubTasks.Contains(subTask));
                if (parentTask != null)
                {
                    _todoService.UpdateItem(parentTask);
                }
            }
        }

        private void OnAddSubTaskClicked(object sender, EventArgs e)
        {
            var entry = new Entry
            {
                Placeholder = "Новая подзадача...",
                HorizontalOptions = LayoutOptions.FillAndExpand
            };

            var deleteButton = new Button
            {
                Text = "🗑️",
                WidthRequest = 40,
                BackgroundColor = Colors.Transparent
            };

            deleteButton.Clicked += (s, ev) =>
            {
                SubTasksContainer.Remove(entry);
                SubTasksContainer.Remove(deleteButton);
                var taskToRemove = _newSubTasks.FirstOrDefault(t => t.Title == entry.Text);
                if (taskToRemove != null)
                    _newSubTasks.Remove(taskToRemove);
            };

            entry.Unfocused += (s, ev) =>
            {
                if (!string.IsNullOrWhiteSpace(entry.Text))
                {
                    var existing = _newSubTasks.FirstOrDefault(t => t.Title == entry.Text);
                    if (existing == null)
                    {
                        _newSubTasks.Add(new SubTask { Title = entry.Text });
                    }
                }
            };

            SubTasksContainer.Add(entry);
            SubTasksContainer.Add(deleteButton);
        }
        private void ApplyFilters(FilterOptions options)
        {
            var allItems = _todoService.GetItems();

            // Поиск
            if (!string.IsNullOrWhiteSpace(options.SearchText))
            {
                var searchText = options.SearchText.ToLowerInvariant();
                allItems = allItems.Where(item =>
                    item.Title?.ToLowerInvariant().Contains(searchText) == true  ||
                    item.SubTasks?.Any(st => st.Title?.ToLowerInvariant().Contains(searchText) == true) == true
                ).ToList();
            }

            // Фильтры
            if (options.ShowOverdueOnly)
            {
                var now = DateTime.Now;
                allItems = allItems.Where(item => item.DueDate.HasValue && item.DueDate < now).ToList();
            }

            if (options.ShowNoDueDate)
            {
                allItems = allItems.Where(item => !item.DueDate.HasValue).ToList();
            }

            if (options.ShowHighPriority)
            {
                allItems = allItems.Where(item => item.Priority == PriorityLevel.High).ToList();
            }

            // Сортировка
            var sorted = SortItems(allItems, options.SortBy);

            // Обновление коллекции на UI-потоке
            MainThread.BeginInvokeOnMainThread(() =>
            {
                TodoItems.Clear();
                foreach (var item in sorted)
                    TodoItems.Add(item);
            });
        }

        private List<TodoItem> SortItems(List<TodoItem> items, SortOption sortBy)
        {
            var now = DateTime.Now;
            return sortBy switch
            {
                SortOption.Priority => items
                    .OrderByDescending(item => item.Priority)
                    .ThenBy(item => item.IsCompleted ? 1 : 0)
                    .ThenBy(item => item.DueDate ?? DateTime.MaxValue)
                    .ToList(),

                SortOption.Title => items
                    .OrderBy(item => item.Title)
                    .ThenBy(item => item.IsCompleted ? 1 : 0)
                    .ToList(),

                _ => items
                    .OrderBy(item => item.IsCompleted ? 1 : 0)
                    .ThenBy(item =>
                    {
                        if (item.DueDate == null) return 3;
                        if (item.DueDate.Value < now) return 0;
                        if (item.DueDate.Value.Date == now.Date) return 1;
                        return 2;
                    })
                    .ThenBy(item => item.DueDate ?? DateTime.MaxValue)
                    .ThenByDescending(item => item.Priority)
                    .ToList()
            };
        }
        private void OnThemeToggled(object sender, ToggledEventArgs e)
        {
            _themeService?.SetTheme(e.Value);
        }

    }
}
