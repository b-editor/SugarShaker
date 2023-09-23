using Beutl.Extensibility;
using Beutl.Services;

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
            )
        );
    }
}
