using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace BallDataVisualizer
{
    public partial class OffsetCalculationForm : Form
    {
        private Button selectFile1Button;
        private TextBox file1TextBox;
        private Button selectFile2Button;
        private TextBox file2TextBox;
        private Button selectPredictedFileButton;
        private TextBox predictedFileTextBox;
        private Button calculateOffsetButton;
        private Label offsetLabel;
        private Button applyOffsetButton;
        private NumericUpDown offsetNumericUpDown;

        private string filePath1;
        private string filePath2;
        private string predictedFilePath;

        public OffsetCalculationForm()
        {
            InitializeComponent();

            selectFile1Button = new Button();
            file1TextBox = new TextBox();
            selectFile2Button = new Button();
            file2TextBox = new TextBox();
            selectPredictedFileButton = new Button();
            predictedFileTextBox = new TextBox();
            calculateOffsetButton = new Button();
            offsetLabel = new Label();
            applyOffsetButton = new Button();
            offsetNumericUpDown = new NumericUpDown();

            SuspendLayout();
            // 
            // selectFile1Button
            // 
            selectFile1Button.Location = new System.Drawing.Point(12, 12);
            selectFile1Button.Name = "selectFile1Button";
            selectFile1Button.Size = new System.Drawing.Size(100, 23);
            selectFile1Button.TabIndex = 0;
            selectFile1Button.Text = "Select File 1";
            selectFile1Button.UseVisualStyleBackColor = true;
            selectFile1Button.Click += new System.EventHandler(selectFile1Button_Click);
            // 
            // file1TextBox
            // 
            file1TextBox.Location = new System.Drawing.Point(120, 12);
            file1TextBox.Name = "file1TextBox";
            file1TextBox.Size = new System.Drawing.Size(265, 20);
            file1TextBox.TabIndex = 1;
            // 
            // selectFile2Button
            // 
            selectFile2Button.Location = new System.Drawing.Point(12, 41);
            selectFile2Button.Name = "selectFile2Button";
            selectFile2Button.Size = new System.Drawing.Size(100, 23);
            selectFile2Button.TabIndex = 2;
            selectFile2Button.Text = "Select File 2";
            selectFile2Button.UseVisualStyleBackColor = true;
            selectFile2Button.Click += new System.EventHandler(selectFile2Button_Click);
            // 
            // file2TextBox
            // 
            file2TextBox.Location = new System.Drawing.Point(120, 41);
            file2TextBox.Name = "file2TextBox";
            file2TextBox.Size = new System.Drawing.Size(265, 20);
            file2TextBox.TabIndex = 3;
            // 
            // selectPredictedFileButton
            // 
            selectPredictedFileButton.Location = new System.Drawing.Point(12, 70);
            selectPredictedFileButton.Name = "selectPredictedFileButton";
            selectPredictedFileButton.Size = new System.Drawing.Size(100, 23);
            selectPredictedFileButton.TabIndex = 4;
            selectPredictedFileButton.Text = "Select Predicted 2";
            selectPredictedFileButton.UseVisualStyleBackColor = true;
            selectPredictedFileButton.Click += new System.EventHandler(selectPredictedFileButton_Click);
            // 
            // predictedFileTextBox
            // 
            predictedFileTextBox.Location = new System.Drawing.Point(120, 70);
            predictedFileTextBox.Name = "predictedFileTextBox";
            predictedFileTextBox.Size = new System.Drawing.Size(265, 20);
            predictedFileTextBox.TabIndex = 5;
            // 
            // calculateOffsetButton
            // 
            calculateOffsetButton.Location = new System.Drawing.Point(12, 100);
            calculateOffsetButton.Name = "calculateOffsetButton";
            calculateOffsetButton.Size = new System.Drawing.Size(100, 23);
            calculateOffsetButton.TabIndex = 6;
            calculateOffsetButton.Text = "Calculate Offset";
            calculateOffsetButton.UseVisualStyleBackColor = true;
            calculateOffsetButton.Click += new System.EventHandler(calculateOffsetButton_Click);
            // 
            // offsetLabel
            // 
            offsetLabel.AutoSize = true;
            offsetLabel.Location = new System.Drawing.Point(120, 105);
            offsetLabel.Name = "offsetLabel";
            offsetLabel.Size = new System.Drawing.Size(61, 13);
            offsetLabel.TabIndex = 7;
            offsetLabel.Text = "Offset: N/A";
            // 
            // offsetNumericUpDown
            // 
            offsetNumericUpDown.Location = new System.Drawing.Point(120, 130);
            offsetNumericUpDown.Maximum = new decimal(new int[] { 10000, 0, 0, 0 });
            offsetNumericUpDown.Minimum = new decimal(new int[] { 0, 0, 0, 0 });
            offsetNumericUpDown.Name = "offsetNumericUpDown";
            offsetNumericUpDown.Size = new System.Drawing.Size(120, 20);
            offsetNumericUpDown.TabIndex = 8;
            // 
            // applyOffsetButton
            // 
            applyOffsetButton.Location = new System.Drawing.Point(12, 160);
            applyOffsetButton.Name = "applyOffsetButton";
            applyOffsetButton.Size = new System.Drawing.Size(100, 23);
            applyOffsetButton.TabIndex = 9;
            applyOffsetButton.Text = "Apply Offset";
            applyOffsetButton.UseVisualStyleBackColor = true;
            applyOffsetButton.Click += new System.EventHandler(applyOffsetButton_Click);
            // 
            // OffsetCalculationForm
            // 
            ClientSize = new System.Drawing.Size(400, 200);
            Controls.Add(applyOffsetButton);
            Controls.Add(offsetNumericUpDown);
            Controls.Add(offsetLabel);
            Controls.Add(calculateOffsetButton);
            Controls.Add(predictedFileTextBox);
            Controls.Add(selectPredictedFileButton);
            Controls.Add(file2TextBox);
            Controls.Add(selectFile2Button);
            Controls.Add(file1TextBox);
            Controls.Add(selectFile1Button);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Name = "OffsetCalculationForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Offset Calculation and Adjustment";
            ResumeLayout(false);
            PerformLayout();
        }

        private void selectFile1Button_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "CSV files (*.csv)|*.csv";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    filePath1 = openFileDialog.FileName;
                    file1TextBox.Text = filePath1;
                }
            }
        }

        private void selectFile2Button_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "CSV files (*.csv)|*.csv";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    filePath2 = openFileDialog.FileName;
                    file2TextBox.Text = filePath2;
                }
            }
        }

        private void selectPredictedFileButton_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "CSV files (*.csv)|*.csv";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    predictedFilePath = openFileDialog.FileName;
                    predictedFileTextBox.Text = predictedFilePath;
                }
            }
        }

        private void calculateOffsetButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(filePath1) || string.IsNullOrEmpty(filePath2))
            {
                MessageBox.Show("Please select both actual data files.");
                return;
            }

            try
            {
                var offset = CalculateOptimalOffset(filePath1, filePath2);
                offsetLabel.Text = $"Offset: {offset} lines";
                offsetNumericUpDown.Value = offset;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error calculating offset: " + ex.Message);
            }
        }

        private void applyOffsetButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(predictedFilePath) || !File.Exists(predictedFilePath))
            {
                MessageBox.Show("Please select a valid predicted data file.");
                return;
            }

            int offset = (int)offsetNumericUpDown.Value;

            try
            {
                ApplyOffsetToFile(predictedFilePath, offset);
                MessageBox.Show("Offset has been applied successfully.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error applying offset: " + ex.Message);
            }
        }

        private List<(double[] Data, bool IsIgnorable)> ReadDataFromFile(string path)
        {
            var data = new List<(double[] Data, bool IsIgnorable)>();
            foreach (var line in File.ReadLines(path))
            {
                var values = line.Split(',')
                                 .Select(v => double.TryParse(v, out var result) ? result : double.NaN)
                                 .ToArray();

                if (!values.Any(double.IsNaN))
                {
                    bool isIgnorable = values.Count(v => v == 0) > values.Length / 2;  // More than 50% zeros
                    data.Add((values, isIgnorable));
                }
            }
            return data;
        }

        private int CalculateOptimalOffset(string path1, string path2)
        {
            var data1 = ReadDataFromFile(path1);
            var data2 = ReadDataFromFile(path2);

            int bestOffset = 0;
            double bestScore = double.MaxValue;
            int maxOffset = Math.Min(data1.Count, data2.Count) / 2;

            for (int offset = -maxOffset; offset <= maxOffset; offset++)
            {
                double score = CalculateSimilarityScore(data1, data2, offset);
                if (score < bestScore)
                {
                    bestScore = score;
                    bestOffset = offset;
                }
            }

            return bestOffset;
        }

        private double CalculateSimilarityScore(List<(double[] Data, bool IsIgnorable)> data1, List<(double[] Data, bool IsIgnorable)> data2, int offset)
        {
            int start = offset < 0 ? -offset : 0;
            int end = Math.Min(data1.Count, data2.Count - offset);
            double totalError = 0;
            int validComparisons = 0;

            for (int i = start; i < end; i++)
            {
                if (!data1[i].IsIgnorable && !data2[i + offset].IsIgnorable)
                {
                    double[] row1 = data1[i].Data;
                    double[] row2 = data2[i + offset].Data;
                    totalError += row1.Zip(row2, (x, y) => Math.Pow(x - y, 2)).Sum(); // Sum of squared differences
                    validComparisons++;
                }
            }

            return validComparisons > 0 ? totalError / validComparisons : double.MaxValue;  // Normalized total error
        }

        private void ApplyOffsetToFile(string filePath, int offset)
        {
            string directory = Path.GetDirectoryName(filePath);
            string filenameWithoutExtension = Path.GetFileNameWithoutExtension(filePath);
            string extension = Path.GetExtension(filePath);
            string newFilePath = Path.Combine(directory, $"{filenameWithoutExtension}_offset{offset}{extension}");

            using (StreamReader reader = new StreamReader(filePath))
            using (StreamWriter writer = new StreamWriter(newFilePath))
            {
                string line;
                int lineNumber = 0;

                // Write the header
                if ((line = reader.ReadLine()) != null)
                {
                    writer.WriteLine(line); // Write the header unchanged
                }

                // Skip lines until the offset
                while (lineNumber < offset && (line = reader.ReadLine()) != null)
                {
                    lineNumber++;
                }

                // Process remaining lines
                while ((line = reader.ReadLine()) != null)
                {
                    var values = line.Split(',');
                    if (values.Length > 0 && int.TryParse(values[0], out int index))
                    {
                        index -= offset;  // Adjust the index
                        values[0] = index.ToString();
                        writer.WriteLine(string.Join(",", values));
                    }
                }
            }
        }
    }
}
