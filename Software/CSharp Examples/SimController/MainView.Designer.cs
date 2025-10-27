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
            telemetryStatusLabel = new Label();
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
            simHubStatusLabel = new Label();
            rollScaleLabel = new Label();
            trackBarRollScale = new TrackBar();
            pitchScaleLabel = new Label();
            trackBarPitchScale = new TrackBar();
            label12 = new Label();
            yawScaleLabel = new Label();
            trackBarYawScale = new TrackBar();
            homingStatusLabel = new Label();
            rollRateCmdLabel = new Label();
            pitchRateCmdLabel = new Label();
            yawRateCmdLabel = new Label();
            label11 = new Label();
            testMoveButton = new Button();
            rollCmdLabel = new Label();
            pitchCmdLabel = new Label();
            yawCmdLabel = new Label();
            label1 = new Label();
            simRollLabel = new Label();
            simPitchLabel = new Label();
            simYawLabel = new Label();
            label13 = new Label();
            label14 = new Label();
            label15 = new Label();
            simGoToZeroButton = new Button();
            simStartStopHomingButton = new Button();
            remoteControlStatusLabel = new Label();
            labelLastRemoteCommand = new Label();
            groupBox1.SuspendLayout();
            groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)trackBarRollScale).BeginInit();
            ((System.ComponentModel.ISupportInitialize)trackBarPitchScale).BeginInit();
            ((System.ComponentModel.ISupportInitialize)trackBarYawScale).BeginInit();
            SuspendLayout();
            // 
            // estopButton
            // 
            estopButton.BackColor = Color.Red;
            estopButton.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            estopButton.ForeColor = Color.White;
            estopButton.Location = new Point(495, 28);
            estopButton.Margin = new Padding(4);
            estopButton.Name = "estopButton";
            estopButton.Size = new Size(95, 89);
            estopButton.TabIndex = 0;
            estopButton.Text = "STOP";
            estopButton.UseVisualStyleBackColor = false;
            estopButton.Click += estopButton_Click;
            // 
            // simEnableDisableButton
            // 
            simEnableDisableButton.Enabled = false;
            simEnableDisableButton.Location = new Point(7, 28);
            simEnableDisableButton.Margin = new Padding(4);
            simEnableDisableButton.Name = "simEnableDisableButton";
            simEnableDisableButton.Size = new Size(84, 34);
            simEnableDisableButton.TabIndex = 1;
            simEnableDisableButton.Text = "Enable";
            simEnableDisableButton.UseVisualStyleBackColor = true;
            simEnableDisableButton.Click += simEnableDisableButton_Click;
            // 
            // telemetryRawPacketLabel
            // 
            telemetryRawPacketLabel.AutoSize = true;
            telemetryRawPacketLabel.Location = new Point(10, 143);
            telemetryRawPacketLabel.Margin = new Padding(4, 0, 4, 0);
            telemetryRawPacketLabel.Name = "telemetryRawPacketLabel";
            telemetryRawPacketLabel.Size = new Size(120, 20);
            telemetryRawPacketLabel.TabIndex = 4;
            telemetryRawPacketLabel.Text = "Telemetry Packet";
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(telemetryStatusLabel);
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
            groupBox1.Location = new Point(15, 290);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(597, 212);
            groupBox1.TabIndex = 5;
            groupBox1.TabStop = false;
            groupBox1.Text = "Game Telemetry";
            // 
            // telemetryStatusLabel
            // 
            telemetryStatusLabel.AutoSize = true;
            telemetryStatusLabel.Location = new Point(10, 177);
            telemetryStatusLabel.Name = "telemetryStatusLabel";
            telemetryStatusLabel.Size = new Size(118, 20);
            telemetryStatusLabel.TabIndex = 20;
            telemetryStatusLabel.Text = "Telemetry Status";
            // 
            // enableTelemetryLinkButton
            // 
            enableTelemetryLinkButton.Enabled = false;
            enableTelemetryLinkButton.Location = new Point(495, 32);
            enableTelemetryLinkButton.Margin = new Padding(4);
            enableTelemetryLinkButton.Name = "enableTelemetryLinkButton";
            enableTelemetryLinkButton.Size = new Size(85, 83);
            enableTelemetryLinkButton.TabIndex = 6;
            enableTelemetryLinkButton.Text = "Enable Motion";
            enableTelemetryLinkButton.UseVisualStyleBackColor = true;
            enableTelemetryLinkButton.Click += enableTelemetryLinkButton_Click;
            // 
            // telemetrySwayLabel
            // 
            telemetrySwayLabel.AutoSize = true;
            telemetrySwayLabel.Location = new Point(316, 95);
            telemetrySwayLabel.Name = "telemetrySwayLabel";
            telemetrySwayLabel.Size = new Size(18, 20);
            telemetrySwayLabel.TabIndex = 17;
            telemetrySwayLabel.Text = "...";
            // 
            // telemetrySurgeLabel
            // 
            telemetrySurgeLabel.AutoSize = true;
            telemetrySurgeLabel.Location = new Point(316, 67);
            telemetrySurgeLabel.Name = "telemetrySurgeLabel";
            telemetrySurgeLabel.Size = new Size(18, 20);
            telemetrySurgeLabel.TabIndex = 16;
            telemetrySurgeLabel.Text = "...";
            // 
            // telemetryHeaveLabel
            // 
            telemetryHeaveLabel.AutoSize = true;
            telemetryHeaveLabel.Location = new Point(316, 38);
            telemetryHeaveLabel.Name = "telemetryHeaveLabel";
            telemetryHeaveLabel.Size = new Size(18, 20);
            telemetryHeaveLabel.TabIndex = 15;
            telemetryHeaveLabel.Text = "...";
            // 
            // telemetryRollRateLabel
            // 
            telemetryRollRateLabel.AutoSize = true;
            telemetryRollRateLabel.Location = new Point(188, 95);
            telemetryRollRateLabel.Name = "telemetryRollRateLabel";
            telemetryRollRateLabel.Size = new Size(18, 20);
            telemetryRollRateLabel.TabIndex = 14;
            telemetryRollRateLabel.Text = "...";
            // 
            // telemetryPitchRateLabel
            // 
            telemetryPitchRateLabel.AutoSize = true;
            telemetryPitchRateLabel.Location = new Point(188, 67);
            telemetryPitchRateLabel.Name = "telemetryPitchRateLabel";
            telemetryPitchRateLabel.Size = new Size(18, 20);
            telemetryPitchRateLabel.TabIndex = 13;
            telemetryPitchRateLabel.Text = "...";
            // 
            // telemetryYawRateLabel
            // 
            telemetryYawRateLabel.AutoSize = true;
            telemetryYawRateLabel.Location = new Point(187, 38);
            telemetryYawRateLabel.Name = "telemetryYawRateLabel";
            telemetryYawRateLabel.Size = new Size(18, 20);
            telemetryYawRateLabel.TabIndex = 12;
            telemetryYawRateLabel.Text = "...";
            // 
            // telemetryRollLabel
            // 
            telemetryRollLabel.AutoSize = true;
            telemetryRollLabel.Location = new Point(51, 95);
            telemetryRollLabel.Name = "telemetryRollLabel";
            telemetryRollLabel.Size = new Size(18, 20);
            telemetryRollLabel.TabIndex = 11;
            telemetryRollLabel.Text = "...";
            // 
            // telemetryPitchLabel
            // 
            telemetryPitchLabel.AutoSize = true;
            telemetryPitchLabel.Location = new Point(51, 67);
            telemetryPitchLabel.Name = "telemetryPitchLabel";
            telemetryPitchLabel.Size = new Size(18, 20);
            telemetryPitchLabel.TabIndex = 10;
            telemetryPitchLabel.Text = "...";
            // 
            // telemetryYawLabel
            // 
            telemetryYawLabel.AutoSize = true;
            telemetryYawLabel.Location = new Point(50, 38);
            telemetryYawLabel.Name = "telemetryYawLabel";
            telemetryYawLabel.Size = new Size(18, 20);
            telemetryYawLabel.TabIndex = 9;
            telemetryYawLabel.Text = "...";
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Location = new Point(258, 95);
            label8.Name = "label8";
            label8.Size = new Size(43, 20);
            label8.TabIndex = 8;
            label8.Text = "Sway";
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Location = new Point(258, 67);
            label9.Name = "label9";
            label9.Size = new Size(47, 20);
            label9.TabIndex = 7;
            label9.Text = "Surge";
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.Location = new Point(257, 38);
            label10.Name = "label10";
            label10.Size = new Size(51, 20);
            label10.TabIndex = 6;
            label10.Text = "Heave";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(112, 95);
            label5.Name = "label5";
            label5.Size = new Size(69, 20);
            label5.TabIndex = 5;
            label5.Text = "Roll Rate";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(112, 67);
            label6.Name = "label6";
            label6.Size = new Size(75, 20);
            label6.TabIndex = 4;
            label6.Text = "Pitch Rate";
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new Point(111, 38);
            label7.Name = "label7";
            label7.Size = new Size(69, 20);
            label7.TabIndex = 3;
            label7.Text = "Yaw Rate";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(6, 95);
            label4.Name = "label4";
            label4.Size = new Size(35, 20);
            label4.TabIndex = 2;
            label4.Text = "Roll";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(6, 67);
            label3.Name = "label3";
            label3.Size = new Size(41, 20);
            label3.TabIndex = 1;
            label3.Text = "Pitch";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(5, 38);
            label2.Name = "label2";
            label2.Size = new Size(35, 20);
            label2.TabIndex = 0;
            label2.Text = "Yaw";
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(simHubStatusLabel);
            groupBox2.Controls.Add(rollScaleLabel);
            groupBox2.Controls.Add(trackBarRollScale);
            groupBox2.Controls.Add(pitchScaleLabel);
            groupBox2.Controls.Add(trackBarPitchScale);
            groupBox2.Controls.Add(label12);
            groupBox2.Controls.Add(yawScaleLabel);
            groupBox2.Controls.Add(trackBarYawScale);
            groupBox2.Controls.Add(homingStatusLabel);
            groupBox2.Controls.Add(rollRateCmdLabel);
            groupBox2.Controls.Add(pitchRateCmdLabel);
            groupBox2.Controls.Add(yawRateCmdLabel);
            groupBox2.Controls.Add(label11);
            groupBox2.Controls.Add(testMoveButton);
            groupBox2.Controls.Add(rollCmdLabel);
            groupBox2.Controls.Add(pitchCmdLabel);
            groupBox2.Controls.Add(yawCmdLabel);
            groupBox2.Controls.Add(label1);
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
            groupBox2.Location = new Point(15, 50);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(597, 234);
            groupBox2.TabIndex = 6;
            groupBox2.TabStop = false;
            groupBox2.Text = "Simulator";
            // 
            // simHubStatusLabel
            // 
            simHubStatusLabel.AutoSize = true;
            simHubStatusLabel.Location = new Point(6, 66);
            simHubStatusLabel.Name = "simHubStatusLabel";
            simHubStatusLabel.Size = new Size(175, 20);
            simHubStatusLabel.TabIndex = 19;
            simHubStatusLabel.Text = "Status: NOT CONNECTED";
            // 
            // rollScaleLabel
            // 
            rollScaleLabel.AutoSize = true;
            rollScaleLabel.Location = new Point(428, 192);
            rollScaleLabel.Name = "rollScaleLabel";
            rollScaleLabel.Size = new Size(28, 20);
            rollScaleLabel.TabIndex = 36;
            rollScaleLabel.Text = "1.0";
            // 
            // trackBarRollScale
            // 
            trackBarRollScale.LargeChange = 10;
            trackBarRollScale.Location = new Point(313, 192);
            trackBarRollScale.Maximum = 100;
            trackBarRollScale.Name = "trackBarRollScale";
            trackBarRollScale.Size = new Size(116, 56);
            trackBarRollScale.TabIndex = 35;
            trackBarRollScale.TickStyle = TickStyle.None;
            trackBarRollScale.ValueChanged += trackBarRollScale_ValueChanged;
            // 
            // pitchScaleLabel
            // 
            pitchScaleLabel.AutoSize = true;
            pitchScaleLabel.Location = new Point(428, 162);
            pitchScaleLabel.Name = "pitchScaleLabel";
            pitchScaleLabel.Size = new Size(28, 20);
            pitchScaleLabel.TabIndex = 34;
            pitchScaleLabel.Text = "1.0";
            // 
            // trackBarPitchScale
            // 
            trackBarPitchScale.LargeChange = 10;
            trackBarPitchScale.Location = new Point(313, 162);
            trackBarPitchScale.Maximum = 100;
            trackBarPitchScale.Name = "trackBarPitchScale";
            trackBarPitchScale.Size = new Size(116, 56);
            trackBarPitchScale.TabIndex = 33;
            trackBarPitchScale.TickStyle = TickStyle.None;
            trackBarPitchScale.ValueChanged += trackBarPitchScale_ValueChanged;
            // 
            // label12
            // 
            label12.AutoSize = true;
            label12.Location = new Point(314, 108);
            label12.Name = "label12";
            label12.Size = new Size(97, 20);
            label12.TabIndex = 32;
            label12.Text = "Range Adjust";
            // 
            // yawScaleLabel
            // 
            yawScaleLabel.AutoSize = true;
            yawScaleLabel.Location = new Point(428, 133);
            yawScaleLabel.Name = "yawScaleLabel";
            yawScaleLabel.Size = new Size(28, 20);
            yawScaleLabel.TabIndex = 31;
            yawScaleLabel.Text = "1.0";
            // 
            // trackBarYawScale
            // 
            trackBarYawScale.LargeChange = 10;
            trackBarYawScale.Location = new Point(313, 133);
            trackBarYawScale.Maximum = 100;
            trackBarYawScale.Name = "trackBarYawScale";
            trackBarYawScale.Size = new Size(116, 56);
            trackBarYawScale.TabIndex = 30;
            trackBarYawScale.TickStyle = TickStyle.None;
            trackBarYawScale.ValueChanged += trackBarYawScale_ValueChanged;
            // 
            // homingStatusLabel
            // 
            homingStatusLabel.AutoSize = true;
            homingStatusLabel.Location = new Point(338, 66);
            homingStatusLabel.Name = "homingStatusLabel";
            homingStatusLabel.Size = new Size(88, 20);
            homingStatusLabel.TabIndex = 29;
            homingStatusLabel.Text = "Not Homed";
            // 
            // rollRateCmdLabel
            // 
            rollRateCmdLabel.AutoSize = true;
            rollRateCmdLabel.Location = new Point(212, 190);
            rollRateCmdLabel.Name = "rollRateCmdLabel";
            rollRateCmdLabel.Size = new Size(18, 20);
            rollRateCmdLabel.TabIndex = 28;
            rollRateCmdLabel.Text = "...";
            // 
            // pitchRateCmdLabel
            // 
            pitchRateCmdLabel.AutoSize = true;
            pitchRateCmdLabel.Location = new Point(212, 162);
            pitchRateCmdLabel.Name = "pitchRateCmdLabel";
            pitchRateCmdLabel.Size = new Size(18, 20);
            pitchRateCmdLabel.TabIndex = 27;
            pitchRateCmdLabel.Text = "...";
            // 
            // yawRateCmdLabel
            // 
            yawRateCmdLabel.AutoSize = true;
            yawRateCmdLabel.Location = new Point(212, 133);
            yawRateCmdLabel.Name = "yawRateCmdLabel";
            yawRateCmdLabel.Size = new Size(18, 20);
            yawRateCmdLabel.TabIndex = 26;
            yawRateCmdLabel.Text = "...";
            // 
            // label11
            // 
            label11.AutoSize = true;
            label11.Location = new Point(212, 108);
            label11.Name = "label11";
            label11.Size = new Size(76, 20);
            label11.TabIndex = 25;
            label11.Text = "CMD Rate";
            // 
            // testMoveButton
            // 
            testMoveButton.Enabled = false;
            testMoveButton.Location = new Point(235, 28);
            testMoveButton.Name = "testMoveButton";
            testMoveButton.Size = new Size(85, 34);
            testMoveButton.TabIndex = 24;
            testMoveButton.Text = "Test Move";
            testMoveButton.UseVisualStyleBackColor = true;
            testMoveButton.Click += testMoveButton_Click;
            // 
            // rollCmdLabel
            // 
            rollCmdLabel.AutoSize = true;
            rollCmdLabel.Location = new Point(126, 190);
            rollCmdLabel.Name = "rollCmdLabel";
            rollCmdLabel.Size = new Size(18, 20);
            rollCmdLabel.TabIndex = 23;
            rollCmdLabel.Text = "...";
            // 
            // pitchCmdLabel
            // 
            pitchCmdLabel.AutoSize = true;
            pitchCmdLabel.Location = new Point(126, 162);
            pitchCmdLabel.Name = "pitchCmdLabel";
            pitchCmdLabel.Size = new Size(18, 20);
            pitchCmdLabel.TabIndex = 22;
            pitchCmdLabel.Text = "...";
            // 
            // yawCmdLabel
            // 
            yawCmdLabel.AutoSize = true;
            yawCmdLabel.Location = new Point(126, 133);
            yawCmdLabel.Name = "yawCmdLabel";
            yawCmdLabel.Size = new Size(18, 20);
            yawCmdLabel.TabIndex = 21;
            yawCmdLabel.Text = "...";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(126, 108);
            label1.Name = "label1";
            label1.Size = new Size(42, 20);
            label1.TabIndex = 20;
            label1.Text = "CMD";
            // 
            // simRollLabel
            // 
            simRollLabel.AutoSize = true;
            simRollLabel.Location = new Point(51, 190);
            simRollLabel.Name = "simRollLabel";
            simRollLabel.Size = new Size(18, 20);
            simRollLabel.TabIndex = 17;
            simRollLabel.Text = "...";
            // 
            // simPitchLabel
            // 
            simPitchLabel.AutoSize = true;
            simPitchLabel.Location = new Point(51, 162);
            simPitchLabel.Name = "simPitchLabel";
            simPitchLabel.Size = new Size(18, 20);
            simPitchLabel.TabIndex = 16;
            simPitchLabel.Text = "...";
            // 
            // simYawLabel
            // 
            simYawLabel.AutoSize = true;
            simYawLabel.Location = new Point(50, 133);
            simYawLabel.Name = "simYawLabel";
            simYawLabel.Size = new Size(18, 20);
            simYawLabel.TabIndex = 15;
            simYawLabel.Text = "...";
            // 
            // label13
            // 
            label13.AutoSize = true;
            label13.Location = new Point(6, 190);
            label13.Name = "label13";
            label13.Size = new Size(35, 20);
            label13.TabIndex = 14;
            label13.Text = "Roll";
            // 
            // label14
            // 
            label14.AutoSize = true;
            label14.Location = new Point(6, 162);
            label14.Name = "label14";
            label14.Size = new Size(41, 20);
            label14.TabIndex = 13;
            label14.Text = "Pitch";
            // 
            // label15
            // 
            label15.AutoSize = true;
            label15.Location = new Point(5, 133);
            label15.Name = "label15";
            label15.Size = new Size(35, 20);
            label15.TabIndex = 12;
            label15.Text = "Yaw";
            // 
            // simGoToZeroButton
            // 
            simGoToZeroButton.Enabled = false;
            simGoToZeroButton.Location = new Point(110, 28);
            simGoToZeroButton.Margin = new Padding(4);
            simGoToZeroButton.Name = "simGoToZeroButton";
            simGoToZeroButton.Size = new Size(106, 34);
            simGoToZeroButton.TabIndex = 3;
            simGoToZeroButton.Text = "Go To Zero";
            simGoToZeroButton.UseVisualStyleBackColor = true;
            simGoToZeroButton.Click += simGoToZeroButton_Click;
            // 
            // simStartStopHomingButton
            // 
            simStartStopHomingButton.Enabled = false;
            simStartStopHomingButton.Location = new Point(338, 28);
            simStartStopHomingButton.Margin = new Padding(4);
            simStartStopHomingButton.Name = "simStartStopHomingButton";
            simStartStopHomingButton.Size = new Size(128, 34);
            simStartStopHomingButton.TabIndex = 2;
            simStartStopHomingButton.Text = "Start Homing";
            simStartStopHomingButton.UseVisualStyleBackColor = true;
            simStartStopHomingButton.Click += simStartStopHomingButton_Click;
            // 
            // remoteControlStatusLabel
            // 
            remoteControlStatusLabel.AutoSize = true;
            remoteControlStatusLabel.Location = new Point(17, 18);
            remoteControlStatusLabel.Name = "remoteControlStatusLabel";
            remoteControlStatusLabel.Size = new Size(147, 20);
            remoteControlStatusLabel.TabIndex = 7;
            remoteControlStatusLabel.Text = "Remote Disconnectd";
            // 
            // labelLastRemoteCommand
            // 
            labelLastRemoteCommand.AutoSize = true;
            labelLastRemoteCommand.Location = new Point(286, 18);
            labelLastRemoteCommand.Name = "labelLastRemoteCommand";
            labelLastRemoteCommand.Size = new Size(72, 20);
            labelLastRemoteCommand.TabIndex = 8;
            labelLastRemoteCommand.Text = "Last CMD";
            labelLastRemoteCommand.TextAlign = ContentAlignment.MiddleRight;
            // 
            // MainView
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(628, 520);
            Controls.Add(labelLastRemoteCommand);
            Controls.Add(remoteControlStatusLabel);
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
            ((System.ComponentModel.ISupportInitialize)trackBarRollScale).EndInit();
            ((System.ComponentModel.ISupportInitialize)trackBarPitchScale).EndInit();
            ((System.ComponentModel.ISupportInitialize)trackBarYawScale).EndInit();
            ResumeLayout(false);
            PerformLayout();
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
        private Label simHubStatusLabel;
        private Label rollCmdLabel;
        private Label pitchCmdLabel;
        private Label yawCmdLabel;
        private Label label1;
        private Button testMoveButton;
        private Label rollRateCmdLabel;
        private Label pitchRateCmdLabel;
        private Label yawRateCmdLabel;
        private Label label11;
        private Label telemetryStatusLabel;
        private Label homingStatusLabel;
        private TrackBar trackBarYawScale;
        private Label yawScaleLabel;
        private Label label12;
        private Label rollScaleLabel;
        private TrackBar trackBarRollScale;
        private Label pitchScaleLabel;
        private TrackBar trackBarPitchScale;
        private Label remoteControlStatusLabel;
        private Label labelLastRemoteCommand;
    }
}
