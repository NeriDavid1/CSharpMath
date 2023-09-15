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
      checkBox2 = new CheckBox();
      MouseCheckBox = new CheckBox();
      KeyText = new TextBox();
      GetKeyTest = new Button();
      SuspendLayout();
      // 
      // CommandFire
      // 
      CommandFire.FormattingEnabled = true;
      CommandFire.Location = new Point(14, 331);
      CommandFire.Margin = new Padding(3, 4, 3, 4);
      CommandFire.Name = "CommandFire";
      CommandFire.Size = new Size(365, 28);
      CommandFire.TabIndex = 0;
      CommandFire.KeyDown += CommandFire_KeyDown;
      // 
      // CommandList
      // 
      CommandList.Location = new Point(14, 16);
      CommandList.Margin = new Padding(3, 4, 3, 4);
      CommandList.Name = "CommandList";
      CommandList.Size = new Size(365, 305);
      CommandList.TabIndex = 1;
      CommandList.UseCompatibleStateImageBehavior = false;
      // 
      // AddCommand
      // 
      AddCommand.Location = new Point(14, 369);
      AddCommand.Margin = new Padding(3, 4, 3, 4);
      AddCommand.Name = "AddCommand";
      AddCommand.Size = new Size(86, 31);
      AddCommand.TabIndex = 2;
      AddCommand.Text = "Add";
      AddCommand.UseVisualStyleBackColor = true;
      AddCommand.Click += AddCommand_Click;
      // 
      // Fire
      // 
      Fire.Location = new Point(202, 369);
      Fire.Margin = new Padding(3, 4, 3, 4);
      Fire.Name = "Fire";
      Fire.Size = new Size(94, 31);
      Fire.TabIndex = 3;
      Fire.Text = "Fire";
      Fire.UseVisualStyleBackColor = true;
      Fire.Click += Fire_Click;
      // 
      // LatexLable
      // 
      LatexLable.AutoSize = true;
      LatexLable.Font = new Font("Tahoma", 24F, FontStyle.Regular, GraphicsUnit.Point);
      LatexLable.Location = new Point(97, 444);
      LatexLable.Name = "LatexLable";
      LatexLable.Size = new Size(184, 48);
      LatexLable.TabIndex = 4;
      LatexLable.Text = "MathText";
      LatexLable.TextAlign = ContentAlignment.MiddleCenter;
      // 
      // Delete
      // 
      Delete.Location = new Point(106, 369);
      Delete.Margin = new Padding(3, 4, 3, 4);
      Delete.Name = "Delete";
      Delete.Size = new Size(89, 31);
      Delete.TabIndex = 5;
      Delete.Text = "Delete";
      Delete.UseVisualStyleBackColor = true;
      Delete.Click += Delete_Click;
      // 
      // DebugButton
      // 
      DebugButton.Location = new Point(303, 369);
      DebugButton.Margin = new Padding(3, 4, 3, 4);
      DebugButton.Name = "DebugButton";
      DebugButton.Size = new Size(77, 31);
      DebugButton.TabIndex = 6;
      DebugButton.Text = "Debug";
      DebugButton.UseVisualStyleBackColor = true;
      DebugButton.Click += DebugButton_Click;
      // 
      // label1
      // 
      label1.AutoSize = true;
      label1.Location = new Point(641, 11);
      label1.Name = "label1";
      label1.Size = new Size(80, 20);
      label1.TabIndex = 8;
      label1.Text = "GrafhTable";
      // 
      // Left
      // 
      Left.Location = new Point(14, 407);
      Left.Name = "Left";
      Left.Size = new Size(86, 29);
      Left.TabIndex = 9;
      Left.Text = "Left";
      Left.UseVisualStyleBackColor = true;
      Left.Click += Left_Click;
      // 
      // Right
      // 
      Right.Location = new Point(303, 407);
      Right.Name = "Right";
      Right.Size = new Size(75, 29);
      Right.TabIndex = 10;
      Right.Text = "Right";
      Right.UseVisualStyleBackColor = true;
      Right.Click += Right_Click;
      // 
      // Up
      // 
      Up.Location = new Point(106, 407);
      Up.Name = "Up";
      Up.Size = new Size(89, 29);
      Up.TabIndex = 11;
      Up.Text = "Up";
      Up.UseVisualStyleBackColor = true;
      Up.Click += Up_Click;
      // 
      // Down
      // 
      Down.Location = new Point(202, 407);
      Down.Name = "Down";
      Down.Size = new Size(95, 29);
      Down.TabIndex = 12;
      Down.Text = "Down";
      Down.UseVisualStyleBackColor = true;
      Down.Click += Down_Click;
      // 
      // checkBox2
      // 
      checkBox2.AutoSize = true;
      checkBox2.Location = new Point(135, 560);
      checkBox2.Margin = new Padding(3, 4, 3, 4);
      checkBox2.Name = "checkBox2";
      checkBox2.Size = new Size(69, 24);
      checkBox2.TabIndex = 14;
      checkBox2.Text = "בבניה";
      checkBox2.UseVisualStyleBackColor = true;
      checkBox2.CheckedChanged += checkBox2_CheckedChanged;
      // 
      // MouseCheckBox
      // 
      MouseCheckBox.AutoSize = true;
      MouseCheckBox.Location = new Point(309, 560);
      MouseCheckBox.Margin = new Padding(3, 4, 3, 4);
      MouseCheckBox.Name = "MouseCheckBox";
      MouseCheckBox.Size = new Size(88, 24);
      MouseCheckBox.TabIndex = 15;
      MouseCheckBox.Text = "עם עכבר";
      MouseCheckBox.UseVisualStyleBackColor = true;
      MouseCheckBox.CheckedChanged += MouseCheckBox_CheckedChanged;
      // 
      // KeyText
      // 
      KeyText.Location = new Point(106, 507);
      KeyText.Name = "KeyText";
      KeyText.Size = new Size(243, 27);
      KeyText.TabIndex = 17;
      // 
      // GetKeyTest
      // 
      GetKeyTest.Location = new Point(14, 505);
      GetKeyTest.Margin = new Padding(3, 4, 3, 4);
      GetKeyTest.Name = "GetKeyTest";
      GetKeyTest.Size = new Size(86, 31);
      GetKeyTest.TabIndex = 18;
      GetKeyTest.Text = "GetKeyTest";
      GetKeyTest.UseVisualStyleBackColor = true;
      GetKeyTest.Click += GetKeyTest_Click;
      // 
      // Form1
      // 
      AutoScaleDimensions = new SizeF(8F, 20F);
      AutoScaleMode = AutoScaleMode.Font;
      ClientSize = new Size(1525, 597);
      Controls.Add(GetKeyTest);
      Controls.Add(KeyText);
      Controls.Add(MouseCheckBox);
      Controls.Add(checkBox2);
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
      Margin = new Padding(3, 4, 3, 4);
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
    private CheckBox checkBox2;
    private CheckBox MouseCheckBox;
    private TextBox KeyText;
    private Button GetKeyTest;
  }
}