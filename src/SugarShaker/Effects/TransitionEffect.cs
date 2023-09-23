using System.ComponentModel.DataAnnotations;

using Beutl;
using Beutl.Graphics.Effects;

namespace SugarShaker.Effects;

public abstract class TransitionEffect: FilterEffect
{
    public static readonly CoreProperty<float> InTimeProperty;
    public static readonly CoreProperty<float> OutTimeProperty;
    private float _inTime = 0.5f;
    private float _outTime = 0.5f;

    static TransitionEffect()
    {
        InTimeProperty = ConfigureProperty<float, TransitionEffect>(nameof(InTime))
            .Accessor(o => o.InTime, (o, v) => o.InTime = v)
            .DefaultValue(0.5f)
            .Register();

        OutTimeProperty = ConfigureProperty<float, TransitionEffect>(nameof(OutTime))
            .Accessor(o => o.OutTime, (o, v) => o.OutTime = v)
            .DefaultValue(0.5f)
            .Register();

        AffectsRender<TransitionEffect>(InTimeProperty, OutTimeProperty);
    }

    [Range(0, float.MaxValue)]
    [Display(Name = "イン (秒)")]
    public float InTime
    {
        get => _inTime;
        set => SetAndRaise(InTimeProperty, ref _inTime, value);
    }

    [Range(0, float.MaxValue)]
    [Display(Name = "アウト (秒)")]
    public float OutTime
    {
        get => _outTime;
        set => SetAndRaise(OutTimeProperty, ref _outTime, value);
    }
}
