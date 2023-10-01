using Beutl.Operators.Configure.Effects;
using Beutl.Styling;

using SugarShaker.Effects;

namespace SugarShaker.Operators;

public sealed class MakeTransparentOperator : FilterEffectOperator<MakeTransparent>
{
    public Setter<float> Opacity { get; set; } = new(MakeTransparent.OpacityProperty, 50);
}
