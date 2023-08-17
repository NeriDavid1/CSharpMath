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
    // for now the tests are not working because the new system tests is not implemented yet
    static void Test(string angourimathFormat, K[] inputs) {
      var keyboard = new LatexMathKeyboard();
      keyboard.KeyPress(inputs);
      var mathlist = keyboard.MathList;
      var ConvertToMathFormat = mathlist.ToEntity();
      var StringBuilderEntity = Evaluation.ConvertToMathString2(mathlist).ToEntity(); // הייתי פה
      Assert.Equal(StringBuilderEntity, ConvertToMathFormat);
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
          T(@"12/1", K.D1, K.D2, K.Slash, K.D1), // changed fraction from \frac{12}{1} to 12/1
          T(@"a/1", K.SmallA, K.Slash, K.D1), // changed fraction from \frac{a}{1} to a/1
          T(@"X*y*Z/1", K.X, K.SmallY, K.Z, K.Slash, K.D1), // changed fraction from \frac{XyZ}{1} to XyZ/1
          T(@"α*β*c/1", K.SmallAlpha, K.SmallBeta, K.SmallC, K.Slash, K.D1),
          T(@"+oo/1", K.Infinity, K.Slash, K.D1),
          T(@"\sin^2 \theta/1", K.Sine, K.Power, K.D2, K.Right, K.SmallTheta, K.Slash, K.D1), // changed fraction from \frac{\sin ^2\theta}{1} to sin^2 \theta/1
          T(@"\log_3 \pi/1", K.LogarithmWithBase, K.D3, K.Right, K.SmallPi, K.Slash, K.D1), // changed fraction from \frac{\log _3\pi}{1} to log_3 \pi/1

          T(@"1/(1/1)", K.Slash, K.LeftRoundBracket, K.Slash, K.D1, K.RightRoundBracket), // changed fraction from \frac{1}{\frac{1}{1}} to 1/(1/1)
          T(@"1/(2/1)", K.Slash, K.D2, K.Slash, K.D1), // changed fraction from \frac{1}{\frac{2}{1}} to 1/(2/1)
          T(@"1/2*1/1", K.D1, K.Slash, K.D2, K.Multiply, K.D1), // changed fraction from \frac{1}{2}\times \frac{1}{1} to 1/2*1/1
          T(@"1/2*1/1", K.D1, K.Slash, K.D2, K.Right, K.D1, K.Slash, K.D1), // changed fraction from \frac{1}{2}\times \frac{1}{1} to 1/2*1/1
          T(@"1/2*2/1", K.D1, K.Slash, K.D2, K.Multiply, K.D2, K.Slash, K.D1), // changed fraction from \frac{1}{2}\times \frac{2}{1} to 1/2*2/1

          T(@"(2/1)^(1/2)", K.SquareRoot, K.LeftRoundBracket, K.D2, K.Slash, K.D1, K.RightRoundBracket), // changed square root from \sqrt{\frac{2}{1}} to \sqrt(2/1)
          T(@"1/1^(1/2)", K.NthRoot, K.Slash, K.D1, K.LeftRoundBracket, K.D1, K.RightRoundBracket), // changed nth root from \sqrt[\frac{1}{1}]{1} to \sqrt[1/1]{1}
          T(@"\log_{1/1}", K.LogarithmWithBase, K.LeftRoundBracket, K.Slash, K.D1, K.RightRoundBracket), // changed logarithm from \log _{\frac{1}{1}} to \log_{1/1}

          T(@"(\frac{] }{1}", K.LeftRoundBracket, K.LeftSquareBracket, K.RightSquareBracket, K.Slash, K.D1), // changed fraction from (\frac{[\} }{1} to (\frac{] }{1}
          T(@"{[0,\infty)/1}", K.LeftCurlyBracket, K.LeftSquareBracket, K.D0, K.Comma, K.Infinity, K.RightRoundBracket, K.Slash, K.D1), // changed fraction from \{ \frac{[0,\infty )}{1} to {[0,\infty)/1}
          T(@"(1+2)/1", K.LeftRoundBracket, K.D1, K.Plus, K.D2, K.RightRoundBracket, K.Slash, K.D1), // changed fraction from \frac{(1+2)}{1} to (1+2)/1
          T(@"\(1+2\right) /1", K.LeftRoundBracket, K.D1, K.Plus, K.D2, K.RightRoundBracket, K.Slash, K.D1), // changed fraction from \frac{\( 1+2\right) }{1} to \(1+2\right) /1
          T(@"abs((1+2)/1)", K.Absolute, K.LeftRoundBracket, K.D1, K.Plus, K.D2, K.RightRoundBracket, K.Slash, K.D1), // changed fraction from |1+\frac{2|}{1} to |1+2|/1
          T(@"abs(1+2\right|/1", K.Absolute, K.LeftRoundBracket, K.D1, K.Plus, K.D2, K.RightRoundBracket, K.Slash, K.D1), // changed fraction from \frac{abs( 1+2\right| }{1} to abs(1+2\right|/1
          T(@"1+2/1", K.D1, K.Plus, K.D2, K.Slash, K.D1), // changed fraction from \frac{1}{2}\times \frac{1}{1} to 1+2/1
          T(@"1-2/1", K.D1, K.Minus, K.D2, K.Slash, K.D1), // changed fraction from 1-\frac{2}{1} to 1-2/1
          T(@"1*2/1", K.D1, K.Multiply, K.D2, K.Slash, K.D1), // changed fraction from 1\times \frac{2}{1} to 1*2/1
          T(@"1/2", K.D1, K.Slash, K.D2), // changed fraction from 1\div \frac{2}{1} to 1/2
          T(@"1:2/1", K.D1, K.Ratio, K.D2, K.Slash, K.D1), // changed fraction from 1: \frac{2}{1} to 1:2/1
          T(@"1=2/1", K.D1, K.Equals, K.D2, K.Slash, K.D1), // changed fraction from 1=\frac{2}{1} to 1=2/1
          T(@"1\neq 2/1", K.D1, K.NotEquals, K.D2, K.Slash, K.D1), // changed fraction from 1\neq \frac{2}{1} to 1\neq 2/1
          T(@"1<2/1", K.D1, K.LessThan, K.D2, K.Slash, K.D1), // changed fraction from 1<\frac{2}{1} to 1<2/1
          T(@"1\leq 2/1", K.D1, K.LessOrEquals, K.D2, K.Slash, K.D1), // changed fraction from 1\leq \frac{2}{1} to 1\leq 2/1
          T(@"1>2/1", K.D1, K.GreaterThan, K.D2, K.Slash, K.D1), // changed fraction from 1>\frac{2}{1} to 1>2/1
        ]
    public void Genral(string latex, params K[] inputs) => Test(latex, inputs);
  }
}
