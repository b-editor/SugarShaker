using Beutl;
using Beutl.Animation;
using Beutl.Graphics;
using Beutl.Graphics.Effects;
using Beutl.Media;
using Beutl.Media.Immutable;
using Beutl.Media.Source;
using Beutl.Utilities;

using SkiaSharp;

namespace SugarShaker.Effects;

public sealed class WipeEffect : TransitionEffect
{
    public static readonly CoreProperty<float> BlurProperty;
    public static readonly CoreProperty<bool> InvertInProperty;
    public static readonly CoreProperty<bool> InvertOutProperty;
    private float _blur;
    private bool _invertIn;
    private bool _invertOut;

    // ApplyAnimationsで受け取る
    private double _duration;
    private double _currentTime;

    static WipeEffect()
    {
        BlurProperty = ConfigureProperty<float, WipeEffect>(nameof(Blur))
            .Accessor(o => o.Blur, (o, v) => o.Blur = v)
            .Register();

        InvertInProperty = ConfigureProperty<bool, WipeEffect>(nameof(InvertIn))
            .Accessor(o => o.InvertIn, (o, v) => o.InvertIn = v)
            .Register();

        InvertOutProperty = ConfigureProperty<bool, WipeEffect>(nameof(InvertOut))
            .Accessor(o => o.InvertOut, (o, v) => o.InvertOut = v)
            .Register();

        AffectsRender<WipeEffect>(BlurProperty, InvertInProperty, InvertOutProperty);
    }

    public float Blur
    {
        get => _blur;
        set => SetAndRaise(BlurProperty, ref _blur, value);
    }

    public bool InvertIn
    {
        get => _invertIn;
        set => SetAndRaise(InvertInProperty, ref _invertIn, value);
    }

    public bool InvertOut
    {
        get => _invertOut;
        set => SetAndRaise(InvertOutProperty, ref _invertOut, value);
    }

    public override void ApplyTo(FilterEffectContext context)
    {
        if (InTime != 0 && _currentTime < InTime)
        {
            double inProgress = _currentTime / InTime;
            context.Custom((InvertIn ? 1 - inProgress : inProgress, Blur, InvertIn), ApplyCore, (_, r) => r);
        }
        else if (OutTime != 0)
        {
            double lastTime = _duration - _currentTime;

            if (lastTime < OutTime)
            {
                double outProgress = lastTime / OutTime;
                context.Custom((InvertOut ? 1 - outProgress : outProgress, Blur, InvertOut), ApplyCore, (_, r) => r);
            }
        }
    }

    private void ApplyCore((double Progress, float Blur, bool Invert) data, FilterEffectCustomOperationContext context)
    {
        if (context.Target.Surface?.Value is SKSurface surface)
        {
            Size size = context.Target.Size;
            Rect rect = new(size);
            using EffectTarget newTarget = context.CreateTarget((int)size.Width, (int)size.Height);

            float length = MathF.Sqrt(MathF.Pow(size.Width, 2) + MathF.Pow(size.Height, 2)) * (float)data.Progress;
            if (!MathUtilities.AreClose(length, 0))
            {
                using (ImmediateCanvas canvas = context.Open(newTarget))
                {
                    float maskCanvasSize = length + (data.Blur * 3);
                    using EffectTarget maskShape = context.CreateTarget((int)maskCanvasSize, (int)maskCanvasSize);
                    using (var paint = new SKPaint())
                    {
                        if (data.Blur != 0)
                        {
                            paint.ImageFilter = SKImageFilter.CreateBlur(data.Blur, data.Blur);
                        }

                        paint.Color = SKColors.White;
                        maskShape.Surface!.Value!.Canvas.DrawCircle(maskCanvasSize / 2, maskCanvasSize / 2, length / 2, paint);
                    }

                    using SKImage skImage = maskShape.Surface.Value!.Snapshot();
                    using var bitmapRef = Ref<IBitmap>.Create(skImage.ToBitmap());
                    using var bmpSource = new BitmapSource(bitmapRef, "Tmp");
                    var brush = new ImmutableImageBrush(bmpSource, stretch: Stretch.None);

                    using (canvas.PushOpacityMask(brush, rect, data.Invert))
                    {
                        canvas.DrawSurface(surface, default);
                    }
                }
            }

            context.ReplaceTarget(newTarget);
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
