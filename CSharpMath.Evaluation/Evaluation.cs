using System;
using System.Collections.Generic;
using System.Linq;
using AngouriMath;
using AngouriMath.Core;

namespace CSharpMath {
  using System.Collections;
  using Atom;
  using Atoms = Atom.Atoms;
  using Structures;
  using AngouriMath.Extensions;
  using CSharpMath.Atom.Atoms;
  using System.Text.RegularExpressions;
  using AngouriMath.Core.Exceptions;
  using System.Text;

  public static partial class Evaluation {
    enum Precedence {
      DefaultContext,
      BraceContext,
      BracketContext,
      ParenthesisContext,
      // Lowest
      Comma,
      SetOperation,
      AddSubtract,
      MultiplyDivide,
      FunctionApplication,
      UnaryPlusMinus,
      PercentDegree
      // Highest
    }
    /// מסקנות שכרגע הוא לא רשם את הטסטרים ושיש בעיה עם הסינטקס שלו צריך לשכתב את הפונקציה

    public static Entity ParseExpression(string latexIn) {
      string StringMath = ConvertToMathString(latexIn)!;
      return latexIn.ToEntity();
    }
    public static Entity ParseExpression(MathList mathlist) {
      string StringMath = ConvertToMathString(mathlist)!;
      return mathlist.ToEntity();
    }
    public static string? ConvertToMathString(string latexIn) {
      var parseResult = LaTeXParser.MathListFromLaTeX(latexIn);
      if (parseResult.Error == null) {
        parseResult.Deconstruct(out MathList atoms, out _);
        return ConvertToMathString(atoms);
      }
      return null;
    }

    public static string ConvertToMathString(IList<MathAtom> atoms) {
      StringBuilder output = new StringBuilder();

      for (int i = 0; i < atoms.Count; i++) {
        var atom = atoms[i];
        switch (atom) {
          case Comment { Nucleus: var comment }:
            break;
          case Fraction fraction:
            output.Append($"({ConvertToMathString(fraction.Numerator)})/({ConvertToMathString(fraction.Denominator)})");
            break;
          case Radical radical:
            string degree = ConvertToMathString(radical.Degree);
            if (degree == "") degree = "2";
            string radicand = ConvertToMathString(radical.Radicand);
            output.Append($"( {radicand} )^( 1/{degree} )");
            break;
          case Inner { LeftBoundary: { Nucleus: null }, InnerList: var list }:
            output.Append($"({ConvertToMathString(list)})");
            break;
          case Inner { LeftBoundary: { Nucleus: "〈" }, InnerList: var list }:
            output.Append($"({ConvertToMathString(list)})");
            break;
          case Inner { LeftBoundary: { Nucleus: "|" }, InnerList: var list }:
            output.Append($"abs({ConvertToMathString(list)})");
            break;
          case Inner { LeftBoundary: var left, InnerList: var list }:
            output.Append($"({ConvertToMathString(list)})");
            break;
          case Accent accent:
          case Colored colored:
          case ColorBox colorBox:
          case RaiseBox r:
            break;
          case Variable variable:
            output.Append(atom.Nucleus);
            if (i + 1 < atoms.Count && atoms[i + 1] is Variable)
              output.Append("*");
            break;
          case BinaryOperator binaryOperator:
            if (binaryOperator.Nucleus == "·") {
              output.Append("*");
            } else if (binaryOperator.Nucleus == "−") {
              output.Append("-");
            } else if (binaryOperator.Nucleus == @"×") {
              output.Append("*");
            } else {
              output.Append(binaryOperator.Nucleus);
            }
            break;
          case LargeOperator largeOperator:
            if (largeOperator.Nucleus == "∫") {
              // Figure out which kind of intergral we're dealing with
              if (largeOperator.Subscript.Count > 0 || largeOperator.Superscript.Count > 0) {
                // Definite integral

                // Find the variable to integrate with respect to
                bool foundWRT = false;
                int idxOfWRT = i + 1;
                while (!foundWRT && idxOfWRT + 1 < atoms.Count) {
                  foundWRT = atoms[idxOfWRT] is Variable intWRTMarker && intWRTMarker.Nucleus == "d"
                      && atoms[idxOfWRT + 1] is Variable;
                  idxOfWRT++;
                }

                // Get the bounds of integration
                LaTeXParser.MathListFromLaTeX(@"\infty").Deconstruct(out MathList defaultUpperBound, out _);
                LaTeXParser.MathListFromLaTeX(@"-\infty").Deconstruct(out MathList defaultLowerBound, out _);
                var upperBound = MathS.FromString(
                    ConvertToMathString(largeOperator.Superscript.Count == 0 ? defaultUpperBound : largeOperator.Superscript)
                );
                var lowerBound = MathS.FromString(
                    ConvertToMathString(largeOperator.Subscript.Count == 0 ? defaultLowerBound : largeOperator.Subscript)
                );

                // Get the list of atoms that we need to integrate
                // i+1 to skip the integral symbol, and idxOfWRT-i-2 to remove the dx
                var intAtoms = atoms.Skip(i + 1).Take(idxOfWRT - i - 2).ToList();

                // Calculate the integral of the expression
                var varWRT = MathS.Var(foundWRT ? atoms[idxOfWRT].Nucleus : "x");
                var antiderivative = MathS.FromString(ConvertToMathString(intAtoms)).Integrate(varWRT).Simplify();
                output.Append((antiderivative.Substitute(varWRT, upperBound) - antiderivative.Substitute(varWRT, lowerBound)).Simplify().ToString());

                // Make sure the atoms involved in the integration aren't parsed again
                i = idxOfWRT;
                continue;
              } else {
                // Indefinite integral

                // Find the variable to integrate with respect to
                bool foundWRT = false;
                int idxOfWRT = i + 1;
                while (!foundWRT && idxOfWRT + 1 < atoms.Count) {
                  foundWRT = atoms[idxOfWRT] is Variable intWRTMarker && intWRTMarker.Nucleus == "d"
                      && atoms[idxOfWRT + 1] is Variable;
                  idxOfWRT++;
                }

                // Get the list of atoms that we need to integrate
                // i+1 to skip the integral symbol, and idxOfWRT-i-2 to remove the dx
                var intAtoms = atoms.Skip(i + 1).Take(idxOfWRT - i - 2).ToList();

                // Calculate the integral of the expression
                var varWRT = MathS.Var(foundWRT ? atoms[idxOfWRT].Nucleus : "x");
                output.Append(MathS.FromString(ConvertToMathString(intAtoms)).Integrate(varWRT).Simplify().ToString());

                // Make sure the atoms involved in the integration aren't parsed again
                i = idxOfWRT;
                continue;
              }
            } else {
              output.Append(atom.Nucleus);
            }

            break;
          case Ordinary ordinary: // to add
            var nuc = ordinary.Nucleus;
            if (nuc == "∞") {
              output.Append(@"+oo");
            } else if (nuc == "-∞") {
              output.Append(@"-oo");
            } else {
              output.Append(nuc);
            }
            break;
          case Relation relation:
            if(relation.Nucleus == "≤") {
              output.Append("<=");
            } else if(relation.Nucleus == "≥") {
              output.Append(">=");
               } else if(relation.Nucleus == "≠") {
                 output.Append("!=");
            }
              else if (relation.Nucleus == "∶") {
                output.Append("/");
              }
             else {
              output.Append(relation.Nucleus);
            }
            break;
          default:
            // for numbers etc.
            string nucl = atom.Nucleus;
            // Replace the minus to the right minus sign
            nucl = Regex.Replace(nucl, @"−", "-");
            output.Append(nucl);
            break;
        }
        if (atom.Superscript.Count > 0) {
          output.Append($"^({ConvertToMathString(atom.Superscript)})");
        }
        if (atom.Subscript.Count > 0) {
          output.Append($"({ConvertToMathString(atom.Subscript)})");
        }
      }
      return output.ToString();
    }

    public static string HandleSpecialCases(string mathString) {
      return PlusMinus(mathString);
      static string PlusMinus(string inputString) {
        string plusminusString = "±";
        StringBuilder output = new StringBuilder(inputString);
        Func<int> firstPMIndex = () => output.ToString().IndexOf(plusminusString);
        int index = firstPMIndex();
        while (index != -1) {
          var copystring = output.ToString();
          var PlusCopy = copystring.Remove(index, plusminusString.Length).Insert(index, "+");
          PlusCopy = HandleSpecialCases(PlusCopy);
          var minusCopy = copystring.Remove(index, plusminusString.Length).Insert(index, "-");
          minusCopy = HandleSpecialCases(minusCopy);
          output.Clear().Append(PlusCopy).Append(" or ").Append(minusCopy);
          index = firstPMIndex();
        }
        return output.ToString();
      }
    }

    /// <summary>
    /// Parses a LaTeX command and returns a list of AngouriMath-ready expressions
    /// </summary>
    public static List<string> ParseParameters(string latexIn) {
      var matches = Regex.Matches(latexIn, @"\{(.*?)\}");
      List<string> parameters = new List<string>(matches.Count);
      foreach (Match m in matches) {
        // Remove curly brackets
        string latex = m.Value.Remove(0, 1).Remove(m.Value.Length - 2);
        parameters.Add(ConvertToMathString(latex)!);
      }
      return parameters;
    }

    /// <summary>
    /// A dirty way to convert to an AngouriMath string without actually parsing LaTeX.
    /// </summary>
    /// <remarks>
    /// This converter is much more picky than <see cref="ConvertToMathString(string)"/>,
    /// but in most cases it will be faster.
    /// </remarks>
    public static string DirtyConvertToMathString(string latexIn) {
      string output = "";
      var matches = Regex.Matches(latexIn.Replace(@"\ ", " "), @"\\?(\{.*\}|[^\\\s])*");
      foreach (Match m in matches) {
        string lPart = m.Value;
        if (lPart.Length == 0)
          continue;

        if (Char.IsDigit(lPart[0]) && Char.IsLetter(lPart.Last()) && lPart.All(c => Char.IsLetterOrDigit(c))) {
          int startOfVar = 1;
          while (Char.IsDigit(lPart[startOfVar])) {
            if (startOfVar + 1 < lPart.Length)
              startOfVar++;
          }
          lPart = lPart.Insert(startOfVar, "*");
        } else if (Char.IsLetter(lPart[0]) && Char.IsDigit(lPart.Last()) && lPart.All(c => Char.IsLetterOrDigit(c))) {
          int startOfNum = 1;
          while (Char.IsLetter(lPart[startOfNum])) {
            if (startOfNum + 1 < lPart.Length)
              startOfNum++;
          }
          lPart = lPart.Insert(startOfNum, "*");
        } else if (!lPart.StartsWith("\\")) {
          // Do nothing, but skip the branches that check for LaTeX commands
        } else if (lPart.StartsWith(@"\,") || lPart.StartsWith(@"\:") || lPart.StartsWith(@"\;") ||
              lPart.StartsWith(@"\,") || lPart.StartsWith(@"\ ")) {
          // Replace the LaTeX command with a space
          lPart = " " + lPart.Remove(0, 2);
        } else if (lPart.StartsWith(@"\quad")) {
          lPart = "  " + lPart.Remove(0, @"\quad".Length);
        } else if (lPart.StartsWith(@"\qquad")) {
          lPart = "   " + lPart.Remove(0, @"\qquad".Length);
        } else if (lPart.StartsWith(@"\cdot")) {
          lPart = lPart.Replace(@"\cdot", "*");
        } else if (lPart.StartsWith(@"\sin") || lPart.StartsWith(@"\cos") || lPart.StartsWith(@"\tan") ||
              lPart.StartsWith(@"\arcsin") || lPart.StartsWith(@"\arccos") || lPart.StartsWith(@"\arctan") ||
              lPart.StartsWith(@"\csc") || lPart.StartsWith(@"\sec") || lPart.StartsWith(@"\cot")) {
          lPart = lPart.Remove(0, 1); // Just remove the slash
        } else if (lPart.StartsWith(@"\left")) {
          lPart = lPart.Remove(0, @"\left".Length);
        } else if (lPart.StartsWith(@"\right")) {
          lPart = lPart.Remove(0, @"\right".Length);
        } else if (lPart.StartsWith(@"\over")) {
          lPart = lPart.Replace(@"\over", "/");
        } else if (lPart.StartsWith(@"\frac")) {
          var parameters = ParseParameters(lPart);

          // Handle derivatives
          if (parameters.Count >= 3 && parameters[0].StartsWith("d") && parameters[1].StartsWith("d")) {
            // Get the variable to derive with respect to
            string varName = Regex.Match(parameters[1], @"d(\S)$").Value.Substring(1);
            var varWRT = MathS.Var(varName);

            // Derive the function with respect to varWRT
            var derFunc = MathS.FromString(parameters[2], true).Differentiate(varWRT);
            lPart = derFunc.Simplify().ToString();
          } else {
            lPart = $"({parameters[0]})/({parameters[1]})";
          }
        } else if (lPart.StartsWith(@"\sqrt")) {
          var parameters = ParseParameters(lPart);
          lPart = $"sqrt({parameters[0]})";
        } else if (lPart.StartsWith(@"\int")) {
          // User must put paratheses around the expression to integrate
          var parameters = ParseParameters(lPart);
          if (parameters.Count == 3) {
            string expression = parameters[2];

            // Get the variable to integrate with respect to
            string varName = Regex.Match(expression, @"d(\S)$").Value;
            if (!String.IsNullOrEmpty(varName)) {
              expression = expression.Replace(varName, String.Empty);
              varName = varName.Substring(1);
            } else
              varName = "x";
            var varWRT = MathS.Var(varName);

            // TODO: Support expressions for start and end
            // Get start and end of interval
            double start = Double.Parse(parameters[0]);
            double end = Double.Parse(parameters[1]);

            var intFunc = MathS.FromString(expression).Integrate(varWRT);
            lPart = (intFunc.Substitute(varWRT, end) - intFunc.Substitute(varWRT, start)).Simplify().ToString();
          } else if (parameters.Count == 1) {
            string expression = parameters[0];

            // Get the variable to integrate with respect to
            string varName = Regex.Match(expression, @"d(\S)$").Value;
            if (!String.IsNullOrEmpty(varName)) {
              expression = expression.Replace(varName, String.Empty);
              varName = varName.Substring(1);
            } else
              varName = "x";
            var varWRT = MathS.Var(varName);

            lPart = MathS.FromString(expression).Integrate(varWRT).Simplify().ToString();
          }
        }

        output += lPart;
      }
      return output;
    }

  }
}