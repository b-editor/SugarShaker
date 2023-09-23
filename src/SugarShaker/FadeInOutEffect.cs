using System.ComponentModel.DataAnnotations;

using Beutl;
using Beutl.Animation;
using Beutl.Graphics;
using Beutl.Graphics.Effects;

namespace SugarShaker;

public class FadeInOutEffect : FilterEffect
{
    public static readonly CoreProperty<TimeSpan> FadeInTimeProperty;
    public static readonly CoreProperty<TimeSpan> FadeOutTimeProperty;
    private TimeSpan _fadeInTime = TimeSpan.FromSeconds(0.5);
    private TimeSpan _fadeOutTime = TimeSpan.FromSeconds(0.5);

    // ApplyAnimationsで受け取る
    private TimeSpan _duration;
    private TimeSpan _currentTime;

    static FadeInOutEffect()
    {
        FadeInTimeProperty = ConfigureProperty<TimeSpan, FadeInOutEffect>(nameof(FadeInTime))
            .Accessor(o => o.FadeInTime, (o, v) => o.FadeInTime = v)
            .DefaultValue(TimeSpan.FromSeconds(0.5))
            .Register();

        FadeOutTimeProperty = ConfigureProperty<TimeSpan, FadeInOutEffect>(nameof(FadeOutTime))
            .Accessor(o => o.FadeOutTime, (o, v) => o.FadeOutTime = v)
            .DefaultValue(TimeSpan.FromSeconds(0.5))
            .Register();

        AffectsRender<FadeInOutEffect>(FadeInTimeProperty, FadeOutTimeProperty);
    }

    [Range(typeof(TimeSpan), "00:00:00", "24:00:00")]
    public TimeSpan FadeInTime
    {
        get => _fadeInTime;
        set => SetAndRaise(FadeInTimeProperty, ref _fadeInTime, value);
    }

    [Range(typeof(TimeSpan), "00:00:00", "24:00:00")]
    public TimeSpan FadeOutTime
    {
        get => _fadeOutTime;
        set => SetAndRaise(FadeOutTimeProperty, ref _fadeOutTime, value);
    }

    public override void ApplyTo(FilterEffectContext context)
    {
        static void MakeTransparency(double opacity, FilterEffectContext context)
        {
            context.LookupTable(opacity, 1, (double data, (byte[] A, byte[] R, byte[] G, byte[] B) t) =>
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

        if (_fadeInTime != TimeSpan.Zero && _currentTime < _fadeInTime)
        {
            double fadeIn = _currentTime / _fadeInTime;
            MakeTransparency(fadeIn, context);
        }
        else if (_fadeInTime != TimeSpan.Zero)
        {
            TimeSpan lastTime = _duration - _currentTime;

            if (lastTime < _fadeOutTime)
            {
                double fadeOut = lastTime / _fadeInTime;
                MakeTransparency(fadeOut, context);
            }
        }
    }

    public override void ApplyAnimations(IClock clock)
    {
        base.ApplyAnimations(clock);

        _duration = clock.DurationTime;
        _currentTime = clock.CurrentTime;

        RaiseInvalidated(new(this));
    }
}
