using System.Linq;
using System.Text.RegularExpressions;
using Xunit;
using AngouriMath;
namespace CSharpMath.EvaluationTests {
  using AngouriMath.Extensions;
  using Atom;
  using CSharpMath.Editor;
  using K = CSharpMath.Editor.MathKeyboardInput; // 'K'ey
  using T = Xunit.InlineDataAttribute; // 'T'est
  public class EvalutionNewSystem {
    static void Test(string angourimathFormat, K[] inputs) {
      var keyboard = new LatexMathKeyboard();
      keyboard.KeyPress(inputs);
      var mathlist = keyboard.MathList;
      var ConvertToMathFormat = mathlist.ToEntity();
      var StringResult = ConvertToMathFormat.Stringize();
      // remove all spaces
      StringResult = Regex.Replace(StringResult, @"\s+", "");
      Assert.Equal(angourimathFormat, StringResult);
    }
    internal static MathList ParseLaTeX(string latex) =>
      LaTeXParser.MathListFromLaTeX(latex).Match(list => list, e => throw new Xunit.Sdk.XunitException(e));
    static Entity ParseMath(string latex) =>
      Evaluation.ParseExpression(latex);

    [
      Theory,
          T(@"27^(1/3)", K.CubeRoot, K.D2, K.D7),
          T(@"12/1", K.D1, K.D2, K.Slash, K.D1),
          T(@"a/1", K.SmallA, K.Slash, K.D1),
          T(@"X*y*Z/1", K.X, K.SmallY, K.Z, K.Slash, K.D1),
          T(@"α*β*c/1", K.SmallAlpha, K.SmallBeta, K.SmallC, K.Slash, K.D1),
          T(@"+oo/1", K.Infinity, K.Slash, K.D1),
          T(@"sin^2*θ/1", K.Sine, K.Power, K.D2, K.Right, K.SmallTheta, K.Slash, K.D1),
          T(@"log(10,3)*π/1", K.LogarithmWithBase, K.D3, K.Right, K.SmallPi, K.Slash, K.D1),

          T(@"1/(1/1)", K.Slash, K.LeftRoundBracket, K.Slash, K.D1, K.RightRoundBracket),
          T(@"1/(2/1)", K.Slash, K.D2, K.Slash, K.D1),
          T(@"1/2*1/1", K.D1, K.Slash, K.D2, K.Multiply, K.D1),
          T(@"1/2*1/1", K.D1, K.Slash, K.D2, K.Right, K.D1, K.Slash, K.D1),
          T(@"1/2*2/1", K.D1, K.Slash, K.D2, K.Multiply, K.D2, K.Slash, K.D1),

          T(@"(2/1)^(1/2)", K.SquareRoot, K.LeftRoundBracket, K.D2, K.Slash, K.D1, K.RightRoundBracket),
          T(@"1/(1*1)", K.D1, K.Slash, K.D1, K.LeftRoundBracket, K.D1, K.RightRoundBracket),
          T(@"log(10,1/1)", K.LogarithmWithBase, K.LeftRoundBracket, K.Slash, K.D1, K.RightRoundBracket),

          T(@"[0,+oo]", K.LeftSquareBracket, K.D0, K.Comma, K.Infinity ,K.RightSquareBracket),
          T(@"(1+2)/1", K.LeftRoundBracket, K.D1, K.Plus, K.D2, K.RightRoundBracket, K.Slash, K.D1),
          T(@"abs((1+2)/1)", K.Absolute, K.LeftRoundBracket, K.D1, K.Plus, K.D2, K.RightRoundBracket, K.Slash, K.D1),
          T(@"1+2/1", K.D1, K.Plus, K.D2, K.Slash, K.D1),
          T(@"1-2/1", K.D1, K.Minus, K.D2, K.Slash, K.D1),
          T(@"1*2/1", K.D1, K.Multiply, K.D2, K.Slash, K.D1),
          T(@"1/2", K.D1, K.Slash, K.D2),
          T(@"1/2/1", K.D1, K.Ratio, K.D2, K.Slash, K.D1),
          T(@"1=2/1", K.D1, K.Equals, K.D2, K.Slash, K.D1),
          T(@"1!=2/1", K.D1, K.NotEquals, K.D2, K.Slash, K.D1),
          T(@"1<2/1", K.D1, K.LessThan, K.D2, K.Slash, K.D1),
          T(@"1<=2/1", K.D1, K.LessOrEquals, K.D2, K.Slash, K.D1),
          T(@"1>2/1", K.D1, K.GreaterThan, K.D2, K.Slash, K.D1),
          // add some test to plusminus
          T(@"1+2/1or1-2/1", K.D1, K.PlusMinus, K.D2, K.Slash, K.D1),
          T(@"56=X+5or56=X-5", K.D5, K.D6, K.Equals, K.X, K.PlusMinus, K.D5),
        ]
    public void Genral(string latex, params K[] inputs) => Test(latex, inputs);
  }
}
