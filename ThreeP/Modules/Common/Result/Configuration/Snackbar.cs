namespace Mac.Modules.Snackbar;

public static class Snackbar
{
    public static IServiceCollection AddSnackbarConfig(this IServiceCollection services)
    {
        services.Configure<SnackbarConfiguration>(config =>
        {
            config.PositionClass = Defaults.Classes.Position.BottomLeft;

            config.PreventDuplicates = false;
            config.NewestOnTop = false;
            config.ShowCloseIcon = true;
            config.VisibleStateDuration = 10000;
            config.HideTransitionDuration = 500;
            config.ShowTransitionDuration = 500;
            config.SnackbarVariant = Variant.Outlined;
        });
        return services;
    }
}