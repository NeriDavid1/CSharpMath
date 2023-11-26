using System;
using System.Linq;
using CSharpMath.Display.FrontEnd;
using CSharpMath.CoreTests.FrontEnd;
using Xunit;
using TGlyph = System.Text.Rune;
using T = Xunit.InlineDataAttribute; // 'T'est
using K = CSharpMath.Editor.MathKeyboardInput; // 'K'ey
using CSharpMath.Atom;
using System.Collections.Generic;

namespace CSharpMath.Editor.Tests {
  public class AtomListTest {
    static void DeoploymentTest(int number,K[] inputs) {
      //var keyboard = new MathKeyboard<TestFont, TGlyph>(context, new TestFont());
      var keyboard = new LatexMathKeyboard();
      keyboard.KeyPress(inputs);
      List<MathAtom> Deployed = keyboard.MathList.Deployment();
      Assert.Equal(Deployed.Count, number);
    }

    [
  Theory,
  T(2, K.D1, K.D2),
  T( 3,K.SmallX, K.Power, K.D2, K.D1),
  T( 4,K.SmallY, K.Subscript, K.D3, K.Subscript, K.D4, K.D5),
  T( 4,K.D5, K.Power, K.Iota, K.Kappa, K.SmallEta),
  T( 3,K.Fraction,K.D5,K.Right,K.D6),
]
    public void ItemsNumber(int number,params K[] inputs) => DeoploymentTest(number,inputs);
  }
}
