using Beutl.Extensibility;
using Beutl.Services;
using SugarShaker.Effects;

namespace SugarShaker;

[Export]
public sealed class SugarShakerExtension : Extension
{
    public override string Name => "Sugar Shaker";

    public override string DisplayName => "Sugar Shaker";

    public override void Load()
    {
        base.Load();

        LibraryService.Current.RegisterGroup(
            "Sugar Shaker",
            group => group.AddMultiple(
                "フェードインアウト",
                item => item.Bind<FadeInOutEffect>(KnownLibraryItemFormats.FilterEffect)
            ).AddMultiple(
                "ワイプ",
                item => item.Bind<WipeEffect>(KnownLibraryItemFormats.FilterEffect)
            ).AddMultiple(
                "透明度にする",
                item => item.Bind<MakeTransparent>(KnownLibraryItemFormats.FilterEffect)
            )
        );
    }
}
