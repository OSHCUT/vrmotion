namespace SimController
{
    partial class MainView
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
            estopButton = new Button();
            simEnableDisableButton = new Button();
            telemetryRawPacketLabel = new Label();
            groupBox1 = new GroupBox();
            label4 = new Label();
            label3 = new Label();
            label2 = new Label();
            label5 = new Label();
            label6 = new Label();
            label7 = new Label();
            label8 = new Label();
            label9 = new Label();
            label10 = new Label();
            telemetrySwayLabel = new Label();
            telemetrySurgeLabel = new Label();
            telemetryHeaveLabel = new Label();
            telemetryRollRateLabel = new Label();
            telemetryPitchRateLabel = new Label();
            telemetryYawRateLabel = new Label();
            telemetryRollLabel = new Label();
            telemetryPitchLabel = new Label();
            telemetryYaw = new Label();
            enableTelemetryLinkButton = new Button();
            groupBox2 = new GroupBox();
            simStartStopHomingButton = new Button();
            simGoToZeroButton = new Button();
            simRollLabel = new Label();
            simPitchLabel = new Label();
            simYawLabel = new Label();
            label13 = new Label();
            label14 = new Label();
            label15 = new Label();
            simYawZeroButton = new Button();
            groupBox1.SuspendLayout();
            groupBox2.SuspendLayout();
            SuspendLayout();
            // 
            // estopButton
            // 
            estopButton.BackColor = Color.Red;
            estopButton.ForeColor = Color.White;
            estopButton.Location = new Point(549, 33);
            estopButton.Margin = new Padding(4);
            estopButton.Name = "estopButton";
            estopButton.Size = new Size(189, 158);
            estopButton.TabIndex = 0;
            estopButton.Text = "STOP";
            estopButton.UseVisualStyleBackColor = false;
            estopButton.Click += estopButton_Click;
            // 
            // simEnableDisableButton
            // 
            simEnableDisableButton.Location = new Point(7, 29);
            simEnableDisableButton.Margin = new Padding(4);
            simEnableDisableButton.Name = "simEnableDisableButton";
            simEnableDisableButton.Size = new Size(95, 36);
            simEnableDisableButton.TabIndex = 1;
            simEnableDisableButton.Text = "Enable";
            simEnableDisableButton.UseVisualStyleBackColor = true;
            // 
            // telemetryRawPacketLabel
            // 
            telemetryRawPacketLabel.AutoSize = true;
            telemetryRawPacketLabel.Location = new Point(9, 141);
            telemetryRawPacketLabel.Margin = new Padding(4, 0, 4, 0);
            telemetryRawPacketLabel.Name = "telemetryRawPacketLabel";
            telemetryRawPacketLabel.Size = new Size(125, 21);
            telemetryRawPacketLabel.TabIndex = 4;
            telemetryRawPacketLabel.Text = "Telemetry Packet";
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(enableTelemetryLinkButton);
            groupBox1.Controls.Add(telemetrySwayLabel);
            groupBox1.Controls.Add(telemetryRawPacketLabel);
            groupBox1.Controls.Add(telemetrySurgeLabel);
            groupBox1.Controls.Add(telemetryHeaveLabel);
            groupBox1.Controls.Add(telemetryRollRateLabel);
            groupBox1.Controls.Add(telemetryPitchRateLabel);
            groupBox1.Controls.Add(telemetryYawRateLabel);
            groupBox1.Controls.Add(telemetryRollLabel);
            groupBox1.Controls.Add(telemetryPitchLabel);
            groupBox1.Controls.Add(telemetryYaw);
            groupBox1.Controls.Add(label8);
            groupBox1.Controls.Add(label9);
            groupBox1.Controls.Add(label10);
            groupBox1.Controls.Add(label5);
            groupBox1.Controls.Add(label6);
            groupBox1.Controls.Add(label7);
            groupBox1.Controls.Add(label4);
            groupBox1.Controls.Add(label3);
            groupBox1.Controls.Add(label2);
            groupBox1.Location = new Point(17, 228);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(513, 176);
            groupBox1.TabIndex = 5;
            groupBox1.TabStop = false;
            groupBox1.Text = "Telemetry";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(7, 100);
            label4.Name = "label4";
            label4.Size = new Size(37, 21);
            label4.TabIndex = 2;
            label4.Text = "Roll";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(7, 70);
            label3.Name = "label3";
            label3.Size = new Size(44, 21);
            label3.TabIndex = 1;
            label3.Text = "Pitch";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(6, 40);
            label2.Name = "label2";
            label2.Size = new Size(38, 21);
            label2.TabIndex = 0;
            label2.Text = "Yaw";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(109, 100);
            label5.Name = "label5";
            label5.Size = new Size(72, 21);
            label5.TabIndex = 5;
            label5.Text = "Roll Rate";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(109, 70);
            label6.Name = "label6";
            label6.Size = new Size(79, 21);
            label6.TabIndex = 4;
            label6.Text = "Pitch Rate";
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new Point(108, 40);
            label7.Name = "label7";
            label7.Size = new Size(73, 21);
            label7.TabIndex = 3;
            label7.Text = "Yaw Rate";
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Location = new Point(256, 100);
            label8.Name = "label8";
            label8.Size = new Size(47, 21);
            label8.TabIndex = 8;
            label8.Text = "Sway";
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Location = new Point(256, 70);
            label9.Name = "label9";
            label9.Size = new Size(51, 21);
            label9.TabIndex = 7;
            label9.Text = "Surge";
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.Location = new Point(255, 40);
            label10.Name = "label10";
            label10.Size = new Size(53, 21);
            label10.TabIndex = 6;
            label10.Text = "Heave";
            // 
            // telemetrySwayLabel
            // 
            telemetrySwayLabel.AutoSize = true;
            telemetrySwayLabel.Location = new Point(322, 100);
            telemetrySwayLabel.Name = "telemetrySwayLabel";
            telemetrySwayLabel.Size = new Size(19, 21);
            telemetrySwayLabel.TabIndex = 17;
            telemetrySwayLabel.Text = "...";
            // 
            // telemetrySurgeLabel
            // 
            telemetrySurgeLabel.AutoSize = true;
            telemetrySurgeLabel.Location = new Point(322, 70);
            telemetrySurgeLabel.Name = "telemetrySurgeLabel";
            telemetrySurgeLabel.Size = new Size(19, 21);
            telemetrySurgeLabel.TabIndex = 16;
            telemetrySurgeLabel.Text = "...";
            // 
            // telemetryHeaveLabel
            // 
            telemetryHeaveLabel.AutoSize = true;
            telemetryHeaveLabel.Location = new Point(321, 40);
            telemetryHeaveLabel.Name = "telemetryHeaveLabel";
            telemetryHeaveLabel.Size = new Size(19, 21);
            telemetryHeaveLabel.TabIndex = 15;
            telemetryHeaveLabel.Text = "...";
            // 
            // telemetryRollRateLabel
            // 
            telemetryRollRateLabel.AutoSize = true;
            telemetryRollRateLabel.Location = new Point(194, 100);
            telemetryRollRateLabel.Name = "telemetryRollRateLabel";
            telemetryRollRateLabel.Size = new Size(19, 21);
            telemetryRollRateLabel.TabIndex = 14;
            telemetryRollRateLabel.Text = "...";
            // 
            // telemetryPitchRateLabel
            // 
            telemetryPitchRateLabel.AutoSize = true;
            telemetryPitchRateLabel.Location = new Point(194, 70);
            telemetryPitchRateLabel.Name = "telemetryPitchRateLabel";
            telemetryPitchRateLabel.Size = new Size(19, 21);
            telemetryPitchRateLabel.TabIndex = 13;
            telemetryPitchRateLabel.Text = "...";
            // 
            // telemetryYawRateLabel
            // 
            telemetryYawRateLabel.AutoSize = true;
            telemetryYawRateLabel.Location = new Point(193, 40);
            telemetryYawRateLabel.Name = "telemetryYawRateLabel";
            telemetryYawRateLabel.Size = new Size(19, 21);
            telemetryYawRateLabel.TabIndex = 12;
            telemetryYawRateLabel.Text = "...";
            // 
            // telemetryRollLabel
            // 
            telemetryRollLabel.AutoSize = true;
            telemetryRollLabel.Location = new Point(57, 100);
            telemetryRollLabel.Name = "telemetryRollLabel";
            telemetryRollLabel.Size = new Size(19, 21);
            telemetryRollLabel.TabIndex = 11;
            telemetryRollLabel.Text = "...";
            // 
            // telemetryPitchLabel
            // 
            telemetryPitchLabel.AutoSize = true;
            telemetryPitchLabel.Location = new Point(57, 70);
            telemetryPitchLabel.Name = "telemetryPitchLabel";
            telemetryPitchLabel.Size = new Size(19, 21);
            telemetryPitchLabel.TabIndex = 10;
            telemetryPitchLabel.Text = "...";
            // 
            // telemetryYaw
            // 
            telemetryYaw.AutoSize = true;
            telemetryYaw.Location = new Point(56, 40);
            telemetryYaw.Name = "telemetryYaw";
            telemetryYaw.Size = new Size(19, 21);
            telemetryYaw.TabIndex = 9;
            telemetryYaw.Text = "...";
            // 
            // enableTelemetryLinkButton
            // 
            enableTelemetryLinkButton.Location = new Point(392, 37);
            enableTelemetryLinkButton.Margin = new Padding(4);
            enableTelemetryLinkButton.Name = "enableTelemetryLinkButton";
            enableTelemetryLinkButton.Size = new Size(96, 87);
            enableTelemetryLinkButton.TabIndex = 6;
            enableTelemetryLinkButton.Text = "Enable Motion";
            enableTelemetryLinkButton.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(simYawZeroButton);
            groupBox2.Controls.Add(simRollLabel);
            groupBox2.Controls.Add(simPitchLabel);
            groupBox2.Controls.Add(simYawLabel);
            groupBox2.Controls.Add(label13);
            groupBox2.Controls.Add(label14);
            groupBox2.Controls.Add(label15);
            groupBox2.Controls.Add(simGoToZeroButton);
            groupBox2.Controls.Add(simStartStopHomingButton);
            groupBox2.Controls.Add(simEnableDisableButton);
            groupBox2.Location = new Point(17, 16);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(513, 190);
            groupBox2.TabIndex = 6;
            groupBox2.TabStop = false;
            groupBox2.Text = "Simulator";
            // 
            // simStartStopHomingButton
            // 
            simStartStopHomingButton.Location = new Point(344, 29);
            simStartStopHomingButton.Margin = new Padding(4);
            simStartStopHomingButton.Name = "simStartStopHomingButton";
            simStartStopHomingButton.Size = new Size(144, 36);
            simStartStopHomingButton.TabIndex = 2;
            simStartStopHomingButton.Text = "Start Homing";
            simStartStopHomingButton.UseVisualStyleBackColor = true;
            // 
            // simGoToZeroButton
            // 
            simGoToZeroButton.Location = new Point(117, 29);
            simGoToZeroButton.Margin = new Padding(4);
            simGoToZeroButton.Name = "simGoToZeroButton";
            simGoToZeroButton.Size = new Size(95, 36);
            simGoToZeroButton.TabIndex = 3;
            simGoToZeroButton.Text = "Go To Zero";
            simGoToZeroButton.UseVisualStyleBackColor = true;
            // 
            // simRollLabel
            // 
            simRollLabel.AutoSize = true;
            simRollLabel.Location = new Point(58, 146);
            simRollLabel.Name = "simRollLabel";
            simRollLabel.Size = new Size(19, 21);
            simRollLabel.TabIndex = 17;
            simRollLabel.Text = "...";
            // 
            // simPitchLabel
            // 
            simPitchLabel.AutoSize = true;
            simPitchLabel.Location = new Point(58, 116);
            simPitchLabel.Name = "simPitchLabel";
            simPitchLabel.Size = new Size(19, 21);
            simPitchLabel.TabIndex = 16;
            simPitchLabel.Text = "...";
            // 
            // simYawLabel
            // 
            simYawLabel.AutoSize = true;
            simYawLabel.Location = new Point(57, 86);
            simYawLabel.Name = "simYawLabel";
            simYawLabel.Size = new Size(19, 21);
            simYawLabel.TabIndex = 15;
            simYawLabel.Text = "...";
            // 
            // label13
            // 
            label13.AutoSize = true;
            label13.Location = new Point(8, 146);
            label13.Name = "label13";
            label13.Size = new Size(37, 21);
            label13.TabIndex = 14;
            label13.Text = "Roll";
            // 
            // label14
            // 
            label14.AutoSize = true;
            label14.Location = new Point(8, 116);
            label14.Name = "label14";
            label14.Size = new Size(44, 21);
            label14.TabIndex = 13;
            label14.Text = "Pitch";
            // 
            // label15
            // 
            label15.AutoSize = true;
            label15.Location = new Point(7, 86);
            label15.Name = "label15";
            label15.Size = new Size(38, 21);
            label15.TabIndex = 12;
            label15.Text = "Yaw";
            // 
            // simYawZeroButton
            // 
            simYawZeroButton.Location = new Point(118, 78);
            simYawZeroButton.Margin = new Padding(4);
            simYawZeroButton.Name = "simYawZeroButton";
            simYawZeroButton.Size = new Size(95, 36);
            simYawZeroButton.TabIndex = 18;
            simYawZeroButton.Text = "Go To Zero";
            simYawZeroButton.UseVisualStyleBackColor = true;
            // 
            // MainView
            // 
            AutoScaleDimensions = new SizeF(9F, 21F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(758, 429);
            Controls.Add(groupBox2);
            Controls.Add(estopButton);
            Controls.Add(groupBox1);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Margin = new Padding(4);
            Name = "MainView";
            Text = "Simulator Link";
            Load += MainView_Load;
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Button estopButton;
        private Button simEnableDisableButton;
        private Label telemetryRawPacketLabel;
        private GroupBox groupBox1;
        private Label label4;
        private Label label3;
        private Label label2;
        private Label telemetrySwayLabel;
        private Label telemetrySurgeLabel;
        private Label telemetryHeaveLabel;
        private Label telemetryRollRateLabel;
        private Label telemetryPitchRateLabel;
        private Label telemetryYawRateLabel;
        private Label telemetryRollLabel;
        private Label telemetryPitchLabel;
        private Label telemetryYaw;
        private Label label8;
        private Label label9;
        private Label label10;
        private Label label5;
        private Label label6;
        private Label label7;
        private Button enableTelemetryLinkButton;
        private GroupBox groupBox2;
        private Button simGoToZeroButton;
        private Button simStartStopHomingButton;
        private Label simRollLabel;
        private Label simPitchLabel;
        private Label simYawLabel;
        private Label label13;
        private Label label14;
        private Label label15;
        private Button simYawZeroButton;
    }
}
