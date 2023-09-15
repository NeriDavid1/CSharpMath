using System;

namespace CSharpMath.Editor {
  using System.Collections.Generic;
  using System.Linq;
  using Atom;
  using Atoms = Atom.Atoms;
  using CSharpMath.Atom.Atoms;
  using Structures;
  using System.Runtime.CompilerServices;

  partial class Extensions {
    // this method is used to insert an atom into a mathlist and advance the index to the next position
    public static void InsertNav(this MathList self, MathAtom atom, ref MathNavigation list) {
      MathList CurrectMathlist = list.GetCurrentList;
      // check wather the current atom is a placeholder and if it is active then replace it with the new atom
      if (ActivePlaceHolder(list)) {
        ReplacePlaceHolderWithAtom(atom, ref list);
      }
      // add atom by his type
      switch (atom) {
        case Atoms.Fraction frac:
          FractionAtom(frac, ref list);
          break;
        case Atoms.Radical radical:
          RadicalAtom(radical, ref list);
          break;
        case Atoms.Inner inner:
          InnerAtom(inner, ref list);
          break;
        default:
          NormalAtom(atom, ref list);
          break;
      }
      // index cannot be on the right side of the atom so we move it to the left
      list.OnRightSide = false;

      void NormalAtom(MathAtom mathAtom, ref MathNavigation list) {
        ShouldTakeTheScript(list);
        CurrectMathlist.Insert(list.Index + 1, mathAtom);
        list.Next();
        void TakeAtomScript(MathAtom atom) {
          SetSuperAndSubScript(atom, mathAtom);
          atom.Subscript.Clear();
          atom.Superscript.Clear();
        }
        void ShouldTakeTheScript(MathNavigation list) {
          if (list.GetCurrentAtom is MathAtom atom) {
            if (!list.OnRightSide && atom.HasScripts) {
              TakeAtomScript(atom);
            }
          }
        }
      }
      void FractionAtom(Atoms.Fraction fraction, ref MathNavigation list) {
        NormalAtom(fraction, ref list);
        list.MoveUp(true);
        if (fraction.Numerator.Atoms.Count != 0) {
          list.MoveToLastAtom();
        }
      }
      void RadicalAtom(Atoms.Radical radical, ref MathNavigation list) {
        NormalAtom(radical, ref list);
        list.MoveUp(true);
        list.Next();
        // some radicals already have a degree so we move to the right place
        if (list.GetCurrentAtom is Number) {
          list.NextList();
          list.Next();
        }
      }
      void InnerAtom(Inner inner, ref MathNavigation list) {
        NormalAtom(inner, ref list);
        list.MoveUp(true);
        if (inner.InnerList.Count != 0) {
          list.MoveToLastAtom();
        }
      }
      bool ActivePlaceHolder(MathNavigation list) {
        return list.GetCurrentAtom is Placeholder placeholder && (!placeholder.HasScripts || list.OnRightSide == false);

      }
      void ReplacePlaceHolderWithAtom(MathAtom atom, ref MathNavigation list) {
        if (list.GetCurrentAtom is Placeholder placeholder) {
          atom.Subscript.Append(placeholder.Subscript);
          atom.Superscript.Append(placeholder.Superscript);
          // replace and move if needed
          self.RemoveCurrentAtom(ref list);
        }

      }
    }
    private static void RemoveCurrentAtom(this MathList self, ref MathNavigation list) {
      list.GetCurrentList.RemoveAt(list.Index);
      int index = list.Index;
      list.Previous();
      if (RemovingList()) {
        list.MoveDown();
      }
      bool RemovingList() {
        return index < 0;
      }
    }
    public static void RemoveLast(this MathList self, ref MathNavigation list) {
      var LastAtom = list.GetCurrentAtom;

      switch (LastAtom) {
        case Atoms.Placeholder placeholder:
          // place holder have many cases
          RemovePlaceHolder(placeholder, ref list);
          break;
        case var _ when LastAtom == null:
          break;
        case var atom when LastAtom.HasScripts:
          var prev = list.GetPrevAtom;
          if (prev == null || !CanHandleScript(prev)) {
            ReplaceWithPlaceHolder(atom, ref list);
            break;
          }
          SetSuperAndSubScript(atom, prev);
          list.GetCurrentList.RemoveAt(list.Index);
          list.Previous();
          break;
        case Atoms.Fraction frac:
        case Atoms.Radical radical:
        default:
          list.GetCurrentList.RemoveAt(list.Index);
          list.Previous();
          list.OnRightSide = true;
          break;
      }
      if (PlaceHolderRequierd(list)) {
        var emptyAtom = LaTeXSettings.Placeholder;
        InsertNav(self, emptyAtom, ref list);
      }
      bool PlaceHolderRequierd(MathNavigation list) {
        if (list.GetCurrentList.IsEmpty()) {
          switch (list.GetListPerent()) {
            case Fraction:
            case Number:
            case Radical:
              return true;
            default:
              return false;
          }
        }
        return false;
      }
      static bool CanHandleScript(MathAtom previous) {
        return previous switch {
          Atoms.BinaryOperator _ => false,
          Atoms.UnaryOperator _ => false,
          Atoms.Relation _ => false,
          Atoms.Punctuation _ => false,
          Atoms.Space _ => false,
          _ => true
        };
      }
    }

    private static void RemovePlaceHolder(Placeholder placeholder, ref MathNavigation list) {
      var perent = list.GetListPerent();
      switch (perent) {
        case Fraction frac:
          // exmple {[]|}{44}
          if (list.GetCurrentList == frac.Numerator) {
            RemoveContainer(ref list);
          } else {
            // exmple {55}{4|4}
            // remove fraction and bring numerator to the current list
            Queue<MathAtom> atoms = new Queue<MathAtom>();
            foreach (var atom in frac.Numerator.Atoms) {
              atoms.Enqueue(atom);
            }
            RemoveContainer(ref list);
            while (atoms.Count != 0) {
              var currectmathlist = list.GetCurrentList;
              currectmathlist.InsertNav(atoms.Dequeue(), ref list);
            }
          }
          break;
        case Atoms.Radical radical:
          if (list.GetCurrentList == radical.Radicand) {
            RemoveContainer(ref list);
          }
          break;
        case var _ when perent is null:
          list.GetCurrentList.RemoveAt(list.Index);
          list.Previous();
          break;
        case var _ when perent.Superscript.IsNonEmpty():
          perent.Superscript.Clear();
          list.MoveDown();
          break;
        case var _ when perent.Subscript.IsNonEmpty():
          perent.Subscript.Clear();
          list.MoveDown();
          break;
      }

      static void RemoveContainer(ref MathNavigation list) {
        list.MoveDown();
        list.GetCurrentList.RemoveAt(list.Index);
        list.Previous();
      }
    }

    /// <summary>
    /// Return list of all the objects inside the mathlists
    /// </summary>
    /// <param name="self"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static List<MathAtom> Deployment(this MathList self) {
      var atomlist = new List<MathAtom>();
      if (self is null) throw new ArgumentNullException(nameof(self));
      foreach (var atom in self.Atoms.ToList()) {
        atomlist.Add(atom);
        switch (atom) {
          case Comment { Nucleus: var comment }:
            break;
          case Fraction fraction: {
              AddToList(fraction.Numerator);
              AddToList(fraction.Denominator);
            }
            break;
          case Radical radical:
            radical.Degree.Deployment();
            radical.Radicand.Deployment();
            break;
          case Inner { LeftBoundary: { Nucleus: null }, InnerList: var list }:
            AddToList(list);
            break;
          case Inner { LeftBoundary: { Nucleus: "〈" }, InnerList: var list }:
            AddToList(list);
            break;
          case Inner { LeftBoundary: { Nucleus: "|" }, InnerList: var list }:
            AddToList(list);
            break;
          case Inner { LeftBoundary: var left, InnerList: var list }:
            AddToList(list);
            break;
          case Atoms.Caret caret:
            AddToList(caret.CartList.InnerList);
            break;
          case Overline over:
            AddToList(over.InnerList);
            break;
          case Underline under:
            AddToList(under.InnerList);
            break;
          case Accent accent:
            //MathAtomToLaTeX(accent, builder, out _);
            AddToList(accent.InnerList);
            break;
          case Colored colored:
            AddToList(colored.InnerList);
            break;
          case ColorBox colorBox:
            AddToList(colorBox.InnerList);
            break;
          case RaiseBox r:
            AddToList(r.InnerList);
            break;
        }
        AddToList(atom.Subscript);
        AddToList(atom.Superscript);
      }
      return atomlist;
      void AddToList(MathList list) {
        if (list.IsNonEmpty()) {
          atomlist.AddRange(list.Deployment());
        }
      }
    }
    // split the mathlist by the given atom
    public static List<MathList>? SplitByAtom(this MathList self, MathAtom atom) {
      if (self.Count == 0) return null;
      int targetAtomCounter = 0;
      List<MathList> mathlist = new();
      MathNavigation list = new(self);
      for (int i = 0; i < self.Count; i++) {
        var currectAtom = list.GetCurrentAtom ?? throw new Exception("null");
        if (currectAtom.EqualsAtom(atom)) {
          mathlist.Add(self.Slice(targetAtomCounter, list.Index - targetAtomCounter));
          targetAtomCounter = list.Index + 1;
        }
        if (i == self.Count - 1) {
          mathlist.Add(self.Slice(targetAtomCounter, list.Index - targetAtomCounter + 1));
        }
        list.Next();
      }
      return mathlist;
    }
    private static void SetSuperAndSubScript(MathAtom currentAtom, MathAtom ToAtom) {
      ToAtom.Superscript.Append(currentAtom.Superscript);
      ToAtom.Subscript.Append(currentAtom.Subscript);
    }

    private static void ReplaceWithPlaceHolder(MathAtom Atom, ref MathNavigation list) {
      var PlaceHolder = LaTeXSettings.Placeholder;
      if (Atom.Superscript.IsNonEmpty()) {
        PlaceHolder.Superscript.Append(Atom.Superscript);
      }
      if (Atom.Subscript.IsNonEmpty()) {
        PlaceHolder.Subscript.Append(Atom.Subscript);
      }
      list.GetCurrentList.RemoveAt(list.Index);
      list.GetCurrentList.Insert(list.Index, PlaceHolder);
    }
  }
}

