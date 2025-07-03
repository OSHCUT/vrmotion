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
            telemetryZeroYawButton = new Button();
            enableTelemetryLinkButton = new Button();
            telemetrySwayLabel = new Label();
            telemetrySurgeLabel = new Label();
            telemetryHeaveLabel = new Label();
            telemetryRollRateLabel = new Label();
            telemetryPitchRateLabel = new Label();
            telemetryYawRateLabel = new Label();
            telemetryRollLabel = new Label();
            telemetryPitchLabel = new Label();
            telemetryYawLabel = new Label();
            label8 = new Label();
            label9 = new Label();
            label10 = new Label();
            label5 = new Label();
            label6 = new Label();
            label7 = new Label();
            label4 = new Label();
            label3 = new Label();
            label2 = new Label();
            groupBox2 = new GroupBox();
            rollCmdLabel = new Label();
            pitchCmdLabel = new Label();
            yawRateCmdLabel = new Label();
            label1 = new Label();
            simHubStatusLabel = new Label();
            simYawZeroButton = new Button();
            simRollLabel = new Label();
            simPitchLabel = new Label();
            simYawLabel = new Label();
            label13 = new Label();
            label14 = new Label();
            label15 = new Label();
            simGoToZeroButton = new Button();
            simStartStopHomingButton = new Button();
            testMoveButton = new Button();
            groupBox1.SuspendLayout();
            groupBox2.SuspendLayout();
            SuspendLayout();
            // 
            // estopButton
            // 
            estopButton.BackColor = Color.Red;
            estopButton.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            estopButton.ForeColor = Color.White;
            estopButton.Location = new Point(696, 73);
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
            simEnableDisableButton.Enabled = false;
            simEnableDisableButton.Location = new Point(9, 73);
            simEnableDisableButton.Margin = new Padding(4);
            simEnableDisableButton.Name = "simEnableDisableButton";
            simEnableDisableButton.Size = new Size(95, 36);
            simEnableDisableButton.TabIndex = 1;
            simEnableDisableButton.Text = "Enable";
            simEnableDisableButton.UseVisualStyleBackColor = true;
            simEnableDisableButton.Click += simEnableDisableButton_Click;
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
            groupBox1.Controls.Add(telemetryZeroYawButton);
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
            groupBox1.Controls.Add(telemetryYawLabel);
            groupBox1.Controls.Add(label8);
            groupBox1.Controls.Add(label9);
            groupBox1.Controls.Add(label10);
            groupBox1.Controls.Add(label5);
            groupBox1.Controls.Add(label6);
            groupBox1.Controls.Add(label7);
            groupBox1.Controls.Add(label4);
            groupBox1.Controls.Add(label3);
            groupBox1.Controls.Add(label2);
            groupBox1.Location = new Point(17, 361);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(721, 176);
            groupBox1.TabIndex = 5;
            groupBox1.TabStop = false;
            groupBox1.Text = "Telemetry";
            // 
            // telemetryZeroYawButton
            // 
            telemetryZeroYawButton.Location = new Point(428, 32);
            telemetryZeroYawButton.Margin = new Padding(4);
            telemetryZeroYawButton.Name = "telemetryZeroYawButton";
            telemetryZeroYawButton.Size = new Size(161, 36);
            telemetryZeroYawButton.TabIndex = 19;
            telemetryZeroYawButton.Text = "Zero Telemetry Yaw";
            telemetryZeroYawButton.UseVisualStyleBackColor = true;
            telemetryZeroYawButton.Click += telemetryZeroYawButton_Click;
            // 
            // enableTelemetryLinkButton
            // 
            enableTelemetryLinkButton.Enabled = false;
            enableTelemetryLinkButton.Location = new Point(609, 29);
            enableTelemetryLinkButton.Margin = new Padding(4);
            enableTelemetryLinkButton.Name = "enableTelemetryLinkButton";
            enableTelemetryLinkButton.Size = new Size(96, 87);
            enableTelemetryLinkButton.TabIndex = 6;
            enableTelemetryLinkButton.Text = "Enable Motion";
            enableTelemetryLinkButton.UseVisualStyleBackColor = true;
            enableTelemetryLinkButton.Click += enableTelemetryLinkButton_Click;
            // 
            // telemetrySwayLabel
            // 
            telemetrySwayLabel.AutoSize = true;
            telemetrySwayLabel.Location = new Point(356, 100);
            telemetrySwayLabel.Name = "telemetrySwayLabel";
            telemetrySwayLabel.Size = new Size(19, 21);
            telemetrySwayLabel.TabIndex = 17;
            telemetrySwayLabel.Text = "...";
            // 
            // telemetrySurgeLabel
            // 
            telemetrySurgeLabel.AutoSize = true;
            telemetrySurgeLabel.Location = new Point(356, 70);
            telemetrySurgeLabel.Name = "telemetrySurgeLabel";
            telemetrySurgeLabel.Size = new Size(19, 21);
            telemetrySurgeLabel.TabIndex = 16;
            telemetrySurgeLabel.Text = "...";
            // 
            // telemetryHeaveLabel
            // 
            telemetryHeaveLabel.AutoSize = true;
            telemetryHeaveLabel.Location = new Point(355, 40);
            telemetryHeaveLabel.Name = "telemetryHeaveLabel";
            telemetryHeaveLabel.Size = new Size(19, 21);
            telemetryHeaveLabel.TabIndex = 15;
            telemetryHeaveLabel.Text = "...";
            // 
            // telemetryRollRateLabel
            // 
            telemetryRollRateLabel.AutoSize = true;
            telemetryRollRateLabel.Location = new Point(211, 100);
            telemetryRollRateLabel.Name = "telemetryRollRateLabel";
            telemetryRollRateLabel.Size = new Size(19, 21);
            telemetryRollRateLabel.TabIndex = 14;
            telemetryRollRateLabel.Text = "...";
            // 
            // telemetryPitchRateLabel
            // 
            telemetryPitchRateLabel.AutoSize = true;
            telemetryPitchRateLabel.Location = new Point(211, 70);
            telemetryPitchRateLabel.Name = "telemetryPitchRateLabel";
            telemetryPitchRateLabel.Size = new Size(19, 21);
            telemetryPitchRateLabel.TabIndex = 13;
            telemetryPitchRateLabel.Text = "...";
            // 
            // telemetryYawRateLabel
            // 
            telemetryYawRateLabel.AutoSize = true;
            telemetryYawRateLabel.Location = new Point(210, 40);
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
            // telemetryYawLabel
            // 
            telemetryYawLabel.AutoSize = true;
            telemetryYawLabel.Location = new Point(56, 40);
            telemetryYawLabel.Name = "telemetryYawLabel";
            telemetryYawLabel.Size = new Size(19, 21);
            telemetryYawLabel.TabIndex = 9;
            telemetryYawLabel.Text = "...";
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Location = new Point(290, 100);
            label8.Name = "label8";
            label8.Size = new Size(47, 21);
            label8.TabIndex = 8;
            label8.Text = "Sway";
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Location = new Point(290, 70);
            label9.Name = "label9";
            label9.Size = new Size(51, 21);
            label9.TabIndex = 7;
            label9.Text = "Surge";
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.Location = new Point(289, 40);
            label10.Name = "label10";
            label10.Size = new Size(53, 21);
            label10.TabIndex = 6;
            label10.Text = "Heave";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(126, 100);
            label5.Name = "label5";
            label5.Size = new Size(72, 21);
            label5.TabIndex = 5;
            label5.Text = "Roll Rate";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(126, 70);
            label6.Name = "label6";
            label6.Size = new Size(79, 21);
            label6.TabIndex = 4;
            label6.Text = "Pitch Rate";
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new Point(125, 40);
            label7.Name = "label7";
            label7.Size = new Size(73, 21);
            label7.TabIndex = 3;
            label7.Text = "Yaw Rate";
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
            // groupBox2
            // 
            groupBox2.Controls.Add(testMoveButton);
            groupBox2.Controls.Add(rollCmdLabel);
            groupBox2.Controls.Add(pitchCmdLabel);
            groupBox2.Controls.Add(yawRateCmdLabel);
            groupBox2.Controls.Add(label1);
            groupBox2.Controls.Add(simHubStatusLabel);
            groupBox2.Controls.Add(simYawZeroButton);
            groupBox2.Controls.Add(estopButton);
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
            groupBox2.Size = new Size(904, 249);
            groupBox2.TabIndex = 6;
            groupBox2.TabStop = false;
            groupBox2.Text = "Simulator";
            // 
            // rollCmdLabel
            // 
            rollCmdLabel.AutoSize = true;
            rollCmdLabel.Location = new Point(145, 207);
            rollCmdLabel.Name = "rollCmdLabel";
            rollCmdLabel.Size = new Size(19, 21);
            rollCmdLabel.TabIndex = 23;
            rollCmdLabel.Text = "...";
            // 
            // pitchCmdLabel
            // 
            pitchCmdLabel.AutoSize = true;
            pitchCmdLabel.Location = new Point(145, 177);
            pitchCmdLabel.Name = "pitchCmdLabel";
            pitchCmdLabel.Size = new Size(19, 21);
            pitchCmdLabel.TabIndex = 22;
            pitchCmdLabel.Text = "...";
            // 
            // yawRateCmdLabel
            // 
            yawRateCmdLabel.AutoSize = true;
            yawRateCmdLabel.Location = new Point(145, 147);
            yawRateCmdLabel.Name = "yawRateCmdLabel";
            yawRateCmdLabel.Size = new Size(19, 21);
            yawRateCmdLabel.TabIndex = 21;
            yawRateCmdLabel.Text = "...";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(145, 129);
            label1.Name = "label1";
            label1.Size = new Size(45, 21);
            label1.TabIndex = 20;
            label1.Text = "CMD";
            // 
            // simHubStatusLabel
            // 
            simHubStatusLabel.AutoSize = true;
            simHubStatusLabel.Location = new Point(10, 35);
            simHubStatusLabel.Name = "simHubStatusLabel";
            simHubStatusLabel.Size = new Size(185, 21);
            simHubStatusLabel.TabIndex = 19;
            simHubStatusLabel.Text = "Status: NOT CONNECTED";
            // 
            // simYawZeroButton
            // 
            simYawZeroButton.Enabled = false;
            simYawZeroButton.Location = new Point(291, 122);
            simYawZeroButton.Margin = new Padding(4);
            simYawZeroButton.Name = "simYawZeroButton";
            simYawZeroButton.Size = new Size(118, 36);
            simYawZeroButton.TabIndex = 18;
            simYawZeroButton.Text = "Set Yaw Home";
            simYawZeroButton.UseVisualStyleBackColor = true;
            simYawZeroButton.Click += simYawZeroButton_Click;
            // 
            // simRollLabel
            // 
            simRollLabel.AutoSize = true;
            simRollLabel.Location = new Point(60, 207);
            simRollLabel.Name = "simRollLabel";
            simRollLabel.Size = new Size(19, 21);
            simRollLabel.TabIndex = 17;
            simRollLabel.Text = "...";
            // 
            // simPitchLabel
            // 
            simPitchLabel.AutoSize = true;
            simPitchLabel.Location = new Point(60, 177);
            simPitchLabel.Name = "simPitchLabel";
            simPitchLabel.Size = new Size(19, 21);
            simPitchLabel.TabIndex = 16;
            simPitchLabel.Text = "...";
            // 
            // simYawLabel
            // 
            simYawLabel.AutoSize = true;
            simYawLabel.Location = new Point(59, 147);
            simYawLabel.Name = "simYawLabel";
            simYawLabel.Size = new Size(19, 21);
            simYawLabel.TabIndex = 15;
            simYawLabel.Text = "...";
            // 
            // label13
            // 
            label13.AutoSize = true;
            label13.Location = new Point(10, 207);
            label13.Name = "label13";
            label13.Size = new Size(37, 21);
            label13.TabIndex = 14;
            label13.Text = "Roll";
            // 
            // label14
            // 
            label14.AutoSize = true;
            label14.Location = new Point(10, 177);
            label14.Name = "label14";
            label14.Size = new Size(44, 21);
            label14.TabIndex = 13;
            label14.Text = "Pitch";
            // 
            // label15
            // 
            label15.AutoSize = true;
            label15.Location = new Point(9, 147);
            label15.Name = "label15";
            label15.Size = new Size(38, 21);
            label15.TabIndex = 12;
            label15.Text = "Yaw";
            // 
            // simGoToZeroButton
            // 
            simGoToZeroButton.Enabled = false;
            simGoToZeroButton.Location = new Point(290, 73);
            simGoToZeroButton.Margin = new Padding(4);
            simGoToZeroButton.Name = "simGoToZeroButton";
            simGoToZeroButton.Size = new Size(119, 36);
            simGoToZeroButton.TabIndex = 3;
            simGoToZeroButton.Text = "Go To Zero";
            simGoToZeroButton.UseVisualStyleBackColor = true;
            simGoToZeroButton.Click += simGoToZeroButton_Click;
            // 
            // simStartStopHomingButton
            // 
            simStartStopHomingButton.Enabled = false;
            simStartStopHomingButton.Location = new Point(466, 73);
            simStartStopHomingButton.Margin = new Padding(4);
            simStartStopHomingButton.Name = "simStartStopHomingButton";
            simStartStopHomingButton.Size = new Size(144, 36);
            simStartStopHomingButton.TabIndex = 2;
            simStartStopHomingButton.Text = "Start Homing";
            simStartStopHomingButton.UseVisualStyleBackColor = true;
            simStartStopHomingButton.Click += simStartStopHomingButton_Click;
            // 
            // testMoveButton
            // 
            testMoveButton.Location = new Point(463, 198);
            testMoveButton.Name = "testMoveButton";
            testMoveButton.Size = new Size(94, 29);
            testMoveButton.TabIndex = 24;
            testMoveButton.Text = "Test Move";
            testMoveButton.UseVisualStyleBackColor = true;
            testMoveButton.Click += testMoveButton_Click;
            // 
            // MainView
            // 
            AutoScaleDimensions = new SizeF(9F, 21F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(947, 563);
            Controls.Add(groupBox2);
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
        private Label telemetryYawLabel;
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
        private Button telemetryZeroYawButton;
        private Label simHubStatusLabel;
        private Label rollCmdLabel;
        private Label pitchCmdLabel;
        private Label yawRateCmdLabel;
        private Label label1;
        private Button testMoveButton;
    }
}
