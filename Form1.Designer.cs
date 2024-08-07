using System.Drawing;
using System.Windows.Forms;

namespace BallDataVisualizer
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
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
        private Button openOffsetCalculationFormButton;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(964, 714);
            this.Name = "Form1";
            this.Text = "Ball Prediction Analyzer | by yoyo | suprahd on discord";
            this.ResumeLayout(false);

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

            this.openOffsetCalculationFormButton = new System.Windows.Forms.Button();
            // 
            // openOffsetCalculationFormButton
            // 
            this.openOffsetCalculationFormButton.Text = "Open Offset Calculation";
            this.openOffsetCalculationFormButton.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.openOffsetCalculationFormButton.Height = 30;
            this.openOffsetCalculationFormButton.Click += new System.EventHandler(this.OpenOffsetCalculationFormButton_Click);
            // 
            // Adding the button to the form
            this.Controls.Add(this.openOffsetCalculationFormButton);
        }

        #endregion
    }
}

