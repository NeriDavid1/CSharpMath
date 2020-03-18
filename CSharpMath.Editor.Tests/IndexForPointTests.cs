using System.Drawing;
using CSharpMath.Atom;
using CSharpMath.Tests.FrontEnd;
using Xunit;

namespace CSharpMath.Editor.Tests {
  // Use the "CSharpMath.Editor Test Checker" project in the _Utils folder to visualize the test cases
  using SubIndex = MathListSubIndexType;
  public class IndexForPointTests {
    public class TestData : TheoryData {
      // Format of test data
      public void Add((double x, double y) point,
        int index, params (SubIndex subType, int subIndex)[] subIndexRecursive) {
        switch (subIndexRecursive.Length) {
          case 0:
            var mathListIndex = MathListIndex.Level0Index(index);
            goto default;
          case var _:
            mathListIndex = MathListIndex.Level0Index(
              subIndexRecursive[^1].subIndex);
            for (var i = subIndexRecursive.Length - 2; i >= 0; i--)
              mathListIndex = MathListIndex.IndexAtLocation(
                subIndexRecursive[i].subIndex,
                subIndexRecursive[i + 1].subType,
                mathListIndex);
            mathListIndex = MathListIndex.IndexAtLocation(index,
              subIndexRecursive[0].subType, mathListIndex);
            goto default;
          default:
            AddRow(new PointF((float)point.x, (float)point.y), mathListIndex);
            break;
        }
      }
    }
    public static Structures.Result<Display.Displays.ListDisplay<TestFont, char>>
      CreateDisplay(string latex) =>
      LaTeXBuilder.TryMathListFromLaTeX(latex).Bind(mathList =>
        Display.Typesetter.CreateLine(
          mathList, new TestFont(20), TestTypesettingContexts.Instance, LineStyle.Display));
    void Test(string latex, PointF point, MathListIndex expected) =>
      CreateDisplay(latex).Match(
        display => Assert.Equal(expected,
          display.IndexForPoint(TestTypesettingContexts.Instance, point)),
        s => throw new Xunit.Sdk.XunitException(s)
      );

    public static TestData FractionData =>
      new TestData {
        { (-10, -20), 0 },
        { (-10, 0), 0 },
        { (-10, 8), 0 },
        { (-10, 40), 0 },
        { (-2.5, -20), 0 },
        { (-2.5, 0), 0 },
        { (-2.5, 8), 0 },
        { (-2.5, 40), 0 },
        { (-1, -20), 0, (SubIndex.Denominator, 0) },
        { (-1, 0), 0, (SubIndex.Denominator, 0) },
        { (-1, 8), 0, (SubIndex.Numerator, 0) },
        { (-1, 40), 0, (SubIndex.Numerator, 0) },
        { (3, -20), 0, (SubIndex.Denominator, 0) },
        { (3, 0), 0, (SubIndex.Denominator, 0) },
        { (3, 8), 0, (SubIndex.Numerator, 0) },
        { (3, 40), 0, (SubIndex.Numerator, 0) },
        { (7, -20), 0, (SubIndex.Denominator, 1) },
        { (7, 0), 0, (SubIndex.Denominator, 1) },
        { (7, 8), 0, (SubIndex.Numerator, 1) },
        { (7, 40), 0, (SubIndex.Numerator, 1) },
        { (11, -20), 1 },  // because it is below the height of the fraction
        { (11, 0), 0, (SubIndex.Denominator, 1) },
        { (11, 8), 0, (SubIndex.Numerator, 1) },
        { (11, 40), 0, (SubIndex.Numerator, 1) },
        { (12.5, -20), 1 },
        { (12.5, 0), 1 },
        { (12.5, 8), 1 },
        { (12.5, 40), 1 },
        { (20, -20), 1 },
        { (20, 0), 1 },
        { (20, 8), 1 },
        { (20, 40), 1 },
       };
    [Theory, MemberData(nameof(FractionData))]
    public void Fraction(PointF point, MathListIndex expected) => Test(@"\frac32", point, expected);

    public static TestData RegularData =>
      new TestData {
        { (-10, -20), 0 },
        { (-10, 0), 0 },
        { (-10, 8), 0 },
        { (-10, 40), 0 },
        { (0, -20), 0 },
        { (0, 0), 0 },
        { (0, 8), 0 },
        { (0, 40), 0 },
        { (10, -20), 1 },
        { (10, 0), 1 },
        { (10, 8), 1 },
        { (10, 40), 1 },
        { (15, -20), 1 },
        { (15, 0), 1 },
        { (15, 8), 1 },
        { (15, 40), 1 },
        { (25, -20), 2 },
        { (25, 0), 2 },
        { (25, 8), 2 },
        { (25, 40), 2 },
        { (35, -20), 3 },
        { (35, 0), 3 },
        { (35, 8), 3 },
        { (35, 40), 3 },
        { (45, -20), 3 },
        { (45, 0), 3 },
        { (45, 8), 3 },
        { (45, 40), 3 },
        { (55, -20), 3 },
        { (55, 0), 3 },
        { (55, 8), 3 },
        { (55, 40), 3 },
      };
    [Theory, MemberData(nameof(RegularData))]
    public void Regular(PointF point, MathListIndex expected) => Test(@"4+2", point, expected);

    public static TestData RegularPlusFractionData =>
      new TestData {
        { (26, -20), 2 },
        { (26, 0), 2 },
        { (26, 8), 2 },
        { (26, 40), 2 },
        { (28, -20), 2, (SubIndex.Denominator, 0) },
        { (28, 0), 2, (SubIndex.Denominator, 0) },
        { (28, 8), 2, (SubIndex.Numerator, 0) },
        { (28, 40), 2, (SubIndex.Numerator, 0) },
        { (33, -20), 2, (SubIndex.Denominator, 0) },
        { (33, 0), 2, (SubIndex.Denominator, 0) },
        { (33, 8), 2, (SubIndex.Numerator, 0) },
        { (33, 40), 2, (SubIndex.Numerator, 0) },
        { (35, -20), 2, (SubIndex.Denominator, 1) },
        { (35, 0), 2, (SubIndex.Denominator, 1) },
        { (35, 8), 2, (SubIndex.Numerator, 1) },
        { (35, 40), 2, (SubIndex.Numerator, 1) },
      };
    [Theory, MemberData(nameof(RegularPlusFractionData))]
    public void RegularPlusFraction(PointF point, MathListIndex expected) =>
      Test(@"1+\frac{3}{2}", point, expected);

    public static TestData FractionPlusRegularData =>
      new TestData {
        { (9, -20), 0, (SubIndex.Denominator, 1) },
        { (9, 0), 0, (SubIndex.Denominator, 1) },
        { (9, 8), 0, (SubIndex.Numerator, 1) },
        { (9, 40), 0, (SubIndex.Numerator, 1) },
        { (11, -20), 0, (SubIndex.Denominator, 1) },
        { (11, 0), 0, (SubIndex.Denominator, 1) },
        { (11, 8), 0, (SubIndex.Numerator, 1) },
        { (11, 40), 0, (SubIndex.Numerator, 1) },
        { (13, -20), 1 },
        { (13, 0), 1 },
        { (13, 8), 1 },
        { (13, 40), 1 },
        { (15, -20), 1 },
        { (15, 0), 1 },
        { (15, 8), 1 },
        { (15, 40), 1 },
      };
    [Theory, MemberData(nameof(FractionPlusRegularData))]
    public void FractionPlusRegular(PointF point, MathListIndex expected) =>
      Test(@"\frac32+1", point, expected);

    public static TestData RadicalData =>
      new TestData {
        { (-3, -20), 0 },
        { (-3, 0), 0 },
        { (-3, 8), 0 },
        { (-3, 40), 0 },
        { (0, -20), 0 },
        { (0, 0), 0 },
        { (0, 8), 0 },
        { (0, 40), 0, (SubIndex.Radicand, 0) },
        { (4, -20), 0 },
        { (4, 0), 0 },
        { (4, 8), 0, (SubIndex.Radicand, 0) },
        { (4, 40), 0, (SubIndex.Radicand, 0)  },
        { (9, -20), 0, (SubIndex.Radicand, 0) },
        { (9, 0), 0, (SubIndex.Radicand, 0) },
        { (9, 8), 0, (SubIndex.Radicand, 0) },
        { (9, 40), 0, (SubIndex.Radicand, 0) },
        { (11, -20), 0, (SubIndex.Radicand, 0) },
        { (11, 0), 0, (SubIndex.Radicand, 0) },
        { (11, 8), 0, (SubIndex.Radicand, 0) },
        { (11, 40), 0, (SubIndex.Radicand, 0) },
        { (13, -20), 0, (SubIndex.Radicand, 0) },
        { (13, 0), 0, (SubIndex.Radicand, 0) },
        { (13, 8), 0, (SubIndex.Radicand, 0) },
        { (13, 40), 0, (SubIndex.Radicand, 0) },
        { (15, -20), 0, (SubIndex.Radicand, 1) },
        { (15, 0), 0, (SubIndex.Radicand, 1) },
        { (15, 8), 0, (SubIndex.Radicand, 1) },
        { (15, 40), 0, (SubIndex.Radicand, 1) },
        { (17, -20), 0, (SubIndex.Radicand, 1) },
        { (17, 0), 0, (SubIndex.Radicand, 1) },
        { (17, 8), 0, (SubIndex.Radicand, 1) },
        { (17, 40), 0, (SubIndex.Radicand, 1) },
        { (19, -20), 1 },
        { (19, 0), 0, (SubIndex.Radicand, 1) },
        { (19, 8), 0, (SubIndex.Radicand, 1) },
        { (19, 40), 0, (SubIndex.Radicand, 1) },
        { (21, -20), 1 },
        { (21, 0), 0, (SubIndex.Radicand, 1) },
        { (21, 8), 0, (SubIndex.Radicand, 1) },
        { (21, 40), 0, (SubIndex.Radicand, 1) },
        { (23, -20), 1 },
        { (23, 0), 1 },
        { (23, 8), 1 },
        { (23, 40), 1 },
        { (26, -20), 1 },
        { (26, 0), 1 },
        { (26, 8), 1 },
        { (26, 40), 1 },
        { (35, -20), 1 },
        { (35, 0), 1 },
        { (35, 8), 1 },
        { (35, 40), 1 },
      };
    [Theory, MemberData(nameof(RadicalData))]
    public void Radical(PointF point, MathListIndex expected) =>
      Test(@"\sqrt2", point, expected);
    public static TestData RadicalDegreeData =>
      new TestData {
        { (-3, -20), 0 },
        { (-3, 0), 0 },
        { (-3, 8), 0 },
        { (-3, 40), 0 },
        { (0, -20), 0, (SubIndex.Radicand, 0) },
        { (0, 0), 0, (SubIndex.Radicand, 0) },
        { (0, 8), 0, (SubIndex.Degree, 0) },
        { (0, 40), 0, (SubIndex.Degree, 0) },
        { (4, -20), 0, (SubIndex.Radicand, 0) },
        { (4, 0), 0, (SubIndex.Radicand, 0) },
        { (4, 8), 0, (SubIndex.Degree, 0) },
        { (4, 40), 0, (SubIndex.Degree, 0)  },
        { (9, -20), 0, (SubIndex.Radicand, 0) },
        { (9, 0), 0, (SubIndex.Radicand, 0) },
        { (9, 8), 0, (SubIndex.Degree, 0) },
        { (9, 40), 0, (SubIndex.Degree, 0) },
        { (11, -20), 0, (SubIndex.Radicand, 0) },
        { (11, 0), 0, (SubIndex.Radicand, 0) },
        { (11, 8), 0, (SubIndex.Radicand, 0) },
        { (11, 40), 0, (SubIndex.Degree, 1) },
        { (13, -20), 0, (SubIndex.Radicand, 0) },
        { (13, 0), 0, (SubIndex.Radicand, 0) },
        { (13, 8), 0, (SubIndex.Radicand, 0) },
        { (13, 40), 0, (SubIndex.Degree, 1) },
        { (15, -20), 0, (SubIndex.Radicand, 0) },
        { (15, 0), 0, (SubIndex.Radicand, 0) },
        { (15, 8), 0, (SubIndex.Radicand, 0) },
        { (15, 40), 0, (SubIndex.Degree, 1) },
        { (17, -20), 0, (SubIndex.Radicand, 1) },
        { (17, 0), 0, (SubIndex.Radicand, 1) },
        { (17, 8), 0, (SubIndex.Radicand, 1) },
        { (17, 40), 0, (SubIndex.Radicand, 1) },
        { (19, -20), 0, (SubIndex.Radicand, 1) },
        { (19, 0), 0, (SubIndex.Radicand, 1) },
        { (19, 8), 0, (SubIndex.Radicand, 1) },
        { (19, 40), 0, (SubIndex.Radicand, 1) },
        { (21, -20), 1 },
        { (21, 0), 0, (SubIndex.Radicand, 1) },
        { (21, 8), 0, (SubIndex.Radicand, 1) },
        { (21, 40), 0, (SubIndex.Radicand, 1) },
        { (23, -20), 1 },
        { (23, 0), 0, (SubIndex.Radicand, 1) },
        { (23, 8), 0, (SubIndex.Radicand, 1) },
        { (23, 40), 0, (SubIndex.Radicand, 1) },
        { (26, -20), 1 },
        { (26, 0), 1 },
        { (26, 8), 1 },
        { (26, 40), 1 },
        { (35, -20), 1 },
        { (35, 0), 1 },
        { (35, 8), 1 },
        { (35, 40), 1 },
      };
    [Theory, MemberData(nameof(RadicalDegreeData))]
    public void RadicalDegree(PointF point, MathListIndex expected) =>
      Test(@"\sqrt[3]2", point, expected);
    public static TestData ExponentData =>
      new TestData {
        { (-10, -20), 0 },
        { (-10, 0), 0 },
        { (-10, 8), 0 },
        { (-10, 40), 0 },
        { (0, -20), 0 },
        { (0, 0), 0 },
        { (0, 8), 0 },
        { (0, 40), 0 },
        { (9, -20), 0, (SubIndex.BetweenBaseAndScripts, 1) },
        { (9, 0), 0, (SubIndex.BetweenBaseAndScripts, 1) },
        { (9, 8), 0, (SubIndex.BetweenBaseAndScripts, 1) },
        // The superscript is closer than the nucleus (and the touch boundaries overlap)
        { (9, 40), 0, (SubIndex.Superscript, 0) },
        { (10, -20), 0, (SubIndex.BetweenBaseAndScripts, 1) },
        { (10, 0), 0, (SubIndex.BetweenBaseAndScripts, 1) },
        // The nucleus is closer and the touch boundaries overlap
        { (10, 8), 0, (SubIndex.BetweenBaseAndScripts, 1) },
        { (10, 40), 0, (SubIndex.Superscript, 0) },
        { (11, -20), 0, (SubIndex.BetweenBaseAndScripts, 1) },
        { (11, 0), 0, (SubIndex.BetweenBaseAndScripts, 1) },
        { (11, 8), 0, (SubIndex.Superscript, 0) },
        { (11, 40), 0, (SubIndex.Superscript, 0) },
        { (17, -20), 1 },
        { (17, 0), 1 },
        { (17, 8), 0, (SubIndex.Superscript, 1) },
        { (17, 40), 0, (SubIndex.Superscript, 1) },
        { (30, -20), 1 },
        { (30, 0), 1 },
        { (30, 8), 1 },
        { (30, 40), 1 },
      };
    [Theory, MemberData(nameof(ExponentData))]
    public void Exponent(PointF point, MathListIndex expected) => Test("2^3", point, expected);

    public static TestData Exponent2Data =>
      new TestData {
        { (55, 0), 1 },
        { (55, 20), 1 },
        { (55, 40), 1 },
    };
    [Theory, MemberData(nameof(Exponent2Data))] // https://github.com/verybadcat/CSharpMath/issues/49
    public void Exponent2(PointF point, MathListIndex expected) => Test("2^{x+y-4}", point, expected);

    public static TestData Issue46Data =>
      new TestData {
        { (50, 10), 4 },
        { (90, 0), 5 },
        { (90, 20), 5 },
        { (90, 40), 5 },
      };
    [Theory, MemberData(nameof(Issue46Data))] // https://github.com/verybadcat/CSharpMath/issues/46
    public void Issue46(PointF point, MathListIndex expected) => Test("2+x+x^y", point, expected);

    public static TestData ComplexData =>
      new TestData {
        { (-10, -20), 0 },
        { (-10, 0), 0 },
        { (-10, 8), 0 },
        { (-10, 40), 0 },
        // \frac a\frac bc
        { (0, -20), 0, (SubIndex.Denominator, 0), (SubIndex.Denominator, 0) },
        { (0, 0), 0, (SubIndex.Denominator, 0), (SubIndex.Numerator, 0) },
        { (0, 8), 0, (SubIndex.Numerator, 0) },
        { (0, 40), 0, (SubIndex.Numerator, 0) },
        { (6, -20), 0, (SubIndex.Denominator, 0), (SubIndex.Denominator, 1) },
        { (6, 0), 0, (SubIndex.Denominator, 0), (SubIndex.Numerator, 1) },
        { (6, 8), 0, (SubIndex.Numerator, 1) },
        { (6, 40), 0, (SubIndex.Numerator, 1) },
        { (8, -20), 0, (SubIndex.Denominator, 1) },
        { (8, 0), 0, (SubIndex.Denominator, 0), (SubIndex.Numerator, 1) },
        { (8, 8), 0, (SubIndex.Numerator, 1) },
        { (8, 40), 0, (SubIndex.Numerator, 1) },
        { (10, -20), 0, (SubIndex.Denominator, 1) },
        { (10, 0), 0, (SubIndex.Denominator, 0), (SubIndex.Numerator, 1) },
        { (10, 8), 0, (SubIndex.Numerator, 1) },
        { (10, 40), 0, (SubIndex.Numerator, 1) },
        { (11, -20), 0, (SubIndex.Denominator, 1) },
        { (11, 0), 0, (SubIndex.Denominator, 1) },
        { (11, 8), 0, (SubIndex.Numerator, 1) },
        { (11, 40), 0, (SubIndex.Numerator, 1) },
        // \frac\frac123
        { (17, -20), 1, (SubIndex.Denominator, 0) },
        { (17, 0), 1, (SubIndex.Denominator, 0) },
        { (17, 8), 1, (SubIndex.Numerator, 0), (SubIndex.Denominator, 0) },
        { (17, 40), 1, (SubIndex.Numerator, 0), (SubIndex.Numerator, 0) },
        { (23, -20), 1, (SubIndex.Denominator, 1) },
        { (23, 0), 1, (SubIndex.Denominator, 1) },
        { (23, 8), 1, (SubIndex.Numerator, 1) },
        { (23, 40), 1, (SubIndex.Numerator, 0), (SubIndex.Numerator, 1) },
        // \sqrt d^e
        { (27, -20), 2, (SubIndex.Radicand, 0) },
        { (27, 0), 2, (SubIndex.Radicand, 0) },
        { (27, 8), 2, (SubIndex.Radicand, 0) },
        { (27, 40), 2, (SubIndex.Radicand, 0) },
        { (40, -20), 2, (SubIndex.Radicand, 0) },
        { (40, 0), 2, (SubIndex.Radicand, 0) },
        { (40, 8), 2, (SubIndex.Radicand, 0) },
        { (40, 40), 2, (SubIndex.Radicand, 0) },
        { (45, -20), 2, (SubIndex.Radicand, 1) },
        { (45, 0), 2, (SubIndex.Radicand, 1) },
        { (45, 8), 2, (SubIndex.Radicand, 1) },
        { (45, 40), 2, (SubIndex.Superscript, 0) },
        { (50, -20), 2, (SubIndex.Superscript, 0) },
        { (50, 0), 2, (SubIndex.Superscript, 0) },
        { (50, 8), 2, (SubIndex.Superscript, 0) },
        { (50, 40), 2, (SubIndex.Superscript, 0) },
        { (55, -20), 2, (SubIndex.Superscript, 1) },
        { (55, 0), 2, (SubIndex.Superscript, 1) },
        { (55, 8), 2, (SubIndex.Superscript, 1) },
        { (55, 40), 2, (SubIndex.Superscript, 1) },
        // \sqrt[5]6
        { (60, -20), 3, (SubIndex.Radicand, 0) },
        { (60, 0), 3, (SubIndex.Radicand, 0) },
        { (60, 8), 3, (SubIndex.Degree, 0) },
        { (60, 40), 3, (SubIndex.Degree, 0) },
        { (67, -20), 3, (SubIndex.Radicand, 0) },
        { (67, 0), 3, (SubIndex.Radicand, 0) },
        { (67, 8), 3, (SubIndex.Degree, 0) },
        { (67, 40), 3, (SubIndex.Degree, 0) },
        { (73, -20), 3, (SubIndex.Radicand, 0) },
        { (73, 0), 3, (SubIndex.Radicand, 0) },
        { (73, 8), 3, (SubIndex.Radicand, 0) },
        { (73, 40), 3, (SubIndex.Degree, 1) },
        { (80, -20), 3, (SubIndex.Radicand, 1) },
        { (80, 0), 3, (SubIndex.Radicand, 1) },
        { (80, 8), 3, (SubIndex.Radicand, 1) },
        { (80, 40), 3, (SubIndex.Radicand, 1) },
        // \sqrt[f]g^{7_8}_{9^0}
        { (87, -20), 4, (SubIndex.Radicand, 0) },
        { (87, 0), 4, (SubIndex.Radicand, 0) },
        { (87, 8), 4, (SubIndex.Degree, 0) },
        { (87, 40), 4, (SubIndex.Degree, 0) },
        { (90, -20), 4, (SubIndex.Radicand, 0) },
        { (90, 0), 4, (SubIndex.Radicand, 0) },
        { (90, 8), 4, (SubIndex.Degree, 0) },
        { (90, 40), 4, (SubIndex.Degree, 0) },
        { (95, -20), 4, (SubIndex.Radicand, 0) },
        { (95, 0), 4, (SubIndex.Radicand, 0) },
        { (95, 8), 4, (SubIndex.Degree, 1) },
        { (95, 40), 4, (SubIndex.Degree, 1) },
        { (100, -20), 4, (SubIndex.Radicand, 0) },
        { (100, 0), 4, (SubIndex.Radicand, 0) },
        { (100, 8), 4, (SubIndex.Radicand, 0) },
        { (100, 40), 4, (SubIndex.Degree, 1) },
        { (105, -20), 4, (SubIndex.Subscript, 0) },
        { (105, 0), 4, (SubIndex.Radicand, 1) },
        { (105, 8), 4, (SubIndex.Radicand, 1) },
        { (105, 40), 4, (SubIndex.Superscript, 0) },
        { (110, -20), 4, (SubIndex.Subscript, 0) },
        { (110, 0), 4, (SubIndex.Subscript, 0) },
        { (110, 8), 4, (SubIndex.Subscript, 0) },
        { (118, 20), 4, (SubIndex.Superscript, 0), (SubIndex.Subscript, 1) },
        { (118, 32), 4, (SubIndex.Superscript, 0), (SubIndex.Subscript, 1) },
        { (110, 40), 4, (SubIndex.Superscript, 0) },
        { (115, -20), 4, (SubIndex.Subscript, 0), (SubIndex.BetweenBaseAndScripts, 1) },
        { (115, 0), 4, (SubIndex.Subscript, 0), (SubIndex.Superscript, 0) },
        { (115, 8), 4, (SubIndex.Subscript, 0), (SubIndex.Superscript, 0) },
        { (118, 20), 4, (SubIndex.Superscript, 0), (SubIndex.Subscript, 1) },
        { (118, 32), 4, (SubIndex.Superscript, 0), (SubIndex.Subscript, 1) },
        { (115, 40), 4, (SubIndex.Superscript, 0), (SubIndex.BetweenBaseAndScripts, 1) },
        { (118, -20), 4, (SubIndex.Subscript, 1) },
        { (118, 0), 4, (SubIndex.Subscript, 0), (SubIndex.Superscript, 1) },
        { (118, 8), 4, (SubIndex.Subscript, 0), (SubIndex.Superscript, 1) },
        { (118, 20), 4, (SubIndex.Superscript, 0), (SubIndex.Subscript, 1) },
        { (118, 32), 4, (SubIndex.Superscript, 0), (SubIndex.Subscript, 1) },
        { (118, 40), 4, (SubIndex.Superscript, 0), (SubIndex.Subscript, 1) },
        { (120, -20), 4, (SubIndex.Subscript, 1) },
        { (120, 0), 4, (SubIndex.Subscript, 0), (SubIndex.Superscript, 1) },
        { (120, 8), 4, (SubIndex.Subscript, 0), (SubIndex.Superscript, 1) },
        { (120, 20), 4, (SubIndex.Superscript, 0), (SubIndex.Subscript, 1) },
        { (120, 32), 4, (SubIndex.Superscript, 0), (SubIndex.Subscript, 1) },
        { (120, 40), 4, (SubIndex.Superscript, 0), (SubIndex.Subscript, 1) },
        { (122, -20), 5 },
        { (122, 0), 5 },
        { (122, 8), 5 },
        { (122, 20), 5 },
        { (122, 32), 5 },
        { (122, 40), 5 },
        { (150, -20), 5 },
        { (150, 0), 5 },
        { (150, 8), 5 },
        { (150, 20), 5 },
        { (150, 32), 5 },
        { (150, 40), 5 },
      };
    [Theory, MemberData(nameof(ComplexData))]
    public void Complex(PointF point, MathListIndex expected) => 
      Test(@"\frac a\frac bc\frac\frac123\sqrt d^e\sqrt[5]6\sqrt[f]g^{7_8}_{9^0}", point, expected);
    public static TestData SineData =>
      new TestData {
        { (-10, 10), 0 },
        { (1, 10), 0 },
        { (9, 10), 0 },
        { (16, 10), 1 },
        { (20, 10), 1 },
        { (28, 10), 1 },
        { (35, 10), 1 },
        { (42, 10), 2 },
        { (69, 10), 2 },
    };
    [Theory, MemberData(nameof(SineData))]
    public void Sine(PointF point, MathListIndex expected) => Test(@"\sin\pi", point, expected);

    public static TestData Issue64Data =>
      new TestData {
        { (0, 15), 0 },
        { (10, 10), 0, (SubIndex.BetweenBaseAndScripts, 1) },
        { (10, 15), 0, (SubIndex.BetweenBaseAndScripts, 1) },
        { (12, 10), 0, (SubIndex.Superscript, 0) },
        { (12, 15), 0, (SubIndex.Superscript, 0) },
        { (35, 10), 1 },
        { (50, 15), 2 },
    };
    [Theory, MemberData(nameof(Issue64Data))]
    public void Issue64(PointF point, MathListIndex expected) => Test(@"1^{123}+", point, expected);

    public static TestData IntegralData =>
      new TestData {
        { (0, 0), 0 },
        { (5, 5), 0 },
        { (5, 10), 0 },
        { (5, 15), 0 },
        { (8, 5), 0, (SubIndex.BetweenBaseAndScripts, 1) },
        { (8, 10), 0, (SubIndex.BetweenBaseAndScripts, 1) },
        { (8, 15), 0, (SubIndex.BetweenBaseAndScripts, 1) },
        { (10, -10), 0, (SubIndex.Subscript, 0) },
        { (10, 0), 0, (SubIndex.Subscript, 0) },
        { (10, 5), 0, (SubIndex.Subscript, 0) },
        { (10, 6), 0, (SubIndex.Subscript, 0) },
        { (10, 7), 0, (SubIndex.BetweenBaseAndScripts, 1) },
        { (10, 8), 0, (SubIndex.BetweenBaseAndScripts, 1) },
        { (10, 9), 0, (SubIndex.BetweenBaseAndScripts, 1) },
        { (10, 10), 0, (SubIndex.Superscript, 0) },
        { (10, 15), 0, (SubIndex.Superscript, 0) },
        { (15, -10), 0, (SubIndex.Subscript, 1) },
        { (15, 8), 0, (SubIndex.Subscript, 1) },
        { (15, 9), 0, (SubIndex.Superscript, 1) },
        { (15, 10), 0, (SubIndex.Superscript, 1) },
        { (18, 8), 0, (SubIndex.Subscript, 1) },
        { (18, 9), 0, (SubIndex.Superscript, 1) },
        { (19, 8), 1 },
        { (19, 9), 1 },
        { (20, 0), 1 },
        { (30, 0), 2 },
        { (40, 0), 3 },
        { (50, 0), 4 },
        { (60, 0), 5 },
      };
    [Theory, MemberData(nameof(IntegralData))]
    public void Integral(PointF point, MathListIndex expected) => Test(@"\int_a^b x\ dx", point, expected);
    public static TestData IntegralLimitsData =>
      new TestData {
        { (0, -100), 0, (SubIndex.Subscript, 0) },
        { (0, -5), 0, (SubIndex.Subscript, 0) },
        { (0, 0), 0 },
        { (0, 10), 0 },
        { (0, 20), 0, (SubIndex.Superscript, 0) },
        { (0, 100), 0, (SubIndex.Superscript, 0) },
        { (4, -100), 0, (SubIndex.Subscript, 0) },
        { (4, -5), 0, (SubIndex.Subscript, 0) },
        { (4, 0), 0 },
        { (4, 10), 0 },
        { (4, 20), 0, (SubIndex.Superscript, 0) },
        { (4, 100), 0, (SubIndex.Superscript, 0) },
        { (5, -100), 0, (SubIndex.Subscript, 1) },
        { (5, -5), 0, (SubIndex.Subscript, 1) },
        { (5, 0), 0 },
        { (5, 10), 0 },
        { (5, 20), 0, (SubIndex.Superscript, 1) },
        { (5, 100), 0, (SubIndex.Superscript, 1) },
        { (9, -100), 0, (SubIndex.Subscript, 1) },
        { (9, -5), 0, (SubIndex.Subscript, 1) },
        { (9, 0), 1 },
        { (9, 10), 1 },
        { (9, 20), 0, (SubIndex.Superscript, 1) },
        { (9, 100), 0, (SubIndex.Superscript, 1) },
        { (10, -100), 0, (SubIndex.Subscript, 1) },
        { (10, -5), 0, (SubIndex.Subscript, 1) },
        { (10, 0), 1 },
        { (10, 10), 1 },
        { (10, 20), 0, (SubIndex.Superscript, 1) },
        { (10, 100), 0, (SubIndex.Superscript, 1) },
        { (14, -100), 1 },
        { (14, -5), 1 },
        { (14, 0), 1 },
        { (14, 10), 1 },
        { (14, 20), 1 },
        { (14, 100), 1 },
        { (25, 0), 2 },
        { (35, 0), 3 },
        { (45, 0), 4 },
        { (55, 0), 5 },
      };
    [Theory, MemberData(nameof(IntegralLimitsData))]
    public void IntegralLimits(PointF point, MathListIndex expected) => Test(@"\int\limits_a^b x\ dx", point, expected);
    public static TestData SummationData =>
      new TestData {
        { (0, -100), 0 },
        { (0, -5), 0 },
        { (0, 0), 0 },
        { (0, 30), 0 },
        { (0, 100), 0 },
        { (10, -100), 1 },
        { (10, -5), 1 },
        { (10, 0), 1 },
        { (10, 30), 1 },
        { (10, 100), 1 },
        { (20, -100), 2 },
        { (20, -5), 2 },
        { (20, 0), 2 },
        { (20, 30), 2 },
        { (20, 100), 2 },
        { (25, -100), 2, (SubIndex.Subscript, 0) },
        { (25, -5), 2, (SubIndex.Subscript, 0) },
        { (25, 0), 2 },
        { (25, 30), 2, (SubIndex.Superscript, 0) },
        { (25, 100), 2, (SubIndex.Superscript, 0) },
        { (30, -100), 2, (SubIndex.Subscript, 1) },
        { (30, -5), 2, (SubIndex.Subscript, 1) },
        { (30, 0), 2 },
        { (30, 30), 2, (SubIndex.Superscript, 0) },
        { (30, 100), 2, (SubIndex.Superscript, 0) },
        { (35, -100), 2, (SubIndex.Subscript, 2) },
        { (35, -5), 2, (SubIndex.Subscript, 2) },
        { (35, 0), 2, (SubIndex.BetweenBaseAndScripts, 1) },
        { (35, 30), 2, (SubIndex.Superscript, 1) },
        { (35, 100), 2, (SubIndex.Superscript, 1) },
        { (40, -100), 2, (SubIndex.Subscript, 2) },
        { (40, -5), 2, (SubIndex.Subscript, 2) },
        { (40, 0), 3 },
        { (40, 30), 2, (SubIndex.Superscript, 2) },
        { (40, 100), 2, (SubIndex.Superscript, 2) },
        { (45, -100), 2, (SubIndex.Subscript, 3) },
        { (45, -5), 2, (SubIndex.Subscript, 3) },
        { (45, 0), 3 },
        { (45, 30), 2, (SubIndex.Superscript, 2) },
        { (45, 100), 2, (SubIndex.Superscript, 2) },
        { (50, -100), 3 },
        { (50, -5), 3 },
        { (50, 0), 3 },
        { (50, 30), 3 },
        { (50, 100), 3 },
        { (60, -100), 4 },
        { (60, -5), 4 },
        { (60, 0), 4 },
        { (60, 30), 4 },
        { (60, 100), 4 },
        { (70, -100), 5 },
        { (70, -5), 5 },
        { (70, 0), 5 },
        { (70, 30), 5 },
        { (70, 100), 5 },
      };
    [Theory, MemberData(nameof(SummationData))]
    public void Summation(PointF point, MathListIndex expected) => Test(@"77 \sum_{777}^{77} 77", point, expected);

  }
}