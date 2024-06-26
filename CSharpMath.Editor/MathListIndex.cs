namespace CSharpMath.Editor {
  using System.Diagnostics;
  using System.Linq;
  ///<summary>The type of the subindex denotes what branch the path to the atom that this index points to takes.</summary>
  [DebuggerDisplay("{FinalSubIndexType}")]
  public enum MathListSubIndexType : byte {
    ///<summary>The index denotes the whole atom, subIndex is null</summary>
    None = 0,
    ///<summary>The position in the subindex is an index into the nucleus, must be 1</summary>
    BetweenBaseAndScripts,
    ///<summary>The subindex indexes into the superscript</summary>
    Superscript,
    ///<summary>The subindex indexes into the subscript</summary>
    Subscript,
    ///<summary>The subindex indexes into the numerator (only valid for fractions)</summary>
    Numerator,
    ///<summary>The subindex indexes into the denominator (only valid for fractions)</summary>
    Denominator,
    ///<summary>The subindex indexes into the radicand (only valid for radicals)</summary>
    Radicand,
    ///<summary>The subindex indexes into the degree (only valid for radicals)</summary>
    Degree,
    ///<summary>The subindex indexes into the inner list (only valid for inners)</summary>
    Inner
  }

  /** <summary>
* An index that points to a particular character in the MathList. The index is a LinkedList that represents
* a path from the beginning of the MathList to reach a particular atom in the list. The next node of the path
* is represented by the subIndex. The path terminates when the subIndex is nil.
*
* If there is a subIndex, the subIndexType denotes what branch the path takes (i.e. superscript, subscript, 
* numerator, denominator etc.).
* e.g in the expression 25^{2/4} the index of the character 4 is represented as:
* (1, superscript) -> (0, denominator) -> (0, none)
* This can be interpreted as start at index 1 (i.e. the 5) go up to the superscript.
* Then look at index 0 (i.e. 2/4) and go to the denominator. Then look up index 0 (i.e. the 4) which this final
* index.
* 
* The level of an index is the number of nodes in the LinkedList to get to the final path.
* </summary>*/
  public class MathListIndex {
    private MathListIndex() { }

    ///<summary>The index of the associated atom.</summary>
    public int AtomIndex { get; set; }
    ///<summary>The type of subindex, e.g. superscript, numerator etc.</summary>
    public MathListSubIndexType SubIndexType { get; set; }
    ///<summary>The index into the sublist.</summary>
    public MathListIndex? SubIndex;

    /** <summary>Factory function to create a `MathListIndex` with no subindexes.</summary>
        <param name="index">The index of the atom that the `MathListIndex` points at.</param>
     */
    public static MathListIndex Level0Index(int index) => new MathListIndex { AtomIndex = index };
    /** <summary>Factory function to create at `MathListIndex` with a given subIndex.</summary>
        <param name="location">The location at which the subIndex should is present.</param>
        <param name="subIndex">The subIndex to be added. Can be nil.</param> 
        <param name="type">The type of the subIndex.</param> 
     */
    public static MathListIndex IndexAtLocation(int location, MathListSubIndexType type, MathListIndex? subIndex) =>
      new MathListIndex { AtomIndex = location, SubIndexType = type, SubIndex = subIndex };

    ///<summary>Creates a new index by attaching this index at the end of the current one.</summary>
    public MathListIndex LevelUpWithSubIndex(MathListSubIndexType type, MathListIndex? subIndex) =>
      SubIndexType is MathListSubIndexType.None ? IndexAtLocation(AtomIndex, type, subIndex) :
      IndexAtLocation(AtomIndex, SubIndexType, SubIndex?.LevelUpWithSubIndex(type, subIndex));

    ///<summary>Creates a new index by removing the last index item. If this is the last one, then returns nil.</summary>
    ///<example> 15^{[index]2/4} -> [ 0 , superscript, [ 0, denominator ] ] ->
    /// after leveldown -> 15^{[index]{2}/{4}} -> [ 0 , superscript ] </example>
    /// and after another level down it will be 15^{{2}/{4}}[index] -> [ 0, none ]
    /// ofcourse the index wouldn't will move to a different atom if the subindex is not none.
    public MathListIndex? LevelDown() =>
      SubIndexType is MathListSubIndexType.None ? null :
      SubIndex?.LevelDown() is MathListIndex subIndex ? IndexAtLocation(AtomIndex, SubIndexType, subIndex) :
      Level0Index(AtomIndex);

    /** <summary>
     * Returns the previous index if this index is not at the beginning of a line.
     * Note there may be multiple lines in a MathList,
     * e.g. a superscript or a fraction numerator.
     * This returns <see cref="null"/> if there is no previous index, i.e.
     * the innermost subindex points to the beginning of a line.</summary>
     */
    /// <example> 15^{45[index]] -> [ 0, superscript, [ 1, none ] ] -> previous -> [ 0, superscript, [ 0, none ] ] </example>
    public MathListIndex? Previous => SubIndexType switch
    {
      MathListSubIndexType.None => AtomIndex > 0 ? Level0Index(AtomIndex - 1) : null,
      _ => SubIndex?.Previous is MathListIndex prevSubIndex ? IndexAtLocation(AtomIndex, SubIndexType, prevSubIndex) : null,
    };

    /// <summary>Should be used inside <see cref="Extensions.RemoveAt(Atom.MathList, ref MathListIndex)"/> only!</summary>
    internal MathListIndex? PreviousOrBeforeWholeList => SubIndexType switch
    {
      MathListSubIndexType.None => AtomIndex > -1 ? Level0Index(AtomIndex - 1) : null,
      _ => SubIndex?.PreviousOrBeforeWholeList is MathListIndex prevSubIndex ? IndexAtLocation(AtomIndex, SubIndexType, prevSubIndex) : null,
    };


    ///<summary>Returns the next index.</summary>
    public MathListIndex Next => SubIndexType switch
    {
      MathListSubIndexType.None => Level0Index(AtomIndex + 1),
      MathListSubIndexType.BetweenBaseAndScripts => IndexAtLocation(AtomIndex + 1, SubIndexType, SubIndex),
      _ => IndexAtLocation(AtomIndex, SubIndexType, SubIndex?.Next),
    };

    ///<summary>Returns true if any of the subIndexes of this index have the given type.</summary>
    public bool HasSubIndexOfType(MathListSubIndexType subIndexType) =>
    SubIndexType == subIndexType || (SubIndex != null && SubIndex.HasSubIndexOfType(subIndexType));

        public MathListIndex? GetParentIndexByType(params MathListSubIndexType[] subIndexType) =>
      subIndexType.Any(x => x == FinalSubIndexType) ?  this.LevelDown() : SubIndex is not null
      ? this.LevelDown()?.GetParentIndexByType(subIndexType) : null;

    public bool AtSameLevel(MathListIndex other) =>
     SubIndexType == other.SubIndexType &&
      // No subindexes, they are at the same level.
        (SubIndexType == MathListSubIndexType.None ||
      // the subindexes are used in different atoms
          (AtomIndex == other.AtomIndex &&
            (SubIndex == null || other.SubIndex == null || SubIndex.AtSameLevel(other.SubIndex))));

    public int FinalIndex =>
      SubIndexType is MathListSubIndexType.None || SubIndex is null ? AtomIndex : SubIndex.FinalIndex;

    ///<summary>Returns the type of the innermost sub index.</summary>
    /// actually it returns the type of the subindex that is the last one in the chain.
    /// <example> if we have 12^{34}_56, and the index is point on 5 then the subindex type is <see cref="MathListSubIndexType.Denominator"/>.
    public MathListSubIndexType FinalSubIndexType =>
      SubIndex?.SubIndex is null ? SubIndexType : SubIndex.FinalSubIndexType;
   ///<summary>Returns the innermost sub index.</summary>`
    public MathListIndex FinalSubIndexParent =>
      SubIndex?.SubIndex is null ? this : SubIndex.FinalSubIndexParent;

    public override string ToString() =>
      SubIndex is null ?
      $@"[{AtomIndex}]" :
      $@"[{AtomIndex}, {SubIndexType}:{SubIndex.ToString().Trim('[', ']')}]";

    public bool EqualsToIndex(MathListIndex index) =>
      !(index is null) && AtomIndex == index.AtomIndex && SubIndexType == index.SubIndexType &&
        (SubIndex != null && index.SubIndex != null ? SubIndex.EqualsToIndex(index.SubIndex) :
      index.SubIndex == null);

    public override bool Equals(object obj) =>
      obj is MathListIndex index && EqualsToIndex(index);
    public override int GetHashCode() =>
      unchecked((AtomIndex * 31 + (int)SubIndexType) * 31 + (SubIndex?.GetHashCode() ?? -1));
  }
}