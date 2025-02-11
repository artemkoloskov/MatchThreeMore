namespace MatchThreeMore;

/// <summary>
/// Класс игрового уровня. Отвечает создание, перемешивание, хранение массива камешков
/// </summary>
public class Level
{
    public Gem?[,] GemArray { get; set; } = new Gem[Properties.LEVEL_ROWS, Properties.LEVEL_COLUMNS];
    public GemList BonusesToAnimate { get; set; } = [];
    public GemList BonusesToAddSpritesTo { get; set; } = [];
    public GemList GemList => new(GemArray);
    public List<GemList> DestroyedChains { get; set; } = [];
    public List<Swap> Swaps { get; set; } = [];
    public int Score { get; set; }

    public int RowsNumber { get; }
    public int ColumnsNumber { get; }
    public bool DevModeIsOn { get; }
    public LevelData? LevelData { get; }

    /// <summary>
    /// Конструктор класса <see cref="T:MatchThreeMore.Level"/>.
    /// </summary>
    /// <param name="devMode">Если <c>true</c> активирует режим разработчика.</param>
    public Level(bool devMode)
    {
        DevModeIsOn = devMode;

        // Если режим разработчика включен - загружает уровень из файла
        if (DevModeIsOn)
        {
            LevelData = LevelData.LoadFrom("Dev_Level_1.json");

            // Запоминаем размеры уровня для анимации и интеракции
            RowsNumber = (int)Math.Sqrt(LevelData.Level.Length);
            ColumnsNumber = RowsNumber;
        }
        else
        {
            // размеры уровня берем из файла со свойствами
            RowsNumber = Properties.LEVEL_ROWS;
            ColumnsNumber = Properties.LEVEL_COLUMNS;
        }
    }

    /// <summary>
    /// Заполнение массива камешков случайными камешками
    /// </summary>
    public void Shuffle()
    {
        GemArray = new Gem[RowsNumber, ColumnsNumber];

        // Если включен режим разработчика - заполняем поле из level data
        if (DevModeIsOn)
        {
            if (LevelData is null)
            {
                throw new Exception("Не удалось загрузить уровень");
            }

            for (int row = 0; row < RowsNumber; row++)
            {
                for (int column = 0; column < ColumnsNumber; column++)
                {
                    (GemType gemType, bool isALineDestroyer, bool isHorisontal, bool isABomb) =
                        LevelData.GetGemTypeFromLevelDataAt(row, column);

                    if (isABomb)
                    {
                        GemArray[row, column] = new Gem(
                            isABomb,
                            gemType,
                            row,
                            column);
                    }
                    else
                    {
                        GemArray[row, column] = new Gem(
                            isALineDestroyer,
                            isHorisontal,
                            gemType,
                            row,
                            column);
                    }
                }
            }

            DetectPossibleSwaps();

            return;
        }

        // Случайное заполнение уровня, если игра в обычном режиме
        Random rnd = new();

        for (int row = 0; row < RowsNumber; row++)
        {
            for (int column = 0; column < ColumnsNumber; column++)
            {
                var newGemType = (GemType)rnd.Next(Enum.GetNames<GemType>().Length);

                // Проверка на то, чтобы цепочки одинаковых камешков не появились при построении уровня
                // Массив проверяем только назад, т.к. впереди массив еще не заполнен
                while (column >= 2
                        && GemArray[row, column - 1]?.GemType == newGemType
                        && GemArray[row, column - 2]?.GemType == newGemType
                    || row >= 2
                        && GemArray[row - 1, column]?.GemType == newGemType
                        && GemArray[row - 2, column]?.GemType == newGemType)
                {
                    newGemType = (GemType)rnd.Next(Enum.GetNames<GemType>().Length);
                }

                GemArray[row, column] = new Gem(newGemType, row, column);
            }
        }

        DetectPossibleSwaps();

        //______ДЛЯ ДЕБАГА______
        PrintOutGemArrayToConsole(false);
    }

    /// <summary>
    /// Сканирует игровое поле на возможные смены камешков
    /// </summary>
    public void DetectPossibleSwaps()
    {
        // очищаем список доступных свопов
        Swaps.Clear();

        for (int row = 0; row < RowsNumber; row++)
        {
            for (int column = 0; column < ColumnsNumber; column++)
            {
                // Запоминаем камешек в текущей  позиции
                Gem? gem = GemArray[row, column];

                if (gem is not null)
                {
                    // Проверка обмена вправо
                    // Последний ряд не сканируем
                    if (column < ColumnsNumber - 1)
                    {
                        // Запоминаем камешек в следующем столбце
                        Gem? otherGem = GemArray[row, column + 1];

                        if (otherGem is not null)
                        {
                            // Меняем местами камешки в массиве
                            GemArray[row, column] = otherGem;
                            GemArray[row, column + 1] = gem;

                            // Если получилась цепь - запоминаем свопы в одну и в другую сторону
                            if (GetChainAt(row, column).Count >= 3
                                || GetChainAt(row, column + 1).Count >= 3)
                            {
                                Swaps.Add(new Swap(gem, otherGem));
                                Swaps.Add(new Swap(otherGem, gem));
                            }

                            // Возвращаем массив в исходное состояние
                            GemArray[row, column] = gem;
                            GemArray[row, column + 1] = otherGem;
                        }
                    }

                    // Проверка обмена вверх, принцип тот же
                    if (row < RowsNumber - 1)
                    {
                        Gem? otherGem = GemArray[row + 1, column];

                        if (otherGem is not null)
                        {
                            GemArray[row, column] = otherGem;
                            GemArray[row + 1, column] = gem;

                            if (GetChainAt(row, column).Count >= 3 || GetChainAt(row + 1, column).Count >= 3)
                            {
                                Swaps.Add(new Swap(gem, otherGem));
                                Swaps.Add(new Swap(otherGem, gem));
                            }

                            GemArray[row, column] = gem;
                            GemArray[row + 1, column] = otherGem;
                        }
                    }
                }
            }
        }

        // ________ДЛЯ ДЕБАГА_______
        foreach (Swap swap in Swaps)
        {
            Console.Write(swap + "\n");
        }
    }

    /// <summary>
    /// Обмена местами в массиве двух камешков.
    /// </summary>
    /// <param name="swap">Объект с камешками на обмен</param>
    public void Perform(Swap swap)
    {
        int columnA = swap.GemA.Column;
        int rowA = swap.GemA.Row;
        int columnB = swap.GemB.Column;
        int rowB = swap.GemB.Row;

        // Меняем камешки местами
        GemArray[rowA, columnA] = swap.GemB;
        swap.GemB.Column = columnA;
        swap.GemB.Row = rowA;

        GemArray[rowB, columnB] = swap.GemA;
        swap.GemA.Column = columnB;
        swap.GemA.Row = rowB;

        GemList chain = GetChainAt(rowB, columnB);

        Gem gemAA = GemArray[rowA, columnA] ?? throw new Exception("Камешка нет в массиве");
        Gem gemBB = GemArray[rowB, columnB] ?? throw new Exception("Камешка нет в массиве");
        // проверяем получившуюся цепочку на длину. если цепочка больше трех на
        // месте перемещенного камешка ставим бонус
        // записываем бонус в список на добавление спрайтов для сцены
        if (chain.Count == 4)
        {
            bool isHorizontal = chain.ToArray()[0].Row == chain.ToArray()[1].Row;

            BonusesToAddSpritesTo.Add(new Gem(
                isALineDestroyer: true,
                isHorizontal,
                gemBB.GemType,
                rowB,
                columnB));
        }

        if (chain.Count >= 5)
        {
            BonusesToAddSpritesTo.Add(new Gem(
                isABomb: true,
                gemBB.GemType,
                rowB,
                columnB));
        }

        chain = GetChainAt(rowA, columnA);

        if (chain.Count == 4)
        {
            bool isHorizontal = chain.ToArray()[0].Row == chain.ToArray()[1].Row;
            BonusesToAddSpritesTo.Add(new Gem(
                isALineDestroyer: true,
                isHorizontal,
                gemAA.GemType,
                rowA,
                columnA));
        }

        if (chain.Count >= 5)
        {
            BonusesToAddSpritesTo.Add(new Gem(
                isABomb: true,
                gemAA.GemType,
                rowA,
                columnA));
        }
    }

    /// <summary>
    /// Вывод состояния массива в консоль для дебага
    /// </summary>
    /// <param name="withFullGemDescriptions">Если <c>true</c> - вывод с полным
    /// описанием камешков.</param>
    private void PrintOutGemArrayToConsole(bool withFullGemDescriptions)
    {
        Console.Write("Новое состояние\n\n");

        for (int row = RowsNumber - 1; row >= 0; row--)
        {
            for (int column = 0; column < ColumnsNumber; column++)
            {
                Gem? gem = GemArray[row, column];

                if (gem is null)
                {
                    Console.Write("_ ");

                    continue;
                }

                if (withFullGemDescriptions)
                {
                    Console.Write(gem + " ");

                    continue;
                }

                if (gem.IsALineDestroyer)
                {
                    Console.Write(gem.GemType.ToString()[..1].ToUpper() + " ");
                }
                else
                {
                    Console.Write(gem.GemType.ToString()[..1] + " ");
                }
            }

            Console.Write("\n");
        }
    }

    /// <summary>
    /// Проверка нахождения камешка в цепочке одинаковых камешков
    /// В этом проекте приоритет отдается горизонтальным цепочкам.
    /// Иерархия появилась т.к. Т- и Х- образные цепочки не проверяются и не просчитываются.
    /// </summary>
    /// <returns>Цепочка камешков</returns>
    /// <param name="gemRow">Ряд</param>
    /// <param name="gemColumn">Колонка</param>
    private GemList GetChainAt(int gemRow, int gemColumn)
    {
        GemList possibleHorizontalChain = [];
        Gem? gem = GemArray[gemRow, gemColumn];

        // Если камешка в данной позиции вообще нет - возвращаем пустой список
        if (gem is null)
        {
            return possibleHorizontalChain;
        }

        possibleHorizontalChain.Add(gem);

        GemType gemTypeToCheck = gem.GemType;

        // Флаг разрыва цепочки
        bool chainIsNotBroken = true;

        //Проверка влево
        for (int column = gemColumn - 1; column >= 0
            && chainIsNotBroken; column--)
        {
            Gem? gemToCheck = GemArray[gemRow, column];

            if (gemToCheck is not null && gemTypeToCheck == gemToCheck.GemType)
            {
                possibleHorizontalChain.Add(gemToCheck);
            }
            else
            {
                chainIsNotBroken = false;
            }
        }

        chainIsNotBroken = true;

        // Проверка вправо
        for (int column = gemColumn + 1; column < ColumnsNumber
            && chainIsNotBroken; column++)
        {
            Gem? gemToCheck = GemArray[gemRow, column];

            if (gemToCheck is not null && gemTypeToCheck == gemToCheck.GemType)
            {
                possibleHorizontalChain.Add(gemToCheck);
            }
            else
            {
                chainIsNotBroken = false;
            }
        }

        // Сбрасываем цепочку, если не нашлось цепочки по горизонтали
        GemList possibleVerticalChain = [GemArray[gemRow, gemColumn]];

        chainIsNotBroken = true;

        // Проверка вниз
        for (int row = gemRow - 1; row >= 0 && chainIsNotBroken; row--)
        {
            Gem? gemToCheck = GemArray[row, gemColumn];

            if (gemToCheck is not null && gemTypeToCheck == gemToCheck.GemType)
            {
                possibleVerticalChain.Add(gemToCheck);
            }
            else
            {
                chainIsNotBroken = false;
            }
        }

        chainIsNotBroken = true;

        // Проверка вверх
        for (int row = gemRow + 1; row < RowsNumber && chainIsNotBroken; row++)
        {
            Gem? gemToCheck = GemArray[row, gemColumn];

            if (gemToCheck is not null && gemTypeToCheck == gemToCheck.GemType)
            {
                possibleVerticalChain.Add(gemToCheck);
            }
            else
            {
                chainIsNotBroken = false;
            }
        }

        if (possibleHorizontalChain.Count >= 3 && possibleVerticalChain.Count >= 3)
        {
            possibleHorizontalChain.AddRange(possibleVerticalChain);
        }

        if (possibleHorizontalChain.Count >= 3)
        {
            return possibleHorizontalChain;
        }

        return possibleVerticalChain;
    }

    /// <summary>
    /// Сканирование на наличие цепочки
    /// </summary>
    /// <returns><c>true</c>, если цепочка найдена, <c>false</c> если не найдена.</returns>
    public GemList? RetrieveChain()
    {
        for (int row = 0; row < RowsNumber; row++)
        {
            for (int column = 0; column < ColumnsNumber; column++)
            {
                GemList chain = GetChainAt(row, column);
                if (chain is not null && chain.Count >= 3)
                {
                    return chain;
                }
            }
        }

        return null;
    }

    public List<GemList> RetrieveAllChainsOnLevel()
    {
        List<GemList> chains = [];

        for (int row = 0; row < RowsNumber; row++)
        {
            for (int column = 0; column < ColumnsNumber; column++)
            {
                GemList chain = GetChainAt(row, column);

                if (chain is not null && chain.Count >= 3 && !chains.Contains(chain))
                {
                    chains.Add(chain);
                }
            }
        }

        return chains;
    }

    /// <summary>
    /// Уничтожение цепей в массиве камешков:
    /// метод сканирует массив на цепочки до тех пор, пока они есть,
    /// найдя цепочку заносит ее в список удаляемых цепочек, после чего
    /// уничтожает соответствующие камешки в массиве. Так же проверяет цепочки
    /// в них бонусов, заполняет список с бонусами.
    /// </summary>
    /// <returns><c>true</c> если нашелся хотя бы один активируемый бонус</returns>
    public void DestroyChains()
    {
        GemList? chain = RetrieveChain();

        // Повторяем процесс пока находим цепочку на уровне
        while (chain is not null)
        {
            GemList bonuses;
            bool needToReiterate = true;

            while (needToReiterate)
            {
                needToReiterate = false;

                bonuses = chain.GetAllBonuses();

                foreach (Gem bonus in bonuses)
                {
                    // Запоминаем разрушитель в списке разрушителей на анимацию
                    if (!BonusesToAnimate.Contains(bonus))
                    {
                        BonusesToAnimate.Add(bonus);
                    }

                    // Заносим в цепочку весь ряд или столбец,
                    // в зависимости от направленности разрушителя
                    if (bonus.IsALineDestroyer)
                    {
                        if (bonus.IsHorizontal)
                        {
                            for (int column = 0; column < ColumnsNumber; column++)
                            {
                                Gem? gem = GemArray[bonus.Row, column];

                                if (gem is not null && !chain.Contains(gem))
                                {
                                    chain.Add(gem);
                                }
                            }
                        }
                        else
                        {
                            for (int row = 0; row < RowsNumber; row++)
                            {
                                Gem? gem = GemArray[row, bonus.Column];

                                if (gem is not null && !chain.Contains(gem))
                                {
                                    chain.Add(gem);
                                }
                            }
                        }
                    }

                    if (bonus.IsABomb)
                    {
                        int i;
                        int j;

                        if (bonus.Row == 0)
                        {
                            i = 0;
                        }
                        else
                        {
                            if (bonus.Row - Properties.BOMB_BLAST_RADIUS < 0)
                            {
                                i = 0;
                            }
                            else
                            {
                                i = bonus.Row - Properties.BOMB_BLAST_RADIUS;
                            }
                        }

                        if (bonus.Column == 0)
                        {
                            j = 0;
                        }
                        else
                        {
                            if (bonus.Column - Properties.BOMB_BLAST_RADIUS < 0)
                            {
                                j = 0;
                            }
                            else
                            {
                                j = bonus.Column - Properties.BOMB_BLAST_RADIUS;
                            }
                        }

                        for (int row = i; row <= bonus.Row + Properties.BOMB_BLAST_RADIUS && row <= RowsNumber - 1; row++)
                        {
                            for (int column = j; column <= bonus.Column + Properties.BOMB_BLAST_RADIUS && column <= ColumnsNumber - 1; column++)
                            {
                                Gem? gem = GemArray[row, column];

                                if (gem is not null && !chain.Contains(gem))
                                {
                                    chain.Add(gem);
                                }
                            }
                        }
                    }
                }

                needToReiterate = bonuses.Count < chain.GetAllBonuses().Count;
            }

            // подсчитываем очки за цепочку, добавляем их к общему счету
            // int chainScore = (chain.Count - 2) * 10;
            Score += chain.GetScore();

            // заносим цепочку в список на анимацию
            DestroyedChains.Add(chain);

            // удаляем из массива камешки в соответствии с цепью
            chain.ForEach(gem => GemArray[gem.Row, gem.Column] = null);

            // Получаем новую цепочку
            chain = RetrieveChain();
        }

        //______ДЛЯ ДЕБАГА______
        PrintOutGemArrayToConsole(false);
    }

    /// <summary>
    /// Заполнение пустых ячеек в массиве модели.
    /// </summary>
    /// <returns>Список новых камешков на обогащение спрайтами</returns>
    public GemList FillBlanks()
    {
        GemList newGems = [];
        Random rnd = new();

        for (int row = 0; row < RowsNumber; row++)
        {
            for (int column = 0; column < ColumnsNumber; column++)
            {
                if (GemArray[row, column] is not null)
                {
                    continue;
                }

                bool foundAppropriateGemType = false;

                // Создаем камешек, записываем его в массив, проверяем не создал ли он цепочку
                // если создал - повторяем до те пор, пока не найдем камешек, который не создаст цепочку
                Gem? gem = null;

                while (!foundAppropriateGemType)
                {
                    var newGemType = (GemType)rnd.Next(Enum.GetNames<GemType>().Length);

                    gem = new Gem(newGemType, row, column);
                    GemArray[row, column] = gem;

                    foundAppropriateGemType = GetChainAt(row, column).Count < 3;
                }

                // Добавляем новый камешек в список камешков, которым позже добавят спрайты
                if (gem is null)
                {
                    throw new Exception("Камешек не создан");
                }

                newGems.Add(gem);
            }
        }

        //______ДЛЯ ДЕБАГА______
        PrintOutGemArrayToConsole(false);

        return newGems;
    }

    /// <summary>
    /// Падение камешков на пустые места. Сканирует колонку на пустоту,
    /// найдя пустоту сканирует снова, на наличие камешка сверху. При наличии -
    /// "роняет" его на пустое место. Упавшие камешки заносятся в колонки,
    /// Колонки заносятся в список на обновление спрайтов
    /// </summary>
    /// <returns>Список колонок камешков, которые переместились,
    /// для обновления позиции спрайтов</returns>
    public List<GemList> DropGems()
    {
        List<GemList> columns = [];

        // пробегаем по столбцам
        for (int column = 0; column < ColumnsNumber; column++)
        {
            GemList columnOfFallenGems = [];

            for (int row = 0; row < RowsNumber; row++)
            {
                // Если текущая ячейка пустая - сканируем текущий столбец вверх
                if (GemArray[row, column] is not null)
                {
                    continue;
                }

                bool foundNewGemAbove = false;

                for (int aboveRow = row + 1; aboveRow < RowsNumber && !foundNewGemAbove; aboveRow++)
                {
                    // находим непустую ячейку и переносим камешек оттуда в текущую ячейку
                    if (GemArray[aboveRow, column] is null)
                    {
                        continue;
                    }

                    GemArray[row, column] = GemArray[aboveRow, column];

                    // ячейку из которой спустили камешек зануляем
                    GemArray[aboveRow, column] = null;
                    // Обновляем координату ряда у камешка
                    Gem? gem = GemArray[row, column]
                        ?? throw new Exception("Камешек не найден");

                    gem.Row = row;

                    //запоминаем камешек для анимации спрайтов
                    columnOfFallenGems.Add(gem);

                    foundNewGemAbove = true;
                }
            }

            // столбец из перенесенных камешков запоминаем в списке таких столбцов
            // чтобы анимировать их спрайты
            if (columnOfFallenGems.Count > 0)
            {
                columns.Add(columnOfFallenGems);
            }
        }

        //_______ДЛЯ ДЕБАГА______
        PrintOutGemArrayToConsole(false);

        return columns;
    }
}
