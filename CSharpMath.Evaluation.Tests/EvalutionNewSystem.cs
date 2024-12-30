using System.Linq;
using System.Text.RegularExpressions;
using Xunit;
using AngouriMath;
namespace CSharpMath.EvaluationTests {
  using System;
  using System.ComponentModel.DataAnnotations;
  using System.Runtime.InteropServices;
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

    static void PatternTest(string latex, Predicate<MathList> Pattern, bool expected) {
      var mathlist = ParseLaTeX(latex);
      var result = Pattern(mathlist);
      Assert.Equal(expected, result);
    }

    static void MixedNumberTest(string latex, bool expected) =>
      PatternTest(latex, PatternHandler.HasMixedNumber, expected);
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
          T(@"1/(2*1)", K.D1, K.Slash, K.D2, K.Multiply, K.D1),
          T(@"1/2*1/1", K.D1, K.Slash, K.D2, K.Right, K.D1, K.Slash, K.D1),
          T(@"1/(2*2/1)", K.D1, K.Slash, K.D2, K.Multiply, K.D2, K.Slash, K.D1),

          T(@"(2/1)^(1/2)", K.SquareRoot, K.LeftRoundBracket, K.D2, K.Slash, K.D1, K.RightRoundBracket),
          T(@"1/(1*1)", K.D1, K.Slash, K.D1, K.LeftRoundBracket, K.D1, K.RightRoundBracket),
          T(@"log(10,1/1)", K.LogarithmWithBase, K.LeftRoundBracket, K.Slash, K.D1, K.RightRoundBracket),

          T(@"[0,+oo]", K.LeftSquareBracket, K.D0, K.Comma, K.Infinity, K.RightSquareBracket),
          T(@"(1+2)/1", K.LeftRoundBracket, K.D1, K.Plus, K.D2, K.RightRoundBracket, K.Slash, K.D1),
          T(@"abs((1+2)/1)", K.Absolute, K.LeftRoundBracket, K.D1, K.Plus, K.D2, K.RightRoundBracket, K.Slash, K.D1),
          T(@"1+2/1", K.D1, K.Plus, K.D2, K.Slash, K.D1),
          T(@"1-2/1", K.D1, K.Minus, K.D2, K.Slash, K.D1),
          T(@"1*2/1", K.D1, K.Multiply, K.D2, K.Slash, K.D1),
          T(@"1/2", K.D1, K.Slash, K.D2),
          T(@"5*6=x", K.D5, K.Multiply, K.D6, K.Equals, K.SmallX),
          T(@"1/(2/1)", K.D1, K.Ratio, K.D2, K.Slash, K.D1),
          T(@"1=2/1", K.D1, K.Equals, K.D2, K.Slash, K.D1),
          T(@"1!=2/1", K.D1, K.NotEquals, K.D2, K.Slash, K.D1),
          T(@"1<2/1", K.D1, K.LessThan, K.D2, K.Slash, K.D1),
          T(@"1<=2/1", K.D1, K.LessOrEquals, K.D2, K.Slash, K.D1),
          T(@"1>2/1", K.D1, K.GreaterThan, K.D2, K.Slash, K.D1),
          // add some test to plusminus
          T(@"1+2/1or1-2/1", K.D1, K.PlusMinus, K.D2, K.Slash, K.D1),
          T(@"56=X+5or56=X-5", K.D5, K.D6, K.Equals, K.X, K.PlusMinus, K.D5),
        T(@"x^4-2^2+3*x", K.SmallX, K.Power, K.D4, K.Right, K.Minus, K.D2, K.Power, K.D2, K.Right, K.Plus, K.D3, K.SmallX),
             T(@"5/5/(4/4)", K.D5, K.Slash, K.D5, K.Right, K.Divide, K.D4, K.Slash, K.D4),
             T(@"1/4/(1/3)", K.D1, K.Slash, K.D4, K.Right, K.Divide, K.D1, K.Slash, K.D3),
             T(@"1/4/(1/4)/(1/3)", K.D1, K.Slash, K.D4, K.Right, K.Divide, K.D1, K.Slash, K.D4, K.Right, K.Divide, K.D1, K.Slash, K.D3),



        ]

    public void Genral(string latex, params K[] inputs) => Test(latex, inputs);


    [Theory]
    [T(@"3\frac{1}{2}", true)]
    [T(@"7\frac{3}{4}", true)]
    [T(@"5\frac{2}{5} + 2", true)]
    [T(@"2 + \frac{1}{3}", false)]
    [T(@"\frac{4}{5}", false)]
    [T(@"6", false)]
    [T(@"\frac{5}{2}", false)]
    [T(@"3 \cdot \frac{1}{4}", false)]
    [T(@"8\frac{1}{2} y", true)]
    [T(@"x + 2\frac{3}{5}", true)]
    [T(@"\frac{2}{3} x", false)]
    [T(@"4\frac{1}{2} + 5\frac{1}{2}", true)]
    [T(@"\frac{1}{2} + \frac{3}{4}", false)]
    [T(@"\frac{\frac{1}{2}}{2} + 3", false)]
    [T(@"\frac{2\frac{1}{2}}{2} + 3", true)]

    public void MixedNumberTests(string latex, bool expected) => MixedNumberTest(latex, expected);

    [Theory]
    [T(@"3\frac{1}{2}", @"((3)+((1)/(2)))")]
    [T(@"7\frac{3}{4}", @"((7)+((3)/(4)))")]
    [T(@"5\frac{2}{5} + 2", @"((5)+((2)/(5)))+2")]
    [T(@"2 + \frac{1}{3}", @"2+((1)/(3))")]
    [T(@"\frac{4}{5}", @"((4)/(5))")]
    [T(@"6", "6")]
    [T(@"\frac{5}{2}", @"((5)/(2))")]
    [T(@"3 \cdot \frac{1}{4}", @"3*((1)/(4))")]
    [T(@"8\frac{1}{2} y", @"((8)+((1)/(2)))y")]
    [T(@"x + 2\frac{3}{5}", @"x+((2)+((3)/(5)))")]
    [T(@"\frac{2}{3} x", @"((2)/(3))x")]
    [T(@"4\frac{1}{2} + 5\frac{1}{2}", @"((4)+((1)/(2)))+((5)+((1)/(2)))")]
    [T(@"\frac{1}{2} + \frac{3}{4}", @"((1)/(2))+((3)/(4))")]
    //edge
    [T(@"10\frac{2}{3}x", @"((10)+((2)/(3)))x")]
    [T(@"12\frac{5}{6} + \frac{7}{8}", @"((12)+((5)/(6)))+((7)/(8))")]
    [T(@"\frac{9}{4}", @"((9)/(4))")]
    [T(@"x\frac{1}{2}", @"x((1)/(2))")]

    public void MixedNumberConversionTests(string latex, string expected) {
      // Set HandleMixedNumber to true to enable mixed number handling
      MathListConverator.HandleMixedNumber = true;

      // Parse the LaTeX input to a MathList
      var mathlist = ParseLaTeX(latex);

      // Convert the MathList to a mathematical string
      var result = MathListConverator.ConvertToMathString(mathlist);

      // Remove any whitespace for comparison
      result = Regex.Replace(result, @"\s+", "");
      expected = Regex.Replace(expected, @"\s+", "");

      // Assert that the result matches the expected string
      Assert.Equal(expected, result);
    }
  }
}
