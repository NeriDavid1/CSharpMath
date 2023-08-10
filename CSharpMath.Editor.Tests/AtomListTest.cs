using System;
using System.Linq;
using CSharpMath.Display.FrontEnd;
using CSharpMath.CoreTests.FrontEnd;
using Xunit;
using TGlyph = System.Text.Rune;
using T = Xunit.InlineDataAttribute; // 'T'est
using k = CSharpMath.Editor.MathKeyboardInput; // 'K'ey
using CSharpMath.Atom;
using CSharpMath.Structures;
using System.Collections.Generic;
using Xunit.Sdk;
using CSharpMath.Editor;
using CSharpMath.Atom.Atoms;

namespace CSharpMath.Editor.Tests {
  public class AtomListTest {
    public static void Test() {
      var keyBoard = new LatexMathKeyboard();
      var list = keyBoard.MathList;
    }

  }
}
