namespace SimulatorRemoteControl
{
    partial class SimulatorRemote
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            buttonConnect = new Button();
            remoteConnectionStatusLabel = new Label();
            buttonZeroAxes = new Button();
            buttonGoHome = new Button();
            buttonStartStop = new Button();
            labelLastData = new Label();
            textBox1 = new TextBox();
            SuspendLayout();
            // 
            // buttonConnect
            // 
            buttonConnect.Location = new Point(164, 14);
            buttonConnect.Name = "buttonConnect";
            buttonConnect.Size = new Size(94, 29);
            buttonConnect.TabIndex = 0;
            buttonConnect.Text = "Connect";
            buttonConnect.UseVisualStyleBackColor = true;
            buttonConnect.Click += buttonConnect_Click;
            // 
            // remoteConnectionStatusLabel
            // 
            remoteConnectionStatusLabel.AutoSize = true;
            remoteConnectionStatusLabel.Location = new Point(264, 18);
            remoteConnectionStatusLabel.Name = "remoteConnectionStatusLabel";
            remoteConnectionStatusLabel.Size = new Size(109, 20);
            remoteConnectionStatusLabel.TabIndex = 1;
            remoteConnectionStatusLabel.Text = "Not Connected";
            // 
            // buttonZeroAxes
            // 
            buttonZeroAxes.Location = new Point(674, 12);
            buttonZeroAxes.Name = "buttonZeroAxes";
            buttonZeroAxes.Size = new Size(94, 29);
            buttonZeroAxes.TabIndex = 2;
            buttonZeroAxes.Text = "Zero Axes";
            buttonZeroAxes.UseVisualStyleBackColor = true;
            buttonZeroAxes.Click += buttonZeroAxes_Click;
            // 
            // buttonGoHome
            // 
            buttonGoHome.Font = new Font("Segoe UI", 36F);
            buttonGoHome.Location = new Point(29, 68);
            buttonGoHome.Name = "buttonGoHome";
            buttonGoHome.Size = new Size(353, 344);
            buttonGoHome.TabIndex = 3;
            buttonGoHome.Text = "Go Home";
            buttonGoHome.UseVisualStyleBackColor = true;
            buttonGoHome.Click += buttonGoHome_Click;
            // 
            // buttonStartStop
            // 
            buttonStartStop.Font = new Font("Segoe UI", 36F);
            buttonStartStop.Location = new Point(415, 68);
            buttonStartStop.Name = "buttonStartStop";
            buttonStartStop.Size = new Size(353, 344);
            buttonStartStop.TabIndex = 4;
            buttonStartStop.Text = "START";
            buttonStartStop.UseVisualStyleBackColor = true;
            buttonStartStop.Click += buttonStartStop_Click;
            // 
            // labelLastData
            // 
            labelLastData.AutoSize = true;
            labelLastData.Location = new Point(29, 441);
            labelLastData.Name = "labelLastData";
            labelLastData.Size = new Size(78, 20);
            labelLastData.TabIndex = 5;
            labelLastData.Text = "Last Data: ";
            // 
            // textBox1
            // 
            textBox1.Location = new Point(29, 14);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(129, 27);
            textBox1.TabIndex = 6;
            textBox1.Text = "127.0.0.1";
            textBox1.TextChanged += textBox1_TextChanged;
            // 
            // SimulatorRemote
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 480);
            Controls.Add(textBox1);
            Controls.Add(labelLastData);
            Controls.Add(buttonStartStop);
            Controls.Add(buttonGoHome);
            Controls.Add(buttonZeroAxes);
            Controls.Add(remoteConnectionStatusLabel);
            Controls.Add(buttonConnect);
            Name = "SimulatorRemote";
            Text = "Simulator Remote Control";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button buttonConnect;
        private Label remoteConnectionStatusLabel;
        private Button buttonZeroAxes;
        private Button buttonGoHome;
        private Button buttonStartStop;
        private Label labelLastData;
        private TextBox textBox1;
    }
}
