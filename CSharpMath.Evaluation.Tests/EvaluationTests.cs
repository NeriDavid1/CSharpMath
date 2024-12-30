using System.Linq;
using System.Text.RegularExpressions;
using Xunit;
using AngouriMath;

namespace CSharpMath.EvaluationTests {
  using AngouriMath.Extensions;
  using Atom;
  public class EvaluationTests {
    internal static MathList ParseLaTeX(string latex) =>
      LaTeXParser.MathListFromLaTeX(latex).Match(list => list, e => throw new Xunit.Sdk.XunitException(e));
    static Entity ParseMath(string latex) =>
      Evaluation.ParseExpression(latex);
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
    [Theory]
    [InlineData("1", "1", "1")]
    [InlineData("01", "1", "1")]
    [InlineData("10", "10", "10")]
    [InlineData("010", "10", "10")]
    [InlineData("1.", "1", "1")]
    [InlineData("01.", "1", "1")]
    [InlineData("1.0", "1", "1")]
    [InlineData("01.0", "1", "1")]
    [InlineData(".1", @"\frac{1}{10}", @"\frac{1}{10}")]
    [InlineData(".10", @"\frac{1}{10}", @"\frac{1}{10}")]
    [InlineData("1.1", @"\frac{11}{10}", @"\frac{11}{10}")]
    [InlineData("01.1", @"\frac{11}{10}", @"\frac{11}{10}")]
    [InlineData("1.10", @"\frac{11}{10}", @"\frac{11}{10}")]
    [InlineData("01.10", @"\frac{11}{10}", @"\frac{11}{10}")]
    [InlineData("0.1", @"\frac{1}{10}", @"\frac{1}{10}")]
    [InlineData("00.1", @"\frac{1}{10}", @"\frac{1}{10}")]
    [InlineData("0.10", @"\frac{1}{10}", @"\frac{1}{10}")]
    [InlineData("00.10", @"\frac{1}{10}", @"\frac{1}{10}")]
    [InlineData("1234", "1234", "1234")]
    [InlineData("0123456789", "123456789", "123456789")]
    [InlineData("1234.", "1234", "1234")]
    [InlineData(".5678", @"\frac{2839}{5000}", @"\frac{2839}{5000}")]
    [InlineData(".9876543210", "0.9876543210", "0.9876543210")]
    [InlineData("1234.5678", @"\frac{6172839}{5000}", @"\frac{6172839}{5000}")]
    public void Numbers(string input, string converted, string output) =>
      Test(input, converted, output);
    [Theory]
    [InlineData("a", "a", "a")]
    [InlineData("ab", @"a b", @"a b")]
    [InlineData("abc", @"a b c", @"a b c")]
    [InlineData("3a", @"3 a", @"3 a")]
    [InlineData("3ab", @"3 a b", @"3 a b")]
    [InlineData("3a3", @"3 a 3", @"9 a")]
    [InlineData("3aa", @"3 a a", @"3 a^2")]
    [InlineData(@"abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ",
      @"a b c d e f g h i j k l m " +
      @"n o p q r s t u v w x y z " +
      @"A B C D E F G H I J K L M " +
      @"N O P Q R S T U V W X Y Z",
      // i is considered as a number instead of a variable like other alphabets, so it is sorted to the front
      @"i a A b B c C d D e E f F " +
      @"g G h H I j J k K l L m M " +
      @"n N o O p P q Q r R s S t " +
      @"T u U v V w W x X y Y z Z")]
    [InlineData(@"\alpha\beta\gamma\delta\epsilon\varepsilon\zeta\eta\theta\iota\kappa\varkappa" +
      @"\lambda\mu\nu\xi\omicron\pi\varpi\rho\varrho\sigma\varsigma\tau\upsilon\phi\varphi\chi" +
      @"\psi\omega\Gamma\Delta\Theta\Lambda\Xi\Pi\Sigma\Upsilon\Phi\Psi\Omega",
      @"\alpha  \beta  \gamma  \delta  \epsilon  \varepsilon  \zeta " +
      @" \eta  \theta  \iota  \kappa  \varkappa  \lambda  \mu " +
      @" \nu  \xi  \omicron  \pi  \varpi  \rho  \varrho " +
      @" \sigma  \varsigma  \tau  \upsilon  \phi  \varphi  \chi " +
      @" \psi  \omega  \Gamma  \Delta  \Theta  \Lambda  \Xi " +
      @" \Pi  \Sigma  \Upsilon  \Phi  \Psi  \Omega ",
      @"\alpha  \beta  \chi  \delta  \Delta  \epsilon  \eta " +
      @" \gamma  \Gamma  \iota  \kappa  \lambda  \Lambda  \mu " +
      @" \nu  \omega  \Omega  \omicron  \phi  \Phi  \Pi " +
      @" \psi  \Psi  \rho  \sigma  \Sigma  \tau  \theta " +
      @" \Theta  \upsilon  \Upsilon  \varepsilon  \varkappa  \varphi " +
      @" \varpi  \varrho  \varsigma  \xi  \Xi  \zeta  \pi ")]
    [InlineData(@"a_2", @"a_2", @"a_2")]
    [InlineData(@"a_2+a_2", @"a_2+a_2", @"2 a_2")]
    [InlineData(@"a_{23}", @"a_{23}", @"a_{23}")]
    [InlineData(@"\pi_a", @"\pi _a", @"\pi _a")]
    [InlineData(@"ii", @"i i", @"-1")]
    [InlineData(@"i\infty", @"i \infty ", @"\infty i")]
    [InlineData(@"\infty\infty", @"\infty  \infty ", @"\infty ")]
    public void Variables(string input, string converted, string result) => Test(input, converted, result);
    [Theory]
    [InlineData("a + b", @"a+b", "a+b")]
    [InlineData("a - b", @"a-b", "a-b")]
    [InlineData("a * b", @"a b", @"a b")]
    [InlineData(@"a b", @"a b", @"a b")]
    [InlineData(@"a\cdot b", @"a b", @"a b")]
    [InlineData(@"a / b", @"\frac{a}{b}", @"\frac{a}{b}")]
    [InlineData(@"a\div b", @"\frac{a}{b}", @"\frac{a}{b}")]
    [InlineData(@"\frac ab", @"\frac{a}{b}", @"\frac{a}{b}")]
    [InlineData("a + b + c", @"a+b+c", "a+b+c")]
    [InlineData("a + b - c", @"a+b-c", "a+b-c")]
    [InlineData("a + b * c", @"a+b c", @"a+b c")]
    [InlineData("a + b / c", @"a+\frac{b}{c}", @"a+\frac{b}{c}")]
    [InlineData("a - b + c", @"a-b+c", "a-b+c")]
    [InlineData("a - b - c", @"a-b-c", @"a-b-c")]
    [InlineData("a - b * c", @"a-b c", @"a-b c")]
    [InlineData("a - b / c", @"a-\frac{b}{c}", @"a-\frac{b}{c}")]
    [InlineData("a * b + c", @"a b+c", @"a b+c")]
    [InlineData("a * b - c", @"a b-c", @"a b-c")]
    [InlineData("a * b * c", @"a b c", @"a b c")]
    [InlineData("a * b / c", @"\frac{a b}{c}", @"\frac{a b}{c}")]
    [InlineData("a / b + c", @"\frac{a}{b}+c", @"\frac{a}{b}+c")]
    [InlineData("a / b - c", @"\frac{a}{b}-c", @"\frac{a}{b}-c")]
    [InlineData("a / b * c", @"\frac{a}{b} c", @"\frac{a}{b} c")]
    [InlineData("a / b / c", @"\frac{\frac{a}{b}}{c}", @"\frac{a}{b c}")]
    [InlineData(@"2+\frac ab", @"2+\frac{a}{b}", @"2+\frac{a}{b}")]
    [InlineData(@"\frac ab+2", @"\frac{a}{b}+2", @"\frac{a}{b}+2")]
    [InlineData(@"2-\frac ab", @"2-\frac{a}{b}", @"2-\frac{a}{b}")]
    [InlineData(@"\frac ab-2", @"\frac{a}{b}-2", @"\frac{a}{b}-2")]
    [InlineData(@"2*\frac ab", @"2 \frac{a}{b}", @"2 \frac{a}{b}")]
    [InlineData(@"\frac ab*2", @"\frac{a}{b} 2", @"\frac{a}{b} 2")]
    [InlineData(@"2/\frac ab", @"\frac{2}{\frac{a}{b}}", @"2 \frac{b}{a}")]
    [InlineData(@"\frac ab/2", @"\frac{\frac{a}{b}}{2}", @"\frac{\frac{1}{2} a}{b}")]
    [InlineData(@"1+i", @"1+i", @"1+i")]
    [InlineData(@"1-i", @"1-i", @"1-i")]
    [InlineData(@"i+1", @"i+1", @"1+i")]
    [InlineData(@"i-1", @"i-1", @"-1+i")]
    [InlineData(@"\infty+1", @"\infty +1", @"\infty ")]
    [InlineData(@"\infty+i", @"\infty +i", @"\infty +i")]
    [InlineData(@"\infty+\infty", @"\infty +\infty ", @"\infty ")]
    [InlineData(@"\infty \infty", @"\infty  \infty ", @"\infty ")]
    [InlineData(@"i \infty", @"i \infty ", @"\infty i")]
    [InlineData(@"\infty i", @"\infty  i", @"\infty i")]
    [InlineData(@"i i", @"i i", @"-1")]
    [InlineData(@"\frac00", @"\frac{0}{0}", @"\mathrm{undefined}")]
    [InlineData(@"\frac0\infty", @"\frac{0}{\infty }", @"0")]
    [InlineData(@"\frac2\infty", @"\frac{2}{\infty }", @"0")]
    [InlineData(@"\frac{-2}\infty", @"\frac{-2}{\infty }", @"0")]
    [InlineData(@"\frac20", @"\frac{2}{0}", @"\mathrm{undefined}")]
    [InlineData(@"\frac{-2}0", @"\frac{-2}{0}", @"\mathrm{undefined}")]
    [InlineData(@"\frac\infty0", @"\frac{\infty }{0}", @"\mathrm{undefined}")]
    [InlineData(@"\frac{-\infty}0", @"\frac{-\infty }{0}", @"\mathrm{undefined}")]
    [InlineData(@"\frac\infty\infty", @"\frac{\infty }{\infty }", @"\mathrm{undefined}")]
    [InlineData(@"\frac{-\infty}{\infty}", @"\frac{-\infty }{\infty }", @"\mathrm{undefined}")]
    public void BinaryOperators(string latex, string converted, string result) => Test(latex, converted, result);
    [Theory]
    [InlineData("+i", "i", "i")]
    [InlineData("-i", "-i", "-i")]
    [InlineData("+a", "a", "a")]
    [InlineData("-a", "-a", "-a")]
    [InlineData("++a", "a", "a")]
    [InlineData("+-a", "-a", "-a")]
    [InlineData("-+a", "-a", "-a")]
    [InlineData("--a", "--a", "a")]
    [InlineData("+++a", "a", "a")]
    [InlineData("---a", "---a", "-a")]
    [InlineData("a++a", "a+a", @"2 a")]
    [InlineData("a+-a", "a-a", "0")]
    [InlineData("a-+a", "a-a", "0")]
    [InlineData("a--a", "a--a", @"2 a")]
    [InlineData("a+++a", "a+a", @"2 a")]
    [InlineData("a---a", "a---a", "0")]
    [InlineData("a*+a", @"a a", "a^2")]
    [InlineData("a*-a", @"a -a", "-a^2")]
    [InlineData("+a*+a", @"a a", "a^2")]
    [InlineData("-a*-a", @"-a -a", "a^2")]
    [InlineData("a/+a", @"\frac{a}{a}", "1")]
    [InlineData("a/-a", @"\frac{a}{-a}", "-1")]
    [InlineData("+a/+a", @"\frac{a}{a}", "1")]
    [InlineData("-a/-a", @"\frac{-a}{-a}", "1")]
    [InlineData("-2+-2+-2", @"-2-2-2", "-6")]
    [InlineData("-2--2--2", @"-2--2--2", "2")]
    [InlineData("-2*-2*-2", @"-2 -2 -2", "-8")]
    [InlineData("-2/-2/-2", @"\frac{\frac{-2}{-2}}{-2}", @"\frac{-1}{2}")]
    public void UnaryOperators(string latex, string converted, string result) => Test(latex, converted, result);
    [Theory]
    [InlineData(@"9\%", @"\frac{9}{100}", @"\frac{9}{100}")]
    [InlineData(@"a\%", @"\frac{a}{100}", @"\frac{1}{100} a")]
    [InlineData(@"\pi\%", @"\frac{\pi }{100}", @"\frac{1}{100} \pi ")]
    [InlineData(@"a\%\%", @"\frac{\frac{a}{100}}{100}", @"\frac{1}{10000} a")]
    [InlineData(@"9\%+3", @"\frac{9}{100}+3", @"\frac{309}{100}")]
    [InlineData(@"-9\%+3", @"-\frac{9}{100}+3", @"\frac{291}{100}")]
    [InlineData(@"2^2\%", @"\frac{2^2}{100}", @"\frac{1}{25}")]
    [InlineData(@"2\%^2", @"\left( \frac{2}{100}\right) ^2", @"\frac{1}{2500}")]
    [InlineData(@"2\%2", @"\frac{2}{100} 2", @"\frac{1}{25}")]
    [InlineData(@"1+2\%^2", @"1+\left( \frac{2}{100}\right) ^2", @"\frac{2501}{2500}")]
    [InlineData(@"9\degree", @"\frac{9 \pi }{180}", @"\frac{1}{20} \pi ")]
    [InlineData(@"a\degree", @"\frac{a \pi }{180}", @"\frac{1}{180} a \pi ")]
    [InlineData(@"\pi\degree", @"\frac{\pi  \pi }{180}", @"\frac{1}{180} \pi ^2")]
    [InlineData(@"a\%\degree", @"\frac{\frac{a}{100} \pi }{180}", @"\frac{1}{18000} a \pi ")]
    [InlineData(@"a\degree\degree", @"\frac{\frac{a \pi }{180} \pi }{180}", @"\frac{1}{32400} a \pi ^2")]
    [InlineData(@"9\degree+3", @"\frac{9 \pi }{180}+3", @"3+\frac{1}{20} \pi ")]
    [InlineData(@"-9\degree+3", @"-\frac{9 \pi }{180}+3", @"3+\frac{-1}{20} \pi ")]
    [InlineData(@"2^2\degree", @"\frac{2^2 \pi }{180}", @"\frac{1}{45} \pi ")]
    [InlineData(@"2\degree^2", @"\left( \frac{2 \pi }{180}\right) ^2", @"\left( \frac{1}{90} \pi \right) ^2")]
    [InlineData(@"2\degree2", @"\frac{2 \pi }{180} 2", @"\frac{1}{45} \pi ")]
    [InlineData(@"1+2\degree^2", @"1+\left( \frac{2 \pi }{180}\right) ^2", @"1+\left( \frac{1}{90} \pi \right) ^2")]
    public void PostfixOperators(string latex, string converted, string result) => Test(latex, converted, result);
    [Theory]
    [InlineData("2^2", "2^2", "4")]
    [InlineData(".2^2", @"\left( \frac{1}{5}\right) ^2", @"\frac{1}{25}")]
    [InlineData("2.^2", "2^2", "4")]
    [InlineData("2.1^2", @"\left( \frac{21}{10}\right) ^2", @"\frac{441}{100}")]
    [InlineData("a^a", "a^a", "a^a")]
    [InlineData("a^{a+b}", "a^{a+b}", "a^{a+b}")]
    [InlineData("a^{-2}", "a^{-2}", "a^{-2}")]
    [InlineData("2^{3^4}", "2^{3^4}", "2417851639229258349412352")]
    [InlineData("4^{3^2}", "4^{3^2}", "262144")]
    [InlineData("4^3+2", "4^3+2", "66")]
    [InlineData("2+3^4", "2+3^4", "83")]
    [InlineData("4^3*2", @"4^3 2", "128")]
    [InlineData("2*3^4", @"2 3^4", "162")]
    [InlineData("1/x", @"\frac{1}{x}", @"\frac{1}{x}")]
    [InlineData("2/x", @"\frac{2}{x}", @"\frac{2}{x}")]
    [InlineData("0^x", @"0^x", @"0")]
    [InlineData("1^x", @"1^x", @"1")]
    [InlineData("x^0", @"x^0", @"1")]
    [InlineData("x^{-1}", @"x^{-1}", @"\frac{1}{x}")]
    [InlineData("-i^{-1}", @"-i^{-1}", @"i")]
    [InlineData("i^{-2}", @"i^{-2}", @"-1")]
    [InlineData("i^{-1}", @"i^{-1}", @"-i")]
    [InlineData("i^0", @"i^0", @"1")]
    [InlineData("i^1", @"i^1", @"i")]
    [InlineData("i^2", @"i^2", @"-1")]
    [InlineData("i^3", @"i^3", @"-i")]
    [InlineData("i^4", @"i^4", @"1")]
    [InlineData("i^5", @"i^5", @"i")]
    [InlineData("10^2", @"10^2", @"100")]
    [InlineData(".1^2", @"\left( \frac{1}{10}\right) ^2", @"\frac{1}{100}")]
    [InlineData("10^x", @"10^x", @"10^x")]
    [InlineData(@"{\frac 12}^4", @"\left( \frac{1}{2}\right) ^4", @"\frac{1}{16}")]
    [InlineData(@"\sqrt2", @"\sqrt{2}", @"\sqrt{2}")]
    [InlineData(@"\sqrt2^2", @"\left( \sqrt{2}\right) ^2", "2")]
    [InlineData(@"\sqrt[3]2", @"2^{\frac{1}{3}}", @"\sqrt[3]{2}")]
    [InlineData(@"\sqrt[3/2]2", @"2^{\frac{1}{\frac{3}{2}}}", @"\sqrt[3]{2}^2")]
    [InlineData(@"\sqrt[3]2^3", @"\left( 2^{\frac{1}{3}}\right) ^3", "2")]
    [InlineData(@"\sqrt[3]2^{1+1+1}", @"\left( 2^{\frac{1}{3}}\right) ^{1+1+1}", "2")]
    [InlineData(@"\sqrt[1+1+1]2^{1+1+1}", @"\left( 2^{\frac{1}{1+1+1}}\right) ^{1+1+1}", "2")]
    public void Exponents(string latex, string converted, string result) => Test(latex, converted, result);
    [Theory]
    [InlineData(@"\sin x", @"\sin \left( x\right) ", @"\sin \left( x\right) ")]
    [InlineData(@"\cos x", @"\cos \left( x\right) ", @"\cos \left( x\right) ")]
    [InlineData(@"\tan x", @"\tan \left( x\right) ", @"\tan \left( x\right) ")]
    [InlineData(@"\cot x", @"\cot \left( x\right) ", @"\cot \left( x\right) ")]
    [InlineData(@"\sec x", @"\frac{1}{\cos \left( x\right) }", @"\frac{1}{\cos \left( x\right) }")]
    [InlineData(@"\csc x", @"\frac{1}{\sin \left( x\right) }", @"\frac{1}{\sin \left( x\right) }")]
    [InlineData(@"\arcsin x", @"\arcsin \left( x\right) ", @"\arcsin \left( x\right) ")]
    [InlineData(@"\arccos x", @"\arccos \left( x\right) ", @"\arccos \left( x\right) ")]
    [InlineData(@"\arctan x", @"\arctan \left( x\right) ", @"\arctan \left( x\right) ")]
    [InlineData(@"\arccot x", @"\arccot \left( x\right) ", @"\arccot \left( x\right) ")]
    [InlineData(@"\arcsec x", @"\arccos \left( \frac{1}{x}\right) ", @"\arccos \left( \frac{1}{x}\right) ")]
    [InlineData(@"\arccsc x", @"\arcsin \left( \frac{1}{x}\right) ", @"\arcsin \left( \frac{1}{x}\right) ")]
    [InlineData(@"\ln x", @"\ln \left( x\right) ", @"\ln \left( x\right) ")]
    [InlineData(@"\log x", @"\log \left( x\right) ", @"\log \left( x\right) ")]
    [InlineData(@"\log_3 x", @"\log _3\left( x\right) ", @"\log _3\left( x\right) ")]
    [InlineData(@"\log_{10} x", @"\log \left( x\right) ", @"\log \left( x\right) ")]
    [InlineData(@"\log_e x", @"\ln \left( x\right) ", @"\ln \left( x\right) ")]
    [InlineData(@"\ln x^2", @"\ln \left( x^2\right) ", @"\ln \left( x^2\right) ")]
    [InlineData(@"\log x^2", @"\log \left( x^2\right) ", @"\log \left( x^2\right) ")]
    [InlineData(@"\log_{10} x^2", @"\log \left( x^2\right) ", @"\log \left( x^2\right) ")]
    [InlineData(@"\log_3 x^2", @"\log _3\left( x^2\right) ", @"\log _3\left( x^2\right) ")]
    [InlineData(@"\log_e x^2", @"\ln \left( x^2\right) ", @"\ln \left( x^2\right) ")]
    [InlineData(@"\ln x^{-1}", @"\ln \left( x^{-1}\right) ", @"\ln \left( \frac{1}{x}\right) ")]
    [InlineData(@"\log x^{-1}", @"\log \left( x^{-1}\right) ", @"\log \left( \frac{1}{x}\right) ")]
    [InlineData(@"\log_{10} x^{-1}", @"\log \left( x^{-1}\right) ", @"\log \left( \frac{1}{x}\right) ")]
    [InlineData(@"\log_3 x^{-1}", @"\log _3\left( x^{-1}\right) ", @"\log _3\left( \frac{1}{x}\right) ")]
    [InlineData(@"\log_e x^{-1}", @"\ln \left( x^{-1}\right) ", @"\ln \left( \frac{1}{x}\right) ")]
    [InlineData(@"2\sin x", @"2 \sin \left( x\right) ", @"2 \sin \left( x\right) ")]
    [InlineData(@"\sin 2x", @"\sin \left( 2 x\right) ", @"\sin \left( 2 x\right) ")]
    [InlineData(@"\sin xy", @"\sin \left( x y\right) ", @"\sin \left( x y\right) ")]
    [InlineData(@"\sin \frac\pi2", @"\sin \left( \frac{\pi }{2}\right) ", @"1")]
    [InlineData(@"\sin \frac\pi2+1", @"\sin \left( \frac{\pi }{2}\right) +1", @"2")]
    [InlineData(@"\cos +x", @"\cos \left( x\right) ", @"\cos \left( x\right) ")]
    [InlineData(@"\cos -x", @"\cos \left( -x\right) ", @"\cos \left( -x\right) ")]
    [InlineData(@"\tan x\%", @"\tan \left( \frac{x}{100}\right) ", @"\tan \left( \frac{1}{100} x\right) ")]
    [InlineData(@"\tan x\%^2", @"\tan \left( \left( \frac{x}{100}\right) ^2\right) ", @"\tan \left( \left( \frac{1}{100} x\right) ^2\right) ")]
    [InlineData(@"\cot x*y", @"\cot \left( x\right)  y", @"\cot \left( x\right)  y")]
    [InlineData(@"\cot x/y", @"\frac{\cot \left( x\right) }{y}", @"\frac{\cot \left( x\right) }{y}")]
    [InlineData(@"\cos \arccos x", @"\cos \left( \arccos \left( x\right) \right) ", @"x")]
    [InlineData(@"\sin^2 x", @"\sin \left( x\right) ^2", @"\sin \left( x\right) ^2")]
    [InlineData(@"\sin^2 xy+\cos^2 yx", @"\sin \left( x y\right) ^2+\cos \left( y x\right) ^2", @"1")]
    [InlineData(@"\log^2 x", @"\log \left( x\right) ^2", @"\log \left( x\right) ^2")]
    [InlineData(@"\ln^2 x", @"\ln \left( x\right) ^2", @"\ln \left( x\right) ^2")]
    [InlineData(@"\log_{10}^2 x", @"\log \left( x\right) ^2", @"\log \left( x\right) ^2")]
    [InlineData(@"\log_3^2 x", @"\log _3\left( x\right) ^2", @"\log _3\left( x\right) ^2")]
    public void Functions(string latex, string converted, string result) => Test(latex, converted, result);
    [Theory]
    [InlineData(@"\sin^{-1} x", @"\arcsin \left( x\right) ", @"\arcsin \left( x\right) ")]
    [InlineData(@"\cos^{-1} x", @"\arccos \left( x\right) ", @"\arccos \left( x\right) ")]
    [InlineData(@"\tan^{-1} x", @"\arctan \left( x\right) ", @"\arctan \left( x\right) ")]
    [InlineData(@"\cot^{-1} x", @"\arccot \left( x\right) ", @"\arccot \left( x\right) ")]
    [InlineData(@"\sec^{-1} x", @"\arccos \left( \frac{1}{x}\right) ", @"\arccos \left( \frac{1}{x}\right) ")]
    [InlineData(@"\csc^{-1} x", @"\arcsin \left( \frac{1}{x}\right) ", @"\arcsin \left( \frac{1}{x}\right) ")]
    [InlineData(@"\arcsin^{-1} x", @"\sin \left( x\right) ", @"\sin \left( x\right) ")]
    [InlineData(@"\arccos^{-1} x", @"\cos \left( x\right) ", @"\cos \left( x\right) ")]
    [InlineData(@"\arctan^{-1} x", @"\tan \left( x\right) ", @"\tan \left( x\right) ")]
    [InlineData(@"\arccot^{-1} x", @"\cot \left( x\right) ", @"\cot \left( x\right) ")]
    [InlineData(@"\arcsec^{-1} x", @"\frac{1}{\cos \left( x\right) }", @"\frac{1}{\cos \left( x\right) }")]
    [InlineData(@"\arccsc^{-1} x", @"\frac{1}{\sin \left( x\right) }", @"\frac{1}{\sin \left( x\right) }")]
    [InlineData(@"\ln^{-1} x", @"e^x", @"e^x")]
    [InlineData(@"\log^{-1} x", @"10^x", @"10^x")]
    [InlineData(@"\log_3^{-1} x", @"3^x", @"3^x")]
    [InlineData(@"\log^{-1}_{10} x", @"10^x", @"10^x")]
    [InlineData(@"\log_e^{-1} x", @"e^x", @"e^x")]
    [InlineData(@"\ln^{-1} x^2", @"e^{x^2}", @"e^{x^2}")]
    [InlineData(@"\log^{-1} x^2", @"10^{x^2}", @"10^{x^2}")]
    [InlineData(@"\log_{10}^{-1} x^2", @"10^{x^2}", @"10^{x^2}")]
    [InlineData(@"\log_3^{-1} x^2", @"3^{x^2}", @"3^{x^2}")]
    [InlineData(@"\log_e^{-1} x^2", @"e^{x^2}", @"e^{x^2}")]
    [InlineData(@"\ln^{-1} x^{-1}", @"e^{x^{-1}}", @"e^{\frac{1}{x}}")]
    [InlineData(@"\log^{-1} x^{-1}", @"10^{x^{-1}}", @"10^{\frac{1}{x}}")]
    [InlineData(@"\log_{10}^{-1} x^{-1}", @"10^{x^{-1}}", @"10^{\frac{1}{x}}")]
    [InlineData(@"\log_3^{-1} x^{-1}", @"3^{x^{-1}}", @"3^{\frac{1}{x}}")]
    [InlineData(@"\log_e^{-1} x^{-1}", @"e^{x^{-1}}", @"e^{\frac{1}{x}}")]
    [InlineData(@"2\sin^{-1} x", @"2 \arcsin \left( x\right) ", @"2 \arcsin \left( x\right) ")]
    [InlineData(@"\sin^{-1} 2x", @"\arcsin \left( 2 x\right) ", @"\arcsin \left( 2 x\right) ")]
    [InlineData(@"\sin^{-1} xy", @"\arcsin \left( x y\right) ", @"\arcsin \left( x y\right) ")]
    [InlineData(@"\cos^{-1} +x", @"\arccos \left( x\right) ", @"\arccos \left( x\right) ")]
    [InlineData(@"\cos^{-1} -x", @"\arccos \left( -x\right) ", @"\arccos \left( -x\right) ")]
    [InlineData(@"\tan^{-1} x\%", @"\arctan \left( \frac{x}{100}\right) ", @"\arctan \left( \frac{1}{100} x\right) ")]
    [InlineData(@"\tan^{-1} x\%^2", @"\arctan \left( \left( \frac{x}{100}\right) ^2\right) ", @"\arctan \left( \left( \frac{1}{100} x\right) ^2\right) ")]
    [InlineData(@"\cot^{-1} x*y", @"\arccot \left( x\right)  y", @"\arccot \left( x\right)  y")]
    [InlineData(@"\cot^{-1} x/y", @"\frac{\arccot \left( x\right) }{y}", @"\frac{\arccot \left( x\right) }{y}")]
    [InlineData(@"\cos^{-1} \arccos^{-1} x", @"\arccos \left( \cos \left( x\right) \right) ", @"x")]
    [InlineData(@"\sin^1 x", @"\sin \left( x\right) ^1", @"\sin \left( x\right) ")]
    [InlineData(@"\sin^{+1} x", @"\sin \left( x\right) ^1", @"\sin \left( x\right) ")]
    [InlineData(@"\sin^{+-1} x", @"\sin \left( x\right) ^{-1}", @"\frac{1}{\sin \left( x\right) }")]
    [InlineData(@"\sin^{-+1} x", @"\sin \left( x\right) ^{-1}", @"\frac{1}{\sin \left( x\right) }")]
    [InlineData(@"\sin^{--1} x", @"\sin \left( x\right) ^{--1}", @"\sin \left( x\right) ")]
    [InlineData(@"\sin^{-1^2} x", @"\sin \left( x\right) ^{-1^2}", @"\frac{1}{\sin \left( x\right) }")]
    [InlineData(@"\sin^{-1+3} xy+\cos^{-1+3} yx", @"\sin \left( x y\right) ^{-1+3}+\cos \left( y x\right) ^{-1+3}", @"1")]
    [InlineData(@"\log^{-a_2} x", @"\log \left( x\right) ^{-a_2}", @"\log \left( x\right) ^{-a_2}")]
    [InlineData(@"\ln^{3-1} x", @"\ln \left( x\right) ^{3-1}", @"\ln \left( x\right) ^2")]
    public void FunctionInverses(string latex, string converted, string result) => Test(latex, converted, result);
    [InlineData(@"1+(2+3)", @"1+2+3", @"6")]
    [InlineData(@"1+((2+3))", @"1+2+3", @"6")]
    [InlineData(@"2*(3+4)", @"2 \left( 3+4\right) ", @"14")]
    [InlineData(@"(3+4)*2", @"\left( 3+4\right)  2", @"14")]
    [InlineData(@"(5+6)^2", @"\left( 5+6\right) ^2", @"121")]
    [InlineData(@"(5+6)", @"5+6", @"11")]
    [InlineData(@"((5+6))", @"5+6", @"11")]
    [InlineData(@"(5+6)2", @"\left( 5+6\right)  2", @"22")]
    [InlineData(@"2(5+6)", @"2 \left( 5+6\right) ", @"22")]
    [InlineData(@"2(5+6)2", @"2 \left( 5+6\right)  2", @"44")]
    [InlineData(@"(5+6)x", @"\left( 5+6\right)  x", @"11 x")]
    [InlineData(@"x(5+6)", @"x \left( 5+6\right) ", @"11 x")]
    [InlineData(@"x(5+6)x", @"x \left( 5+6\right)  x", @"11 x^2")]
    [InlineData(@"(5+6).2", @"\left( 5+6\right)  \frac{1}{5}", @"\frac{11}{5}")]
    [InlineData(@".2(5+6)", @"\frac{1}{5} \left( 5+6\right) ", @"\frac{11}{5}")]
    [InlineData(@".2(5+6).2", @"\frac{1}{5} \left( 5+6\right)  \frac{1}{5}", @"\frac{11}{25}")]
    [InlineData(@"(5+6)2.", @"\left( 5+6\right)  2", @"22")]
    [InlineData(@"2.(5+6)", @"2 \left( 5+6\right) ", @"22")]
    [InlineData(@"2.(5+6)2.", @"2 \left( 5+6\right)  2", @"44")]
    [InlineData(@"(5+6)(2)", @"\left( 5+6\right)  2", @"22")]
    [InlineData(@"(5+6)(1+1)", @"\left( 5+6\right)  \left( 1+1\right) ", @"22")]
    [InlineData(@"(5+6)(-(-2))", @"\left( 5+6\right)  --2", @"22")]
    [InlineData(@"(5+6)(--2)", @"\left( 5+6\right)  --2", @"22")]
    [InlineData(@"+(1)", @"1", @"1")]
    [InlineData(@"+(1)\%", @"\frac{1}{100}", @"\frac{1}{100}")]
    [InlineData(@"+(-1)", @"-1", @"-1")]
    [InlineData(@"-(+1)", @"-1", @"-1")]
    [InlineData(@"-(-1)", @"--1", @"1")]
    [InlineData(@"--(--1)", @"----1", @"1")]
    [InlineData(@"(2+3)^{(4+5)}", @"\left( 2+3\right) ^{4+5}", @"1953125")]
    [InlineData(@"(2+3)^{((4)+5)}", @"\left( 2+3\right) ^{4+5}", @"1953125")]
    [InlineData(@"(1+i)\infty", @"\left( 1+i\right)  \infty ", @"\infty +\infty i")]
    [InlineData(@"2\sin(x)", @"2 \sin \left( x\right) ", @"2 \sin \left( x\right) ")]
    [InlineData(@"(2)\sin(x)", @"2 \sin \left( x\right) ", @"2 \sin \left( x\right) ")]
    [InlineData(@"\sin(x+1)", @"\sin \left( x+1\right) ", @"\sin \left( 1+x\right) ")]
    [InlineData(@"\sin((x+1))", @"\sin \left( x+1\right) ", @"\sin \left( 1+x\right) ")]
    [InlineData(@"\sin(2(x+1))", @"\sin \left( 2 \left( x+1\right) \right) ", @"\sin \left( 2 \left( 1+x\right) \right) ")]
    [InlineData(@"\sin((x+1)+2)", @"\sin \left( x+1+2\right) ", @"\sin \left( 3+x\right) ")]
    [InlineData(@"\sin(x)2", @"\sin \left( x\right)  2", @"2 \sin \left( x\right) ")]
    [InlineData(@"\sin(x)(x+1)", @"\sin \left( x\right)  \left( x+1\right) ", @"\sin \left( x\right)  \left( 1+x\right) ")]
    [InlineData(@"\sin(x)(x+1)(x)", @"\sin \left( x\right)  \left( x+1\right)  x", @"\sin \left( x\right)  \left( 1+x\right)  x")]
    [InlineData(@"\sin(x^2)", @"\sin \left( x^2\right) ", @"\sin \left( x^2\right) ")]
    [InlineData(@"\sin\ (x^2)", @"\sin \left( x^2\right) ", @"\sin \left( x^2\right) ")]
    [InlineData(@"\sin\; (x^2)", @"\sin \left( x^2\right) ", @"\sin \left( x^2\right) ")]
    [InlineData(@"\sin\ \; (x^2)", @"\sin \left( x^2\right) ", @"\sin \left( x^2\right) ")]
    [InlineData(@"\sin^3(x)", @"\sin \left( x\right) ^3", @"\sin \left( x\right) ^3")]
    [InlineData(@"\sin^3\ (x)", @"\sin \left( x\right) ^3", @"\sin \left( x\right) ^3")]
    [InlineData(@"\sin^3\; (x)", @"\sin \left( x\right) ^3", @"\sin \left( x\right) ^3")]
    [InlineData(@"\sin^3\ \; (x)", @"\sin \left( x\right) ^3", @"\sin \left( x\right) ^3")]
    [InlineData(@"\sin^{-1}(x)", @"\arcsin \left( x\right) ", @"\arcsin \left( x\right) ")]
    [InlineData(@"\sin^{-1}\ (x)", @"\arcsin \left( x\right) ", @"\arcsin \left( x\right) ")]
    [InlineData(@"\sin^{-1}\; (x)", @"\arcsin \left( x\right) ", @"\arcsin \left( x\right) ")]
    [InlineData(@"\sin^{-1}\ \; (x)", @"\arcsin \left( x\right) ", @"\arcsin \left( x\right) ")]
    [InlineData(@"\sin(x)^2", @"\sin \left( x\right) ^2", @"\sin \left( x\right) ^2")]
    [InlineData(@"\sin\ (x)^2", @"\sin \left( x\right) ^2", @"\sin \left( x\right) ^2")]
    [InlineData(@"\sin\; (x)^2", @"\sin \left( x\right) ^2", @"\sin \left( x\right) ^2")]
    [InlineData(@"\sin\ \; (x)^2", @"\sin \left( x\right) ^2", @"\sin \left( x\right) ^2")]
    [InlineData(@"\sin(x)^{-1}", @"\sin \left( x\right) ^{-1}", @"\frac{1}{\sin \left( x\right) }")]
    [InlineData(@"\sin\ (x)^{-1}", @"\sin \left( x\right) ^{-1}", @"\frac{1}{\sin \left( x\right) }")]
    [InlineData(@"\sin\; (x)^{-1}", @"\sin \left( x\right) ^{-1}", @"\frac{1}{\sin \left( x\right) }")]
    [InlineData(@"\sin\ \; (x)^{-1}", @"\sin \left( x\right) ^{-1}", @"\frac{1}{\sin \left( x\right) }")]
    [InlineData(@"\sin^3(x)^{-1}", @"\sin \left( x\right) ^{3 -1}", @"\sin \left( x\right) ^{-3}")]
    [InlineData(@"\sin^3\ (x)^{-1}", @"\sin \left( x\right) ^{3 -1}", @"\sin \left( x\right) ^{-3}")]
    [InlineData(@"\sin^3\; (x)^{-1}", @"\sin \left( x\right) ^{3 -1}", @"\sin \left( x\right) ^{-3}")]
    [InlineData(@"\sin^3\ \; (x)^{-1}", @"\sin \left( x\right) ^{3 -1}", @"\sin \left( x\right) ^{-3}")]
    [InlineData(@"\sin^{-1}(x)^{-1}", @"\arcsin \left( x\right) ^{-1}", @"\frac{1}{\arcsin \left( x\right) }")]
    [InlineData(@"\sin^{-1}\ (x)^{-1}", @"\arcsin \left( x\right) ^{-1}", @"\frac{1}{\arcsin \left( x\right) }")]
    [InlineData(@"\sin^{-1}\; (x)^{-1}", @"\arcsin \left( x\right) ^{-1}", @"\frac{1}{\arcsin \left( x\right) }")]
    [InlineData(@"\sin^{-1}\ \; (x)^{-1}", @"\arcsin \left( x\right) ^{-1}", @"\frac{1}{\arcsin \left( x\right) }")]
    [InlineData(@"\sin^a(x)", @"\sin \left( x\right) ^a", @"\sin \left( x\right) ^a")]
    [InlineData(@"\sin^a(x)^2", @"\sin \left( x\right) ^{a 2}", @"\sin \left( x\right) ^{2 a}")]
    [InlineData(@"\sin^a\ (x)^2", @"\sin \left( x\right) ^{a 2}", @"\sin \left( x\right) ^{2 a}")]
    [InlineData(@"\sin^a\; (x)^2", @"\sin \left( x\right) ^{a 2}", @"\sin \left( x\right) ^{2 a}")]
    [InlineData(@"\sin^a\ \; (x)^2", @"\sin \left( x\right) ^{a 2}", @"\sin \left( x\right) ^{2 a}")]
    [InlineData(@"\sin(x)^2(x)", @"\sin \left( x\right) ^2 x", @"\sin \left( x\right) ^2 x")]
    [InlineData(@"\sin\ (x)^2(x)", @"\sin \left( x\right) ^2 x", @"\sin \left( x\right) ^2 x")]
    [InlineData(@"\sin\; (x)^2(x)", @"\sin \left( x\right) ^2 x", @"\sin \left( x\right) ^2 x")]
    [InlineData(@"\sin\ \; (x)^2(x)", @"\sin \left( x\right) ^2 x", @"\sin \left( x\right) ^2 x")]
    [InlineData(@"\sin^a(x)^2(x)", @"\sin \left( x\right) ^{a 2} x", @"\sin \left( x\right) ^{2 a} x")]
    [InlineData(@"\sin^a\ (x)^2(x)", @"\sin \left( x\right) ^{a 2} x", @"\sin \left( x\right) ^{2 a} x")]
    [InlineData(@"\sin^a\; (x)^2(x)", @"\sin \left( x\right) ^{a 2} x", @"\sin \left( x\right) ^{2 a} x")]
    [InlineData(@"\sin^a\ \; (x)^2(x)", @"\sin \left( x\right) ^{a 2} x", @"\sin \left( x\right) ^{2 a} x")]
    [InlineData(@"\sin (\frac\pi2)", @"\sin \left( \frac{\pi }{2}\right) ", @"1")]
    [InlineData(@"\sin (\frac\pi2)+1", @"\sin \left( \frac{\pi }{2}\right) +1", @"2")]
    public void Parentheses(string latex, string converted, string result) {
      Test(latex, converted, result);
      Test(latex.Replace('(', '[').Replace(')', ']'), converted, result);
    }
    [Theory]
    [InlineData(@"\begin{matrix}1\end{matrix}", @"\left( \begin{matrix}1\end{matrix}\right) ", @"\left( \begin{matrix}1\end{matrix}\right) ")]
    [InlineData(@"\begin{pmatrix}1\end{pmatrix}", @"\left( \begin{matrix}1\end{matrix}\right) ", @"\left( \begin{matrix}1\end{matrix}\right) ")]
    [InlineData(@"\begin{bmatrix}1\end{bmatrix}", @"\left( \begin{matrix}1\end{matrix}\right) ", @"\left( \begin{matrix}1\end{matrix}\right) ")]
    [InlineData(@"\begin{pmatrix}1\end{pmatrix}^2", @"\left( \begin{matrix}1\end{matrix}\right) ^2", @"\left( \begin{matrix}1\end{matrix}\right) ^2")]
    [InlineData(@"\begin{bmatrix}1\end{bmatrix}^2", @"\left( \begin{matrix}1\end{matrix}\right) ^2", @"\left( \begin{matrix}1\end{matrix}\right) ^2")]
    public void Vectors(string latex, string converted, string result) => Test(latex, converted, result);
    [Theory]
    [InlineData(@"\begin{matrix}1&2\\3&4\end{matrix}", @"\left( \begin{matrix}1&2\\ 3&4\end{matrix}\right) ", @"\left( \begin{matrix}1&2\\ 3&4\end{matrix}\right) ")]
    [InlineData(@"\begin{pmatrix}1&2\\3&4\end{pmatrix}", @"\left( \begin{matrix}1&2\\ 3&4\end{matrix}\right) ", @"\left( \begin{matrix}1&2\\ 3&4\end{matrix}\right) ")]
    [InlineData(@"\begin{bmatrix}1&2\\3&4\end{bmatrix}", @"\left( \begin{matrix}1&2\\ 3&4\end{matrix}\right) ", @"\left( \begin{matrix}1&2\\ 3&4\end{matrix}\right) ")]
    [InlineData(@"\begin{pmatrix}1&2\\3&4\end{pmatrix}^2", @"\left( \begin{matrix}1&2\\ 3&4\end{matrix}\right) ^2", @"\left( \begin{matrix}1&2\\ 3&4\end{matrix}\right) ^2")]
    [InlineData(@"\begin{bmatrix}1&2\\3&4\end{bmatrix}^2", @"\left( \begin{matrix}1&2\\ 3&4\end{matrix}\right) ^2", @"\left( \begin{matrix}1&2\\ 3&4\end{matrix}\right) ^2")]
    [InlineData(@"\begin{matrix}1&2\\3&4\end{matrix}+\begin{bmatrix}1&2\\3&5\end{bmatrix}", @"\left( \begin{matrix}1&2\\ 3&4\end{matrix}\right) +\left( \begin{matrix}1&2\\ 3&5\end{matrix}\right) ", @"\left( \begin{matrix}1&2\\ 3&4\end{matrix}\right) +\left( \begin{matrix}1&2\\ 3&5\end{matrix}\right) ")]
    [InlineData(@"\begin{pmatrix}1&2\\3&4\end{pmatrix}+\begin{matrix}1&2\\3&5\end{matrix}", @"\left( \begin{matrix}1&2\\ 3&4\end{matrix}\right) +\left( \begin{matrix}1&2\\ 3&5\end{matrix}\right) ", @"\left( \begin{matrix}1&2\\ 3&4\end{matrix}\right) +\left( \begin{matrix}1&2\\ 3&5\end{matrix}\right) ")]
    [InlineData(@"\begin{bmatrix}1&2\\3&4\end{bmatrix}+\begin{pmatrix}1&2\\3&5\end{pmatrix}", @"\left( \begin{matrix}1&2\\ 3&4\end{matrix}\right) +\left( \begin{matrix}1&2\\ 3&5\end{matrix}\right) ", @"\left( \begin{matrix}1&2\\ 3&4\end{matrix}\right) +\left( \begin{matrix}1&2\\ 3&5\end{matrix}\right) ")]
    public void Matrices(string latex, string converted, string result) => Test(latex, converted, result);
    [Theory]
    [InlineData(@"1,2", @"1,2")]
    [InlineData(@"1,2,3", @"1,2,3")]
    [InlineData(@"a,b,c,d", @"a,b,c,d")]
    [InlineData(@"\sqrt2,\sqrt[3]2,\frac34", @"\sqrt{2},2^{\frac{1}{3}},\frac{3}{4}")]
    [InlineData(@"\sin a,\cos b^2,\tan c_3,\cot de,\sec 12f,\csc g+h",
      @"\sin \left( a\right) ,\cos \left( b^2\right) ,\tan \left( c_3\right) ,\cot \left( d e\right) ,\frac{1}{\cos \left( 12 f\right) },\frac{1}{\sin \left( g\right) }+h")]
    public void Comma(string latex, string converted) => Test(latex, converted, null);
    [Theory]
    [InlineData(@"\emptyset", @"\emptyset ")]
    [InlineData(@"\mathbb R", @"\left\{ \left( -\infty ,\infty \right) \right\} ")]
    [InlineData(@"\mathbb C", @"\left\{ \left\{ z\in \mathbb{C}:\Re \left( z\right) \in \left( -\infty ,\infty \right) \wedge \Im \left( z\right) \in \left( -\infty ,\infty \right) \right\} \right\} ")]
    [InlineData(@"\{\}", @"\emptyset ")]
    [InlineData(@"\{1\}", @"\left\{ 1\right\} ")]
    [InlineData(@"\{1,2\}", @"\left\{ 1,2\right\} ")]
    [InlineData(@"\{x,y\}", @"\left\{ x,y\right\} ")]
    [InlineData(@"\{\sqrt[3]2,\frac34,\sin^2x\}", @"\left\{ 2^{\frac{1}{3}},\frac{3}{4},\sin \left( x\right) ^2\right\} ")]
    public void Sets(string latex, string converted) => Test(latex, converted, null);
    [Theory]
    [InlineData(@"\emptyset\cup\{2\}", @"\left\{ 2\right\} ")]
    [InlineData(@"\{1\}\cup\{2\}", @"\left\{ 1,2\right\} ")]
    [InlineData(@"\{3,4\}\cap\emptyset", @"\left( \left\{ 3,4\right\} \cap \right) \left( \emptyset \right) ")]
    [InlineData(@"\{3,4\}\cap\{4,5\}", @"\left\{ 4\right\} ")]
    [InlineData(@"\{2,3,4\}\setminus\{4\}", @"\left\{ 2,3\right\} ")]
    [InlineData(@"\{3\}^\complement", @"\left\{ \left\{ z\in \mathbb{C}:\Re \left( z\right) \in \left( -\infty ,3\right] \wedge \Im \left( z\right) \in \left( -\infty ,0\right) \right\} ,\left\{ z\in \mathbb{C}:\Re \left( z\right) \in \left( -\infty ,3\right) \wedge \Im \left( z\right) \in \left[ 0,\infty \right) \right\} ,\left\{ z\in \mathbb{C}:\Re \left( z\right) \in \left[ 3,\infty \right) \wedge \Im \left( z\right) \in \left( 0,\infty \right) \right\} ,\left\{ z\in \mathbb{C}:\Re \left( z\right) \in \left( 3,\infty \right) \wedge \Im \left( z\right) \in \left( -\infty ,0\right] \right\} \right\} ")]
    public void SetOperations(string latex, string converted) => Test(latex, converted, null);
    [Theory]
    [InlineData(@"(1,2)", @"\left\{ \left( 1,2\right) \right\} ")]
    [InlineData(@"[1,2)", @"\left\{ \left[ 1,2\right) \right\} ")]
    [InlineData(@"(1,2]", @"\left\{ \left( 1,2\right] \right\} ")]
    [InlineData(@"[1,2]", @"\left\{ \left[ 1,2\right] \right\} ")]
    [InlineData(@"(2,1)", @"\left\{ \left( 2,1\right) \right\} ")]
    [InlineData(@"[2,1)", @"\left\{ \left[ 2,1\right) \right\} ")]
    [InlineData(@"(2,1]", @"\left\{ \left( 2,1\right] \right\} ")]
    [InlineData(@"[2,1]", @"\left\{ \left[ 2,1\right] \right\} ")]
    [InlineData(@"[1,2]\setminus\{1\}", @"\left\{ \left( 1,2\right] \right\} ")]
    [InlineData(@"[1,2]\setminus\{2\}", @"\left\{ \left[ 1,2\right) \right\} ")]
    [InlineData(@"[1,2]\cup(2,3)", @"\left\{ \left[ 1,2\right] ,\left( 2,3\right) \right\} ")]
    [InlineData(@"[1,2]\cap[1.5,1.6]", @"\left\{ \left[ \frac{3}{2},\frac{8}{5}\right] \right\} ")]
    [InlineData(@"[1.5,1.5]", @"\left\{ \frac{3}{2}\right\} ")]
    [InlineData(@"(1.5,1.5]", @"\emptyset ")]
    [InlineData(@"[1.5,1.5)", @"\emptyset ")]
    [InlineData(@"(1.5,1.5)", @"\emptyset ")]
    [InlineData(@"[xy,xy]", @"\left\{ x y\right\} ")]
    [InlineData(@"(xy,xy]", @"\emptyset ")]
    [InlineData(@"[xy,xy)", @"\emptyset ")]
    [InlineData(@"(xy,xy)", @"\emptyset ")]
    public void Intervals(string latex, string converted) => Test(latex, converted, null);
    [Theory]
    [InlineData(@"\>\:\pi", @"\pi ", @"\pi ")]
    [InlineData(@"3\quad 2", @"3 2", "6")]
    [InlineData(@"a\textstyle b\displaystyle", @"a b", @"a b")]
    [InlineData(@"a^6+2a^6 % should be 3a^6", @"a^6+2 a^6", @"3 a^6")]
    [InlineData(@"4+\ \mkern1.5mu3", @"4+3", "7")]
    public void SkipInvisible(string latex, string converted, string output) => Test(latex, converted, output);
    [Theory]
    [InlineData("\\sqrt{2}", "\\sqrt{2}", "\\sqrt{2}")]
    //[InlineData(@"sqrt2^2", @"left( \sqrt{2}\right) ^2", "2")]
    //[InlineData(@"\sqrt[3]2", @"2^{\frac{1}{3}}", @"\sqrt[3]{2}")]
    //[InlineData(@"\sqrt[3/2]2", @"2^{\frac{1}{\frac{3}{2}}}", @"\sqrt[3]{2}^2")]
    public void BugTest(string latex, string converted, string output) => Test(latex, converted, output);

  }
}
