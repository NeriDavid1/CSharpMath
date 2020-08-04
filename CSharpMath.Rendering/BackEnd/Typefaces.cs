using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Typography.OpenFont;

namespace CSharpMath.Rendering.BackEnd {
  public class Typefaces : ICollection<Typeface> {
    internal Typefaces(Typeface _default) =>
      _typefaces = new Dictionary<sbyte, Typeface> { [0] = _default };
    private readonly IDictionary<sbyte, Typeface> _typefaces;
    public Typeface this[sbyte index]
      { get => _typefaces[index]; set { if (index != 0) _typefaces[index] = value; } }
    public int Count => _typefaces.Count;
    public bool IsReadOnly => false;
    public void Add(Typeface item) => AddSupplement(item);
    public void AddOverride(Typeface item) =>
      _typefaces.Add(checked((sbyte)(_typefaces.Keys.Min() - 1)), item);
    public void AddSupplement(Typeface item) =>
      _typefaces.Add(checked((sbyte)(_typefaces.Keys.Max() + 1)), item);
    public void Clear()
      { var item = _typefaces[0]; _typefaces.Clear(); _typefaces[0] = item; }
    public bool Contains(Typeface item) => _typefaces.Values.Contains(item);
    public void CopyTo(Typeface[] array, int arrayIndex) {
      foreach (var item in _typefaces.OrderBy(p => p.Key).Select(p => p.Value))
        array[arrayIndex++] = item;
    }
    public sbyte IndexOf(Typeface item) =>
      _typefaces.OrderBy(p => p.Key).First(p => p.Value == item).Key;
    public void Insert(sbyte index, Typeface item) {
      //pushes original items' index away from zero
      if (index != 0) {
        var sign = (sbyte)Math.Sign(index);
        var limit = sign == 1 ? sbyte.MaxValue : sbyte.MinValue;
        var end = index; //end of index of displacement of original items
        while (_typefaces.ContainsKey(end) && end != limit) end += sign;
        for (sbyte i = end; i != index;) _typefaces[i] = _typefaces[i -= sign];
        _typefaces[index] = item;
      }
    }
    public bool Remove(Typeface item) =>
      item != _typefaces[0]
      ? _typefaces.Remove(_typefaces.OrderBy(p => p.Key).First(p => p.Value == item))
      : false;
    public void RemoveAt(sbyte index) { if (index != 0) _typefaces.Remove(index); }
    public IEnumerator<Typeface> GetEnumerator() =>
      _typefaces.OrderBy(p => p.Key).Select(p => p.Value).GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
  }
}