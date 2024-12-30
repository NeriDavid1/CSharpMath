using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using CSharpMath.Atom;

namespace CSharpMath.Editor {
    // Represents a node of a math list, which can be a script or a container
    public class MathNode
    {
        public MathNode()
        {
            Lines = new MathList[MathNavigation.MaxLines];
        }

        public MathNode(params MathList[] mathlists)
        {
            Lines = mathlists;
        }

        public MathList[] Lines = new MathList[MathNavigation.MaxLines];
        public int ListIndex = 0;
        public int Index = -1;

        // Indexer to access a MathAtom at a given line and index
        public MathAtom? this[int Lindex, int index]
        {
            get
            {
                var line = this[Lindex];
                return index > -1 && index < line.Count ? line[index] : null;
            }
        }

        // Indexer to access a MathList at a given line index
        public MathList this[int Lindex] => Lines[Lindex];
    }
    [DebuggerDisplay("Index : {Index}, Atom : {GetCurrentAtom}")]
    // Represents the navigation of the math list
    public class MathNavigation
    {
        public static int MaxLines = 2;
        public Stack<MathNode> Nodes = new();

        public MathNavigation(MathList line)
        {
            var mathNode = new MathNode();
            mathNode.Lines[0] = line;
            CurrectNode = mathNode;
        }

        public MathNode CurrectNode;
        public int Index
        {
            get => CurrectNode.Index;
            set => CurrectNode.Index = value;
        }
        public bool OnRightSide = false;
        public bool OnContainer = false;

        public MathList GetCurrentList => CurrectNode[CurrectNode.ListIndex];
        public MathAtom? GetCurrentAtom => CurrectNode[CurrectNode.ListIndex, CurrectNode.Index];
        public MathAtom? GetPrevAtom => CurrectNode[CurrectNode.ListIndex, CurrectNode.Index - 1];
        public MathAtom? GetNextAtom => CurrectNode[CurrectNode.ListIndex, CurrectNode.Index + 1];

        public bool IsTheOnlyList => Nodes.Count == 0;
        public bool FirstAtomInAllLists => IsTheOnlyList && Index == -1;

        public void MoveUp(bool ToContiner = false)
        {
            Nodes.Push(CurrectNode);
            var lists = ToContiner ? GetAtomContainer() : GetAtomScript();
            OnContainer = ToContiner;
            CurrectNode = new MathNode(lists);
        }

        public void MoveDown()
        {
            var parent = GetListPerent();
            if (parent is null) return;
            if (IsContiner(parent))
            {
                OnContainer = true;
            }
            CurrectNode = Nodes.Pop();

            bool IsContiner(MathAtom parentAtom)
            {
                return parentAtom.Subscript != CurrectNode.Lines[0] && parentAtom.Superscript != CurrectNode.Lines[0];
            }
        }

        public void Next()
        {
            CurrectNode.Index++;
        }

        public void Previous()
        {
            CurrectNode.Index--;
        }

        public void NextList()
        {
            if (CurrectNode[CurrectNode.ListIndex + 1] is null)
                throw new ArgumentNullException();
            CurrectNode.ListIndex++;
            CurrectNode.Index = -1;
        }

        public void MoveToLastList()
        {
            if (CurrectNode.ListIndex == 0 && CurrectNode.Lines.Count(item => item != null) > 1)
            {
                NextList();
            }
        }

        public void MoveToLastAtom()
        {
            Index = GetCurrentList.Count - 1;
        }

        public void PreviousList()
        {
            if (CurrectNode.ListIndex == 0)
                throw new ArgumentNullException();
            CurrectNode.ListIndex--;
        }

        private MathList[] GetAtomScript()
        {
            var atom = GetCurrentAtom;
            if (atom == null) return Array.Empty<MathList>();
            var newList = new List<MathList>();
            if (atom.Subscript.IsNonEmpty())
            {
                newList.Add(atom.Subscript);
            }
            if (atom.Superscript.IsNonEmpty())
            {
                newList.Add(atom.Superscript);
            }
            return newList.ToArray();
        }

        private MathList[] GetAtomContainer()
        {
            var atom = GetCurrentAtom;
            if (atom == null) return Array.Empty<MathList>();
            if (atom is IMathListContainer container)
            {
                return container.InnerLists.Where(x => x.IsNonEmpty()).ToArray();
            }
            return Array.Empty<MathList>();
        }

        // Gets the parent of the current list without moving down
        public MathAtom? GetListPerent()
        {
            if (IsTheOnlyList) return null;
            var downNode = Nodes.Peek();
            return downNode[downNode.ListIndex][downNode.Index];
        }

        public void MoveToScript(MathList script)
        {
            foreach (var list in CurrectNode.Lines)
            {
                if (list == script) return;
                NextList();
                if (CurrectNode.ListIndex == 3)
                {
                    throw new Exception("Out of list index");
                }
            }
        }

        public bool IsFirstIndex => Index == -1;
        public bool IsFirstList => CurrectNode.ListIndex == 0;
        public bool IsLastIndex => Index == GetCurrentList.Count - 1;

        public bool IsLastList
        {
            get
            {
                return CurrectNode.ListIndex == MaxLines ||
                       CurrectNode.Lines.Count(item => item is not null) == CurrectNode.ListIndex + 1 ||
                       IsTheOnlyList;
            }
        }
    }
}
