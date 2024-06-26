using System.Drawing.Text;
using System.Security.AccessControl;
using System.Text;
using CSharpMath.Atom;
using CSharpMath.Editor;
using CSharpMath.Editor.Visualizer.Properties;

namespace CSharpMath.Editor.Visualizer {
  public partial class Form1 : Form {
    private List<MathKeyboardInput> inputList = new();
    private LatexMathKeyboard latexMathKeyboard = new();
    private Func<String> GetText = () => "";
    private bool MathTextWithCert = false;
    public Form1() {
      InitializeComponent();
    }

    private void Form1_Load(object sender, EventArgs e) {

      foreach (MathKeyboardInput input in (MathKeyboardInput[])Enum.GetValues(typeof(MathKeyboardInput))) {
        CommandFire.Items.Add(input);
      }
      if (Settings.Default.commands == "") return;
      string[] commands = Settings.Default.commands.Split('$');
      foreach (string s in commands) {
        if (s == "") continue;
        var t = (MathKeyboardInput)Enum.Parse(typeof(MathKeyboardInput), s, true);
        CommandList.Items.Add(t.ToString());
        inputList.Add(t);
      }
      GetText = () => latexMathKeyboard.LaTeX;

    }

    private void AddCommand_Click(object sender, EventArgs e) {
      if (CommandFire.SelectedIndex == -1) return;
      CommandList.Items.Add(CommandFire.SelectedItem.ToString());
      inputList.Add((MathKeyboardInput)CommandFire.SelectedItem);
      SaveCommands();
    }

    private void SaveCommands() {
      string savecommandline = "";
      foreach (MathKeyboardInput s in inputList) {
        savecommandline += (int)s + "$";
      }
      Settings.Default.commands = savecommandline;
      Settings.Default.Save();
    }

    private void Delete_Click(object sender, EventArgs e) {
      if (CommandList.Items.Count == 0) return;
      int lang = CommandList.Items.Count - 1;
      CommandList.Items.RemoveAt(lang);
      inputList.RemoveAt(lang);
      SaveCommands();
    }

    private void Fire_Click(object sender, EventArgs e) {
      latexMathKeyboard.Clear();
      clearButtons();
      SetGetText();
        foreach (MathKeyboardInput i in inputList) {
          latexMathKeyboard.KeyPress(i);
          var text = GetText();
        }

      LatexLable.Text = GetText();
    }

    private void clearButtons() {
      if (buttons.IsNonEmpty()) {
        lastbutton = new Point(219, 22);
        buttons.ForEach(button => this.Controls.Remove(button));
        buttons.Clear();
      }
    }
    // unnessary since the new version of CSharpMath.Editor
    private void CreateGrafhTableByLeftRight() {
      var pos = GetKeyTest.Location;
      MathList matlist = latexMathKeyboard.MathList;
      MathListIndex.Level0Index(0);
      latexMathKeyboard.InsertionIndex = MathListIndex.Level0Index(0);
      var lastatom = latexMathKeyboard.navigation.GetCurrentAtom;
      for (int i = 0; i < inputList.Count; i++) {
        var atom = latexMathKeyboard.navigation.GetCurrentAtom;
        lastatom = atom;
        Button newButton = new Button();
        newButton.Text = atom?.TypeName + " / " + atom?.DebugString + " / " + index().SubIndexType.ToString() + " / " + index().ToString();
        pos = new Point(pos.X + 200, pos.Y);
        if (pos.X > Form1.ActiveForm.Size.Width - 80) {
          pos.X = GetKeyTest.Location.X + 200;
          pos.Y += 80;
        }
        newButton.Location = new Point(pos.X, pos.Y);
        newButton.Size = new Size(200, 80);
        buttons.Add(newButton);
        this.Controls.Add(newButton);
        latexMathKeyboard.KeyPress(MathKeyboardInput.Right);
      }
      MathListIndex index() => latexMathKeyboard.InsertionIndex;
    }
    Point lastbutton = new Point(219, 22);
    List<Button> buttons = new();
    // unnessary since the new version of CSharpMath.Editor
    private void CreateGrafhTableByBuilding() {
      var atom = latexMathKeyboard.navigation.GetCurrentAtom;
      Button newButton = new Button();
      this.Controls.Add(newButton);
      newButton.Text = atom?.TypeName + " / " + latexMathKeyboard.LaTeX + " / " + index().SubIndexType.ToString() + " / " + index().ToString();
      lastbutton = new Point(lastbutton.X + 200, lastbutton.Y);
      if (Form1.ActiveForm == null) return;
      if (lastbutton.X > Form1.ActiveForm.Size.Width - 80) {
        lastbutton.X = GetKeyTest.Location.X + 200;
        lastbutton.Y += 80;
      }
      newButton.Location = new Point(lastbutton.X, lastbutton.Y);
      newButton.Size = new Size(200, 80);
      buttons.Add(newButton);
      MathListIndex index() => latexMathKeyboard.InsertionIndex;
    }
    private void DebugButton_Click(object sender, EventArgs e) {
      latexMathKeyboard.Clear();
      SetGetText();
      latexMathKeyboard.KeyPress(inputList.ToArray());
      LatexLable.Text = GetText();
    }


    private void CommandFire_KeyDown(object sender, KeyEventArgs e) {
      if (e.KeyCode == Keys.Right) {
        AddCommand.PerformClick();
      }
      if (e.KeyCode == Keys.Left) {
        Delete.PerformClick();
      }
    }

    private void Left_Click(object sender, EventArgs e) {
      CommandList.Items.Add("Left");
      inputList.Add(MathKeyboardInput.Left);
    }

    private void Up_Click(object sender, EventArgs e) {
      CommandList.Items.Add("Up");
      inputList.Add(MathKeyboardInput.Up);
    }

    private void Down_Click(object sender, EventArgs e) {
      CommandList.Items.Add("Down");
      inputList.Add(MathKeyboardInput.Down);
    }

    private void Right_Click(object sender, EventArgs e) {
      CommandList.Items.Add("Right");
      inputList.Add(MathKeyboardInput.Right);
    }

    private void checkBox2_CheckedChanged(object sender, EventArgs e) {

    }

    private void MouseCheckBox_CheckedChanged(object sender, EventArgs e) {
      if (this.MouseCheckBox.Checked) {
        MathTextWithCert = true;
      } else {
        MathTextWithCert = false;
      }
    }

    private String GetAsKeyTest() {
      string Addon = "K.";
      StringBuilder stringBuilder = new StringBuilder();
      foreach (var item in inputList) {
        stringBuilder.Append(Addon);
        stringBuilder.Append(item.ToString());
        stringBuilder.Append(@", ");
      }
      stringBuilder.Remove(stringBuilder.Length - 2, 2);
      return stringBuilder.ToString();
    }

    private void GetKeyTest_Click(object sender, EventArgs e) {
      KeyText.Text = GetAsKeyTest();
    }
    private void SetGetText() {
      if (MathTextWithCert) {
        GetText = () => latexMathKeyboard._LatexWithCert();
      } else {
        GetText = () => latexMathKeyboard.LaTeX;
      }
    }
  }
}