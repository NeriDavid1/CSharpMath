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
      return MathListConverator.ConvertToMathString(atoms);
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
    /// Finds Numbers that comes before a fraction and converts them to one fraction
    /// </summary>
    /// <param name="mathList"></param>
    /// <returns></returns>

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