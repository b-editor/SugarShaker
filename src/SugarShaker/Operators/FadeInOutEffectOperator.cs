using Beutl.Operators.Configure.Effects;
using Beutl.Styling;
using SugarShaker.Effects;

namespace SugarShaker.Operators;

public sealed class FadeInOutEffectOperator : FilterEffectOperator<FadeInOutEffect>
{
    public Setter<float> InTime { get; set; } = new(TransitionEffect.InTimeProperty, 0.5f);

    public Setter<float> OutTime { get; set; } = new(TransitionEffect.OutTimeProperty, 0.5f);
}
