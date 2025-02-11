namespace MatchThreeMore;

/// <summary>
/// Класс с основными параметрами игры
/// </summary>
public static class Properties
{
    // Размеры игрового поля
    public const int LEVEL_COLUMNS = 8;
    public const int LEVEL_ROWS = 8;

    // Отступ игрового поля от края экрана слева и справа
    public const int GAME_FIELD_PADDING = 15;

    // Время анимации обмена камешков
    public const int SWAP_ANIMATION_DURATION = 200;

    // Время анимации "угасания" подсветки выбранного камешка
    public const int SELECTED_GEM_TEXTURE_FADE_DURATION = 200;

    // Время анимации уничтожения камешка
    public const int DESTRUCTION_ANIMATION_DURATION = 200;

    // время анимации разрушителя линии
    public const int LINE_DESTRUCTION_DURATION = 200;

    // Время анимации падения камешков
    public const int FALL_ANIMATION_DURATION = 186;

    // Время раунда
    public const int LEVEL_TIME = 180;
    public const int DEV_LEVEL_TIME = 10;

    // Радиус поражения бомбы
    public const int BOMB_BLAST_RADIUS = 2;

    // Параметры для лэйблов
    public const int HIGH_SCORE_LABEL_WIDTH = 200;
    public const int COMMON_LABEL_HEIGHT = 50;
    public const int COMMON_LABEL_WIDTH = 200;

    // Координаты лэйблов на поле
    public const int HIGH_SCORE_LABEL_Y = 25;
    public const int TIMER_LABEL_Y = 50;
    public const int SCORE_TITLE_LABEL_Y = 75;
    public const int SCORE_LABEL_Y = 100;

    // Цвет кнопок
    public static readonly UIColor ButtonColor = new(red: 0.64f, green: 0.76f, blue: 0.97f, alpha: 1.0f);

    // Шрифт
    public static readonly UIFont CommonFont = UIFont.FromName("GillSans-BoldItalic", 18f);
}

public static class AdditionalMethods
{
    public static UIImage ResizeUIImage(UIImage sourceImage, float widthToScale, float heightToScale)
    {
        CGSize sourceSize = sourceImage.Size;
        double maxResizeFactor = Math.Max(widthToScale / sourceSize.Width, heightToScale / sourceSize.Height);
        double width = maxResizeFactor * sourceSize.Width;
        double height = maxResizeFactor * sourceSize.Height;
        var renderer = new UIGraphicsImageRenderer(new CGSize(width, height));
        UIImage resultImage = renderer.CreateImage((context) =>
        {
            sourceImage.Draw(new CGRect(0, 0, width, height));
        });

        return resultImage;
    }
}
