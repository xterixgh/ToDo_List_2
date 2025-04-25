using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ToDo_List_2
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ObservableCollection<ToDoItem> Tasks { get; set; } = new ObservableCollection<ToDoItem>();
        public ObservableCollection<ToDoItem> FilteredTasks { get; set; } = new ObservableCollection<ToDoItem>();
        public MainWindow()
        {
            InitializeComponent();
            TaskListLb.ItemsSource = FilteredTasks;
            UpdateCounter();
        }

        private void AddTaskBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(TaskInputTb.Text))
            {
                var selectedCategory = (CategoryComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
                Tasks.Add(new ToDoItem 
                { 
                    Title = TaskInputTb.Text, 
                    IsDone = false,  
                    DueDate = DueDatePicker.SelectedDate,
                    Category = selectedCategory   
                });
                TaskInputTb.Text = string.Empty;
                DueDatePicker.SelectedDate = DateTime.Today;
                CategoryComboBox.SelectedIndex = -1;
                UpdateCounter();
                ApplyFilter();
            }
        }

        private void DeleteTaskBtn_Click(object sender, RoutedEventArgs e)
        {
            if(sender is Button btn && btn.DataContext is ToDoItem item)
            {
                Tasks.Remove(item);
                UpdateCounter();
                ApplyFilter();
            }
        }
        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            UpdateCounter();
            ApplyFilter();
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            UpdateCounter();
            ApplyFilter();
        }

        public void UpdateCounter()
        {
            int counter = 0;
            foreach(var task in Tasks)
            {
                if (!task.IsDone)
                {
                    counter++;
                }
            }
            CounterTextTbl.Text = $"Осталось дел: {counter}";
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilter();
        }

        private void CategoryFilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilter();
        }
        private void ApplyFilter()
        {
            var  searchText = SearchTextBox.Text.ToLower();
            var selectedCategory = (CategoryFilterComboBox.SelectedItem as ComboBoxItem)?.Content?.ToString();

            //Поиск по загаловку

            var filteredByTitle = Tasks.Where(task => task.Title != null && task.Title.Contains(searchText)).ToList();

            //Фильтрация по категории

            List<ToDoItem> finalFiltered;
            if(string .IsNullOrEmpty(selectedCategory) || selectedCategory == "Все")
            {
                finalFiltered = filteredByTitle;
            }
            else
            {
                finalFiltered = filteredByTitle.Where(task => task.Category == selectedCategory).ToList();
            }

            //Сортировка

            var selectedSort = (Sort_ComboBox.SelectedItem as ComboBoxItem)?.Content?.ToString();
            switch (selectedSort)
            {
                case "Без сортировки":
                default:
                    break;
                case "По дате (по возворстанию)":
                    finalFiltered = finalFiltered.OrderBy(task => task.DueDate).ToList();
                    break;
                case "По дате (по убыванию":
                    finalFiltered = finalFiltered.OrderByDescending(task => task.DueDate).ToList();
                    break;
                case "По статусу":
                    finalFiltered = finalFiltered.OrderByDescending(task => task.IsDone).ToList();
                    break;
                case "По категории":
                    finalFiltered = finalFiltered.OrderByDescending(task => task.Category).ToList();
                    break;
            }


            FilteredTasks.Clear();
            foreach(var task in finalFiltered)
            {
                FilteredTasks.Add(task);
            }

        }

        private void DeleteCompletedBtn_Click(object sender, RoutedEventArgs e)
        {
            var remaining = Tasks.Where(task => !task.IsDone).ToList();
            Tasks.Clear();
            foreach (var task in remaining)
            {
                Tasks.Add(task);
            }
            UpdateCounter();
            ApplyFilter();
        }

        private void Sort_ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilter();
        }
    }

    public class ToDoItem
    {
        public string Title { get; set; }
        public bool IsDone { get; set; }
        public DateTime? DueDate { get; set; }
        public string Category { get; set; }
    }
    public class DoneToTextDecorationConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isDone = (bool)value;
            return isDone ? TextDecorations.Strikethrough : null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

}

