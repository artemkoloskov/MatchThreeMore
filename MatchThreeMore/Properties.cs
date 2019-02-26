namespace MatchThreeMore
{
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

        // Радиус поражения бомбы
        public const int BombBlastRadius = 2;
    }
}
