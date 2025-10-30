using CommunityToolkit.Maui.Views;
using Microsoft.Maui.Controls;
using TodoMauiApp.Models;

namespace TodoMauiApp.Views;

public class EditTaskPopup : Popup
{
    public event Action<TodoItem> TaskUpdated;

    private readonly TodoItem _originalTask;
    private readonly Entry _titleEntry;
    private readonly Picker _categoryPicker;
    private readonly Picker _priorityPicker;
    private readonly DatePicker _dueDatePicker;
    private readonly CheckBox _noDeadlineCheckBox;
    private readonly Picker _recurrencePicker;
    private readonly Entry _recurrenceIntervalEntry;

    public EditTaskPopup(TodoItem task, List<string> categories)
    {
        // Определяем текущую тему
        var isDark = Application.Current?.RequestedTheme == AppTheme.Dark;
        var backgroundColor = isDark ? Color.FromArgb("#1E1E1E") : Colors.White;
        var textColor = isDark ? Colors.White : Colors.Black;
        var secondaryTextColor = isDark ? Color.FromArgb("#AAAAAA") : Colors.Gray;
        var primaryButtonColor = isDark ? Color.FromArgb("#0A4A7A") : Color.FromArgb("#4A90E2");
        var cancelButtonColor = isDark ? Color.FromArgb("#333333") : Color.FromArgb("#E0E0E0");
        var cancelTextColor = isDark ? Colors.White : Colors.Black;

        _originalTask = new TodoItem
        {
            Id = task.Id,
            Title = task.Title,
            Category = task.Category,
            Priority = task.Priority,
            DueDate = task.DueDate,
            Recurrence = task.Recurrence,
            RecurrenceInterval = task.RecurrenceInterval,
            SubTasks = new List<SubTask>(task.SubTasks)
        };

        _titleEntry = new Entry
        {
            Text = task.Title,
            Placeholder = "Заголовок задачи",
            BackgroundColor = backgroundColor,
            TextColor = textColor,
            PlaceholderColor = secondaryTextColor,
            FontSize = 14
        };

        _categoryPicker = new Picker
        {
            BackgroundColor = backgroundColor,
            TextColor = textColor,
            TitleColor = secondaryTextColor,
            FontSize = 14
        };
        foreach (var cat in categories)
            _categoryPicker.Items.Add(cat);
        _categoryPicker.SelectedItem = task.Category;

        _priorityPicker = new Picker
        {
            ItemsSource = new List<string> { "Низкий", "Средний", "Высокий" },
            SelectedIndex = task.Priority switch
            {
                PriorityLevel.Low => 0,
                PriorityLevel.High => 2,
                _ => 1
            },
            BackgroundColor = backgroundColor,
            TextColor = textColor,
            TitleColor = secondaryTextColor,
            FontSize = 14
        };

        _noDeadlineCheckBox = new CheckBox { IsChecked = !task.DueDate.HasValue };
        _dueDatePicker = new DatePicker
        {
            Date = task.DueDate ?? DateTime.Today,
            BackgroundColor = backgroundColor,
            TextColor = textColor,
            FontSize = 14
        };
        _dueDatePicker.IsEnabled = task.DueDate.HasValue;

        _noDeadlineCheckBox.CheckedChanged += (s, e) =>
        {
            _dueDatePicker.IsEnabled = !e.Value;
        };

        _recurrencePicker = new Picker
        {
            ItemsSource = new List<string>
            {
                "Не повторять",
                "Ежедневно",
                "Еженедельно",
                "Ежемесячно",
                "Каждые N дней"
            },
            SelectedIndex = task.Recurrence switch
            {
                RecurrenceType.Daily => 1,
                RecurrenceType.Weekly => 2,
                RecurrenceType.Monthly => 3,
                RecurrenceType.CustomDays => 4,
                _ => 0
            },
            BackgroundColor = backgroundColor,
            TextColor = textColor,
            TitleColor = secondaryTextColor,
            FontSize = 14
        };

        _recurrenceIntervalEntry = new Entry
        {
            Text = task.RecurrenceInterval.ToString(),
            IsVisible = task.Recurrence == RecurrenceType.CustomDays,
            Keyboard = Keyboard.Numeric,
            BackgroundColor = backgroundColor,
            TextColor = textColor,
            PlaceholderColor = secondaryTextColor,
            FontSize = 14
        };

        _recurrencePicker.SelectedIndexChanged += (s, e) =>
        {
            _recurrenceIntervalEntry.IsVisible = _recurrencePicker.SelectedIndex == 4;
        };

        // Кнопки
        var saveButton = new Button
        {
            Text = "Сохранить",
            WidthRequest = 120,
            HeightRequest = 44,
            BackgroundColor = primaryButtonColor,
            TextColor = Colors.White,
            FontSize = 14,
            CornerRadius = 8
        };
        saveButton.Clicked += OnSaveClicked;

        var cancelButton = new Button
        {
            Text = "Отмена",
            WidthRequest = 120,
            HeightRequest = 44,
            BackgroundColor = cancelButtonColor,
            TextColor = cancelTextColor,
            FontSize = 14,
            CornerRadius = 8
        };
        cancelButton.Clicked += (s, e) => Close();

        var buttonLayout = new HorizontalStackLayout
        {
            Spacing = 12,
            HorizontalOptions = LayoutOptions.Center,
            Children = { saveButton, cancelButton }
        };

        // Метки с цветом
        var createLabel = (string text) => new Label
        {
            Text = text,
            FontAttributes = FontAttributes.Bold,
            FontSize = 14,
            TextColor = textColor,
            Margin = new Thickness(0, 8, 0, 4)
        };

        var contentLayout = new VerticalStackLayout
        {
            Padding = new Thickness(16),
            Spacing = 12,
            BackgroundColor = backgroundColor,
            Children =
            {
                createLabel("Редактирование задачи"),
                createLabel("Заголовок"),
                _titleEntry,

                createLabel("Категория"),
                _categoryPicker,

                createLabel("Приоритет"),
                _priorityPicker,

                createLabel("Дедлайн"),
                new HorizontalStackLayout
                {
                    Spacing = 8,
                    VerticalOptions = LayoutOptions.Center,
                    Children =
                    {
                        _dueDatePicker,
                        _noDeadlineCheckBox,
                        new Label
                        {
                            Text = "Нет даты",
                            VerticalOptions = LayoutOptions.Center,
                            TextColor = textColor,
                            FontSize = 14
                        }
                    }
                },

                createLabel("Повторение"),
                _recurrencePicker,
                _recurrenceIntervalEntry,

                buttonLayout
            }
        };

        Content = new ScrollView { Content = contentLayout };
        Color = backgroundColor; // фон всего popup
        Size = new Size(320, 520); // немного уменьшили высоту
    }

    private void OnSaveClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(_titleEntry.Text))
            return;

        var updatedTask = new TodoItem
        {
            Id = _originalTask.Id,
            Title = _titleEntry.Text.Trim(),
            Category = _categoryPicker.SelectedItem?.ToString() ?? "Все",
            Priority = _priorityPicker.SelectedIndex switch
            {
                0 => PriorityLevel.Low,
                2 => PriorityLevel.High,
                _ => PriorityLevel.Medium
            },
            DueDate = _noDeadlineCheckBox.IsChecked ? null : _dueDatePicker.Date.Add(TimeSpan.Zero),
            Recurrence = _recurrencePicker.SelectedIndex switch
            {
                1 => RecurrenceType.Daily,
                2 => RecurrenceType.Weekly,
                3 => RecurrenceType.Monthly,
                4 => RecurrenceType.CustomDays,
                _ => RecurrenceType.None
            },
            RecurrenceInterval = int.TryParse(_recurrenceIntervalEntry.Text, out var interval) && interval > 0 ? interval : 1,
            SubTasks = new List<SubTask>(_originalTask.SubTasks),
            IsCompleted = _originalTask.IsCompleted
        };

        TaskUpdated?.Invoke(updatedTask);
        Close();
    }
}