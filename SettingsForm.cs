using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace BallDataVisualizer
{
    public partial class SettingsForm : Form
    {
        private TextBox predictionChunkSizeTextBox;
        private TextBox actualDataPathTextBox;
        private ListBox predictedDataPathsListBox;
        private Button browseActualFileButton;
        private Button addPredictedFileButton;
        private Button removePredictedFileButton;
        private Button okButton;
        private Button cancelButton;

        public int PredictionChunkSize { get; private set; }
        public (int Start, int End) VisualizationChunkRange { get; private set; }
        public string ActualDataFilePath { get; private set; }
        public List<string> PredictedDataFilePaths { get; private set; } = new List<string>();

        public SettingsForm(int currentPredictionChunkSize, string currentActualPath, List<string> currentPredictedPaths)
        {
            // Initialize UI components
            InitializeComponent();
            InitializeUI();

            // Set initial values for text boxes
            predictionChunkSizeTextBox.Text = currentPredictionChunkSize.ToString();
            actualDataPathTextBox.Text = currentActualPath;
            PredictedDataFilePaths = currentPredictedPaths;
            UpdatePredictedDataPathsListBox();
        }

        private void InitializeUI()
        {
            // Initialize components
            predictionChunkSizeTextBox = new TextBox { Dock = DockStyle.Fill };
            actualDataPathTextBox = new TextBox { Dock = DockStyle.Fill, ReadOnly = true };
            predictedDataPathsListBox = new ListBox { Dock = DockStyle.Fill, Height = 100 };
            browseActualFileButton = new Button { Text = "Browse...", Dock = DockStyle.Fill };
            addPredictedFileButton = new Button { Text = "Add File", Dock = DockStyle.Fill };
            removePredictedFileButton = new Button { Text = "Remove File", Dock = DockStyle.Fill };
            okButton = new Button { Text = "OK", Dock = DockStyle.Fill };
            cancelButton = new Button { Text = "Cancel", Dock = DockStyle.Fill };

            // Labels
            var predictionChunkSizeLabel = new Label { Text = "Prediction Chunk Size:", Dock = DockStyle.Fill };
            var actualPathLabel = new Label { Text = "Actual Data Path:", Dock = DockStyle.Fill };
            var predictedPathLabel = new Label { Text = "Predicted Data Paths:", Dock = DockStyle.Fill };

            // Event handlers
            browseActualFileButton.Click += BrowseActualFileButton_Click;
            addPredictedFileButton.Click += AddPredictedFileButton_Click;
            removePredictedFileButton.Click += RemovePredictedFileButton_Click;
            okButton.Click += OkButton_Click;
            cancelButton.Click += CancelButton_Click;

            // Layout setup
            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 3,
                RowCount = 8,
                AutoSize = true,
                Padding = new Padding(10)
            };

            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20));

            layout.Controls.Add(predictionChunkSizeLabel, 0, 0);
            layout.Controls.Add(predictionChunkSizeTextBox, 1, 0);
            layout.Controls.Add(new Label { Text = "", Dock = DockStyle.Fill }, 2, 0); // Spacer

            layout.Controls.Add(actualPathLabel, 0, 4);
            layout.Controls.Add(actualDataPathTextBox, 1, 4);
            layout.Controls.Add(browseActualFileButton, 2, 4);

            layout.Controls.Add(predictedPathLabel, 0, 5);
            layout.Controls.Add(predictedDataPathsListBox, 1, 5);
            layout.Controls.Add(addPredictedFileButton, 0, 6);
            layout.Controls.Add(removePredictedFileButton, 1, 6);

            layout.Controls.Add(okButton, 0, 7);
            layout.Controls.Add(cancelButton, 1, 7);

            Controls.Add(layout);
            Text = "Settings";
        }

        private void BrowseActualFileButton_Click(object sender, EventArgs e)
        {
            using (var openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    actualDataPathTextBox.Text = openFileDialog.FileName;
                }
            }
        }

        private void AddPredictedFileButton_Click(object sender, EventArgs e)
        {
            using (var openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*";
                openFileDialog.Multiselect = true;
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    foreach (var fileName in openFileDialog.FileNames)
                    {
                        if (!PredictedDataFilePaths.Contains(fileName))
                        {
                            PredictedDataFilePaths.Add(fileName);
                        }
                    }
                    UpdatePredictedDataPathsListBox();
                }
            }
        }

        private void RemovePredictedFileButton_Click(object sender, EventArgs e)
        {
            var selectedItems = predictedDataPathsListBox.SelectedItems.OfType<string>().ToList();
            foreach (var item in selectedItems)
            {
                PredictedDataFilePaths.Remove(item);
            }
            UpdatePredictedDataPathsListBox();
        }

        private void UpdatePredictedDataPathsListBox()
        {
            predictedDataPathsListBox.Items.Clear();
            predictedDataPathsListBox.Items.AddRange(PredictedDataFilePaths.ToArray());
        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            if (int.TryParse(predictionChunkSizeTextBox.Text, out int newPredictionChunkSize) && newPredictionChunkSize > 0)
            {
                PredictionChunkSize = newPredictionChunkSize;
                ActualDataFilePath = actualDataPathTextBox.Text;
                DialogResult = DialogResult.OK;
                Close();
            }
            else
            {
                MessageBox.Show("Please enter valid chunk range values and sizes.");
            }
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
