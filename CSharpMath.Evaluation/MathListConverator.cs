using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using AngouriMath;
using CSharpMath.Atom;
using CSharpMath.Atom.Atoms;

public static class MathListConverator {

  public static bool HandleMixedNumber { get; set; } = false;
  public static string ConvertToMathString(IList<MathAtom> atoms) {
    StringBuilder output = new StringBuilder();

    for (int i = 0; i < atoms.Count; i++) {
      MathAtom atom = atoms[i];

      // Convert the "base" part of the atom
      string baseString = AtomToString(atom, atoms, ref i);
      output.Append(baseString);

      // After the base is converted, handle superscript & subscript
      if (atom.Superscript?.Count > 0) {
        output.Append($"^({ConvertToMathString(atom.Superscript)})");
      }
      if (atom.Subscript?.Count > 0) {
        output.Append($"({ConvertToMathString(atom.Subscript)})");
      }
    }

    return output.ToString();
  }

  private static string AtomToString(MathAtom atom, IList<MathAtom> atoms, ref int i) {
    return atom switch {
      // -------------------------------------------------------------------
      // Comment atom: we ignore it
      // -------------------------------------------------------------------
      Comment { Nucleus: var _ } => "",

      // -------------------------------------------------------------------
      // Fractions
      // -------------------------------------------------------------------
      Fraction fraction => ProcessFraction(fraction, atoms, ref i),

      // -------------------------------------------------------------------
      // Radicals
      // -------------------------------------------------------------------
      Radical radical => ProcessRadical(radical),

      // -------------------------------------------------------------------
      // "Inner" parentheses, abs, etc.
      // -------------------------------------------------------------------
      Inner { LeftBoundary: { Nucleus: "|" }, InnerList: var list }
          => $"abs({ConvertToMathString(list)})",

      Inner { LeftBoundary: { Nucleus: "〈" }, InnerList: var list }
          => $"({ConvertToMathString(list)})",

      Inner { LeftBoundary: { Nucleus: null }, InnerList: var list }
          => $"({ConvertToMathString(list)})",

      Inner { InnerList: var list }
          => $"({ConvertToMathString(list)})",

      // -------------------------------------------------------------------
      // Accent, Colored, ColorBox, RaiseBox, etc. (currently no special logic)
      // -------------------------------------------------------------------
      Accent => "",
      Colored => "",
      ColorBox => "",
      RaiseBox => "",

      // -------------------------------------------------------------------
      // Variable
      // -------------------------------------------------------------------
      Variable variable => ProcessVariable(variable, atoms, ref i),

      // -------------------------------------------------------------------
      // Binary operator: + - × ÷ etc.
      // -------------------------------------------------------------------
      BinaryOperator binOp => ProcessBinaryOperator(binOp),

      // -------------------------------------------------------------------
      // Large operator (especially ∫ integrals)
      // -------------------------------------------------------------------
      LargeOperator largeOp => ProcessLargeOperator(largeOp, atoms, ref i),

      // -------------------------------------------------------------------
      // Ordinary
      // -------------------------------------------------------------------
      Ordinary ordinary => ProcessOrdinary(ordinary),

      // -------------------------------------------------------------------
      // Relation: ≤ ≥ ≠ ∶ etc.
      // -------------------------------------------------------------------
      Relation relation => ProcessRelation(relation),

      // -------------------------------------------------------------------
      // Number
      // -------------------------------------------------------------------

      Number number => ProcessNumber(number, atoms, ref i),


      // -------------------------------------------------------------------
      // Default case: e.g. simple text, or unknown
      // -------------------------------------------------------------------
      _ => ProcessDefault(atom)
    };
  }

  private static string ProcessFraction(Fraction fraction, IList<MathAtom> atoms, ref int i) {

    // if hand
    return $"(({ConvertToMathString(fraction.Numerator)})/({ConvertToMathString(fraction.Denominator)}))";

  }

  private static string ProcessRadical(Radical radical) {
    string degree = ConvertToMathString(radical.Degree);
    if (string.IsNullOrEmpty(degree))
      degree = "2";

    string radicand = ConvertToMathString(radical.Radicand);
    return $"( {radicand} )^( 1/{degree} )";
  }


  private static string ProcessRelation(Relation relation) {
    return relation.Nucleus switch {
      "≤" => "<=",
      "≥" => ">=",
      "≠" => "!=",
      "∶" => "/",
      _ => relation.Nucleus
    };
  }

  private static string ProcessVariable(Variable variable, IList<MathAtom> atoms, ref int i) {
    StringBuilder sb = new StringBuilder();
    sb.Append(variable.Nucleus);

    // If next atom is a variable, insert '*'
    if (i + 1 < atoms.Count && atoms[i + 1] is Variable) {
      sb.Append("*");
    }

    return sb.ToString();
  }

  private static string ProcessBinaryOperator(BinaryOperator binOp) {
    return binOp.Nucleus switch {
      "·" => "*",
      "−" => "-",
      "×" => "*",
      "÷" => "/",
      _ => binOp.Nucleus
    };
  }

  private static string ProcessLargeOperator(LargeOperator largeOperator, IList<MathAtom> atoms, ref int i) {
    // If it's not ∫, just return the nucleus
    if (largeOperator.Nucleus != "∫") {
      return largeOperator.Nucleus;
    }

    // It's an integral
    if (largeOperator.Subscript.Count > 0 || largeOperator.Superscript.Count > 0) {
      // Definite integral
      return ProcessDefiniteIntegral(largeOperator, atoms, ref i);
    } else {
      // Indefinite integral
      return ProcessIndefiniteIntegral(largeOperator, atoms, ref i);
    }
  }
  private static string ProcessDefiniteIntegral(LargeOperator largeOperator, IList<MathAtom> atoms, ref int i) {
    // Find the variable to integrate w.r.t.
    bool foundWRT = false;
    int idxOfWRT = i + 1;
    while (!foundWRT && idxOfWRT + 1 < atoms.Count) {
      foundWRT = atoms[idxOfWRT] is Variable intWRTMarker
                 && intWRTMarker.Nucleus == "d"
                 && atoms[idxOfWRT + 1] is Variable;
      idxOfWRT++;
    }

    // Build default bounds
    LaTeXParser.MathListFromLaTeX(@"\infty").Deconstruct(out MathList defaultUpperBound, out _);
    LaTeXParser.MathListFromLaTeX(@"-\infty").Deconstruct(out MathList defaultLowerBound, out _);

    // Convert subscript & superscript to strings -> Entities
    var upperBound = MathS.FromString(
        ConvertToMathString(
            largeOperator.Superscript.Count == 0 ? defaultUpperBound : largeOperator.Superscript
        )
    );
    var lowerBound = MathS.FromString(
        ConvertToMathString(
            largeOperator.Subscript.Count == 0 ? defaultLowerBound : largeOperator.Subscript
        )
    );

    // Get the integrand atoms: skip the integral symbol & skip "d var"
    var intAtoms = atoms.Skip(i + 1).Take(idxOfWRT - i - 2).ToList();

    // Identify the variable
    var varWRT = MathS.Var(foundWRT ? atoms[idxOfWRT].Nucleus : "x");

    // Compute integral
    var antiderivative = MathS.FromString(ConvertToMathString(intAtoms))
                            .Integrate(varWRT)
                            .Simplify();

    // Evaluate definite integral
    var definiteValue = (antiderivative.Substitute(varWRT, upperBound)
                         - antiderivative.Substitute(varWRT, lowerBound))
                         .Simplify();

    // Skip the integral range in the main loop
    i = idxOfWRT;

    return definiteValue.ToString();
  }

  private static string ProcessIndefiniteIntegral(LargeOperator largeOperator, IList<MathAtom> atoms, ref int i) {
    // Find the variable to integrate w.r.t.
    bool foundWRT = false;
    int idxOfWRT = i + 1;
    while (!foundWRT && idxOfWRT + 1 < atoms.Count) {
      foundWRT = atoms[idxOfWRT] is Variable intWRTMarker
                 && intWRTMarker.Nucleus == "d"
                 && atoms[idxOfWRT + 1] is Variable;
      idxOfWRT++;
    }

    // Get the integrand atoms
    var intAtoms = atoms.Skip(i + 1).Take(idxOfWRT - i - 2).ToList();

    // Variable
    var varWRT = MathS.Var(foundWRT ? atoms[idxOfWRT].Nucleus : "x");

    // Integrate
    var indefinite = MathS.FromString(ConvertToMathString(intAtoms))
                         .Integrate(varWRT)
                         .Simplify();

    // Move loop index forward
    i = idxOfWRT;

    return indefinite.ToString();
  }

  private static string ProcessOrdinary(Ordinary ordinary) {
    var nuc = ordinary.Nucleus;
    // Replace the minus sign if needed
    nuc = Regex.Replace(nuc, @"−", "-");

    return nuc switch {
      "∞" => "+oo",
      "-∞" => "-oo",
      _ => nuc
    };
  }

  private static string ProcessDefault(MathAtom atom) {
    // For any atom that doesn't fit the known cases
    // If it might contain a minus sign, handle it:
    string nucl = Regex.Replace(atom.Nucleus, @"−", "-");
    return nucl;
  }

    private static string ProcessNumber(Number atom, IList<MathAtom> atoms, ref int i)
    {
        if (!ShouldHandleMixedNumber(atoms, i))
        {
            return atom.Nucleus;
        }
        return ProcessMixedNumber(atoms, ref i);

        static bool ShouldHandleMixedNumber(IList<MathAtom> atoms, int index)
        {
            return HandleMixedNumber && IsFractionAhead(atoms, index);
        }

        static bool IsFractionAhead(IList<MathAtom> atoms, int index)
        {
            int tempIndex = index + 1; // Start checking from the next atom
            while (tempIndex < atoms.Count && atoms[tempIndex] is Number)
            {
                tempIndex++;
            }
            return tempIndex < atoms.Count && atoms[tempIndex] is Fraction;
        }
    }

    private static string ProcessMixedNumber(IList<MathAtom> atoms, ref int i)
    {
        string wholeNumber = GetWholeNumberPart(atoms, ref i);

        if (i >= atoms.Count || !(atoms[i] is Fraction fraction))
        {
            // No fraction found; return the whole number
            return wholeNumber;
        }

        string fractionString = ProcessFraction(fraction, atoms, ref i);


        // Combine the whole number and fraction
        return $"(({wholeNumber})+{fractionString})";

        static string GetWholeNumberPart(IList<MathAtom> atoms, ref int index)
        {
            StringBuilder sb = new StringBuilder();
            while (index < atoms.Count && atoms[index] is Number number)
            {
                sb.Append(number.Nucleus);
                index++;
            }
            return sb.ToString();
        }
    }
}

