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
      CommandFire = new ComboBox();
      CommandList = new ListView();
      AddCommand = new Button();
      Fire = new Button();
      LatexLable = new Label();
      Delete = new Button();
      DebugButton = new Button();
      label1 = new Label();
      Left = new Button();
      Right = new Button();
      Up = new Button();
      Down = new Button();
      checkBox1 = new CheckBox();
      checkBox2 = new CheckBox();
      MouseCheckBox = new CheckBox();
      KeyText = new TextBox();
      GetKeyTest = new Button();
      SuspendLayout();
      // 
      // CommandFire
      // 
      CommandFire.FormattingEnabled = true;
      CommandFire.Location = new Point(12, 248);
      CommandFire.Name = "CommandFire";
      CommandFire.Size = new Size(320, 23);
      CommandFire.TabIndex = 0;
      CommandFire.KeyDown += CommandFire_KeyDown;
      // 
      // CommandList
      // 
      CommandList.Location = new Point(12, 12);
      CommandList.Name = "CommandList";
      CommandList.Size = new Size(320, 230);
      CommandList.TabIndex = 1;
      CommandList.UseCompatibleStateImageBehavior = false;
      // 
      // AddCommand
      // 
      AddCommand.Location = new Point(12, 277);
      AddCommand.Name = "AddCommand";
      AddCommand.Size = new Size(75, 23);
      AddCommand.TabIndex = 2;
      AddCommand.Text = "Add";
      AddCommand.UseVisualStyleBackColor = true;
      AddCommand.Click += AddCommand_Click;
      // 
      // Fire
      // 
      Fire.Location = new Point(177, 277);
      Fire.Name = "Fire";
      Fire.Size = new Size(82, 23);
      Fire.TabIndex = 3;
      Fire.Text = "Fire";
      Fire.UseVisualStyleBackColor = true;
      Fire.Click += Fire_Click;
      // 
      // LatexLable
      // 
      LatexLable.AutoSize = true;
      LatexLable.Font = new Font("Tahoma", 24F, FontStyle.Regular, GraphicsUnit.Point);
      LatexLable.Location = new Point(85, 333);
      LatexLable.Name = "LatexLable";
      LatexLable.Size = new Size(151, 39);
      LatexLable.TabIndex = 4;
      LatexLable.Text = "MathText";
      LatexLable.TextAlign = ContentAlignment.MiddleCenter;
      // 
      // Delete
      // 
      Delete.Location = new Point(93, 277);
      Delete.Name = "Delete";
      Delete.Size = new Size(78, 23);
      Delete.TabIndex = 5;
      Delete.Text = "Delete";
      Delete.UseVisualStyleBackColor = true;
      Delete.Click += Delete_Click;
      // 
      // DebugButton
      // 
      DebugButton.Location = new Point(265, 277);
      DebugButton.Name = "DebugButton";
      DebugButton.Size = new Size(67, 23);
      DebugButton.TabIndex = 6;
      DebugButton.Text = "Debug";
      DebugButton.UseVisualStyleBackColor = true;
      DebugButton.Click += DebugButton_Click;
      // 
      // label1
      // 
      label1.AutoSize = true;
      label1.Location = new Point(561, 8);
      label1.Name = "label1";
      label1.Size = new Size(63, 15);
      label1.TabIndex = 8;
      label1.Text = "GrafhTable";
      // 
      // Left
      // 
      Left.Location = new Point(12, 305);
      Left.Margin = new Padding(3, 2, 3, 2);
      Left.Name = "Left";
      Left.Size = new Size(75, 22);
      Left.TabIndex = 9;
      Left.Text = "Left";
      Left.UseVisualStyleBackColor = true;
      Left.Click += Left_Click;
      // 
      // Right
      // 
      Right.Location = new Point(265, 305);
      Right.Margin = new Padding(3, 2, 3, 2);
      Right.Name = "Right";
      Right.Size = new Size(66, 22);
      Right.TabIndex = 10;
      Right.Text = "Right";
      Right.UseVisualStyleBackColor = true;
      Right.Click += Right_Click;
      // 
      // Up
      // 
      Up.Location = new Point(93, 305);
      Up.Margin = new Padding(3, 2, 3, 2);
      Up.Name = "Up";
      Up.Size = new Size(78, 22);
      Up.TabIndex = 11;
      Up.Text = "Up";
      Up.UseVisualStyleBackColor = true;
      Up.Click += Up_Click;
      // 
      // Down
      // 
      Down.Location = new Point(177, 305);
      Down.Margin = new Padding(3, 2, 3, 2);
      Down.Name = "Down";
      Down.Size = new Size(83, 22);
      Down.TabIndex = 12;
      Down.Text = "Down";
      Down.UseVisualStyleBackColor = true;
      Down.Click += Down_Click;
      // 
      // checkBox1
      // 
      checkBox1.AutoSize = true;
      checkBox1.Location = new Point(12, 420);
      checkBox1.Name = "checkBox1";
      checkBox1.Size = new Size(93, 19);
      checkBox1.TabIndex = 13;
      checkBox1.Text = "משמאל לימין";
      checkBox1.UseVisualStyleBackColor = true;
      checkBox1.CheckedChanged += checkBox1_CheckedChanged;
      // 
      // checkBox2
      // 
      checkBox2.AutoSize = true;
      checkBox2.Location = new Point(118, 420);
      checkBox2.Name = "checkBox2";
      checkBox2.Size = new Size(56, 19);
      checkBox2.TabIndex = 14;
      checkBox2.Text = "בבניה";
      checkBox2.UseVisualStyleBackColor = true;
      checkBox2.CheckedChanged += checkBox2_CheckedChanged;
      // 
      // MouseCheckBox
      // 
      MouseCheckBox.AutoSize = true;
      MouseCheckBox.Location = new Point(270, 420);
      MouseCheckBox.Name = "MouseCheckBox";
      MouseCheckBox.Size = new Size(72, 19);
      MouseCheckBox.TabIndex = 15;
      MouseCheckBox.Text = "עם עכבר";
      MouseCheckBox.UseVisualStyleBackColor = true;
      MouseCheckBox.CheckedChanged += MouseCheckBox_CheckedChanged;
      // 
      // KeyText
      // 
      KeyText.Location = new Point(93, 380);
      KeyText.Margin = new Padding(3, 2, 3, 2);
      KeyText.Name = "KeyText";
      KeyText.Size = new Size(213, 23);
      KeyText.TabIndex = 17;
      // 
      // GetKeyTest
      // 
      GetKeyTest.Location = new Point(12, 379);
      GetKeyTest.Name = "GetKeyTest";
      GetKeyTest.Size = new Size(75, 23);
      GetKeyTest.TabIndex = 18;
      GetKeyTest.Text = "GetKeyTest";
      GetKeyTest.UseVisualStyleBackColor = true;
      GetKeyTest.Click += GetKeyTest_Click;
      // 
      // Form1
      // 
      AutoScaleDimensions = new SizeF(7F, 15F);
      AutoScaleMode = AutoScaleMode.Font;
      ClientSize = new Size(1334, 448);
      Controls.Add(GetKeyTest);
      Controls.Add(KeyText);
      Controls.Add(MouseCheckBox);
      Controls.Add(checkBox2);
      Controls.Add(checkBox1);
      Controls.Add(Down);
      Controls.Add(Up);
      Controls.Add(Right);
      Controls.Add(Left);
      Controls.Add(label1);
      Controls.Add(DebugButton);
      Controls.Add(Delete);
      Controls.Add(LatexLable);
      Controls.Add(Fire);
      Controls.Add(AddCommand);
      Controls.Add(CommandList);
      Controls.Add(CommandFire);
      Name = "Form1";
      Text = "Form1";
      Load += Form1_Load;
      ResumeLayout(false);
      PerformLayout();
    }

    #endregion

    private ComboBox CommandFire;
    private ListView CommandList;
    private Button AddCommand;
    private Button Fire;
    private Label LatexLable;
    private Button Delete;
    private Button DebugButton;
    private Label label1;
    private Button Left;
    private Button Right;
    private Button Up;
    private Button Down;
    private CheckBox checkBox1;
    private CheckBox checkBox2;
    private CheckBox MouseCheckBox;
    private TextBox KeyText;
    private Button GetKeyTest;
  }
}