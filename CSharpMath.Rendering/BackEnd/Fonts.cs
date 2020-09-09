using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Typography.OpenFont;

namespace CSharpMath.Rendering.BackEnd {
  public readonly struct Fonts : Display.FrontEnd.IFont<Glyph>, IEnumerable<Typeface> {
    static Fonts() {
      var reader = new OpenFontReader();
      Typeface LoadFont(string fileName) {
        var typeface = reader.Read(
          System.Reflection.Assembly.GetExecutingAssembly()
          .GetManifestResourceStream($"CSharpMath.Rendering.Reference_Fonts.{fileName}")
        );
        if (typeface == null) throw new Structures.InvalidCodePathException("Invalid predefined font!");
        typeface.UpdateAllCffGlyphBounds();
        return typeface;
      }
      GlobalTypefaces = new Typefaces(LoadFont("latinmodern-math.otf"));
      GlobalTypefaces.AddOverride(LoadFont("AMS-Supplements.otf"));
      GlobalTypefaces.AddSupplement(LoadFont("cyrillic-modern-nmr10.otf"));
    }
    public Fonts(IEnumerable<Typeface> localTypefaces, float pointSize) {
      PointSize = pointSize;
      Typefaces = localTypefaces.Concat(GlobalTypefaces);
    }
    public Fonts(Fonts cloneMe, float pointSize) {
      PointSize = pointSize;
      Typefaces = cloneMe.Typefaces;
    }
    public static Typefaces GlobalTypefaces { get; }
    public float PointSize { get; }
    public IEnumerable<Typeface> Typefaces { get; }
    public Typeface MathTypeface => Typefaces.First(t => t.HasMathTable());
    public Typography.OpenFont.MathGlyphs.MathConstants MathConsts =>
      MathTypeface.MathConsts ?? throw new Structures.InvalidCodePathException(nameof(MathTypeface) + " doesn't have " + nameof(MathConsts));
    public IEnumerator<Typeface> GetEnumerator() => Typefaces.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => Typefaces.GetEnumerator();
  }
}
