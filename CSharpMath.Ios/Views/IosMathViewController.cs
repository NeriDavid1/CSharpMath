﻿using System;
using UIKit;

namespace CSharpMath.Ios
{
  public class IosMathViewController: UIViewController
  {
    public IosMathViewController()
    {
    }

    private const string QuadraticFormula = @"x = -b \pm \frac{\sqrt{b^2-4ac}}{2a}";
    private const string NestedRadical = @"\sqrt{\sqrt{x}}";
    private const string Radical = @"\sqrt{3}";
    private const string RadicalSum = @"2 + \sqrt{3}";
    private const string Fraction = @"\frac{2}{34}";
    private const string RadicalFraction = @"2+ \frac{\sqrt{3}}{2}";
    private const string IntPlusFraction = @"1+\frac23";
    private const string Matrix = @"\begin{pmatrix}
                           a & b\\ c & d
                            \end{pmatrix}
                            \begin{pmatrix}
                            \alpha & \beta \\ \gamma & \delta
                            \end{pmatrix} = 
                            \begin{pmatrix}
                            a\alpha + b\gamma & a\beta + b \delta \\
                            c\alpha + d\gamma & c\beta + d \delta 
                            \end{pmatrix}";
    private const string ShortMatrix = @"\begin{pmatrix} a & b\\ c & d \end{pmatrix}";
    private const string VeryShortMatrix = @"\begin{pmatrix}2\end{pmatrix}";
    private const string EmptyMatrix = @"\begin{pmatrix}\end{pmatrix}";
    private const string LeftRight = @"\left(\frac23\right)";
    private const string LeftRightMinus = @"\left(\frac23\right)-";
    private const string LeftSide = @"\frac{1}{\left(\sqrt{\phi \sqrt{5}}-\phi\right) e^{\frac25 \pi}}";
    private const string RightSide = @"1+\frac{e^{-2\pi}} {1 +\frac{e^{-4\pi}} {1+\frac{e^{-6\pi}} {1+\frac{e^{-8\pi}} {1+\cdots} } } }";
    private const string DeeplyNestedFraction = @"\frac{1}{\left(\sqrt{\phi \sqrt{5}}-\phi\right) e^{\frac25 \pi}} = 1+\frac{e^{-2\pi}} {1 +\frac{e^{-4\pi}} {1+\frac{e^{-6\pi}} {1+\frac{e^{-8\pi}} {1+\cdots} } } }";
    private const string NestedFraction = @"\frac{e^{-6\pi}} {1+\frac{e^{-8\pi}} {1+\cdots} }";
    private const string Exponential = @"e^2";
    private const string ExponentWithFraction = @"e^{4\frac25}";
    private const string ExponentWithProduct = @"e^{2x}";
    private const string ExponentWithPi = @"e^{2\pi}";
    private const string Pi = @"\pi";
    private const string Phi = @"\phi";
    private const string FractionWithRoot = @"\frac{1}{\sqrt{2}}";
    private const string SomeLimit = @"\lim_{x\to\infty}\frac{e^2}{1-x}=\limsup_{\sigma}5";
    private const string SimpleLimit = @"\lim_{x\to\infty}3=3";
    private const string SomeIntegral = @"\int_{0}^{\infty}e^x \,dx=\oint_0^{\Delta}5\Gamma";
    private const string ShortIntegral = @"\int_0^1";
    private const string Commands = @"5\times(-2 \div 1) = -10";
    private const string SummationWithCup = @"\sum_{n=1}^{\infty}\frac{1+n}{1-n}=\bigcup_{1}C\cup B";
    private const string DoubleSummation = @"\sum \sum";
    private const string SummationBigCup = @"234 \bigcup_1";
    private const string SummationWithLimits = @"\sum_{n=1}^{\infty}";
    private const string Taylor = @"\begin{eqnarray} e^x  &=&  \sum_{x=0}^{\infty}\frac{x^n}{n!}
    \\
    \\ 
    \sin(x) &=& \sum_{x=0}^{\infty}(-1)^n\frac{(2x+1)^n}{(2n)!} 
    \\ \\
    -\ln(1-x)   &=& \sum_{x=0}^{\infty}\frac{x^n}{n}  \ \ \ \ \ (-1 <= x < 1) \end{eqnarray}";
    private const string TwoSin = @"2 \sin";

    public override void ViewDidLoad()
    {
      View.BackgroundColor = UIColor.White;
      /// WJWJWJ Set latex here.
      var latexView = IosMathLabels.LatexView(Matrix, 15);
      var size = latexView.SizeThatFits(new CoreGraphics.CGSize(370, 180));
      latexView.Frame = new CoreGraphics.CGRect(0, 20, 370, 180);
      View.Add(latexView);
    }
  }
}