using System;
using System.Collections.Generic;
using System.Text;

namespace CSharpMath.Editor {
  using System;
  using System.Collections.Generic;
  using Atom;
  using CSharpMath.Atom.Atoms;
  using Structures;
  using Atoms = Atom.Atoms;

  public class LatexMathKeyboard {
    public LatexMathKeyboard() {
      navigation = new MathNavigation(MathList);
    }
    private enum Diraction : byte {
      Up,
      Down,
      Left,
      Right
    }
    public MathList MathList { get; private set; } = new MathList();
    public string LaTeX => LaTeXParser.MathListToLaTeX(MathList).ToString();

    public string _LatexWithCert() {
      KeyPress(MathKeyboardInput.Caret);
      string LatexHandle = LaTeX;
      KeyPress(MathKeyboardInput.Backspace);
      return LatexHandle;
    }

    public MathNavigation navigation;

    // unnessery since we have navigation but to not break the code we will keep it for now
    internal MathListIndex _insertionIndex = MathListIndex.Level0Index(0);
    public MathListIndex InsertionIndex {
      get => _insertionIndex;
      set {
        _insertionIndex = value;
      }
    }
    public bool HasText => MathList.Atoms.Count > 0;

    public void KeyPress(params MathKeyboardInput[] inputs) {
      foreach (var input in inputs) KeyPress(input);
    }
    public virtual void KeyPress(MathKeyboardInput input) {
      ConvertClick(input);
      void HandleScriptButtonNew(bool isSuperScript) {
        // if the script is empty add empty atom and move to the right list
        if (!ThreisPreviousAtom()) {
          CreateEmptyAtom();
          return;
        }
        var prevAtom = navigation.GetCurrentAtom;
        if (prevAtom is null)
          throw new InvalidCodePathException("prevAtom is null");
        // check if the atom is non number who required a place holder
        if (IsFullPlaceholderRequired(prevAtom)) {
          CreateEmptyAtom();
          return;
        }
        // add placeholder if the script is empty
        var script = GetScript(prevAtom);
        if (script.IsEmpty()) {
          SetScript(prevAtom, LaTeXSettings.PlaceholderList);
          navigation.MoveUp();
          // move to the script that we added right now
          if (navigation.GetCurrentList != script) {
            navigation.MoveToScript(script);
          }
          navigation.Next();
          return;
        }
        navigation.MoveUp();
        bool ThreisPreviousAtom() { return (navigation.GetCurrentAtom != null); }
        MathList GetScript(MathAtom atom) => isSuperScript ? atom.Superscript : atom.Subscript;
        void SetScript(MathAtom atom, MathList value) => GetScript(atom).Append(value);
        void CreateEmptyAtom() {
          // Create an empty atom and move to the next atom because its a placeholder
          var emptyAtom = LaTeXSettings.Placeholder;
          SetScript(emptyAtom, LaTeXSettings.PlaceholderList);
          MathList.InsertNav(emptyAtom, ref navigation);
          navigation.MoveUp();
          navigation.Next();
        }
        static bool IsFullPlaceholderRequired(MathAtom mathAtom) =>
          mathAtom switch {
            Atoms.BinaryOperator _ => true,
            Atoms.UnaryOperator _ => true,
            Atoms.Relation _ => true,
            Atoms.Open _ => true,
            Atoms.Punctuation _ => true,
            _ => false
          };
      }
      void HandleSlashButtonNew() {
        // slash buttom is used to create fraction which taking all the previous atoms and put them in the numerator
        // for example if we have 1+2 and we press / we will get 1+2/square and another example is 123 and we press / we will get 123/square
        var numerator = new Stack<MathAtom>();
        var parenDepth = 0;

        for (; navigation.GetCurrentAtom is not null; navigation.Previous()) {
          switch (navigation.GetCurrentAtom, parenDepth) {
            case (null, _): throw new InvalidCodePathException("Invalid _insertionIndex");
            // Stop looking behind upon encountering these atoms unparenthesized
            case (Atoms.Open _, _) when --parenDepth < 0: goto stop;
            case (Atoms.Close a, _): parenDepth++; numerator.Push(a); break;
            case (Atoms.UnaryOperator _, 0): goto stop;
            case (Atoms.BinaryOperator _, 0): goto stop;
            case (Atoms.Relation _, 0): goto stop;
            case (Atoms.Fraction _, 0): goto stop;
            case (Atoms.Placeholder _, 0): goto stop;
            case (Atoms.Open _, _) when parenDepth < 0: goto stop;
            // We don't put this atom on the fraction
            case (var atom, _): numerator.Push(atom); break;
          }
        }
      stop: navigation.GetCurrentList.RemoveAtoms(navigation.Index + 1, numerator.Count);
        if (numerator.Count == 0) {
          numerator.Push(new Atoms.Number("1")); ;
          // so we didn't really find any numbers before this, so make the numerator 1
        }
        // if the previous atom is a fraction we will add a symbole of times between them
        if (navigation.GetCurrentAtom is Atoms.Fraction f) {
          MathList.InsertNav(LaTeXSettings.Times, ref navigation);
        }
        // add the fraction
        MathList.InsertNav(new Atoms.Fraction(
          new MathList(numerator),
          LaTeXSettings.PlaceholderList
        ), ref navigation);
        // move to the denominator
        navigation.NextList();
        navigation.Next();
      }
      void InsertInner(string left, string right) =>
        MathList.InsertNav(
          new Atoms.Inner(new Boundary(left), LaTeXSettings.PlaceholderList, new Boundary(right)), ref navigation);
      void MoveCursor(Diraction diraction) {
        switch (diraction) {
          case Diraction.Down:
            // todo
            break;
          case Diraction.Up:
            // todo
            break;
          case Diraction.Left:
            MoveCursorLeftNew();
            break;
          case Diraction.Right:
            MoveCursorRightNew();
            break;
        }
        void MoveCursorLeftNew() {
          if (MoveByPlaceHolder()) {
            // exmple //frac{square|}{2323} >> //frac{|square}{2323} since you cannot add atom before the placeholder
            navigation.Previous();
          }
          if (navigation.IsFirstIndex) {
            if (navigation.IsFirstList) {
              if (navigation.FirstAtomInAllLists) {
                return;
              }
              // go to parent list
              navigation.MoveDown();
              navigation.OnRightSide = false;
              // if parent was a container go to the previous list
              if (navigation.OnContainer) {
                navigation.Previous();
                navigation.OnContainer = false;
              }
              return;
            }
            // in case of two lists we will move to the previous list for exmple //frac{square}{|2323} >> //frac{square|}{2323}
            navigation.PreviousList();
            navigation.MoveToLastAtom();
            return;
          }
          if (navigation.OnRightSide) {
            // if we are on the right side of the atom we will check for scripts or containers in this order
            if (GoToScript()) {
              navigation.OnRightSide = false;
              return;
            }
            if (GoToContainer()) {
              navigation.OnRightSide = false;
              return;
            }
            // if we didn't find any scripts or containers we will move to prv atom
            navigation.Previous();
            return;
          }
          // check and move if there is a container
          if (GoToContainer()) {
            return;
          }
          navigation.Previous();
          navigation.OnRightSide = navigation.GetCurrentAtom?.HasScripts ?? false;
          return;
          bool MoveByPlaceHolder() {
            if (navigation.GetCurrentAtom is Placeholder placeholder) {
              if (navigation.GetCurrentList.Count == 1) {
                if (!navigation.FirstAtomInAllLists) {
                  return true;
                }
              }
            }
            return false;
          }
          bool GoToScript() {
            var CurrentAtom = navigation.GetCurrentAtom;
            if (CurrentAtom is null) return false;
            if (CurrentAtom.HasScripts) {
              navigation.MoveUp();
              navigation.MoveToLastList();
              navigation.MoveToLastAtom();
              return true;
            }
            return false;
          }
          bool GoToContainer() {
            var CurrentAtom = navigation.GetCurrentAtom;
            if (CurrentAtom is null) return false;
            if (CurrentAtom is IMathListContainer) {
              navigation.MoveUp(true);
              navigation.MoveToLastList();
              navigation.MoveToLastAtom();
              return true;
            }
            return false;
          }
        }
        void MoveCursorRightNew() {
          if (navigation.IsLastIndex) {
            if (navigation.IsLastList) {
              if (navigation.IsTheOnlyList) {
                // in case of last atom has scripts we will move to the script
                GoToScript();
                return;
              }

              // if is on continer it could move to scripts for example fraction{566}{44|}{88} 88 is the script
              navigation.OnRightSide = !navigation.OnContainer;

              navigation.MoveDown();
              return;
            }
            // in case of two lists we will move to the next list for exmple //frac{square|}{2323} >> //frac{square}{|2323}
            navigation.NextList();
            navigation.OnRightSide = false;
            // exmple //frac{|square}{2323} >> //frac{square|}{2323} since you cannot add atom before the placeholder
            RightIfPlaceHolder();
            return;
          }
          // go to script if not on the right side and there is a script
          if (GoToScript()) {
            return;
          }
          // default move to next atom
          navigation.Next();
          navigation.OnRightSide = false;

          // get in the container if there is one
          var currectatom = navigation.GetCurrentAtom;
          if (currectatom == null) return;
          if (currectatom is IMathListContainer) {
            navigation.MoveUp(true);
            RightIfPlaceHolder();
          }

          bool GoToScript() {
            var currectatom = navigation.GetCurrentAtom;
            if (!navigation.OnRightSide && currectatom != null && currectatom.HasScripts) {
              navigation.MoveUp();
              navigation.OnRightSide = false;
              RightIfPlaceHolder();
              return true;
            }
            return false;
          }
          void RightIfPlaceHolder() {
            if (navigation.GetNextAtom is Placeholder) {
              navigation.Next();
              navigation.OnRightSide = false;
            }
          }
        }
      }
      void DeleteBackwardsNew() {
        if (navigation.FirstAtomInAllLists) {
          return;
        }
        if (ShouldMoveLeft()) {
          MoveCursor(Diraction.Left);
          return;
        }
        MathList.RemoveLast(ref navigation);
        bool ShouldMoveLeft() {
          // if the current atom is a placeholder and it is the only atom in the list or last atom is a container 
          return (navigation.OnRightSide && navigation.GetCurrentAtom is MathAtom M && M.HasScripts || navigation.GetCurrentAtom is IMathListContainer);
        }
      }
      void InsertAtomNew(MathAtom atom) {
        MathList.InsertNav(atom, ref navigation);
      }
      void InsertSymbolNameNew(string name, bool subscript = false, bool superscript = false) {
        // get atomcommand
        var atom =
          LaTeXSettings.AtomForCommand(name) ??
            throw new InvalidCodePathException("Looks like someone mistyped atom symbol name...");
        // insert atom symbole
        InsertAtomNew(atom);
        // \int _2^{\square } expected
        // insert scripts (first the super and then the sub) by the needs
        switch (subscript, superscript) {
          case (true, true):
            HandleScriptButtonNew(true);
            navigation.MoveDown();
            HandleScriptButtonNew(false);
            break;
          case (false, true):
            HandleScriptButtonNew(true);
            break;
          case (true, false):
            HandleScriptButtonNew(false);
            break;
          case (false, false):
            break;
        }
      }
      // Convert the input to a command and insert it or handle it
      void ConvertClick(MathKeyboardInput input) {
        switch (input) {
          // TODO: Implement up/down buttons
          case MathKeyboardInput.Up:
            // searchcheck();
            MoveCursor(Diraction.Up);
            break;
          case MathKeyboardInput.Down:
            break;
          case MathKeyboardInput.Left:
            MoveCursor(Diraction.Left);
            break;
          case MathKeyboardInput.Right:
            MoveCursor(Diraction.Right);
            break;
          case MathKeyboardInput.Backspace:
            DeleteBackwardsNew();
            break;
          case MathKeyboardInput.Clear:
            MathList.Clear();
            navigation = new MathNavigation(MathList);
            break;
          case MathKeyboardInput.Return:
            return;
          case MathKeyboardInput.Dismiss:
            return;
          case MathKeyboardInput.Slash:
            HandleSlashButtonNew();
            break;
          case MathKeyboardInput.Power:
            HandleScriptButtonNew(true);
            break;
          case MathKeyboardInput.Subscript:
            HandleScriptButtonNew(false);
            break;
          case MathKeyboardInput.Fraction:
            InsertAtomNew(new Atoms.Fraction(LaTeXSettings.PlaceholderList, LaTeXSettings.PlaceholderList));
            break;
          case MathKeyboardInput.SquareRoot:
            InsertAtomNew(new Atoms.Radical(new MathList(), LaTeXSettings.PlaceholderList));
            break;
          case MathKeyboardInput.CubeRoot:
            InsertAtomNew(new Atoms.Radical(new MathList(new Atoms.Number("3")), LaTeXSettings.PlaceholderList));
            break;
          case MathKeyboardInput.NthRoot:
            InsertAtomNew(new Atoms.Radical(LaTeXSettings.PlaceholderList, LaTeXSettings.PlaceholderList));
            break;
          case MathKeyboardInput.BothRoundBrackets:
            InsertInner("(", ")");
            break;
          case MathKeyboardInput.BothSquareBrackets:
            InsertInner("[", "]");
            break;
          case MathKeyboardInput.BothCurlyBrackets:
            InsertInner("{", "}");
            break;
          case MathKeyboardInput.Absolute:
            InsertInner("|", "|");
            break;
          case MathKeyboardInput.BaseEPower:
            InsertAtomNew(LaTeXSettings.AtomForCommand("e")
              ?? throw new InvalidCodePathException($"{nameof(LaTeXSettings.AtomForCommand)} returned null for e"));
            HandleScriptButtonNew(true);
            break;
          case MathKeyboardInput.Caret:
            InsertSymbolNameNew(@"\Caret");
            break;
          case MathKeyboardInput.PlusMinus:
          InsertSymbolNameNew(@"\pm");
            break;
          case MathKeyboardInput.Logarithm:
            InsertSymbolNameNew(@"\log");
            break;
          case MathKeyboardInput.NaturalLogarithm:
            InsertSymbolNameNew(@"\ln");
            break;
          case MathKeyboardInput.LogarithmWithBase:
            InsertSymbolNameNew(@"\log", subscript: true);
            break;
          case MathKeyboardInput.Sine:
            InsertSymbolNameNew(@"\sin");
            break;
          case MathKeyboardInput.Cosine:
            InsertSymbolNameNew(@"\cos");
            break;
          case MathKeyboardInput.Tangent:
            InsertSymbolNameNew(@"\tan");
            break;
          case MathKeyboardInput.Cotangent:
            InsertSymbolNameNew(@"\cot");
            break;
          case MathKeyboardInput.Secant:
            InsertSymbolNameNew(@"\sec");
            break;
          case MathKeyboardInput.Cosecant:
            InsertSymbolNameNew(@"\csc");
            break;
          case MathKeyboardInput.ArcSine:
            InsertSymbolNameNew(@"\arcsin");
            break;
          case MathKeyboardInput.ArcCosine:
            InsertSymbolNameNew(@"\arccos");
            break;
          case MathKeyboardInput.ArcTangent:
            InsertSymbolNameNew(@"\arctan");
            break;
          case MathKeyboardInput.ArcCotangent:
            InsertSymbolNameNew(@"\arccot");
            break;
          case MathKeyboardInput.ArcSecant:
            InsertSymbolNameNew(@"\arcsec");
            break;
          case MathKeyboardInput.ArcCosecant:
            InsertSymbolNameNew(@"\arccsc");
            break;
          case MathKeyboardInput.HyperbolicSine:
            InsertSymbolNameNew(@"\sinh");
            break;
          case MathKeyboardInput.HyperbolicCosine:
            InsertSymbolNameNew(@"\cosh");
            break;
          case MathKeyboardInput.HyperbolicTangent:
            InsertSymbolNameNew(@"\tanh");
            break;
          case MathKeyboardInput.HyperbolicCotangent:
            InsertSymbolNameNew(@"\coth");
            break;
          case MathKeyboardInput.HyperbolicSecant:
            InsertSymbolNameNew(@"\sech");
            break;
          case MathKeyboardInput.HyperbolicCosecant:
            InsertSymbolNameNew(@"\csch");
            break;
          case MathKeyboardInput.AreaHyperbolicSine:
            InsertSymbolNameNew(@"\arsinh");
            break;
          case MathKeyboardInput.AreaHyperbolicCosine:
            InsertSymbolNameNew(@"\arcosh");
            break;
          case MathKeyboardInput.AreaHyperbolicTangent:
            InsertSymbolNameNew(@"\artanh");
            break;
          case MathKeyboardInput.AreaHyperbolicCotangent:
            InsertSymbolNameNew(@"\arcoth");
            break;
          case MathKeyboardInput.AreaHyperbolicSecant:
            InsertSymbolNameNew(@"\arsech");
            break;
          case MathKeyboardInput.AreaHyperbolicCosecant:
            InsertSymbolNameNew(@"\arcsch");
            break;
          case MathKeyboardInput.LimitWithBase:
            InsertSymbolNameNew(@"\lim", subscript: true);
            break;
          case MathKeyboardInput.Integral:
            InsertSymbolNameNew(@"\int");
            break;
          case MathKeyboardInput.IntegralLowerLimit:
            InsertSymbolNameNew(@"\int", subscript: true);
            break;
          case MathKeyboardInput.IntegralUpperLimit:
            InsertSymbolNameNew(@"\int", superscript: true);
            break;
          case MathKeyboardInput.IntegralBothLimits:
            InsertSymbolNameNew(@"\int", subscript: true, superscript: true);
            break;
          case MathKeyboardInput.Summation:
            InsertSymbolNameNew(@"\sum");
            break;
          case MathKeyboardInput.SummationLowerLimit:
            InsertSymbolNameNew(@"\sum", subscript: true);
            break;
          case MathKeyboardInput.SummationUpperLimit:
            InsertSymbolNameNew(@"\sum", superscript: true);
            break;
          case MathKeyboardInput.SummationBothLimits:
            InsertSymbolNameNew(@"\sum", subscript: true, superscript: true);
            break;
          case MathKeyboardInput.Product:
            InsertSymbolNameNew(@"\prod");
            break;
          case MathKeyboardInput.ProductLowerLimit:
            InsertSymbolNameNew(@"\prod", subscript: true);
            break;
          case MathKeyboardInput.ProductUpperLimit:
            InsertSymbolNameNew(@"\prod", superscript: true);
            break;
          case MathKeyboardInput.ProductBothLimits:
            InsertSymbolNameNew(@"\prod", subscript: true, superscript: true);
            break;
          case MathKeyboardInput.DoubleIntegral:
            InsertSymbolNameNew(@"\iint");
            break;
          case MathKeyboardInput.TripleIntegral:
            InsertSymbolNameNew(@"\iiint");
            break;
          case MathKeyboardInput.QuadrupleIntegral:
            InsertSymbolNameNew(@"\iiiint");
            break;
          case MathKeyboardInput.ContourIntegral:
            InsertSymbolNameNew(@"\oint");
            break;
          case MathKeyboardInput.DoubleContourIntegral:
            InsertSymbolNameNew(@"\oiint");
            break;
          case MathKeyboardInput.TripleContourIntegral:
            InsertSymbolNameNew(@"\oiiint");
            break;
          case MathKeyboardInput.ClockwiseIntegral:
            InsertSymbolNameNew(@"\intclockwise");
            break;
          case MathKeyboardInput.ClockwiseContourIntegral:
            InsertSymbolNameNew(@"\varointclockwise");
            break;
          case MathKeyboardInput.CounterClockwiseContourIntegral:
            InsertSymbolNameNew(@"\ointctrclockwise");
            break;
          case MathKeyboardInput.LeftArrow:
            InsertSymbolNameNew(@"\leftarrow");
            break;
          case MathKeyboardInput.UpArrow:
            InsertSymbolNameNew(@"\uparrow");
            break;
          case MathKeyboardInput.RightArrow:
            InsertSymbolNameNew(@"\rightarrow");
            break;
          case MathKeyboardInput.DownArrow:
            InsertSymbolNameNew(@"\downarrow");
            break;
          case MathKeyboardInput.PartialDifferential:
            InsertSymbolNameNew(@"\partial");
            break;
          case MathKeyboardInput.NotEquals:
            InsertSymbolNameNew(@"\neq");
            break;
          case MathKeyboardInput.LessOrEquals:
            InsertSymbolNameNew(@"\leq");
            break;
          case MathKeyboardInput.GreaterOrEquals:
            InsertSymbolNameNew(@"\geq");
            break;
          case MathKeyboardInput.Multiply:
            InsertSymbolNameNew(@"\times");
            break;
          case MathKeyboardInput.Divide:
            InsertSymbolNameNew(@"\div");
            break;
          case MathKeyboardInput.Infinity:
            InsertSymbolNameNew(@"\infty");
            break;
          case MathKeyboardInput.Degree:
            InsertSymbolNameNew(@"\degree");
            break;
          case MathKeyboardInput.Angle:
            InsertSymbolNameNew(@"\angle");
            break;
          case MathKeyboardInput.LeftCurlyBracket:
            InsertSymbolNameNew(@"\{");
            break;
          case MathKeyboardInput.RightCurlyBracket:
            InsertSymbolNameNew(@"\}");
            break;
          case MathKeyboardInput.Percentage:
            InsertSymbolNameNew(@"\%");
            break;
          case MathKeyboardInput.Space:
            InsertSymbolNameNew(@"\ ");
            break;
          case MathKeyboardInput.Prime:
            InsertAtomNew(new Atoms.Prime(1));
            break;
          case MathKeyboardInput.LeftRoundBracket:
          case MathKeyboardInput.RightRoundBracket:
          case MathKeyboardInput.LeftSquareBracket:
          case MathKeyboardInput.RightSquareBracket:
          case MathKeyboardInput.D0:
          case MathKeyboardInput.D1:
          case MathKeyboardInput.D2:
          case MathKeyboardInput.D3:
          case MathKeyboardInput.D4:
          case MathKeyboardInput.D5:
          case MathKeyboardInput.D6:
          case MathKeyboardInput.D7:
          case MathKeyboardInput.D8:
          case MathKeyboardInput.D9:
          case MathKeyboardInput.Decimal:
          case MathKeyboardInput.Plus:
          case MathKeyboardInput.Minus:
          case MathKeyboardInput.Ratio:
          case MathKeyboardInput.Comma:
          case MathKeyboardInput.Semicolon:
          case MathKeyboardInput.Factorial:
          case MathKeyboardInput.VerticalBar:
          case MathKeyboardInput.Equals:
          case MathKeyboardInput.LessThan:
          case MathKeyboardInput.GreaterThan:
          case MathKeyboardInput.A:
          case MathKeyboardInput.B:
          case MathKeyboardInput.C:
          case MathKeyboardInput.D:
          case MathKeyboardInput.E:
          case MathKeyboardInput.F:
          case MathKeyboardInput.G:
          case MathKeyboardInput.H:
          case MathKeyboardInput.I:
          case MathKeyboardInput.J:
          case MathKeyboardInput.K:
          case MathKeyboardInput.L:
          case MathKeyboardInput.M:
          case MathKeyboardInput.N:
          case MathKeyboardInput.O:
          case MathKeyboardInput.P:
          case MathKeyboardInput.Q:
          case MathKeyboardInput.R:
          case MathKeyboardInput.S:
          case MathKeyboardInput.T:
          case MathKeyboardInput.U:
          case MathKeyboardInput.V:
          case MathKeyboardInput.W:
          case MathKeyboardInput.X:
          case MathKeyboardInput.Y:
          case MathKeyboardInput.Z:
          case MathKeyboardInput.SmallA:
          case MathKeyboardInput.SmallB:
          case MathKeyboardInput.SmallC:
          case MathKeyboardInput.SmallD:
          case MathKeyboardInput.SmallE:
          case MathKeyboardInput.SmallF:
          case MathKeyboardInput.SmallG:
          case MathKeyboardInput.SmallH:
          case MathKeyboardInput.SmallI:
          case MathKeyboardInput.SmallJ:
          case MathKeyboardInput.SmallK:
          case MathKeyboardInput.SmallL:
          case MathKeyboardInput.SmallM:
          case MathKeyboardInput.SmallN:
          case MathKeyboardInput.SmallO:
          case MathKeyboardInput.SmallP:
          case MathKeyboardInput.SmallQ:
          case MathKeyboardInput.SmallR:
          case MathKeyboardInput.SmallS:
          case MathKeyboardInput.SmallT:
          case MathKeyboardInput.SmallU:
          case MathKeyboardInput.SmallV:
          case MathKeyboardInput.SmallW:
          case MathKeyboardInput.SmallX:
          case MathKeyboardInput.SmallY:
          case MathKeyboardInput.SmallZ:

            var Atom = LaTeXSettings.AtomForCommand(new string((char)input, 1));
            InsertAtomNew(Atom ?? throw new InvalidCodePathException($"{nameof(LaTeXSettings.AtomForCommand)} returned null for {input}"));
            break;
          case MathKeyboardInput.Alpha:
          case MathKeyboardInput.Beta:
          case MathKeyboardInput.Gamma:
          case MathKeyboardInput.Delta:
          case MathKeyboardInput.Epsilon:
          case MathKeyboardInput.Zeta:
          case MathKeyboardInput.Eta:
          case MathKeyboardInput.Theta:
          case MathKeyboardInput.Iota:
          case MathKeyboardInput.Kappa:
          case MathKeyboardInput.Lambda:
          case MathKeyboardInput.Mu:
          case MathKeyboardInput.Nu:
          case MathKeyboardInput.Xi:
          case MathKeyboardInput.Omicron:
          case MathKeyboardInput.Pi:
          case MathKeyboardInput.Rho:
          case MathKeyboardInput.Sigma:
          case MathKeyboardInput.Tau:
          case MathKeyboardInput.Upsilon:
          case MathKeyboardInput.Phi:
          case MathKeyboardInput.Chi:
          case MathKeyboardInput.Psi:
          case MathKeyboardInput.Omega:
          case MathKeyboardInput.SmallAlpha:
          case MathKeyboardInput.SmallBeta:
          case MathKeyboardInput.SmallGamma:
          case MathKeyboardInput.SmallDelta:
          case MathKeyboardInput.SmallEpsilon:
          case MathKeyboardInput.SmallEpsilon2:
          case MathKeyboardInput.SmallZeta:
          case MathKeyboardInput.SmallEta:
          case MathKeyboardInput.SmallTheta:
          case MathKeyboardInput.SmallIota:
          case MathKeyboardInput.SmallKappa:
          case MathKeyboardInput.SmallKappa2:
          case MathKeyboardInput.SmallLambda:
          case MathKeyboardInput.SmallMu:
          case MathKeyboardInput.SmallNu:
          case MathKeyboardInput.SmallXi:
          case MathKeyboardInput.SmallOmicron:
          case MathKeyboardInput.SmallPi:
          case MathKeyboardInput.SmallPi2:
          case MathKeyboardInput.SmallRho:
          case MathKeyboardInput.SmallRho2:
          case MathKeyboardInput.SmallSigma:
          case MathKeyboardInput.SmallSigma2:
          case MathKeyboardInput.SmallTau:
          case MathKeyboardInput.SmallUpsilon:
          case MathKeyboardInput.SmallPhi:
          case MathKeyboardInput.SmallPhi2:
          case MathKeyboardInput.SmallChi:
          case MathKeyboardInput.SmallPsi:
          case MathKeyboardInput.SmallOmega:
            // All Greek letters are rendered as variables.
            var variableAtom = new Atoms.Variable(((char)input).ToStringInvariant());
            InsertAtomNew(variableAtom);
            break;
          default:
            break;
        }
      }
    }

    public void Clear() {
      MathList.Clear();
      navigation = new MathNavigation(MathList);
      InsertionIndex = MathListIndex.Level0Index(0);
    }
    /// <summary>
    /// move left/right untill the condition is equal to true.
    /// </summary>
    /// <param name="Condition"></param>
    /// <param name="CountMovement"></param>
    /// <param name="ToLeft"></param>
    /// <param name="BackToFirstPos"></param>
    /// <returns></returns>
    public MathNavigation? SerachFor(Func<MathNavigation, bool> Condition,
      out int CountMovement, bool BackToFirstPos = true) {

      var MathNavigationCopy = navigation;

      var isFirst = () => navigation.FirstAtomInAllLists;

      Action Movement = () => KeyPress(MathKeyboardInput.Left);
      // to right
        // move to the end
        navigation.OnRightSide = true;
        navigation.MoveToLastList();
        navigation.MoveToLastAtom();
      Func<bool> OnTheEdge = isFirst;

      CountMovement = 0;

      while (!OnTheEdge()) {
        if (Condition(navigation)) {
          break;
        }
        Movement();
        CountMovement++;
      }

      var resultIndex = navigation;

      if (BackToFirstPos)
        navigation = MathNavigationCopy;

      return resultIndex;



    }
    public void ChangeMathlistByAtom(MathList mathlist, MathAtom atom, bool setIndexBeforeAtom = false) {
      MathList = mathlist;
      navigation = new MathNavigation(MathList);
      ChangeInsertionIndexByAtom(atom, setIndexBeforeAtom);
    }
    public void ChangeInsertionIndexByAtom(MathAtom atom, bool setIndexBeforeAtom = false) {

      var refernceEqual = (MathNavigation i) => {
        var currectAtom = navigation.GetCurrentAtom;
        if (object.ReferenceEquals(currectAtom, (atom))) {
          return true;
        }
        return false;
      };

      if (MathList.Count == 0) return;

      if (!refernceEqual(navigation)) {
        SerachFor(refernceEqual, out _, false);
      }

      if (setIndexBeforeAtom)
        KeyPress(MathKeyboardInput.Left);

    }
    public void ChangeMathList(MathList mathlist) {
      MathList = mathlist;
      navigation = new MathNavigation(MathList);
    }
  }
}
