using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using CSharpMath.Atom;

namespace CSharpMath.Editor {
  // represent a node of math list a script or a container
  public class MathNode {
    public MathNode() { }
    public MathNode(params MathList[] mathlists) {
      Lines = mathlists;
    }
    public MathList[] Lines = new MathList[MathNavigation.MaxLines];
    public MathAtom? this[int Lindex, int index] { get => this[Lindex].Count > index && index > -1 ? Lines[Lindex][index] : null; }
    public MathList this[int Lindex] => Lines[Lindex];

    public int ListIndex = 0;
    public int Index = -1;
  }
  [DebuggerDisplay("Index : {Index}" + "Atom : {GetCurrentAtom}")]
  // represent the navigation of the math list 
  public class MathNavigation {
    public static int MaxLines = 2;
    public Stack<MathNode> Nodes = new();
    public MathNavigation(MathList Line) {
      var mathNode = new MathNode();
      mathNode.Lines[0] = Line;
      CurrectNode = mathNode;
    }
    public MathNode CurrectNode;
    public int Index { get => CurrectNode.Index; set => CurrectNode.Index = value; }
    public bool OnRightSide = false;
    public bool OnContainer = false;
    public MathList GetCurrentList => CurrectNode[CurrectNode.ListIndex];
    public MathAtom? GetCurrentAtom => CurrectNode[CurrectNode.ListIndex, CurrectNode.Index];
    public MathAtom? GetPrevAtom => CurrectNode[CurrectNode.ListIndex, CurrectNode.Index - 1];
    public MathAtom? GetNextAtom => CurrectNode[CurrectNode.ListIndex, CurrectNode.Index + 1];

    public bool IsTheOnlyList => Nodes.Count == 0;

    public bool FirstAtomInAllLists => IsTheOnlyList && Index == -1;

    public void MoveUp(bool ToContiner = false) {
      Nodes.Push(CurrectNode);
      var Lists = ToContiner ? GetAtomContainer() : GetAtomScript();
      OnContainer = ToContiner;
      CurrectNode = new MathNode(Lists);
    }
    public void MoveDown() {
      var prent = GetListPerent();
      if (prent is null) return;
      if (IsContiner(prent)) {
        OnContainer = true;
      }
      CurrectNode = Nodes.Pop();

      bool IsContiner(MathAtom prent) {
        return prent.Subscript != CurrectNode.Lines[0] && prent.Superscript != CurrectNode.Lines[0];
      }
    }
    public void Next() {
      CurrectNode.Index++;
    }
    public void Previous() {
      CurrectNode.Index--;
    }
    public void NextList() {
      if (CurrectNode[CurrectNode.ListIndex + 1] is null) throw new ArgumentNullException();
      CurrectNode.ListIndex++;
      CurrectNode.Index = -1;
    }
    public void MoveToLastList() {
      if(CurrectNode.ListIndex == 0 && CurrectNode.Lines.Count(item => item != null) > 1)
        NextList();
    }
    public void MoveToLastAtom() {
      Index = GetCurrentList.Count - 1;
    }
    public void PreviousList() {
      if (CurrectNode.ListIndex == 0) throw new ArgumentNullException();
      CurrectNode.ListIndex--;
    }
    private MathList[] GetAtomScript() {
      var atom = GetCurrentAtom;
      if (atom == null) return new MathList[0];
      var newList = new MathList[2];
      var index = 0;
      if (atom.Subscript.IsNonEmpty()) {
        newList[index] = atom.Subscript;
        index++;
      }
      if (atom.Superscript.IsNonEmpty()) {
        newList[index] = atom.Superscript;
      }
      return newList;
    }
    private MathList[] GetAtomContainer() {
      var atom = GetCurrentAtom;
      var newList = new MathList[2];
      if (atom == null) return new MathList[0];
      if (atom is IMathListContainer container)
        newList = container.InnerLists.Where(x => x.IsNonEmpty()).ToArray();
      return newList;
    }

    // Get the parent of the current list without moving down
    public MathAtom? GetListPerent() {
      if (Nodes.Count == 0) return null;
      var DownNode = Nodes.Peek();
      return DownNode[DownNode.ListIndex][DownNode.Index];
    }

    public void MoveToScript(MathList script) {
      foreach (MathList list in CurrectNode.Lines) {
        if (list == script) return;
        NextList();
        if (CurrectNode.ListIndex == 3) {
          throw new Exception("out of list index");
        }
      }
    }

    public bool IsFirstIndex => Index == -1;
    public bool IsFirstList => CurrectNode.ListIndex == 0;

    public bool IsLastIndex => Index == GetCurrentList.Count - 1;

    public bool IsLastList {
      get {
        return (CurrectNode.ListIndex == MaxLines ||
      CurrectNode.Lines.Count(item => item is not null) == CurrectNode.ListIndex + 1 || Nodes.Count == 0);
      }
    }
  }
}
