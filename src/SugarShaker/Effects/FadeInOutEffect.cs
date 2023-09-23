using Beutl.Animation;
using Beutl.Graphics;
using Beutl.Graphics.Effects;

namespace SugarShaker.Effects;

public sealed class FadeInOutEffect : TransitionEffect
{
    // ApplyAnimationsで受け取る
    private double _duration;
    private double _currentTime;

    public override void ApplyTo(FilterEffectContext context)
    {
        static void MakeTransparency(double opacity, FilterEffectContext context)
        {
            context.LookupTable(opacity, 1, (data, t) =>
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

        if (InTime != 0 && _currentTime < InTime)
        {
            double fadeIn = _currentTime / InTime;
            MakeTransparency(fadeIn, context);
        }
        else if (OutTime != 0)
        {
            double lastTime = _duration - _currentTime;

            if (lastTime < OutTime)
            {
                double fadeOut = lastTime / OutTime;
                MakeTransparency(fadeOut, context);
            }
        }
    }

    public override void ApplyAnimations(IClock clock)
    {
        base.ApplyAnimations(clock);

        _duration = clock.DurationTime.TotalSeconds;
        _currentTime = clock.CurrentTime.TotalSeconds;

        RaiseInvalidated(new(this));
    }
}
