using System.ComponentModel.DataAnnotations;

using Beutl;
using Beutl.Graphics;
using Beutl.Graphics.Effects;

namespace SugarShaker.Effects;

public sealed class MakeTransparent : FilterEffect
{
    public static readonly CoreProperty<float> OpacityProperty;
    private float _opacity = 50;

    static MakeTransparent()
    {
        OpacityProperty = ConfigureProperty<float, MakeTransparent>(nameof(Opacity))
            .Accessor(o => o.Opacity, (o, v) => o.Opacity = v)
            .DefaultValue(50)
            .Register();

        AffectsRender<MakeTransparent>(OpacityProperty);
    }

    [Range(0, 100)]
    public float Opacity
    {
        get => _opacity;
        set => SetAndRaise(OpacityProperty, ref _opacity, value);
    }

    public override void ApplyTo(FilterEffectContext context)
    {
        context.LookupTable(_opacity / 100, 1, (data, t) =>
        {
            LookupTable.Linear(t.R);
            LookupTable.Linear(t.G);
            LookupTable.Linear(t.B);

            for (int i = 0; i < t.A.Length; i++)
            {
                t.A[i] = (byte)(i * data);
            }
        });
    }
}
