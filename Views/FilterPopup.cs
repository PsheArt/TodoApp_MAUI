using CommunityToolkit.Maui.Views;
using Microsoft.Maui.Controls;
using TodoMauiApp.Models;

namespace TodoMauiApp.Views;

public class FilterPopup : Popup
{
    public event Action<FilterOptions> FiltersApplied;

    private readonly Entry _searchEntry;
    private readonly CheckBox _showOverdueOnly;
    private readonly CheckBox _showNoDueDate;
    private readonly CheckBox _showHighPriority;
    private readonly Picker _sortPicker;

    public FilterPopup(FilterOptions initialFilters)
    {
        // Определяем тему
        var isDark = Application.Current?.RequestedTheme == AppTheme.Dark;
        var backgroundColor = isDark ? Color.FromArgb("#1E1E1E") : Colors.White;
        var textColor = isDark ? Colors.White : Colors.Black;
        var secondaryTextColor = isDark ? Color.FromArgb("#AAAAAA") : Colors.Gray;

        _searchEntry = new Entry
        {
            Placeholder = "Поиск по задачам...",
            BackgroundColor = backgroundColor,
            TextColor = textColor,
            PlaceholderColor = secondaryTextColor
        };
        _searchEntry.TextChanged += OnFilterChanged;

        _showOverdueOnly = new CheckBox();
        _showOverdueOnly.CheckedChanged += OnFilterChanged;

        _showNoDueDate = new CheckBox();
        _showNoDueDate.CheckedChanged += OnFilterChanged;

        _showHighPriority = new CheckBox();
        _showHighPriority.CheckedChanged += OnFilterChanged;

        var resetButton = new Button
        {
            Text = "✕",
            FontSize = 18,
            WidthRequest = 36,
            HeightRequest = 36,
            CornerRadius = 18,
            BackgroundColor = Colors.Transparent,
            TextColor = secondaryTextColor,
            HorizontalOptions = LayoutOptions.End
        };
        resetButton.Clicked += (s, e) => ResetFilters();
        var titleLabel = new Label
        {
            Text = "Фильтры",
            FontAttributes = FontAttributes.Bold,
            FontSize = 18,
            TextColor = textColor
        };

        var headerLayout = new Grid
        {
            ColumnDefinitions = new ColumnDefinitionCollection
        {
            new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
            new ColumnDefinition { Width = GridLength.Auto }
        }
        };
        headerLayout.Add(titleLabel, 0, 0);
        headerLayout.Add(resetButton, 1, 0);

        _sortPicker = new Picker
        {
            ItemsSource = new List<string>
            {
                "По дате (ближайшие)",
                "По приоритету",
                "По алфавиту"
            },
            SelectedIndex = 0,
            BackgroundColor = backgroundColor,
            TextColor = textColor
        };
        _sortPicker.SelectedIndexChanged += OnFilterChanged;

        var contentLayout = new VerticalStackLayout
        {
            Padding = new Thickness(16),
            Spacing = 16,
            BackgroundColor = backgroundColor,
            Children =
        {
            headerLayout,
            new Label { Text = "Поиск", FontAttributes = FontAttributes.Bold, FontSize = 14, TextColor = textColor },
            _searchEntry,

            new Label { Text = "Фильтры", FontAttributes = FontAttributes.Bold, FontSize = 14, TextColor = textColor },

            new HorizontalStackLayout
            {
                Spacing = 8,
                VerticalOptions = LayoutOptions.Center,
                Children =
                {
                    _showOverdueOnly,
                    new Label { Text = "Только просроченные", VerticalOptions = LayoutOptions.Center, TextColor = textColor }
                }
            },

            new HorizontalStackLayout
            {
                Spacing = 8,
                VerticalOptions = LayoutOptions.Center,
                Children =
                {
                    _showNoDueDate,
                    new Label { Text = "Без даты", VerticalOptions = LayoutOptions.Center, TextColor = textColor }
                }
            },

            new HorizontalStackLayout
            {
                Spacing = 8,
                VerticalOptions = LayoutOptions.Center,
                Children =
                {
                    _showHighPriority,
                    new Label { Text = "Высокий приоритет", VerticalOptions = LayoutOptions.Center, TextColor = textColor }
                }
            },

            new Label { Text = "Сортировка", FontAttributes = FontAttributes.Bold, FontSize = 14, TextColor = textColor },
            _sortPicker
        }
        };
        _searchEntry.Text = initialFilters.SearchText;
        _showOverdueOnly.IsChecked = initialFilters.ShowOverdueOnly;
        _showNoDueDate.IsChecked = initialFilters.ShowNoDueDate;
        _showHighPriority.IsChecked = initialFilters.ShowHighPriority;
        _sortPicker.SelectedIndex = initialFilters.SortBy switch
        {
            SortOption.Priority => 1,
            SortOption.Title => 2,
            _ => 0
        };

        Content = new ScrollView { Content = contentLayout };
        Color = backgroundColor; // фон Popup
        Size = new Size(320, 480);
    }

    private void OnFilterChanged(object sender, EventArgs e)
    {
        var options = new FilterOptions
        {
            SearchText = _searchEntry.Text,
            ShowOverdueOnly = _showOverdueOnly.IsChecked,
            ShowNoDueDate = _showNoDueDate.IsChecked,
            ShowHighPriority = _showHighPriority.IsChecked,
            SortBy = _sortPicker.SelectedIndex switch
            {
                1 => SortOption.Priority,
                2 => SortOption.Title,
                _ => SortOption.DueDate
            }
        };

        FiltersApplied?.Invoke(options);
    
    }

    private void ResetFilters()
    {
        _searchEntry.Text = string.Empty;
        _showOverdueOnly.IsChecked = false;
        _showNoDueDate.IsChecked = false;
        _showHighPriority.IsChecked = false;
        _sortPicker.SelectedIndex = 0;

        OnFilterChanged(this, EventArgs.Empty);
    }
}