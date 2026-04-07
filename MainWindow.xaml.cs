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
        private bool isBalanced = true;
        private int originalSuppliersCount = 3;
        private int originalConsumersCount = 5;

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
                    MessageBox.Show("Размеры должны быть положительными числами!", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                CreateCostsMatrix();
                CreateSuppliesDemands();
                txtStatus.Text = $"Создана матрица {suppliersCount}×{consumersCount}";
            }
            catch (FormatException)
            {
                MessageBox.Show("Введите корректные числа для размеров!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CreateCostsMatrix()
        {
            costsMatrix.Items.Clear();

            // Заголовок
            var headerPanel = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 0, 0, 4) };
            headerPanel.Children.Add(new TextBlock
            {
                Text = "Поставщик\\Потребитель",
                Width = 120,
                FontWeight = FontWeights.Bold,
                VerticalAlignment = VerticalAlignment.Center
            });

            for (int j = 0; j < consumersCount; j++)
            {
                headerPanel.Children.Add(new TextBlock
                {
                    Text = $"B{j + 1}",
                    Width = 60,
                    TextAlignment = TextAlignment.Center,
                    FontWeight = FontWeights.Bold,
                    VerticalAlignment = VerticalAlignment.Center
                });
            }
            costsMatrix.Items.Add(headerPanel);

            // Данные
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
                        Height = 22,
                        Margin = new Thickness(2),
                        Text = "0",
                        TextAlignment = TextAlignment.Center,
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

            for (int i = 0; i < suppliersCount; i++)
            {
                var stackPanel = new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Margin = new Thickness(0, 0, 4, 0)  // right=4
                };
                stackPanel.Children.Add(new TextBlock
                {
                    Text = $"Поставщик A{i + 1}:",
                    Width = 100,
                    VerticalAlignment = VerticalAlignment.Center
                });

                var textBox = new TextBox
                {
                    Width = 80,
                    Height = 22,
                    Margin = new Thickness(8, 0, 0, 0),  // left=8
                    Text = "0",
                    TextAlignment = TextAlignment.Center
                };
                stackPanel.Children.Add(textBox);
                suppliesList.Items.Add(stackPanel);
            }

            for (int j = 0; j < consumersCount; j++)
            {
                var stackPanel = new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Margin = new Thickness(0, 2, 0, 0)  // top=2
                };
                stackPanel.Children.Add(new TextBlock
                {
                    Text = $"Потребитель B{j + 1}:",
                    Width = 100,
                    VerticalAlignment = VerticalAlignment.Center
                });

                var textBox = new TextBox
                {
                    Width = 80,
                    Height = 22,
                    Margin = new Thickness(8, 0, 0, 0),  // left=8
                    Text = "0",
                    TextAlignment = TextAlignment.Center
                };
                stackPanel.Children.Add(textBox);
                demandsList.Items.Add(stackPanel);
            }
        }

        private void BtnExample_Click(object sender, RoutedEventArgs e)
        {
            txtSuppliers.Text = "3";
            txtConsumers.Text = "5";
            suppliersCount = 3;
            consumersCount = 5;

            CreateCostsMatrix();
            CreateSuppliesDemands();

            // Пример данных
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
                    if (textBox != null) textBox.Text = value;
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
                    if (textBox != null) textBox.Text = value;
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
                    if (textBox != null) textBox.Text = value;
                }
            }
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in costsMatrix.Items)
            {
                if (item is StackPanel rowPanel)
                {
                    foreach (var child in rowPanel.Children)
                    {
                        if (child is TextBox textBox) textBox.Text = "0";
                    }
                }
            }

            foreach (var item in suppliesList.Items)
            {
                if (item is StackPanel panel && panel.Children.Count > 1)
                {
                    if (panel.Children[1] is TextBox textBox) textBox.Text = "0";
                }
            }

            foreach (var item in demandsList.Items)
            {
                if (item is StackPanel panel && panel.Children.Count > 1)
                {
                    if (panel.Children[1] is TextBox textBox) textBox.Text = "0";
                }
            }

            txtStatus.Text = " Все поля очищены";
        }

        private void BtnCalculate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                originalSuppliersCount = suppliersCount;
                originalConsumersCount = consumersCount;

                var costs = GetCostsMatrix();
                var supplies = GetSupplies();
                var demands = GetDemands();

                var balancedData = BalanceProblem(costs, supplies, demands);
                costs = balancedData.costs;
                supplies = balancedData.supplies;
                demands = balancedData.demands;

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

                if (!isBalanced)
                {
                    methodName += " (с фиктивным " + (balancedData.addedRow ? "поставщиком" : "потребителем") + ")";
                }

                txtMethodName.Text = methodName;

                var result = solver.Solve(costs, supplies, demands);
                int realCost = CalculateRealCost(result.TransportPlan, balancedData, costs);

                DisplayResults(result, balancedData, realCost);

                txtStatus.Text = $"Расчет завершен. Стоимость: {realCost}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при расчете: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                txtStatus.Text = " Ошибка при расчете";
            }
        }

        private (int[,] costs, int[] supplies, int[] demands, bool addedRow, bool addedColumn)
            BalanceProblem(int[,] costs, int[] supplies, int[] demands)
        {
            int totalSupply = supplies.Sum();
            int totalDemand = demands.Sum();

            txtBalance.Text = $"Общие запасы: {totalSupply} | Общие потребности: {totalDemand}";

            if (totalSupply == totalDemand)
            {
                isBalanced = true;
                return (costs, supplies, demands, false, false);
            }

            isBalanced = false;
            int balanceDiff = Math.Abs(totalSupply - totalDemand);

            if (totalSupply > totalDemand)
            {
                int newConsumersCount = consumersCount + 1;
                var newCosts = new int[suppliersCount, newConsumersCount];
                var newDemands = new int[newConsumersCount];

                for (int i = 0; i < suppliersCount; i++)
                {
                    for (int j = 0; j < consumersCount; j++) newCosts[i, j] = costs[i, j];
                    newCosts[i, consumersCount] = 0;
                }

                for (int j = 0; j < consumersCount; j++) newDemands[j] = demands[j];
                newDemands[consumersCount] = balanceDiff;

                MessageBox.Show($"⚠️ Задача не сбалансирована!\nДобавлен фиктивный потребитель B{consumersCount + 1} с потребностью {balanceDiff}",
                    "Балансировка", MessageBoxButton.OK, MessageBoxImage.Information);

                return (newCosts, supplies, newDemands, false, true);
            }
            else
            {
                int newSuppliersCount = suppliersCount + 1;
                var newCosts = new int[newSuppliersCount, consumersCount];
                var newSupplies = new int[newSuppliersCount];

                for (int i = 0; i < suppliersCount; i++)
                {
                    for (int j = 0; j < consumersCount; j++) newCosts[i, j] = costs[i, j];
                    newSupplies[i] = supplies[i];
                }

                for (int j = 0; j < consumersCount; j++) newCosts[suppliersCount, j] = 0;
                newSupplies[suppliersCount] = balanceDiff;

                MessageBox.Show($"⚠️ Задача не сбалансирована!\nДобавлен фиктивный поставщик A{suppliersCount + 1} с запасом {balanceDiff}",
                    "Балансировка", MessageBoxButton.OK, MessageBoxImage.Information);

                return (newCosts, newSupplies, demands, true, false);
            }
        }

        private int CalculateRealCost(int[,] transportPlan, (int[,] costs, int[] supplies, int[] demands, bool addedRow, bool addedColumn) balancedData, int[,] originalCosts)
        {
            int realCost = 0;

            if (isBalanced)
            {
                for (int i = 0; i < transportPlan.GetLength(0); i++)
                    for (int j = 0; j < transportPlan.GetLength(1); j++)
                        realCost += transportPlan[i, j] * balancedData.costs[i, j];
                return realCost;
            }

            if (balancedData.addedRow)
            {
                for (int i = 0; i < originalSuppliersCount; i++)
                    for (int j = 0; j < originalConsumersCount; j++)
                        realCost += transportPlan[i, j] * originalCosts[i, j];
            }
            else if (balancedData.addedColumn)
            {
                for (int i = 0; i < originalSuppliersCount; i++)
                    for (int j = 0; j < originalConsumersCount; j++)
                        realCost += transportPlan[i, j] * originalCosts[i, j];
            }

            return realCost;
        }

        private void DisplayResults(SolutionResult result, (int[,] costs, int[] supplies, int[] demands, bool addedRow, bool addedColumn) balancedData, int realCost)
        {
            var dataTable = new System.Data.DataTable();

            dataTable.Columns.Add("Поставщик\\Потребитель");
            int displayConsumersCount = balancedData.addedColumn ? consumersCount + 1 : consumersCount;

            for (int j = 0; j < displayConsumersCount; j++)
            {
                string consumerName = balancedData.addedColumn && j == consumersCount ?
                    $"B{j + 1} (фикт.)" : $"B{j + 1}";
                dataTable.Columns.Add(consumerName);
            }
            dataTable.Columns.Add("Запасы");

            int displaySuppliersCount = balancedData.addedRow ? suppliersCount + 1 : suppliersCount;

            for (int i = 0; i < displaySuppliersCount; i++)
            {
                var row = dataTable.NewRow();
                string supplierName = balancedData.addedRow && i == suppliersCount ?
                    $"A{i + 1} (фикт.)" : $"A{i + 1}";
                row[0] = supplierName;

                for (int j = 0; j < displayConsumersCount; j++)
                    row[j + 1] = result.TransportPlan[i, j];

                row[displayConsumersCount + 1] = balancedData.supplies[i];
                dataTable.Rows.Add(row);
            }

            var demandRow = dataTable.NewRow();
            demandRow[0] = "Потребности";
            for (int j = 0; j < displayConsumersCount; j++)
                demandRow[j + 1] = balancedData.demands[j];
            demandRow[displayConsumersCount + 1] = balancedData.demands.Sum();
            dataTable.Rows.Add(demandRow);

            dgResults.ItemsSource = dataTable.DefaultView;

            if (!isBalanced)
            {
                txtTotalCost.Text = $"ОБЩАЯ СТОИМОСТЬ ПЕРЕВОЗОК: {realCost}\n" +
                                  $"Полная стоимость (с фиктивными): {result.TotalCost}\n" +
                                  $" Задача была несбалансирована, добавлен фиктивный " +
                                  (balancedData.addedRow ? "поставщик" : "потребитель");
            }
            else
            {
                txtTotalCost.Text = $"ОБЩАЯ СТОИМОСТЬ ПЕРЕВОЗОК: {realCost}";
            }

            string stepsWithInfo = result.Steps;
            if (!isBalanced)
            {
                stepsWithInfo += $"\n\n═══════════════════════════════════════\n";
                stepsWithInfo += $" ИНФОРМАЦИЯ О БАЛАНСИРОВКЕ\n";
                if (balancedData.addedRow)
                    stepsWithInfo += $" Добавлен фиктивный поставщик A{suppliersCount + 1} с запасом {balancedData.supplies[suppliersCount]}\n";
                if (balancedData.addedColumn)
                    stepsWithInfo += $" Добавлен фиктивный потребитель B{consumersCount + 1} с потребностью {balancedData.demands[consumersCount]}\n";
                stepsWithInfo += $" Реальная стоимость (без фиктивных перевозок): {realCost}";
            }

            txtSteps.Text = stepsWithInfo;
            tabResults.IsSelected = true;
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
                            costs[i, j] = value;
                        else
                            costs[i, j] = 0;
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
                        supplies[i] = value;
                    else
                        supplies[i] = 0;
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
                        demands[i] = value;
                    else
                        demands[i] = 0;
                }
            }
            return demands;
        }
    }

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

            steps.AppendLine("╔══════════════════════════════════════════════════════════════╗");
            steps.AppendLine("║     МЕТОД СЕВЕРО-ЗАПАДНОГО УГЛА - ПОШАГОВОЕ РЕШЕНИЕ          ║");
            steps.AppendLine("╚══════════════════════════════════════════════════════════════╝");
            steps.AppendLine();
            steps.AppendLine("Начинаем с левого верхнего угла (A1 → B1)");

            int i = 0, j = 0;
            int step = 1;

            while (i < m && j < n)
            {
                int allocation = Math.Min(sup[i], dem[j]);
                plan[i, j] = allocation;
                totalCost += allocation * costs[i, j];

                string cellInfo = $"Шаг {step}: A{i + 1} → B{j + 1} = {allocation}";
                if (costs[i, j] == 0 && allocation > 0)
                    cellInfo += " [фиктивная перевозка]";
                else if (costs[i, j] > 0 && allocation > 0)
                    cellInfo += $" (стоимость: {allocation} × {costs[i, j]} = {allocation * costs[i, j]})";

                steps.AppendLine(cellInfo);

                sup[i] -= allocation;
                dem[j] -= allocation;

                if (sup[i] == 0)
                {
                    steps.AppendLine($"  └─ Запас A{i + 1} исчерпан → переход к следующему поставщику");
                    i++;
                }
                if (dem[j] == 0)
                {
                    steps.AppendLine($"  └─ Потребность B{j + 1} удовлетворена → переход к следующему потребителю");
                    j++;
                }
                step++;
            }

            steps.AppendLine();
            steps.AppendLine("═══════════════════════════════════════════════════════════════");
            steps.AppendLine($"ИТОГО: Полная стоимость перевозок = {totalCost}");
            steps.AppendLine("═══════════════════════════════════════════════════════════════");

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

            steps.AppendLine("╔══════════════════════════════════════════════════════════════╗");
            steps.AppendLine("║       МЕТОД МИНИМАЛЬНЫХ ЭЛЕМЕНТОВ - ПОШАГОВОЕ РЕШЕНИЕ        ║");
            steps.AppendLine("╚══════════════════════════════════════════════════════════════╝");
            steps.AppendLine();
            steps.AppendLine("На каждом шаге выбираем клетку с минимальной стоимостью перевозки");

            int step = 1;

            while (sup.Sum() > 0)
            {
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

                string cellInfo = $"Шаг {step}: A{minI + 1} → B{minJ + 1} = {allocation}";
                if (costs[minI, minJ] == 0 && allocation > 0)
                    cellInfo += " [фиктивная перевозка]";
                else if (costs[minI, minJ] > 0 && allocation > 0)
                    cellInfo += $" (минимальная стоимость: {costs[minI, minJ]})";

                steps.AppendLine(cellInfo);

                sup[minI] -= allocation;
                dem[minJ] -= allocation;
                step++;
            }

            steps.AppendLine();
            steps.AppendLine("═══════════════════════════════════════════════════════════════");
            steps.AppendLine($"ИТОГО: Полная стоимость перевозок = {totalCost}");
            steps.AppendLine("═══════════════════════════════════════════════════════════════");

            return new SolutionResult
            {
                TransportPlan = plan,
                TotalCost = totalCost,
                Steps = steps.ToString()
            };
        }
    }
}