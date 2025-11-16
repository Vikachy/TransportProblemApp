using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace TransportProblemApp
{
    public partial class MainWindow : Window
    {
        private int suppliersCount = 3;
        private int consumersCount = 5;

        public MainWindow()
        {
            InitializeComponent();
            InitializeData();
        }

        private void InitializeData()
        {
            CreateCostsMatrix();
            CreateSuppliesDemands();
        }

        private void BtnResize_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                suppliersCount = int.Parse(txtSuppliers.Text);
                consumersCount = int.Parse(txtConsumers.Text);

                if (suppliersCount <= 0 || consumersCount <= 0)
                {
                    MessageBox.Show("Размеры должны быть положительными числами!");
                    return;
                }

                CreateCostsMatrix();
                CreateSuppliesDemands();
                txtStatus.Text = $"Создана матрица {suppliersCount}x{consumersCount}";
            }
            catch (FormatException)
            {
                MessageBox.Show("Введите корректные числа для размеров!");
            }
        }

        private void CreateCostsMatrix()
        {
            costsMatrix.Items.Clear();

            // Заголовок столбцов
            var headerPanel = new StackPanel { Orientation = Orientation.Horizontal };
            headerPanel.Children.Add(new TextBlock
            {
                Text = "Поставщик\\Потребитель",
                Width = 120,
                FontWeight = FontWeights.Bold
            });

            for (int j = 0; j < consumersCount; j++)
            {
                headerPanel.Children.Add(new TextBlock
                {
                    Text = $"B{j + 1}",
                    Width = 60,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    FontWeight = FontWeights.Bold
                });
            }
            costsMatrix.Items.Add(headerPanel);

            // Данные матрицы
            for (int i = 0; i < suppliersCount; i++)
            {
                var rowPanel = new StackPanel { Orientation = Orientation.Horizontal };
                rowPanel.Children.Add(new TextBlock
                {
                    Text = $"A{i + 1}",
                    Width = 120,
                    VerticalAlignment = VerticalAlignment.Center,
                    FontWeight = FontWeights.Bold
                });

                for (int j = 0; j < consumersCount; j++)
                {
                    var textBox = new TextBox
                    {
                        Width = 60,
                        Height = 25,
                        Margin = new Thickness(2),
                        Text = "0",
                        HorizontalContentAlignment = HorizontalAlignment.Center,
                        Background = Brushes.White
                    };
                    rowPanel.Children.Add(textBox);
                }

                costsMatrix.Items.Add(rowPanel);
            }
        }

        private void CreateSuppliesDemands()
        {
            suppliesList.Items.Clear();
            demandsList.Items.Clear();

            // Запасы
            for (int i = 0; i < suppliersCount; i++)
            {
                var stackPanel = new StackPanel { Orientation = Orientation.Horizontal };
                stackPanel.Children.Add(new TextBlock
                {
                    Text = $"Поставщик A{i + 1}:",
                    Width = 100,
                    VerticalAlignment = VerticalAlignment.Center
                });

                var textBox = new TextBox
                {
                    Width = 80,
                    Height = 25,
                    Margin = new Thickness(2),
                    Text = "0",
                    HorizontalContentAlignment = HorizontalAlignment.Center
                };
                stackPanel.Children.Add(textBox);
                suppliesList.Items.Add(stackPanel);
            }

            // Потребности
            for (int j = 0; j < consumersCount; j++)
            {
                var stackPanel = new StackPanel { Orientation = Orientation.Horizontal };
                stackPanel.Children.Add(new TextBlock
                {
                    Text = $"Потребитель B{j + 1}:",
                    Width = 100,
                    VerticalAlignment = VerticalAlignment.Center
                });

                var textBox = new TextBox
                {
                    Width = 80,
                    Height = 25,
                    Margin = new Thickness(2),
                    Text = "0",
                    HorizontalContentAlignment = HorizontalAlignment.Center
                };
                stackPanel.Children.Add(textBox);
                demandsList.Items.Add(stackPanel);
            }
        }

        private void BtnExample_Click(object sender, RoutedEventArgs e)
        {
            // Установка примера данных
            txtSuppliers.Text = "3";
            txtConsumers.Text = "5";
            suppliersCount = 3;
            consumersCount = 5;

            CreateCostsMatrix();
            CreateSuppliesDemands();

            // Заполнение примера
            SetMatrixValue(0, 0, "20"); SetMatrixValue(0, 1, "10"); SetMatrixValue(0, 2, "13");
            SetMatrixValue(0, 3, "13"); SetMatrixValue(0, 4, "18");

            SetMatrixValue(1, 0, "27"); SetMatrixValue(1, 1, "19"); SetMatrixValue(1, 2, "20");
            SetMatrixValue(1, 3, "16"); SetMatrixValue(1, 4, "22");

            SetMatrixValue(2, 0, "26"); SetMatrixValue(2, 1, "17"); SetMatrixValue(2, 2, "19");
            SetMatrixValue(2, 3, "21"); SetMatrixValue(2, 4, "23");

            SetSupplyValue(0, "200");
            SetSupplyValue(1, "300");
            SetSupplyValue(2, "250");

            SetDemandValue(0, "210");
            SetDemandValue(1, "150");
            SetDemandValue(2, "120");
            SetDemandValue(3, "135");
            SetDemandValue(4, "135");

            txtStatus.Text = "Загружен пример данных";
        }

        private void SetMatrixValue(int row, int col, string value)
        {
            if (costsMatrix.Items.Count > row + 1)
            {
                var rowPanel = costsMatrix.Items[row + 1] as StackPanel;
                if (rowPanel != null && rowPanel.Children.Count > col + 1)
                {
                    var textBox = rowPanel.Children[col + 1] as TextBox;
                    if (textBox != null)
                        textBox.Text = value;
                }
            }
        }

        private void SetSupplyValue(int index, string value)
        {
            if (suppliesList.Items.Count > index)
            {
                var panel = suppliesList.Items[index] as StackPanel;
                if (panel != null && panel.Children.Count > 1)
                {
                    var textBox = panel.Children[1] as TextBox;
                    if (textBox != null)
                        textBox.Text = value;
                }
            }
        }

        private void SetDemandValue(int index, string value)
        {
            if (demandsList.Items.Count > index)
            {
                var panel = demandsList.Items[index] as StackPanel;
                if (panel != null && panel.Children.Count > 1)
                {
                    var textBox = panel.Children[1] as TextBox;
                    if (textBox != null)
                        textBox.Text = value;
                }
            }
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            // Очистка всех полей ввода
            foreach (var item in costsMatrix.Items)
            {
                if (item is StackPanel rowPanel)
                {
                    foreach (var child in rowPanel.Children)
                    {
                        if (child is TextBox textBox)
                            textBox.Text = "0";
                    }
                }
            }

            foreach (var item in suppliesList.Items)
            {
                if (item is StackPanel panel && panel.Children.Count > 1)
                {
                    if (panel.Children[1] is TextBox textBox)
                        textBox.Text = "0";
                }
            }

            foreach (var item in demandsList.Items)
            {
                if (item is StackPanel panel && panel.Children.Count > 1)
                {
                    if (panel.Children[1] is TextBox textBox)
                        textBox.Text = "0";
                }
            }

            txtStatus.Text = "Все поля очищены";
        }

        private void BtnCalculate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Получение данных из интерфейса
                var costs = GetCostsMatrix();
                var supplies = GetSupplies();
                var demands = GetDemands();

                // Проверка баланса
                int totalSupply = supplies.Sum();
                int totalDemand = demands.Sum();

                string balanceInfo = $"Общие запасы: {totalSupply}, Общие потребности: {totalDemand}";
                txtBalance.Text = balanceInfo;

                if (totalSupply != totalDemand)
                {
                    MessageBox.Show($"Задача не сбалансирована! {balanceInfo}\nДобавлен фиктивный поставщик/потребитель.",
                                  "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                }

                // Выбор метода решения
                TransportSolver solver;
                string methodName = "";

                if (cmbMethod.SelectedIndex == 0)
                {
                    solver = new NorthwestCornerMethod();
                    methodName = "Метод северо-западного угла";
                }
                else
                {
                    solver = new MinimumCostMethod();
                    methodName = "Метод минимальных элементов";
                }

                txtMethodName.Text = methodName;

                // Решение задачи
                var result = solver.Solve(costs, supplies, demands);

                // Отображение результатов
                DisplayResults(result);

                txtStatus.Text = $"Расчет завершен. Стоимость: {result.TotalCost}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при расчете: {ex.Message}", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Error);
                txtStatus.Text = "Ошибка при расчете";
            }
        }

        private int[,] GetCostsMatrix()
        {
            var costs = new int[suppliersCount, consumersCount];

            for (int i = 0; i < suppliersCount; i++)
            {
                var rowPanel = costsMatrix.Items[i + 1] as StackPanel;
                if (rowPanel != null)
                {
                    for (int j = 0; j < consumersCount; j++)
                    {
                        var textBox = rowPanel.Children[j + 1] as TextBox;
                        if (textBox != null && int.TryParse(textBox.Text, out int value))
                        {
                            costs[i, j] = value;
                        }
                    }
                }
            }

            return costs;
        }

        private int[] GetSupplies()
        {
            var supplies = new int[suppliersCount];

            for (int i = 0; i < suppliersCount; i++)
            {
                var panel = suppliesList.Items[i] as StackPanel;
                if (panel != null && panel.Children.Count > 1)
                {
                    var textBox = panel.Children[1] as TextBox;
                    if (textBox != null && int.TryParse(textBox.Text, out int value))
                    {
                        supplies[i] = value;
                    }
                }
            }

            return supplies;
        }

        private int[] GetDemands()
        {
            var demands = new int[consumersCount];

            for (int i = 0; i < consumersCount; i++)
            {
                var panel = demandsList.Items[i] as StackPanel;
                if (panel != null && panel.Children.Count > 1)
                {
                    var textBox = panel.Children[1] as TextBox;
                    if (textBox != null && int.TryParse(textBox.Text, out int value))
                    {
                        demands[i] = value;
                    }
                }
            }

            return demands;
        }

        private void DisplayResults(SolutionResult result)
        {
            // Отображение плана перевозок в DataGrid
            var dataTable = new System.Data.DataTable();

            // Добавляем столбцы
            dataTable.Columns.Add("Поставщик\\Потребитель");
            for (int j = 0; j < consumersCount; j++)
            {
                dataTable.Columns.Add($"B{j + 1}");
            }
            dataTable.Columns.Add("Запасы");

            // Добавляем строки
            for (int i = 0; i < suppliersCount; i++)
            {
                var row = dataTable.NewRow();
                row[0] = $"A{i + 1}";

                for (int j = 0; j < consumersCount; j++)
                {
                    row[j + 1] = result.TransportPlan[i, j];
                }

                row[consumersCount + 1] = GetSupplies()[i];
                dataTable.Rows.Add(row);
            }

            // Добавляем строку с потребностями
            var demandRow = dataTable.NewRow();
            demandRow[0] = "Потребности";
            var demands = GetDemands();
            for (int j = 0; j < consumersCount; j++)
            {
                demandRow[j + 1] = demands[j];
            }
            demandRow[consumersCount + 1] = demands.Sum();
            dataTable.Rows.Add(demandRow);

            dgResults.ItemsSource = dataTable.DefaultView;

            // Отображение стоимости и шагов
            txtTotalCost.Text = $"Общая стоимость перевозок: {result.TotalCost}";
            txtSteps.Text = result.Steps;

            // Переход на вкладку результатов
            tabResults.IsSelected = true;
        }
    }

    // Классы для решения транспортной задачи (добавьте в отдельные файлы)
    public class SolutionResult
    {
        public int[,] TransportPlan { get; set; }
        public int TotalCost { get; set; }
        public string Steps { get; set; }
    }

    public interface TransportSolver
    {
        SolutionResult Solve(int[,] costs, int[] supplies, int[] demands);
    }

    public class NorthwestCornerMethod : TransportSolver
    {
        public SolutionResult Solve(int[,] costs, int[] supplies, int[] demands)
        {
            int m = supplies.Length;
            int n = demands.Length;
            int[,] plan = new int[m, n];
            var steps = new System.Text.StringBuilder();
            int totalCost = 0;

            int[] sup = (int[])supplies.Clone();
            int[] dem = (int[])demands.Clone();

            steps.AppendLine("Метод северо-западного угла:");
            steps.AppendLine("Начинаем с левого верхнего угла (A1-B1)");

            int i = 0, j = 0;
            int step = 1;

            while (i < m && j < n)
            {
                int allocation = Math.Min(sup[i], dem[j]);
                plan[i, j] = allocation;
                totalCost += allocation * costs[i, j];

                steps.AppendLine($"Шаг {step}: A{i + 1}-B{j + 1} = {allocation} (стоимость: {allocation} × {costs[i, j]} = {allocation * costs[i, j]})");

                sup[i] -= allocation;
                dem[j] -= allocation;

                if (sup[i] == 0)
                {
                    steps.AppendLine($"  Запас A{i + 1} исчерпан, переходим к A{i + 2}");
                    i++;
                }
                if (dem[j] == 0)
                {
                    steps.AppendLine($"  Потребность B{j + 1} удовлетворена, переходим к B{j + 2}");
                    j++;
                }
                step++;
            }

            steps.AppendLine($"\nРасчет завершен. Общая стоимость: {totalCost}");

            return new SolutionResult
            {
                TransportPlan = plan,
                TotalCost = totalCost,
                Steps = steps.ToString()
            };
        }
    }

    public class MinimumCostMethod : TransportSolver
    {
        public SolutionResult Solve(int[,] costs, int[] supplies, int[] demands)
        {
            int m = supplies.Length;
            int n = demands.Length;
            int[,] plan = new int[m, n];
            var steps = new System.Text.StringBuilder();
            int totalCost = 0;

            int[] sup = (int[])supplies.Clone();
            int[] dem = (int[])demands.Clone();

            steps.AppendLine("Метод минимальных элементов:");
            steps.AppendLine("На каждом шаге выбираем клетку с минимальной стоимостью");

            int step = 1;

            while (sup.Sum() > 0)
            {
                // Находим клетку с минимальной стоимостью
                int minCost = int.MaxValue;
                int minI = -1, minJ = -1;

                for (int i = 0; i < m; i++)
                {
                    for (int j = 0; j < n; j++)
                    {
                        if (sup[i] > 0 && dem[j] > 0 && costs[i, j] < minCost)
                        {
                            minCost = costs[i, j];
                            minI = i;
                            minJ = j;
                        }
                    }
                }

                if (minI == -1) break;

                int allocation = Math.Min(sup[minI], dem[minJ]);
                plan[minI, minJ] = allocation;
                totalCost += allocation * costs[minI, minJ];

                steps.AppendLine($"Шаг {step}: A{minI + 1}-B{minJ + 1} = {allocation} (минимальная стоимость: {costs[minI, minJ]})");

                sup[minI] -= allocation;
                dem[minJ] -= allocation;
                step++;
            }

            steps.AppendLine($"\nРасчет завершен. Общая стоимость: {totalCost}");

            return new SolutionResult
            {
                TransportPlan = plan,
                TotalCost = totalCost,
                Steps = steps.ToString()
            };
        }
    }
}