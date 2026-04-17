using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Win32;
using System.IO;
using System.Text;
using System.Data;
using Excel = Microsoft.Office.Interop.Excel;
using System.Runtime.InteropServices;

namespace TransportProblemApp
{
    /// <summary>
    /// Главное окно приложения для решения транспортной задачи
    /// Разработчик: [Ваше имя]
    /// Версия: 2.0
    /// 
    /// Программа позволяет решать транспортную задачу методами:
    /// - Северо-западного угла
    /// - Минимальных элементов
    /// 
    /// Поддерживается:
    /// - Ручной ввод данных
    /// - Загрузка из текстового файла
    /// - Экспорт результатов в Excel (через Interop.Excel)
    /// - Автоматическая балансировка задачи
    /// - Подтверждение очистки данных
    /// </summary>
    public partial class MainWindow : Window
    {
        // Поля класса для хранения размерности задачи
        private int suppliersCount = 3;          // Количество поставщиков
        private int consumersCount = 4;          // Количество потребителей
        private bool isBalanced = true;          // Флаг сбалансированности задачи
        private int originalSuppliersCount = 3;  // Исходное количество поставщиков
        private int originalConsumersCount = 4;  // Исходное количество потребителей

        /// <summary>
        /// Конструктор главного окна
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            InitializeData();
        }

        /// <summary>
        /// Инициализация начальных данных интерфейса
        /// Создает пустую матрицу и списки запасов/потребностей
        /// </summary>
        private void InitializeData()
        {
            CreateCostsMatrix();
            CreateSuppliesDemands();
        }

        /// <summary>
        /// Обработчик изменения размеров матрицы
        /// Позволяет пользователю настроить количество поставщиков и потребителей
        /// </summary>
        private void BtnResize_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Парсинг введенных значений с проверкой корректности
                suppliersCount = int.Parse(txtSuppliers.Text);
                consumersCount = int.Parse(txtConsumers.Text);

                // Валидация: размеры должны быть положительными
                if (suppliersCount <= 0 || consumersCount <= 0)
                {
                    MessageBox.Show("Размеры должны быть положительными числами!", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Пересоздание матрицы с новыми размерами
                CreateCostsMatrix();
                CreateSuppliesDemands();
                txtStatus.Text = $"✅ Создана матрица {suppliersCount}×{consumersCount}";
            }
            catch (FormatException)
            {
                // Обработка ошибки формата ввода
                MessageBox.Show("Введите корректные числа для размеров!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Создание матрицы стоимости перевозок
        /// Формирует заголовки и поля ввода для каждого маршрута
        /// Соответствует стилевому оформлению:
        /// - Шрифт Tw Cen MT, размер 11
        /// - Цвет текста RGB(55, 71, 79)
        /// - Внутренние отступы согласно style guide
        /// </summary>
        private void CreateCostsMatrix()
        {
            costsMatrix.Items.Clear();

            // Создание заголовка матрицы с обозначениями потребителей
            var headerPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Margin = new Thickness(0, 0, 0, 4)
            };

            headerPanel.Children.Add(new TextBlock
            {
                Text = "Поставщик\\Потребитель",
                Width = 120,
                FontWeight = FontWeights.Bold,
                FontFamily = new FontFamily("Tw Cen MT"),
                FontSize = 11,
                Foreground = new SolidColorBrush(Color.FromRgb(55, 71, 79)),
                VerticalAlignment = VerticalAlignment.Center
            });

            // Добавление заголовков столбцов (потребители)
            for (int j = 0; j < consumersCount; j++)
            {
                headerPanel.Children.Add(new TextBlock
                {
                    Text = $"B{j + 1}",
                    Width = 60,
                    TextAlignment = TextAlignment.Center,
                    FontWeight = FontWeights.Bold,
                    FontFamily = new FontFamily("Tw Cen MT"),
                    FontSize = 11,
                    Foreground = new SolidColorBrush(Color.FromRgb(55, 71, 79)),
                    VerticalAlignment = VerticalAlignment.Center
                });
            }
            costsMatrix.Items.Add(headerPanel);

            // Создание строк матрицы с полями ввода
            for (int i = 0; i < suppliersCount; i++)
            {
                var rowPanel = new StackPanel { Orientation = Orientation.Horizontal };

                rowPanel.Children.Add(new TextBlock
                {
                    Text = $"A{i + 1}",
                    Width = 120,
                    VerticalAlignment = VerticalAlignment.Center,
                    FontWeight = FontWeights.Bold,
                    FontFamily = new FontFamily("Tw Cen MT"),
                    FontSize = 11,
                    Foreground = new SolidColorBrush(Color.FromRgb(55, 71, 79))
                });

                // Поля ввода стоимости для каждого потребителя
                for (int j = 0; j < consumersCount; j++)
                {
                    var textBox = new TextBox
                    {
                        Width = 60,
                        Height = 22,  // Маленький размер согласно style guide
                        Margin = new Thickness(2),
                        Text = "0",
                        TextAlignment = TextAlignment.Center,
                        FontFamily = new FontFamily("Tw Cen MT"),
                        FontSize = 11,
                        Background = Brushes.White
                    };
                    rowPanel.Children.Add(textBox);
                }
                costsMatrix.Items.Add(rowPanel);
            }
        }

        /// <summary>
        /// Создание полей ввода для запасов поставщиков и потребностей потребителей
        /// </summary>
        private void CreateSuppliesDemands()
        {
            suppliesList.Items.Clear();
            demandsList.Items.Clear();

            // Создание полей для запасов поставщиков
            for (int i = 0; i < suppliersCount; i++)
            {
                var stackPanel = new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Margin = new Thickness(0, 0, 4, 0)  // margin согласно style guide
                };

                stackPanel.Children.Add(new TextBlock
                {
                    Text = $"Поставщик A{i + 1}:",
                    Width = 100,
                    VerticalAlignment = VerticalAlignment.Center,
                    FontFamily = new FontFamily("Tw Cen MT"),
                    FontSize = 11,
                    Foreground = new SolidColorBrush(Color.FromRgb(55, 71, 79))
                });

                var textBox = new TextBox
                {
                    Width = 80,
                    Height = 22,  // Маленький размер
                    Margin = new Thickness(8, 0, 0, 0),  // margin 8px
                    Text = "0",
                    TextAlignment = TextAlignment.Center,
                    FontFamily = new FontFamily("Tw Cen MT"),
                    FontSize = 11
                };
                stackPanel.Children.Add(textBox);
                suppliesList.Items.Add(stackPanel);
            }

            // Создание полей для потребностей потребителей
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
                    VerticalAlignment = VerticalAlignment.Center,
                    FontFamily = new FontFamily("Tw Cen MT"),
                    FontSize = 11,
                    Foreground = new SolidColorBrush(Color.FromRgb(55, 71, 79))
                });

                var textBox = new TextBox
                {
                    Width = 80,
                    Height = 22,
                    Margin = new Thickness(8, 0, 0, 0),
                    Text = "0",
                    TextAlignment = TextAlignment.Center,
                    FontFamily = new FontFamily("Tw Cen MT"),
                    FontSize = 11
                };
                stackPanel.Children.Add(textBox);
                demandsList.Items.Add(stackPanel);
            }
        }

        /// <summary>
        /// Загрузка примера данных для демонстрации работы программы
        /// Используются данные из Варианта №1 (холодильники)
        /// </summary>
        private void BtnExample_Click(object sender, RoutedEventArgs e)
        {
            // Установка размеров согласно варианту 1
            txtSuppliers.Text = "3";
            txtConsumers.Text = "4";
            suppliersCount = 3;
            consumersCount = 4;

            CreateCostsMatrix();
            CreateSuppliesDemands();

            // Заполнение матрицы стоимости (Вариант №1 - холодильники)
            // Склад 1
            SetMatrixValue(0, 0, "5"); SetMatrixValue(0, 1, "4");
            SetMatrixValue(0, 2, "6"); SetMatrixValue(0, 3, "3");
            // Склад 2
            SetMatrixValue(1, 0, "7"); SetMatrixValue(1, 1, "3");
            SetMatrixValue(1, 2, "3"); SetMatrixValue(1, 3, "2");
            // Склад 3
            SetMatrixValue(2, 0, "9"); SetMatrixValue(2, 1, "5");
            SetMatrixValue(2, 2, "2"); SetMatrixValue(2, 3, "6");

            // Запасы складов
            SetSupplyValue(0, "40");
            SetSupplyValue(1, "22");
            SetSupplyValue(2, "38");

            // Потребности магазинов
            SetDemandValue(0, "20");
            SetDemandValue(1, "15");
            SetDemandValue(2, "35");
            SetDemandValue(3, "30");

            txtStatus.Text = "✅ Загружен пример данных (Вариант №1 - холодильники)";
        }

        /// <summary>
        /// Вспомогательный метод для установки значения в матрице стоимости
        /// </summary>
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

        /// <summary>
        /// Вспомогательный метод для установки значения запаса поставщика
        /// </summary>
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

        /// <summary>
        /// Вспомогательный метод для установки значения потребности
        /// </summary>
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

        /// <summary>
        /// Очистка всех полей ввода с подтверждением
        /// Также очищает результаты расчетов
        /// </summary>
        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            // Запрос подтверждения очистки
            MessageBoxResult result = MessageBox.Show(
                "Вы уверены, что хотите очистить все данные?\n" +
                "Будут удалены введенные значения и результаты расчетов.",
                "Подтверждение очистки",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes)
            {
                return;
            }

            // Очистка матрицы стоимости
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

            // Очистка запасов
            foreach (var item in suppliesList.Items)
            {
                if (item is StackPanel panel && panel.Children.Count > 1)
                {
                    if (panel.Children[1] is TextBox textBox)
                        textBox.Text = "0";
                }
            }

            // Очистка потребностей
            foreach (var item in demandsList.Items)
            {
                if (item is StackPanel panel && panel.Children.Count > 1)
                {
                    if (panel.Children[1] is TextBox textBox)
                        textBox.Text = "0";
                }
            }

            // Очистка результатов
            dgResults.ItemsSource = null;
            txtMethodName.Text = "";
            txtTotalCost.Text = "";
            txtBalance.Text = "";
            txtSteps.Text = "";
            isBalanced = true;

            // Сброс размеров к исходным
            txtSuppliers.Text = "3";
            txtConsumers.Text = "4";
            suppliersCount = 3;
            consumersCount = 4;

            txtStatus.Text = "✅ Все поля и результаты очищены";
        }

        /// <summary>
        /// Загрузка данных из текстового файла
        /// Формат файла:
        /// [количество поставщиков] [количество потребителей]
        /// [матрица стоимости построчно]
        /// [запасы поставщиков через пробел]
        /// [потребности потребителей через пробел]
        /// 
        /// Пример файла для Варианта №1:
        /// 3 4
        /// 5 4 6 3
        /// 7 3 3 2
        /// 9 5 2 6
        /// 40 22 38
        /// 20 15 35 30
        /// </summary>
        private void BtnLoadFile_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Диалог выбора файла
                OpenFileDialog openFileDialog = new OpenFileDialog
                {
                    Title = "Выберите файл с данными транспортной задачи",
                    Filter = "Текстовые файлы (*.txt)|*.txt|Все файлы (*.*)|*.*",
                    DefaultExt = ".txt"
                };

                if (openFileDialog.ShowDialog() == true)
                {
                    // Чтение всех строк из файла
                    string[] lines = File.ReadAllLines(openFileDialog.FileName, Encoding.UTF8);

                    if (lines.Length < 3)
                    {
                        MessageBox.Show("Файл должен содержать минимум 3 строки данных!",
                            "Ошибка формата", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    // Парсинг первой строки: размеры матрицы
                    string[] sizes = lines[0].Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                    if (sizes.Length < 2)
                    {
                        MessageBox.Show("Первая строка должна содержать количество поставщиков и потребителей!",
                            "Ошибка формата", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    suppliersCount = int.Parse(sizes[0]);
                    consumersCount = int.Parse(sizes[1]);
                    txtSuppliers.Text = suppliersCount.ToString();
                    txtConsumers.Text = consumersCount.ToString();

                    // Пересоздание интерфейса под новые размеры
                    CreateCostsMatrix();
                    CreateSuppliesDemands();

                    // Чтение матрицы стоимости
                    for (int i = 0; i < suppliersCount && i + 1 < lines.Length; i++)
                    {
                        string[] costsRow = lines[i + 1].Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                        for (int j = 0; j < Math.Min(consumersCount, costsRow.Length); j++)
                        {
                            SetMatrixValue(i, j, costsRow[j]);
                        }
                    }

                    // Чтение запасов поставщиков
                    int suppliesLine = suppliersCount + 1;
                    if (suppliesLine < lines.Length)
                    {
                        string[] suppliesValues = lines[suppliesLine].Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                        for (int i = 0; i < Math.Min(suppliersCount, suppliesValues.Length); i++)
                        {
                            SetSupplyValue(i, suppliesValues[i]);
                        }
                    }

                    // Чтение потребностей потребителей
                    int demandsLine = suppliersCount + 2;
                    if (demandsLine < lines.Length)
                    {
                        string[] demandsValues = lines[demandsLine].Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                        for (int j = 0; j < Math.Min(consumersCount, demandsValues.Length); j++)
                        {
                            SetDemandValue(j, demandsValues[j]);
                        }
                    }

                    txtStatus.Text = $"✅ Данные загружены из файла: {Path.GetFileName(openFileDialog.FileName)}";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке файла:\n{ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                txtStatus.Text = "❌ Ошибка при загрузке файла";
            }
        }

        /// <summary>
        /// Экспорт результатов в Excel с использованием Microsoft.Office.Interop.Excel
        /// Создает полноценный Excel-файл с форматированием
        /// </summary>
        private void BtnExportExcel_Click(object sender, RoutedEventArgs e)
        {
            Excel.Application excelApp = null;
            Excel.Workbooks workbooks = null;
            Excel.Workbook workbook = null;
            Excel.Sheets sheets = null;
            Excel.Worksheet worksheet = null;

            try
            {
                // Проверка наличия результатов
                if (dgResults.ItemsSource == null)
                {
                    MessageBox.Show("Нет данных для экспорта. Сначала выполните расчет!",
                        "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Диалог сохранения
                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    Title = "Сохранить результаты в Excel",
                    Filter = "Файлы Excel (*.xlsx)|*.xlsx",
                    DefaultExt = ".xlsx",
                    FileName = $"Транспортная_задача_{DateTime.Now:yyyyMMdd_HHmmss}"
                };

                if (saveFileDialog.ShowDialog() != true)
                    return;

                // Создание Excel приложения
                excelApp = new Excel.Application();
                excelApp.Visible = false;
                excelApp.DisplayAlerts = false;

                // Получаем коллекцию книг
                workbooks = excelApp.Workbooks;
                workbook = workbooks.Add();
                sheets = workbook.Sheets;
                worksheet = (Excel.Worksheet)sheets[1];
                worksheet.Name = "Результаты расчета";

                int currentRow = 1;

                // Заголовок
                Excel.Range titleRange = worksheet.Range[$"A{currentRow}:E{currentRow}"];
                titleRange.Merge();
                titleRange.Value = "ТРАНСПОРТНАЯ ЗАДАЧА - РЕЗУЛЬТАТЫ РЕШЕНИЯ";
                titleRange.Font.Bold = true;
                titleRange.Font.Size = 14;
                titleRange.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                currentRow += 2;

                // Информация о расчете
                worksheet.Cells[currentRow, 1].Value = "Дата и время:";
                worksheet.Cells[currentRow, 2].Value = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss");
                currentRow++;

                worksheet.Cells[currentRow, 1].Value = "Метод решения:";
                worksheet.Cells[currentRow, 2].Value = txtMethodName.Text;
                currentRow++;

                // Стоимость
                Excel.Range costRange = worksheet.Range[$"A{currentRow}:D{currentRow}"];
                costRange.Merge();
                costRange.Value = txtTotalCost.Text;
                costRange.Font.Bold = true;
                costRange.Font.Size = 12;
                costRange.Font.Color = 0x0000FF00; // Зеленый цвет (BGR формат: 00FF00 = зеленый)
                currentRow += 2;

                // Опорный план
                worksheet.Cells[currentRow, 1].Value = "ОПОРНЫЙ ПЛАН ПЕРЕВОЗОК:";
                worksheet.Cells[currentRow, 1].Font.Bold = true;
                currentRow++;

                var dataView = dgResults.ItemsSource as DataView;
                int tableStartRow = currentRow;
                int tableEndRow = currentRow;
                int tableEndCol = 1;

                if (dataView != null)
                {
                    var table = dataView.Table;

                    // Заголовки таблицы
                    for (int col = 0; col < table.Columns.Count; col++)
                    {
                        worksheet.Cells[currentRow, col + 1].Value = table.Columns[col].ColumnName;
                        worksheet.Cells[currentRow, col + 1].Font.Bold = true;
                        worksheet.Cells[currentRow, col + 1].Interior.Color = 0x00C0C0C0; // Светло-серый (BGR: C0C0C0)
                    }
                    currentRow++;

                    // Данные таблицы
                    for (int row = 0; row < table.Rows.Count; row++)
                    {
                        for (int col = 0; col < table.Columns.Count; col++)
                        {
                            worksheet.Cells[currentRow + row, col + 1].Value = table.Rows[row][col];
                        }
                    }
                    tableEndRow = currentRow + table.Rows.Count - 1;
                    tableEndCol = table.Columns.Count;
                    currentRow += table.Rows.Count + 2;
                }

                // Информация о балансе
                worksheet.Cells[currentRow, 1].Value = "ИНФОРМАЦИЯ О БАЛАНСЕ:";
                worksheet.Cells[currentRow, 1].Font.Bold = true;
                currentRow++;

                worksheet.Cells[currentRow, 1].Value = txtBalance.Text;
                currentRow += 2;

                // Пошаговое решение
                worksheet.Cells[currentRow, 1].Value = "ПОШАГОВОЕ РЕШЕНИЕ:";
                worksheet.Cells[currentRow, 1].Font.Bold = true;
                currentRow++;

                // Разбиваем текст на строки и записываем
                string[] stepsLines = txtSteps.Text.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string line in stepsLines)
                {
                    worksheet.Cells[currentRow, 1].Value = line;
                    currentRow++;
                }

                // Автоподбор ширины столбцов
                worksheet.Columns.AutoFit();

                // Добавление границ для таблицы
                if (dataView != null && tableEndRow >= tableStartRow)
                {
                    Excel.Range tableRange = worksheet.Range[
                        worksheet.Cells[tableStartRow, 1],
                        worksheet.Cells[tableEndRow, tableEndCol]];
                    tableRange.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                }

                // Сохранение файла
                string filePath = saveFileDialog.FileName;
                workbook.SaveAs(filePath);
                workbook.Close(true);

                txtStatus.Text = $"✅ Результаты экспортированы: {Path.GetFileName(filePath)}";

                MessageBox.Show($"Результаты успешно сохранены!\n{filePath}",
                    "Экспорт завершен", MessageBoxButton.OK, MessageBoxImage.Information);

                // Открытие файла
                if (MessageBox.Show("Открыть созданный файл?", "Экспорт завершен",
                    MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    System.Diagnostics.Process.Start(filePath);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при экспорте в Excel:\n{ex.Message}\n\nУбедитесь, что установлен Microsoft Excel.",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                txtStatus.Text = "❌ Ошибка при экспорте в Excel";
            }
            finally
            {
                // Освобождение ресурсов Excel в правильном порядке
                if (worksheet != null)
                {
                    Marshal.ReleaseComObject(worksheet);
                    worksheet = null;
                }

                if (sheets != null)
                {
                    Marshal.ReleaseComObject(sheets);
                    sheets = null;
                }

                if (workbook != null)
                {
                    // Workbook уже закрыт, не вызываем Close повторно
                    Marshal.ReleaseComObject(workbook);
                    workbook = null;
                }

                if (workbooks != null)
                {
                    Marshal.ReleaseComObject(workbooks);
                    workbooks = null;
                }

                if (excelApp != null)
                {
                    excelApp.Quit();
                    Marshal.ReleaseComObject(excelApp);
                    excelApp = null;
                }

                // Принудительная сборка мусора
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }

        /// <summary>
        /// Основной метод расчета транспортной задачи
        /// Выполняет:
        /// 1. Получение данных из интерфейса
        /// 2. Проверку и балансировку задачи
        /// 3. Решение выбранным методом
        /// 4. Отображение результатов
        /// </summary>
        private void BtnCalculate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Сохранение исходных размеров для корректного отображения
                originalSuppliersCount = suppliersCount;
                originalConsumersCount = consumersCount;

                // Получение данных из интерфейса
                var costs = GetCostsMatrix();
                var supplies = GetSupplies();
                var demands = GetDemands();

                // Проверка на отрицательные значения (валидация)
                if (!ValidateInputData(costs, supplies, demands))
                {
                    return;
                }

                // Балансировка задачи (добавление фиктивных поставщиков/потребителей)
                var balancedData = BalanceProblem(costs, supplies, demands);
                costs = balancedData.costs;
                supplies = balancedData.supplies;
                demands = balancedData.demands;

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

                // Добавление информации о балансировке в название метода
                if (!isBalanced)
                {
                    methodName += " (с фиктивным " + (balancedData.addedRow ? "поставщиком" : "потребителем") + ")";
                }

                txtMethodName.Text = methodName;

                // Выполнение расчета
                var result = solver.Solve(costs, supplies, demands);
                int realCost = CalculateRealCost(result.TransportPlan, balancedData, costs);

                // Отображение результатов
                DisplayResults(result, balancedData, realCost);

                txtStatus.Text = $"✅ Расчет завершен. Стоимость: {realCost}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при расчете: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                txtStatus.Text = "❌ Ошибка при расчете";
            }
        }

        /// <summary>
        /// Валидация входных данных
        /// Проверяет, что все значения неотрицательные
        /// </summary>
        private bool ValidateInputData(int[,] costs, int[] supplies, int[] demands)
        {
            // Проверка стоимости (может быть любым неотрицательным числом)
            for (int i = 0; i < costs.GetLength(0); i++)
            {
                for (int j = 0; j < costs.GetLength(1); j++)
                {
                    if (costs[i, j] < 0)
                    {
                        MessageBox.Show($"Отрицательная стоимость в ячейке A{i + 1}→B{j + 1}!\n" +
                            "Стоимость не может быть отрицательной.", "Ошибка валидации",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                        return false;
                    }
                }
            }

            // Проверка запасов
            for (int i = 0; i < supplies.Length; i++)
            {
                if (supplies[i] < 0)
                {
                    MessageBox.Show($"Отрицательный запас у поставщика A{i + 1}!\n" +
                        "Запас не может быть отрицательным.", "Ошибка валидации",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
            }

            // Проверка потребностей
            for (int j = 0; j < demands.Length; j++)
            {
                if (demands[j] < 0)
                {
                    MessageBox.Show($"Отрицательная потребность у потребителя B{j + 1}!\n" +
                        "Потребность не может быть отрицательной.", "Ошибка валидации",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Балансировка транспортной задачи
        /// Если сумма запасов не равна сумме потребностей,
        /// добавляется фиктивный поставщик или потребитель
        /// </summary>
        private (int[,] costs, int[] supplies, int[] demands, bool addedRow, bool addedColumn)
            BalanceProblem(int[,] costs, int[] supplies, int[] demands)
        {
            int totalSupply = supplies.Sum();
            int totalDemand = demands.Sum();

            txtBalance.Text = $"Общие запасы: {totalSupply} | Общие потребности: {totalDemand}";

            // Задача сбалансирована
            if (totalSupply == totalDemand)
            {
                isBalanced = true;
                return (costs, supplies, demands, false, false);
            }

            // Задача не сбалансирована - требуется добавление фиктивных элементов
            isBalanced = false;
            int balanceDiff = Math.Abs(totalSupply - totalDemand);

            if (totalSupply > totalDemand)
            {
                // Добавление фиктивного потребителя
                int newConsumersCount = consumersCount + 1;
                var newCosts = new int[suppliersCount, newConsumersCount];
                var newDemands = new int[newConsumersCount];

                // Копирование существующих данных
                for (int i = 0; i < suppliersCount; i++)
                {
                    for (int j = 0; j < consumersCount; j++)
                        newCosts[i, j] = costs[i, j];
                    newCosts[i, consumersCount] = 0;  // Нулевая стоимость для фиктивного
                }

                for (int j = 0; j < consumersCount; j++)
                    newDemands[j] = demands[j];
                newDemands[consumersCount] = balanceDiff;

                MessageBox.Show($"⚠️ Задача не сбалансирована!\n" +
                    $"Добавлен фиктивный потребитель B{consumersCount + 1} с потребностью {balanceDiff}",
                    "Балансировка", MessageBoxButton.OK, MessageBoxImage.Information);

                return (newCosts, supplies, newDemands, false, true);
            }
            else
            {
                // Добавление фиктивного поставщика
                int newSuppliersCount = suppliersCount + 1;
                var newCosts = new int[newSuppliersCount, consumersCount];
                var newSupplies = new int[newSuppliersCount];

                // Копирование существующих данных
                for (int i = 0; i < suppliersCount; i++)
                {
                    for (int j = 0; j < consumersCount; j++)
                        newCosts[i, j] = costs[i, j];
                    newSupplies[i] = supplies[i];
                }

                for (int j = 0; j < consumersCount; j++)
                    newCosts[suppliersCount, j] = 0;  // Нулевая стоимость для фиктивного
                newSupplies[suppliersCount] = balanceDiff;

                MessageBox.Show($"⚠️ Задача не сбалансирована!\n" +
                    $"Добавлен фиктивный поставщик A{suppliersCount + 1} с запасом {balanceDiff}",
                    "Балансировка", MessageBoxButton.OK, MessageBoxImage.Information);

                return (newCosts, newSupplies, demands, true, false);
            }
        }

        /// <summary>
        /// Расчет реальной стоимости перевозок (без учета фиктивных)
        /// </summary>
        private int CalculateRealCost(int[,] transportPlan,
            (int[,] costs, int[] supplies, int[] demands, bool addedRow, bool addedColumn) balancedData,
            int[,] originalCosts)
        {
            int realCost = 0;

            if (isBalanced)
            {
                // Для сбалансированной задачи считаем полную стоимость
                for (int i = 0; i < transportPlan.GetLength(0); i++)
                    for (int j = 0; j < transportPlan.GetLength(1); j++)
                        realCost += transportPlan[i, j] * balancedData.costs[i, j];
                return realCost;
            }

            // Для несбалансированной задачи исключаем фиктивные перевозки
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

        /// <summary>
        /// Отображение результатов расчета в интерфейсе
        /// </summary>
        private void DisplayResults(SolutionResult result,
            (int[,] costs, int[] supplies, int[] demands, bool addedRow, bool addedColumn) balancedData,
            int realCost)
        {
            var dataTable = new DataTable();

            // Создание столбцов таблицы
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

            // Заполнение строк таблицы
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

            // Добавление строки с потребностями
            var demandRow = dataTable.NewRow();
            demandRow[0] = "Потребности";
            for (int j = 0; j < displayConsumersCount; j++)
                demandRow[j + 1] = balancedData.demands[j];
            demandRow[displayConsumersCount + 1] = balancedData.demands.Sum();
            dataTable.Rows.Add(demandRow);

            dgResults.ItemsSource = dataTable.DefaultView;

            // Формирование информации о стоимости
            if (!isBalanced)
            {
                txtTotalCost.Text = $"ОБЩАЯ СТОИМОСТЬ ПЕРЕВОЗОК: {realCost}\n" +
                                  $"Полная стоимость (с фиктивными): {result.TotalCost}\n" +
                                  $"Задача была несбалансирована, добавлен фиктивный " +
                                  (balancedData.addedRow ? "поставщик" : "потребитель");
            }
            else
            {
                txtTotalCost.Text = $"ОБЩАЯ СТОИМОСТЬ ПЕРЕВОЗОК: {realCost}";
            }

            // Дополнение шагов решения информацией о балансировке
            string stepsWithInfo = result.Steps;
            if (!isBalanced)
            {
                stepsWithInfo += $"\n\n═══════════════════════════════════════\n";
                stepsWithInfo += $"ИНФОРМАЦИЯ О БАЛАНСИРОВКЕ\n";
                if (balancedData.addedRow)
                    stepsWithInfo += $"Добавлен фиктивный поставщик A{suppliersCount + 1} с запасом {balancedData.supplies[suppliersCount]}\n";
                if (balancedData.addedColumn)
                    stepsWithInfo += $"Добавлен фиктивный потребитель B{consumersCount + 1} с потребностью {balancedData.demands[consumersCount]}\n";
                stepsWithInfo += $"Реальная стоимость (без фиктивных перевозок): {realCost}";
            }

            txtSteps.Text = stepsWithInfo;

            tabResults.IsSelected = true;
        }

        /// <summary>
        /// Получение матрицы стоимости из интерфейса
        /// </summary>
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

        /// <summary>
        /// Получение массива запасов поставщиков
        /// </summary>
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

        /// <summary>
        /// Получение массива потребностей потребителей
        /// </summary>
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

    /// <summary>
    /// Класс для хранения результатов решения транспортной задачи
    /// </summary>
    public class SolutionResult
    {
        /// <summary>Матрица опорного плана перевозок</summary>
        public int[,] TransportPlan { get; set; }

        /// <summary>Общая стоимость перевозок</summary>
        public int TotalCost { get; set; }

        /// <summary>Пошаговое описание решения</summary>
        public string Steps { get; set; }
    }

    /// <summary>
    /// Интерфейс для реализации различных методов решения транспортной задачи
    /// Паттерн "Стратегия" позволяет легко добавлять новые методы решения
    /// </summary>
    public interface TransportSolver
    {
        /// <summary>Метод решения транспортной задачи</summary>
        /// <param name="costs">Матрица стоимости перевозок</param>
        /// <param name="supplies">Запасы поставщиков</param>
        /// <param name="demands">Потребности потребителей</param>
        /// <returns>Результат решения</returns>
        SolutionResult Solve(int[,] costs, int[] supplies, int[] demands);
    }

    /// <summary>
    /// Реализация метода северо-западного угла
    /// Начинает заполнение с левой верхней клетки матрицы
    /// </summary>
    public class NorthwestCornerMethod : TransportSolver
    {
        public SolutionResult Solve(int[,] costs, int[] supplies, int[] demands)
        {
            int m = supplies.Length;
            int n = demands.Length;
            int[,] plan = new int[m, n];
            var steps = new StringBuilder();
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

    /// <summary>
    /// Реализация метода минимальных элементов
    /// На каждом шаге выбирает клетку с минимальной стоимостью
    /// </summary>
    public class MinimumCostMethod : TransportSolver
    {
        public SolutionResult Solve(int[,] costs, int[] supplies, int[] demands)
        {
            int m = supplies.Length;
            int n = demands.Length;
            int[,] plan = new int[m, n];
            var steps = new StringBuilder();
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

                // Поиск клетки с минимальной стоимостью
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