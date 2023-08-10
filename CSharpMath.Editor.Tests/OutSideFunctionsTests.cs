using System;
using System.Linq;
using CSharpMath.Display.FrontEnd;
using CSharpMath.CoreTests.FrontEnd;
using Xunit;
using TGlyph = System.Text.Rune;
using T = Xunit.InlineDataAttribute; // 'T'est
using K = CSharpMath.Editor.MathKeyboardInput; // 'K'ey
using CSharpMath.Atom;
using CSharpMath.Structures;
using System.Collections.Generic;
using Xunit.Sdk;


namespace CSharpMath.Editor.Tests {
  using EventInteractor = Action<MathKeyboard<TestFont, TGlyph>, EventHandler>;
  public class OutSideFunctionsTests {
    private static readonly TypesettingContext<TestFont, TGlyph> context = TestTypesettingContexts.Instance;

    static void Test(string latex, int atomindex, K[] inputs) {
      var keyboard = new LatexMathKeyboard();
      keyboard.KeyPress(inputs);
      var toAtom = keyboard.MathList.Atoms[atomindex];
      keyboard.ChangeMathlistByAtom(keyboard.MathList,toAtom);
      var latexwithcert = keyboard._LatexWithCert();
      Assert.Equal(latex, latexwithcert);
    }

    [
  Theory,
  T(@"12345\color{#000000}{|}6789",4,K.D1,K.D2,K.D3,K.D4,K.D5,K.D6,K.D7,K.D8,K.D9),
]
    public void AtomInput(string latex,int atomindex ,params K[] inputs) => Test(latex,atomindex, inputs);
  }
}
