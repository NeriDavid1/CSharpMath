
using AngouriMath;
using AngouriMath.Extensions;
using CSharpMath.Atom;
namespace CSharpMath {

  public static class Extensions {
    public static Entity ToEntity(this MathList self) {
      var mathstring = Evaluation.ConvertToMathString(self);
      mathstring = Evaluation.HandleSpecialCases(mathstring);
      return mathstring.ToEntity();
    }
  }
}
