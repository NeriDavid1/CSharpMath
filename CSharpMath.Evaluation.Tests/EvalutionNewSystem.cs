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
      var latexs = keyboard.LaTeX;
      var ConvertToMathFormat = ParseMath(keyboard.MathList);
      var result = ConvertToMathFormat.Evaled.Stringize();
      Assert.Equal(angourimathFormat, ConvertToMathFormat);
    }
    internal static MathList ParseLaTeX(string latex) =>
      LaTeXParser.MathListFromLaTeX(latex).Match(list => list, e => throw new Xunit.Sdk.XunitException(e));
    static Entity ParseMath(string latex) =>
      Evaluation.ParseExpression(latex);
        static Entity ParseMath(MathList mathlist) =>
      Evaluation.ParseExpression(mathlist);
    void Test(string input, string converted, string? result) {
      void Test(string input) {
        var math = ParseMath(input);
        Assert.NotNull(math);
        Assert.Equal(Evaluation.ConvertToMathString(converted)!.ToEntity().Simplify(), math.Simplify());


        // Ensure that the converted entity is valid by simplifying it
        //    if (result != null)
        // Assert.Equal(result, math.Simplify().Stringize());
      }
      Test(input);
      // This regex balances (, [ and \{ with ), ] and \} into one group, then inserts \left and \right
      // But does not do this for \sqrt's [ and ]
      Test(Regex.Replace(input, @"(?<!\\sqrt)(\(|\[|\\\{)((?:(?!\(|\[|\\\\{|\)|\]|\\\\}).|(?<open>\(|\[|\\\\{)|(?<-open>\)|\]|\\\\}))+(?(open)(?!)))(\)|\]|\\\\})", @"\left$1$2\right$3"));
    }

    [
      Theory,
      T(@"1/1", K.D1, K.Slash, K.D1),
      T(@"2/1", K.D2, K.Slash, K.D1),
      T(@"\sqrt[3]{27}", K.CubeRoot, K.D2, K.D7),
      T(@"\frac{12}{1}", K.D1, K.D2, K.Slash, K.D1),
      T(@"\frac{a}{1}", K.SmallA, K.Slash, K.D1),
      T(@"\frac{XyZ}{1}", K.X, K.SmallY, K.Z, K.Slash, K.D1),
      T(@"\frac{\alpha \beta c}{1}", K.SmallAlpha, K.SmallBeta, K.SmallC, K.Slash, K.D1),
      T(@"\frac{\infty }{1}", K.Infinity, K.Slash, K.D1),
      T(@"\frac{\sin ^2\theta }{1}", K.Sine, K.Power, K.D2, K.Right, K.SmallTheta, K.Slash, K.D1),
      T(@"\frac{\log _3\pi }{1}", K.LogarithmWithBase, K.D3, K.Right, K.SmallPi, K.Slash, K.D1),

      T(@"\frac{1}{\frac{1}{1}}", K.Slash, K.Slash, K.D1),
      T(@"\frac{1}{\frac{2}{1}}", K.Slash, K.D2, K.Slash, K.D1),
      T(@"\frac{1}{2}\times \frac{1}{1}", K.Slash, K.D2, K.Right, K.Slash, K.D1),
      T(@"\frac{1}{2}\times \frac{1}{1}", K.Slash, K.D2, K.Right, K.D1, K.Slash, K.D1),
      T(@"\frac{1}{2}\times \frac{2}{1}", K.Slash, K.D2, K.Right, K.D2, K.Slash, K.D1),

      T(@"\sqrt{\frac{2}{1}}", K.SquareRoot, K.D2, K.Slash, K.D1),
      T(@"\frac{\sqrt{2}}{1}", K.SquareRoot, K.D2, K.Right, K.Slash, K.D1),
      T(@"\sqrt[\frac{1}{1}]{1}", K.NthRoot, K.Slash, K.D1),
      T(@"\log _{\frac{1}{1}}", K.LogarithmWithBase, K.Slash, K.D1),

      T(@"(\frac{[\} }{1}", K.LeftRoundBracket, K.LeftSquareBracket, K.RightCurlyBracket, K.Slash, K.D1),
      T(@"\{ \frac{[0,\infty )}{1}",
        K.LeftCurlyBracket, K.LeftSquareBracket, K.D0, K.Comma, K.Infinity, K.RightRoundBracket, K.Slash, K.D1),
      T(@"\frac{(1+2)}{1}", K.LeftRoundBracket, K.D1, K.Plus, K.D2, K.RightRoundBracket, K.Slash, K.D1),
      T(@"\frac{\left( 1+2\right) }{1}", K.BothRoundBrackets, K.D1, K.Plus, K.D2, K.Right, K.Slash, K.D1),
      T(@"|1+\frac{2|}{1}", K.VerticalBar, K.D1, K.Plus, K.D2, K.VerticalBar, K.Slash, K.D1),
      T(@"\frac{\left| 1+2\right| }{1}", K.Absolute, K.D1, K.Plus, K.D2, K.Right, K.Slash, K.D1),
      T(@"1+\frac{2}{1}", K.D1, K.Plus, K.D2, K.Slash, K.D1),
      T(@"1-\frac{2}{1}", K.D1, K.Minus, K.D2, K.Slash, K.D1),
      T(@"1\times \frac{2}{1}", K.D1, K.Multiply, K.D2, K.Slash, K.D1),
      T(@"1\div \frac{2}{1}", K.D1, K.Divide, K.D2, K.Slash, K.D1),
      T(@"1:\frac{2}{1}", K.D1, K.Ratio, K.D2, K.Slash, K.D1),
      T(@"1=\frac{2}{1}", K.D1, K.Equals, K.D2, K.Slash, K.D1),
      T(@"1\neq \frac{2}{1}", K.D1, K.NotEquals, K.D2, K.Slash, K.D1),
      T(@"1<\frac{2}{1}", K.D1, K.LessThan, K.D2, K.Slash, K.D1),
      T(@"1\leq \frac{2}{1}", K.D1, K.LessOrEquals, K.D2, K.Slash, K.D1),
      T(@"1>\frac{2}{1}", K.D1, K.GreaterThan, K.D2, K.Slash, K.D1),
      T(@"1\geq \frac{2}{1}", K.D1, K.GreaterOrEquals, K.D2, K.Slash, K.D1),
      T(@"\frac{1}{\frac{2}{1}}", K.D1, K.Slash, K.D2, K.Slash, K.D1),
      T(@"\sqrt{x+\frac{2}{1}}", K.SquareRoot, K.SmallX, K.Plus, K.D2, K.Slash, K.D1),
      T(@"\frac{\left( x+\sqrt{2}\right) }{1}", K.BothRoundBrackets, K.SmallX, K.Plus, K.SquareRoot, K.D2, K.Right, K.Right, K.Slash, K.D1),
      T(@"\frac{(x+\sqrt{2})}{1}", K.LeftRoundBracket, K.SmallX, K.Plus, K.SquareRoot, K.D2, K.Right, K.RightRoundBracket, K.Slash, K.D1),

      T(@"\frac{\int }{1}", K.Integral, K.Slash, K.D1),
      T(@"\frac{1\int }{1}", K.D1, K.Integral, K.Slash, K.D1),
      T(@"\frac{\int 1}{1}", K.Integral, K.D1, K.Slash, K.D1),
      T(@"+\frac{\prod }{1}", K.Plus, K.Product, K.Slash, K.D1),
    ]
    public void Slash(string latex, params K[] inputs) => Test(latex, inputs);
  }
}
