using System;
using CoreGraphics;
using UIKit;

namespace MatchThreeMore;

/// <summary>
/// Класс с основными параметрами игры
/// </summary>
public static class Properties
{
    // Размеры игрового поля
    public const int LevelColumns = 8;
    public const int LevelRows = 8;

    // Отсуп игрового поля от края экрана слева и справа
    public const int GameFieldPadding = 15;

    // Время анимации обмена камешков
    public const int SwapAnimationDuration = 200;

    // Время анимации "угасания" подсветки выбранного камешка
    public const int SelectedGemTextureFadeDuration = 200;

    // Время анимации уничтожения камешка
    public const int DestructionAnimationDuration = 200;

    // время анимации разрушителя линии
    public const int LineDestructionDuration = 200;

    // Время анимации падения камешков
    public const int FallAnimationDuration = 186;

    // Время раунда
    public const int LevelTime = 180;
    public const int DevLevelTime = 10;

    // Радиус поражения бомбы
    public const int BombBlastRadius = 2;

    // Параметры для лэйблов
    public const int HighScoreLabelWidth = 200;
    public const int CommonLabelHeight = 50;
    public const int CommonLabelWidth = 200;

    // Координаты лэйблов на поле
    public const int HighScoreLabelY = 25;
    public const int TimerLabelY = 50;
    public const int ScoreTitleLabelY = 75;
    public const int ScoreLabelY = 100;

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
        UIGraphics.BeginImageContext(new CGSize(width, height));
        sourceImage.Draw(new CGRect(0, 0, width, height));
        var resultImage = UIGraphics.GetImageFromCurrentImageContext();
        UIGraphics.EndImageContext();

        return resultImage;
    }
}
