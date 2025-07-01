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
            enableDisableMotors = new Button();
            label1 = new Label();
            label2 = new Label();
            motorComStatusLabel = new Label();
            SuspendLayout();
            // 
            // estopButton
            // 
            estopButton.Location = new Point(164, 179);
            estopButton.Name = "estopButton";
            estopButton.Size = new Size(75, 66);
            estopButton.TabIndex = 0;
            estopButton.Text = "STOP";
            estopButton.UseVisualStyleBackColor = true;
            estopButton.Click += estopButton_Click;
            // 
            // enableDisableMotors
            // 
            enableDisableMotors.Location = new Point(27, 177);
            enableDisableMotors.Name = "enableDisableMotors";
            enableDisableMotors.Size = new Size(74, 68);
            enableDisableMotors.TabIndex = 1;
            enableDisableMotors.Text = "Enable";
            enableDisableMotors.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(26, 157);
            label1.Name = "label1";
            label1.Size = new Size(51, 17);
            label1.TabIndex = 2;
            label1.Text = "Motors";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(12, 9);
            label2.Name = "label2";
            label2.Size = new Size(43, 17);
            label2.TabIndex = 3;
            label2.Text = "Status";
            // 
            // motorComStatusLabel
            // 
            motorComStatusLabel.AutoSize = true;
            motorComStatusLabel.Location = new Point(61, 9);
            motorComStatusLabel.Name = "motorComStatusLabel";
            motorComStatusLabel.Size = new Size(29, 17);
            motorComStatusLabel.TabIndex = 4;
            motorComStatusLabel.Text = "Idle";
            // 
            // MainView
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(motorComStatusLabel);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(enableDisableMotors);
            Controls.Add(estopButton);
            Name = "MainView";
            Text = "Form1";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button estopButton;
        private Button enableDisableMotors;
        private Label label1;
        private Label label2;
        private Label motorComStatusLabel;
    }
}
