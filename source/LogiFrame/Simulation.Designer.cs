﻿namespace LogiFrame
{
    partial class Simulation
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

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
            this.displayPictureBox = new System.Windows.Forms.PictureBox();
            this.displayGroupBox = new System.Windows.Forms.GroupBox();
            this.buttonsGroupBox = new System.Windows.Forms.GroupBox();
            this.button4 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.propertyGrid = new System.Windows.Forms.PropertyGrid();
            this.propertiesGroupBox = new System.Windows.Forms.GroupBox();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            ((System.ComponentModel.ISupportInitialize)(this.displayPictureBox)).BeginInit();
            this.displayGroupBox.SuspendLayout();
            this.buttonsGroupBox.SuspendLayout();
            this.propertiesGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // displayPictureBox
            // 
            this.displayPictureBox.Location = new System.Drawing.Point(6, 19);
            this.displayPictureBox.Name = "displayPictureBox";
            this.displayPictureBox.Size = new System.Drawing.Size(160, 43);
            this.displayPictureBox.TabIndex = 0;
            this.displayPictureBox.TabStop = false;
            // 
            // displayGroupBox
            // 
            this.displayGroupBox.Controls.Add(this.displayPictureBox);
            this.displayGroupBox.Location = new System.Drawing.Point(12, 12);
            this.displayGroupBox.Name = "displayGroupBox";
            this.displayGroupBox.Size = new System.Drawing.Size(173, 72);
            this.displayGroupBox.TabIndex = 1;
            this.displayGroupBox.TabStop = false;
            this.displayGroupBox.Text = "Display";
            // 
            // buttonsGroupBox
            // 
            this.buttonsGroupBox.Controls.Add(this.button4);
            this.buttonsGroupBox.Controls.Add(this.button3);
            this.buttonsGroupBox.Controls.Add(this.button2);
            this.buttonsGroupBox.Controls.Add(this.button1);
            this.buttonsGroupBox.Location = new System.Drawing.Point(12, 90);
            this.buttonsGroupBox.Name = "buttonsGroupBox";
            this.buttonsGroupBox.Size = new System.Drawing.Size(173, 64);
            this.buttonsGroupBox.TabIndex = 2;
            this.buttonsGroupBox.TabStop = false;
            this.buttonsGroupBox.Text = "Buttons";
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(132, 19);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(36, 36);
            this.button4.TabIndex = 3;
            this.button4.UseVisualStyleBackColor = true;
            this.button4.MouseDown += new System.Windows.Forms.MouseEventHandler(this.button4_MouseDown);
            this.button4.MouseUp += new System.Windows.Forms.MouseEventHandler(this.button4_MouseUp);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(90, 19);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(36, 36);
            this.button3.TabIndex = 2;
            this.button3.UseVisualStyleBackColor = true;
            this.button3.MouseDown += new System.Windows.Forms.MouseEventHandler(this.button3_MouseDown);
            this.button3.MouseUp += new System.Windows.Forms.MouseEventHandler(this.button3_MouseUp);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(48, 19);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(36, 36);
            this.button2.TabIndex = 1;
            this.button2.UseVisualStyleBackColor = true;
            this.button2.MouseDown += new System.Windows.Forms.MouseEventHandler(this.button2_MouseDown);
            this.button2.MouseUp += new System.Windows.Forms.MouseEventHandler(this.button2_MouseUp);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(6, 19);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(36, 36);
            this.button1.TabIndex = 0;
            this.button1.UseVisualStyleBackColor = true;
            this.button1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.button1_MouseDown);
            this.button1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.button1_MouseUp);
            // 
            // propertyGrid
            // 
            this.propertyGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertyGrid.HelpVisible = false;
            this.propertyGrid.Location = new System.Drawing.Point(3, 16);
            this.propertyGrid.Name = "propertyGrid";
            this.propertyGrid.PropertySort = System.Windows.Forms.PropertySort.Alphabetical;
            this.propertyGrid.Size = new System.Drawing.Size(263, 222);
            this.propertyGrid.TabIndex = 3;
            this.propertyGrid.ToolbarVisible = false;
            // 
            // propertiesGroupBox
            // 
            this.propertiesGroupBox.Controls.Add(this.propertyGrid);
            this.propertiesGroupBox.Location = new System.Drawing.Point(191, 12);
            this.propertiesGroupBox.Name = "propertiesGroupBox";
            this.propertiesGroupBox.Size = new System.Drawing.Size(269, 241);
            this.propertiesGroupBox.TabIndex = 4;
            this.propertiesGroupBox.TabStop = false;
            this.propertiesGroupBox.Text = "Properties";
            // 
            // linkLabel1
            // 
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.Location = new System.Drawing.Point(15, 237);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(126, 13);
            this.linkLabel1.TabIndex = 5;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "LogiFrame, by @ikkentim";
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // Simulation
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ClientSize = new System.Drawing.Size(472, 265);
            this.Controls.Add(this.linkLabel1);
            this.Controls.Add(this.propertiesGroupBox);
            this.Controls.Add(this.buttonsGroupBox);
            this.Controls.Add(this.displayGroupBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.KeyPreview = true;
            this.Name = "Simulation";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Simulation";
            this.TopMost = true;
            ((System.ComponentModel.ISupportInitialize)(this.displayPictureBox)).EndInit();
            this.displayGroupBox.ResumeLayout(false);
            this.buttonsGroupBox.ResumeLayout(false);
            this.propertiesGroupBox.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox displayPictureBox;
        private System.Windows.Forms.GroupBox displayGroupBox;
        private System.Windows.Forms.GroupBox buttonsGroupBox;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.PropertyGrid propertyGrid;
        private System.Windows.Forms.GroupBox propertiesGroupBox;
        private System.Windows.Forms.LinkLabel linkLabel1;
    }
}