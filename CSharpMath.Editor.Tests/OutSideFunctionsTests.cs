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
using CSharpMath.Atom.Atoms;

namespace CSharpMath.Editor.Tests {
  using EventInteractor = Action<MathKeyboard<TestFont, TGlyph>, EventHandler>;
  public class OutSideFunctionsTests {
    private static readonly TypesettingContext<TestFont, TGlyph> context = TestTypesettingContexts.Instance;

    static void Test(int left, int atomindex, K[] inputs) {
      var keyboard = new LatexMathKeyboard();
      int i = 0;
      MathAtom? LookForAtom = new Number("0");;
      foreach (var input in inputs) {
        keyboard.KeyPress(input);
        if (atomindex == i) {
          LookForAtom = keyboard.navigation.GetCurrentAtom;
        }
        i++;
      }
      // move left
      for(int j = 0; j < left; j++) {
        keyboard.KeyPress(K.Left);
      }
      keyboard.ChangeMathlistByAtom(keyboard.MathList, LookForAtom);
      Assert.True(keyboard.navigation.GetCurrentAtom == LookForAtom);
    }

    [
  Theory,
  T(3, 2,K.D1, K.Power, K.D2),
  T(0, 2,K.D1, K.Power,K.D2,K.D3),
  T(3,3,K.NthRoot,K.D5,K.D6,K.D7),
  T(0,3,K.NthRoot,K.D5,K.D6,K.D7),
  T(0,2,K.D1,K.D2,K.D3),

]
    public void AtomInput(int moveLeft, int atomindex, params K[] inputs) => Test(moveLeft, atomindex, inputs);
  }
}
