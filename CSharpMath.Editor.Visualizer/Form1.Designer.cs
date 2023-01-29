namespace CSharpMath.Editor.Visualizer {
  partial class Form1 {
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    ///  Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing) {
      if (disposing && (components != null)) {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    ///  Required method for Designer support - do not modify
    ///  the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent() {
            this.CommandFire = new System.Windows.Forms.ComboBox();
            this.CommandList = new System.Windows.Forms.ListView();
            this.AddCommand = new System.Windows.Forms.Button();
            this.Fire = new System.Windows.Forms.Button();
            this.LatexLable = new System.Windows.Forms.Label();
            this.Delete = new System.Windows.Forms.Button();
            this.DebugButton = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.Left = new System.Windows.Forms.Button();
            this.Right = new System.Windows.Forms.Button();
            this.Up = new System.Windows.Forms.Button();
            this.Down = new System.Windows.Forms.Button();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.checkBox2 = new System.Windows.Forms.CheckBox();
            this.MouseCheckBox = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // CommandFire
            // 
            this.CommandFire.FormattingEnabled = true;
            this.CommandFire.Location = new System.Drawing.Point(14, 331);
            this.CommandFire.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.CommandFire.Name = "CommandFire";
            this.CommandFire.Size = new System.Drawing.Size(365, 28);
            this.CommandFire.TabIndex = 0;
            this.CommandFire.KeyDown += new System.Windows.Forms.KeyEventHandler(this.CommandFire_KeyDown);
            // 
            // CommandList
            // 
            this.CommandList.Location = new System.Drawing.Point(14, 16);
            this.CommandList.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.CommandList.Name = "CommandList";
            this.CommandList.Size = new System.Drawing.Size(365, 305);
            this.CommandList.TabIndex = 1;
            this.CommandList.UseCompatibleStateImageBehavior = false;
            // 
            // AddCommand
            // 
            this.AddCommand.Location = new System.Drawing.Point(14, 369);
            this.AddCommand.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.AddCommand.Name = "AddCommand";
            this.AddCommand.Size = new System.Drawing.Size(86, 31);
            this.AddCommand.TabIndex = 2;
            this.AddCommand.Text = "Add";
            this.AddCommand.UseVisualStyleBackColor = true;
            this.AddCommand.Click += new System.EventHandler(this.AddCommand_Click);
            // 
            // Fire
            // 
            this.Fire.Location = new System.Drawing.Point(202, 369);
            this.Fire.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Fire.Name = "Fire";
            this.Fire.Size = new System.Drawing.Size(94, 31);
            this.Fire.TabIndex = 3;
            this.Fire.Text = "Fire";
            this.Fire.UseVisualStyleBackColor = true;
            this.Fire.Click += new System.EventHandler(this.Fire_Click);
            // 
            // LatexLable
            // 
            this.LatexLable.AutoSize = true;
            this.LatexLable.Font = new System.Drawing.Font("Tahoma", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.LatexLable.Location = new System.Drawing.Point(97, 444);
            this.LatexLable.Name = "LatexLable";
            this.LatexLable.Size = new System.Drawing.Size(184, 48);
            this.LatexLable.TabIndex = 4;
            this.LatexLable.Text = "MathText";
            this.LatexLable.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Delete
            // 
            this.Delete.Location = new System.Drawing.Point(106, 369);
            this.Delete.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Delete.Name = "Delete";
            this.Delete.Size = new System.Drawing.Size(89, 31);
            this.Delete.TabIndex = 5;
            this.Delete.Text = "Delete";
            this.Delete.UseVisualStyleBackColor = true;
            this.Delete.Click += new System.EventHandler(this.Delete_Click);
            // 
            // DebugButton
            // 
            this.DebugButton.Location = new System.Drawing.Point(303, 369);
            this.DebugButton.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.DebugButton.Name = "DebugButton";
            this.DebugButton.Size = new System.Drawing.Size(77, 31);
            this.DebugButton.TabIndex = 6;
            this.DebugButton.Text = "Debug";
            this.DebugButton.UseVisualStyleBackColor = true;
            this.DebugButton.Click += new System.EventHandler(this.DebugButton_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(250, 29);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(10, 33);
            this.button1.TabIndex = 7;
            this.button1.Text = "Start";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Visible = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(641, 11);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(80, 20);
            this.label1.TabIndex = 8;
            this.label1.Text = "GrafhTable";
            // 
            // Left
            // 
            this.Left.Location = new System.Drawing.Point(14, 407);
            this.Left.Name = "Left";
            this.Left.Size = new System.Drawing.Size(86, 29);
            this.Left.TabIndex = 9;
            this.Left.Text = "Left";
            this.Left.UseVisualStyleBackColor = true;
            this.Left.Click += new System.EventHandler(this.Left_Click);
            // 
            // Right
            // 
            this.Right.Location = new System.Drawing.Point(303, 407);
            this.Right.Name = "Right";
            this.Right.Size = new System.Drawing.Size(75, 29);
            this.Right.TabIndex = 10;
            this.Right.Text = "Right";
            this.Right.UseVisualStyleBackColor = true;
            this.Right.Click += new System.EventHandler(this.Right_Click);
            // 
            // Up
            // 
            this.Up.Location = new System.Drawing.Point(106, 407);
            this.Up.Name = "Up";
            this.Up.Size = new System.Drawing.Size(89, 29);
            this.Up.TabIndex = 11;
            this.Up.Text = "Up";
            this.Up.UseVisualStyleBackColor = true;
            this.Up.Click += new System.EventHandler(this.Up_Click);
            // 
            // Down
            // 
            this.Down.Location = new System.Drawing.Point(202, 407);
            this.Down.Name = "Down";
            this.Down.Size = new System.Drawing.Size(95, 29);
            this.Down.TabIndex = 12;
            this.Down.Text = "Down";
            this.Down.UseVisualStyleBackColor = true;
            this.Down.Click += new System.EventHandler(this.Down_Click);
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(14, 560);
            this.checkBox1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(115, 24);
            this.checkBox1.TabIndex = 13;
            this.checkBox1.Text = "משמאל לימין";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // checkBox2
            // 
            this.checkBox2.AutoSize = true;
            this.checkBox2.Location = new System.Drawing.Point(135, 560);
            this.checkBox2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.checkBox2.Name = "checkBox2";
            this.checkBox2.Size = new System.Drawing.Size(69, 24);
            this.checkBox2.TabIndex = 14;
            this.checkBox2.Text = "בבניה";
            this.checkBox2.UseVisualStyleBackColor = true;
            this.checkBox2.CheckedChanged += new System.EventHandler(this.checkBox2_CheckedChanged);
            // 
            // MouseCheckBox
            // 
            this.MouseCheckBox.AutoSize = true;
            this.MouseCheckBox.Location = new System.Drawing.Point(309, 560);
            this.MouseCheckBox.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MouseCheckBox.Name = "MouseCheckBox";
            this.MouseCheckBox.Size = new System.Drawing.Size(88, 24);
            this.MouseCheckBox.TabIndex = 15;
            this.MouseCheckBox.Text = "עם עכבר";
            this.MouseCheckBox.UseVisualStyleBackColor = true;
            this.MouseCheckBox.CheckedChanged += new System.EventHandler(this.MouseCheckBox_CheckedChanged);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1525, 597);
            this.Controls.Add(this.MouseCheckBox);
            this.Controls.Add(this.checkBox2);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.Down);
            this.Controls.Add(this.Up);
            this.Controls.Add(this.Right);
            this.Controls.Add(this.Left);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.DebugButton);
            this.Controls.Add(this.Delete);
            this.Controls.Add(this.LatexLable);
            this.Controls.Add(this.Fire);
            this.Controls.Add(this.AddCommand);
            this.Controls.Add(this.CommandList);
            this.Controls.Add(this.CommandFire);
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

    }

    #endregion

    private ComboBox CommandFire;
    private ListView CommandList;
    private Button AddCommand;
    private Button Fire;
    private Label LatexLable;
    private Button Delete;
    private Button DebugButton;
    private Button button1;
    private Label label1;
    private Button Left;
    private Button Right;
    private Button Up;
    private Button Down;
    private CheckBox checkBox1;
    private CheckBox checkBox2;
    private CheckBox MouseCheckBox;
  }
}