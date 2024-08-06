﻿using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BallDataVisualizer
{
    public partial class Form1 : Form
    {
        private int predictionChunkSize = 500; // Number of prediction lines generated per index
        private int currentChunkStart = 0; // Track the start index of the current visualization chunk
        private (int Start, int End) visualizationChunkRange = (0, 500); // Start and End of the indices to visualize
        private List<double[]> currentActualData = new List<double[]>(); // Current chunk of actual data
        private Dictionary<string, List<double[]>> currentPredictedDataSets = new Dictionary<string, List<double[]>>(); // Current chunk of predicted data for each file
        private Dictionary<string, Dictionary<int, List<double[]>>> allPredictedDataSets = new Dictionary<string, Dictionary<int, List<double[]>>>(); // All predicted data for analysis
        private const string baseFolderYoyo = "G:\\.RLMods\\Tools\\Simulation\\BallDatas\\";
        private string actualDataFilePath = baseFolderYoyo + "weird_post_angle\\actual_data.csv";
        private List<string> predictedDataFilePaths = new List<string>(); // Store multiple prediction file paths

        private Button nextIndexButton;
        private Button previousIndexButton;
        private Button settingsButton;
        private TextBox currentIndexTextBox;
        private CheckBox showBestLineCheckBox;
        private LiveCharts.WinForms.CartesianChart chartX, chartY, chartZ;
        private Label infoLabel;
        private TabControl tabControl;

        // New controls for visualization range
        private TextBox visualizationChunkStartTextBox;
        private TextBox visualizationChunkEndTextBox;
        private Label visualizationRangeLabel;

        //////////////////////////////////////// [ INITIALIZATION ] ////////////////////////////////////////

        public Form1()
        {
            InitializeComponent();
            InitializeUI();
            LoadAllPredictionsAsync(); // Load all predictions once for analysis
            LoadAndDisplayChunkAsync();
            tabControl.SelectedIndexChanged += TabControl_SelectedIndexChanged;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private void InitializeUI()
        {
            // Initialize TabControl
            tabControl = new TabControl
            {
                Dock = DockStyle.Fill
            };

            var positionTab = new TabPage("Position");
            var velocityTab = new TabPage("Velocity");
            var angularVelocityTab = new TabPage("Angular Velocity");

            tabControl.TabPages.Add(positionTab);
            tabControl.TabPages.Add(velocityTab);
            tabControl.TabPages.Add(angularVelocityTab);

            // Initialize charts
            chartX = new LiveCharts.WinForms.CartesianChart
            {
                Dock = DockStyle.Top,
                Height = 150
            };

            chartY = new LiveCharts.WinForms.CartesianChart
            {
                Dock = DockStyle.Top,
                Height = 150
            };

            chartZ = new LiveCharts.WinForms.CartesianChart
            {
                Dock = DockStyle.Top,
                Height = 150
            };

            // Add charts to each tab
            positionTab.Controls.Add(chartZ);
            positionTab.Controls.Add(chartY);
            positionTab.Controls.Add(chartX);

            velocityTab.Controls.Add(new LiveCharts.WinForms.CartesianChart
            {
                Dock = DockStyle.Top,
                Height = 150
            });
            velocityTab.Controls.Add(new LiveCharts.WinForms.CartesianChart
            {
                Dock = DockStyle.Top,
                Height = 150
            });
            velocityTab.Controls.Add(new LiveCharts.WinForms.CartesianChart
            {
                Dock = DockStyle.Top,
                Height = 150
            });

            angularVelocityTab.Controls.Add(new LiveCharts.WinForms.CartesianChart
            {
                Dock = DockStyle.Top,
                Height = 150
            });
            angularVelocityTab.Controls.Add(new LiveCharts.WinForms.CartesianChart
            {
                Dock = DockStyle.Top,
                Height = 150
            });
            angularVelocityTab.Controls.Add(new LiveCharts.WinForms.CartesianChart
            {
                Dock = DockStyle.Top,
                Height = 150
            });

            // Initialize buttons
            nextIndexButton = new Button
            {
                Text = "Next Index",
                Dock = DockStyle.Bottom,
                Height = 30
            };
            nextIndexButton.Click += NextIndexButton_Click;

            previousIndexButton = new Button
            {
                Text = "Previous Index",
                Dock = DockStyle.Bottom,
                Height = 30
            };
            previousIndexButton.Click += PreviousIndexButton_Click;

            settingsButton = new Button
            {
                Text = "Settings",
                Dock = DockStyle.Bottom,
                Height = 30
            };
            settingsButton.Click += SettingsButton_Click;

            // Initialize the index input box
            currentIndexTextBox = new TextBox
            {
                Text = currentChunkStart.ToString(),
                Dock = DockStyle.Left,
                Width = 50
            };
            currentIndexTextBox.KeyDown += CurrentIndexTextBox_KeyDown;

            // Initialize the show best line checkbox
            showBestLineCheckBox = new CheckBox
            {
                Text = "Show Best Line",
                Dock = DockStyle.Left,
                Width = 120,
                Checked = false // Default to showing the best line
            };
            showBestLineCheckBox.CheckedChanged += ShowBestLineCheckBox_CheckedChanged;

            // Initialize the visualization range input boxes
            visualizationRangeLabel = new Label
            {
                Text = "Visualization Range:",
                Dock = DockStyle.Left,
                Width = 130
            };

            visualizationChunkStartTextBox = new TextBox
            {
                Text = visualizationChunkRange.Start.ToString(),
                Width = 50
            };
            visualizationChunkStartTextBox.TextChanged += VisualizationChunkStartTextBox_TextChanged;

            visualizationChunkEndTextBox = new TextBox
            {
                Text = visualizationChunkRange.End.ToString(),
                Width = 50
            };
            visualizationChunkEndTextBox.TextChanged += VisualizationChunkEndTextBox_TextChanged;

            // Initialize info label
            infoLabel = new Label
            {
                Text = "Info: ",
                Dock = DockStyle.Top,
                Height = 50,
                AutoSize = false,
                Padding = new Padding(10)
            };

            // Create a panel to hold visualization controls horizontally
            var visualizationControlPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Bottom,
                Height = 35,
                FlowDirection = FlowDirection.LeftToRight,
                Padding = new Padding(10)
            };

            // Adjustments for bottom alignment of the labels
            visualizationRangeLabel.TextAlign = ContentAlignment.BottomCenter; // Align text at the bottom

            visualizationControlPanel.Controls.Add(showBestLineCheckBox);
            visualizationControlPanel.Controls.Add(visualizationRangeLabel);
            visualizationControlPanel.Controls.Add(visualizationChunkStartTextBox);
            visualizationControlPanel.Controls.Add(new Label { Text = "to", TextAlign = ContentAlignment.BottomCenter });
            visualizationControlPanel.Controls.Add(visualizationChunkEndTextBox);
            visualizationControlPanel.Controls.Add(new Label { Text = "Index:", TextAlign = ContentAlignment.BottomCenter });
            visualizationControlPanel.Controls.Add(currentIndexTextBox);

            // Add controls to the form
            Controls.Add(tabControl);
            Controls.Add(infoLabel);
            Controls.Add(visualizationControlPanel);
            Controls.Add(nextIndexButton);
            Controls.Add(previousIndexButton);
            Controls.Add(settingsButton);
        }

        /////////////////////////////////////////// [ LOADING ] ////////////////////////////////////////////

        private async void LoadAllPredictionsAsync()
        {
            try
            {
                await Task.Run(() => LoadAllPredictions());
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading predictions: " + ex.Message);
            }
        }

        private void LoadAllPredictions()
        {
            // Load all predictions from multiple files into memory for analysis
            foreach (var filePath in predictedDataFilePaths)
            {
                using (var reader = new StreamReader(filePath))
                {
                    reader.ReadLine(); // Skip header

                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        var values = line.Split(',');
                        if (!int.TryParse(values[0], out int index)) continue;

                        var parsedValues = ParseLine(values, index, filePath);

                        if (parsedValues != null)
                        {
                            if (!allPredictedDataSets.ContainsKey(filePath))
                            {
                                allPredictedDataSets[filePath] = new Dictionary<int, List<double[]>>();
                            }

                            if (!allPredictedDataSets[filePath].ContainsKey(index))
                            {
                                allPredictedDataSets[filePath][index] = new List<double[]>();
                            }
                            allPredictedDataSets[filePath][index].Add(parsedValues);
                        }
                    }
                }
            }
        }

        private async void LoadAndDisplayChunkAsync()
        {
            try
            {
                await Task.Run(() => LoadDataChunk());
                InitializeCharts();
                UpdateInfoLabel();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading data: " + ex.Message);
            }
        }

        private void LoadDataChunk()
        {
            // Clear previous data
            currentActualData.Clear();
            currentPredictedDataSets.Clear();

            // Calculate the start and size for the actual data
            int actualDataStartLine = currentChunkStart + visualizationChunkRange.Start;
            int chunkSize = visualizationChunkRange.End - visualizationChunkRange.Start;

            // Read actual data chunk
            using (var reader = new StreamReader(actualDataFilePath))
            {
                // Skip the header line
                reader.ReadLine();

                // Skip lines to reach the actual data start line
                for (int i = 0; i < actualDataStartLine; i++)
                {
                    if (reader.ReadLine() == null) return; // Prevent out-of-bounds access
                }

                int linesRead = 0;
                while (linesRead < chunkSize && !reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(',');

                    // Parse values safely with error handling
                    var parsedValues = ParseLine(values, actualDataStartLine + linesRead, "actual_data.csv");
                    if (parsedValues != null)
                    {
                        currentActualData.Add(parsedValues);
                        linesRead++;
                    }
                    else
                    {
                        return; // Stop processing if parsing fails
                    }
                }
            }

            // Read predicted data chunk for each file
            foreach (var filePath in predictedDataFilePaths)
            {
                currentPredictedDataSets[filePath] = new List<double[]>();

                int predictionStartLine = currentChunkStart * predictionChunkSize + visualizationChunkRange.Start;

                using (var reader = new StreamReader(filePath))
                {
                    // Skip the header line
                    reader.ReadLine();

                    // Skip directly to the predictionStartLine
                    for (int i = 0; i < predictionStartLine; i++)
                    {
                        if (reader.ReadLine() == null) return; // Prevent out-of-bounds access
                    }

                    int linesRead = 0;
                    while (linesRead < chunkSize && !reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        var values = line.Split(',');

                        // Parse values safely with error handling
                        var parsedValues = ParseLine(values, predictionStartLine + linesRead, filePath);
                        if (parsedValues != null)
                        {
                            currentPredictedDataSets[filePath].Add(parsedValues);
                            linesRead++;
                        }
                        else
                        {
                            return; // Stop processing if parsing fails
                        }
                    }
                }
            }
        }

        private double[] ParseLine(string[] values, int lineNumber, string fileName)
        {
            var parsedValues = new double[values.Length];
            for (int i = 0; i < values.Length; i++)
            {
                if (!double.TryParse(values[i].Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out parsedValues[i]))
                {
                    MessageBox.Show($"Error parsing line {lineNumber} in {fileName}: '{values[i]}' is not a valid number.");
                    return null;
                }
            }
            return parsedValues;
        }

        //////////////////////////////////////// [ CHARTS DRAWING ] ////////////////////////////////////////

        private static readonly Dictionary<string, System.Windows.Media.Color> BaseColors = new Dictionary<string, System.Windows.Media.Color>
        {
            { "X", System.Windows.Media.Colors.Blue },
            { "Y", System.Windows.Media.Colors.Green },
            { "Z", System.Windows.Media.Colors.Red }
        };

        private void InitializeCharts()
        {
            var currentPage = (VisualizationPage)tabControl.SelectedIndex;
            int pageOffset = 1 + (int)currentPage * 3;

            // Get the correct charts for the current page
            var currentTab = tabControl.TabPages[(int)currentPage];
            var charts = currentTab.Controls.OfType<LiveCharts.WinForms.CartesianChart>().ToList();

            if (charts.Count < 3) return; // Ensure we have three charts

            var chartX = charts[2];
            var chartY = charts[1];
            var chartZ = charts[0];

            // Clear previous series
            chartX.Series.Clear();
            chartY.Series.Clear();
            chartZ.Series.Clear();

            // Add series to charts for each predicted dataset
            int index = 0;
            foreach (var predictedDataSet in currentPredictedDataSets.Keys)
            {
                AddChartSeries(chartX, pageOffset, currentPage, "X", BaseColors["X"], index, predictedDataSet);
                AddChartSeries(chartY, pageOffset + 1, currentPage, "Y", BaseColors["Y"], index, predictedDataSet);
                AddChartSeries(chartZ, pageOffset + 2, currentPage, "Z", BaseColors["Z"], index, predictedDataSet);
                index++;
            }

            // Update axes with correct labels starting at visualizationChunkRange.Start
            int startIndex = currentChunkStart + visualizationChunkRange.Start;

            UpdateAxis(chartX, startIndex, $"{currentPage} X");
            UpdateAxis(chartY, startIndex, $"{currentPage} Y");
            UpdateAxis(chartZ, startIndex, $"{currentPage} Z");
        }

        private void UpdateAxis(LiveCharts.WinForms.CartesianChart chart, int startIndex, string axisTitle)
        {
            chart.AxisX.Clear();
            chart.AxisY.Clear();
            chart.AxisX.Add(new Axis
            {
                Title = "Step Index",
                LabelFormatter = value => (value + startIndex).ToString()
            });
            chart.AxisY.Add(new Axis { Title = axisTitle });
        }

        private void AddChartSeries(
    LiveCharts.WinForms.CartesianChart chart,
    int pageOffset,
    VisualizationPage currentPage,
    string axisLabel,
    System.Windows.Media.Color baseColor,
    int index,
    string predictedDataSet)
        {
            var actualSeries = new LineSeries
            {
                Title = $"Actual {currentPage} {axisLabel}",
                Values = new ChartValues<double>(currentActualData.Select(d => d[pageOffset])),
                PointGeometry = null,
                Stroke = new System.Windows.Media.SolidColorBrush(baseColor),
                Fill = System.Windows.Media.Brushes.Transparent
            };

            var predictedSeries = new LineSeries
            {
                Title = $"{Path.GetFileName(predictedDataSet)} {axisLabel}",
                Values = new ChartValues<double>(currentPredictedDataSets[predictedDataSet].Select(d => d[pageOffset])),
                PointGeometry = null,
                Stroke = new System.Windows.Media.SolidColorBrush(CreateColorVariation(baseColor, index)),
                Fill = System.Windows.Media.Brushes.Transparent,
                StrokeDashArray = new System.Windows.Media.DoubleCollection { 2, 2 } // Dotted line
            };

            chart.Series.Add(actualSeries);
            chart.Series.Add(predictedSeries);

            if (showBestLineCheckBox.Checked)
            {
                var bestPredictionSeries = new LineSeries
                {
                    Title = $"Best {currentPage} {axisLabel}",
                    Values = new ChartValues<double>(GetBestPrediction(pageOffset, predictedDataSet)),
                    PointGeometry = null,
                    Stroke = System.Windows.Media.Brushes.Gray,
                    Fill = System.Windows.Media.Brushes.Transparent,
                    StrokeDashArray = new System.Windows.Media.DoubleCollection { 4, 2 } // More visible dashed line
                };

                chart.Series.Add(bestPredictionSeries);
            }
        }

        private System.Windows.Media.Color CreateColorVariation(System.Windows.Media.Color baseColor, int index)
        {
            // Create a variation of the base color by adjusting its opacity or brightness
            byte variationAmount = (byte)(50 + (index * 20) % 150);
            return System.Windows.Media.Color.FromArgb(variationAmount, baseColor.R, baseColor.G, baseColor.B);
        }

        private IEnumerable<double> GetBestPrediction(int componentIndex, string predictedDataSet)
        {
            var bestPrediction = new List<double>();
            int startLine = visualizationChunkRange.Start;
            int endLine = visualizationChunkRange.End;

            if (startLine < 0 || endLine <= startLine) return bestPrediction;

            for (int i = startLine; i < endLine; i++)
            {
                if (allPredictedDataSets.ContainsKey(predictedDataSet) &&
                    allPredictedDataSets[predictedDataSet].TryGetValue(i, out var predictions))
                {
                    if (predictions.Count == 0)
                    {
                        bestPrediction.Add(0); // Default to zero if no predictions are available
                        continue;
                    }

                    // Check if componentIndex is within bounds for all predictions
                    var validPredictions = predictions
                        .Where(p => componentIndex >= 0 && componentIndex < p.Length)
                        .Select(p => p[componentIndex])
                        .ToList();

                    if (validPredictions.Count == 0)
                    {
                        bestPrediction.Add(0); // Default to zero if no valid prediction
                    }
                    else
                    {
                        var average = validPredictions.Average();
                        bestPrediction.Add(average);
                    }
                }
                else
                {
                    bestPrediction.Add(0); // Default to zero if no data available for the index
                }
            }

            return bestPrediction;
        }


        private void UpdateInfoLabel()
        {
            var currentPage = (VisualizationPage)tabControl.SelectedIndex;
            int pageOffset = 1 + (int)currentPage * 3;

            // Calculate errors for X, Y, and Z components for each prediction dataset
            var infoText = $"Error Metrics for {currentPage}:\n";

            foreach (var predictedDataSet in currentPredictedDataSets.Keys)
            {
                double maeX = CalculateMeanAbsoluteError(pageOffset, predictedDataSet);
                double rmseX = CalculateRootMeanSquaredError(pageOffset, predictedDataSet);
                double maxErrorX = CalculateMaxError(pageOffset, predictedDataSet);

                double maeY = CalculateMeanAbsoluteError(pageOffset + 1, predictedDataSet);
                double rmseY = CalculateRootMeanSquaredError(pageOffset + 1, predictedDataSet);
                double maxErrorY = CalculateMaxError(pageOffset + 1, predictedDataSet);

                double maeZ = CalculateMeanAbsoluteError(pageOffset + 2, predictedDataSet);
                double rmseZ = CalculateRootMeanSquaredError(pageOffset + 2, predictedDataSet);
                double maxErrorZ = CalculateMaxError(pageOffset + 2, predictedDataSet);

                infoText += $"{Path.GetFileName(predictedDataSet)}: X: MAE: {maeX:F2}, RMSE: {rmseX:F2}, Max Error: {maxErrorX:F2} | " +
                            $"Y: MAE: {maeY:F2}, RMSE: {rmseY:F2}, Max Error: {maxErrorY:F2} | " +
                            $"Z: MAE: {maeZ:F2}, RMSE: {rmseZ:F2}, Max Error: {maxErrorZ:F2}\n";
            }

            infoLabel.Text = infoText;
        }

        ////////////////////////////////////// [ ERROR CALCULATIONS ] //////////////////////////////////////

        private double CalculateMeanAbsoluteError(int componentIndex, string predictedDataSet)
        {
            return currentActualData.Zip(currentPredictedDataSets[predictedDataSet], (actual, predicted) => Math.Abs(actual[componentIndex] - predicted[componentIndex]))
                                    .Average();
        }

        private double CalculateRootMeanSquaredError(int componentIndex, string predictedDataSet)
        {
            return Math.Sqrt(currentActualData.Zip(currentPredictedDataSets[predictedDataSet], (actual, predicted) => Math.Pow(actual[componentIndex] - predicted[componentIndex], 2))
                                              .Average());
        }

        private double CalculateMaxError(int componentIndex, string predictedDataSet)
        {
            return currentActualData.Zip(currentPredictedDataSets[predictedDataSet], (actual, predicted) => Math.Abs(actual[componentIndex] - predicted[componentIndex]))
                                    .Max();
        }

        //////////////////////////////////////// [ EVENT HANDLERS ] ////////////////////////////////////////

        private void NextIndexButton_Click(object sender, EventArgs e)
        {
            currentChunkStart++;
            currentIndexTextBox.Text = currentChunkStart.ToString();
            LoadAndDisplayChunkAsync();
        }

        private void PreviousIndexButton_Click(object sender, EventArgs e)
        {
            currentChunkStart = Math.Max(0, currentChunkStart - 1);
            currentIndexTextBox.Text = currentChunkStart.ToString();
            LoadAndDisplayChunkAsync();
        }

        private void CurrentIndexTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (int.TryParse(currentIndexTextBox.Text, out int newIndex))
                {
                    currentChunkStart = Math.Max(0, newIndex);
                    LoadAndDisplayChunkAsync();
                }
                else
                {
                    MessageBox.Show("Invalid index. Please enter a valid number.");
                }
            }
        }

        private void ShowBestLineCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            InitializeCharts();
        }

        private void SettingsButton_Click(object sender, EventArgs e)
        {
            using (var settingsForm = new SettingsForm(predictionChunkSize, visualizationChunkRange, actualDataFilePath, predictedDataFilePaths))
            {
                if (settingsForm.ShowDialog() == DialogResult.OK)
                {
                    // Update settings based on user input
                    predictionChunkSize = settingsForm.PredictionChunkSize;
                    actualDataFilePath = settingsForm.ActualDataFilePath;
                    predictedDataFilePaths = settingsForm.PredictedDataFilePaths;

                    // Reload data with new settings
                    LoadAllPredictionsAsync();
                    LoadAndDisplayChunkAsync();
                }
            }
        }

        private void TabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            InitializeCharts();
            UpdateInfoLabel();
        }

        private void VisualizationChunkStartTextBox_TextChanged(object sender, EventArgs e)
        {
            if (int.TryParse(visualizationChunkStartTextBox.Text, out int start))
            {
                if (start >= 0 && start < predictionChunkSize)
                {
                    visualizationChunkRange = (start, visualizationChunkRange.End);
                    LoadAndDisplayChunkAsync();
                }
                else
                {
                    MessageBox.Show("Start value must be between 0 and the prediction chunk size.");
                }
            }
        }

        private void VisualizationChunkEndTextBox_TextChanged(object sender, EventArgs e)
        {
            if (int.TryParse(visualizationChunkEndTextBox.Text, out int end))
            {
                if (end > visualizationChunkRange.Start && end <= predictionChunkSize)
                {
                    visualizationChunkRange = (visualizationChunkRange.Start, end);
                    LoadAndDisplayChunkAsync();
                }
                else
                {
                    MessageBox.Show("End value must be greater than start and less than or equal to the prediction chunk size.");
                }
            }
        }
    }
}
