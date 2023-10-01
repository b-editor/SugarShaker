using Beutl.Operators.Configure.Effects;
using Beutl.Styling;

using SugarShaker.Effects;

namespace SugarShaker.Operators;

public sealed class WipeEffectOperator : FilterEffectOperator<WipeEffect>
{
    public Setter<float> InTime { get; set; } = new(TransitionEffect.InTimeProperty, 0.5f);

    public Setter<float> OutTime { get; set; } = new(TransitionEffect.OutTimeProperty, 0.5f);

    public Setter<float> Blur { get; set; } = new(WipeEffect.BlurProperty, 0);

    public Setter<bool> InvertIn { get; set; } = new(WipeEffect.InvertInProperty, false);

    public Setter<bool> InvertOut { get; set; } = new(WipeEffect.InvertOutProperty, false);
}
