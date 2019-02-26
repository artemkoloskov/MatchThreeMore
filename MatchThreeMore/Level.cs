using System;
using System.Collections.Generic;

namespace MatchThreeMore
{
    /// <summary>
    /// Класс игрового уровня. Отвечает создание, перемешивание, хранение массива камешков
    /// </summary>
    public class Level
    {
        public Gem[,] GemArray { get; set; }
        public GemList BonusesToAnimate { get; set; } = new GemList();
        public GemList BonusesToAddSpritesTo { get; set; } = new GemList();
        public List<GemList> DestroyedChains { get; set; } = new List<GemList>();
        public List<Swap> Swaps { get; set; } = new List<Swap>();
        public int Score { get; set; }

       
        public int RowsNumber { get; }
        public int ColumnsNumber { get; }
        public bool DevModeIsOn { get; }
        public LevelData LevelData { get; }

        /// <summary>
        /// Конструктор класса <see cref="T:MatchThreeMore.Level"/>.
        /// </summary>
        /// <param name="devMode">Если <c>true</c> активирует режим разработчика.</param>
        public Level (bool devMode)
        {
            DevModeIsOn = devMode;

            // Если режим разработчика включен - загружает уровень из файла
            if (DevModeIsOn)
            {
                LevelData = LevelData.LoadFrom("Dev_Level_1.json");

                // Запоминаем размеры уровня для анимации и интеракции
                RowsNumber = (int)Math.Sqrt(LevelData.level.Length);
                ColumnsNumber = RowsNumber;
            }
            else
            {
                // размеры уровня берем из файла со свойствами
                RowsNumber = Properties.LevelRows;
                ColumnsNumber = Properties.LevelColumns;
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
                for (int row = 0; row < RowsNumber; row++)
                {
                    for (int column = 0; column < ColumnsNumber; column++)
                    {
                        (GemType gemType, bool isALineDestroyer, bool isHorisontal) = LevelData.GetGemTypeFromLevelDataAt(row, column);

                        GemArray[row, column] = new Gem(isALineDestroyer, isHorisontal, gemType, row, column);
                    }
                }

                DetectPossibleSwaps();

                return;
            }

            // Рандомное заполнение уровня, если игра в обычном режиме
            Random rnd = new Random();

            for (int row = 0; row < RowsNumber; row++)
            {
                for (int column = 0; column < ColumnsNumber; column++)
                {
                    GemType newGemType = (GemType)rnd.Next(Enum.GetNames(typeof(GemType)).Length);

                    // Проверка на то, чтобы цепочки одинаковых камешков не появились при построении уровня
                    // Массив проверяем только назад, т.к. впереди массив еще не заполнен
                    while (column >= 2 &&
                            GemArray[row, column - 1].GemType == newGemType &&
                            GemArray[row, column - 2].GemType == newGemType ||
                           row >= 2 &&
                            GemArray[row - 1, column].GemType == newGemType &&
                            GemArray[row - 2, column].GemType == newGemType)
                    {
                        newGemType = (GemType)rnd.Next(Enum.GetNames(typeof(GemType)).Length);
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
                    Gem gem = GemArray[row, column];

                    if (gem != null)
                    {
                        // Проверка обмена вправо
                        // Последний ряд не сканируем
                        if (column < ColumnsNumber - 1)
                        {
                            // Запоминаем камешек в следующем столбце
                            Gem otherGem = GemArray[row, column + 1];
                             if (otherGem != null)
                             {
                                // Меняем местами камешки в массиве
                                GemArray[row, column] = otherGem;
                                GemArray[row, column + 1] = gem;

                                // Если получилась цепь - запоминаем свопы в одну и в другую сторону
                                if (GetChainAt(row, column).Count >= 3 || GetChainAt(row, column + 1).Count >= 3)
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
                            Gem otherGem = GemArray[row + 1, column];

                            if (otherGem != null)
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
            foreach (Swap swap in Swaps) { Console.Write(swap + "\n"); }
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

            // проверяем получившюся цепоку на длину. если цепочка больше трех на
            // месте премещенного камешка ставим бонус
            // записываем бонус в список на добавление спрайтов для сцены
            if (chain.Count == 4)
            {
                bool isHorizontal = chain.ToArray()[0].Row == chain.ToArray()[1].Row;
                BonusesToAddSpritesTo.Add(new Gem(true, isHorizontal, GemArray[rowB, columnB].GemType, rowB, columnB));
            }

            if (chain.Count >= 5)
            {
                BonusesToAddSpritesTo.Add(new Gem(true, GemArray[rowB, columnB].GemType, rowB, columnB));
            }

            chain = GetChainAt(rowA, columnA);

            if (chain.Count == 4)
            {
                bool isHorizontal = chain.ToArray()[0].Row == chain.ToArray()[1].Row;
                BonusesToAddSpritesTo.Add(new Gem(true, isHorizontal, GemArray[rowA, columnA].GemType, rowA, columnA));
            }

            if (chain.Count >= 5)
            {
                BonusesToAddSpritesTo.Add(new Gem(true, GemArray[rowA, columnA].GemType, rowA, columnA));
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
                    if (GemArray[row, column] != null)
                    {
                        if (withFullGemDescriptions)
                            Console.Write(GemArray[row, column] + " ");
                        else
                        {
                            if (GemArray[row, column].IsALineDestroyer)
                                Console.Write(GemArray[row, column].GemType.ToString().Substring(0, 1).ToUpper() + " ");
                            else
                                Console.Write(GemArray[row, column].GemType.ToString().Substring(0, 1) + " ");
                        }

                    } else
                    {
                        Console.Write("_ ");
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
            GemList possibleChain = new GemList();

            // Если камешка в данной позиции вообще нет - возвращаем пустой список
            if (GemArray[gemRow, gemColumn] == null)
            {
                return possibleChain;
            }

            possibleChain.Add(GemArray[gemRow, gemColumn]);

            GemType gemTypeToCheck = GemArray[gemRow, gemColumn].GemType;

            // Флаг разпыва цепочки
            bool chainIsNotBroken = true;

            //Проверка влево
            for (int column = gemColumn - 1; column >= 0 && chainIsNotBroken; column--)
            {
                if (GemArray[gemRow, column] != null && gemTypeToCheck == GemArray[gemRow, column].GemType)
                {
                    possibleChain.Add(GemArray[gemRow, column]);
                }
                else
                {
                    chainIsNotBroken = false;
                }
            }

            chainIsNotBroken = true;

            // Проверка вправо
            for (int column = gemColumn + 1; column < ColumnsNumber && chainIsNotBroken; column++)
            {
                if (GemArray[gemRow, column] != null && gemTypeToCheck == GemArray[gemRow, column].GemType)
                {
                    possibleChain.Add(GemArray[gemRow, column]);
                }
                else
                {
                    chainIsNotBroken = false;
                }
            }

            // Если в цепочке три и более элементов - цепочка возвращается
            if (possibleChain.Count >= 3)
            {
                return possibleChain;
            }

            // Сбрасываем цепочку, если не нашлось цепочки по горизонтали
            possibleChain.Clear();
            possibleChain.Add(GemArray[gemRow, gemColumn]);

            chainIsNotBroken = true;

            // Проверка вниз 
            for (int row = gemRow - 1; row >= 0 && chainIsNotBroken; row--)
            {
                if (GemArray[row, gemColumn] != null && gemTypeToCheck == GemArray[row, gemColumn].GemType)
                {
                    possibleChain.Add(GemArray[row, gemColumn]);
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
                if (GemArray[row, gemColumn] != null && gemTypeToCheck == GemArray[row, gemColumn].GemType)
                {
                    possibleChain.Add(GemArray[row, gemColumn]);
                }
                else
                {
                    chainIsNotBroken = false;
                }
            }

            return possibleChain;
        }

        /// <summary>
        /// Сканирование на наличие цепочки
        /// </summary>
        /// <returns><c>true</c>, если цепочка найдена, <c>false</c> если не найдена.</returns>
        public GemList RetriveChain()
        {
            for (int row = 0; row < RowsNumber; row++)
            {
                for (int column = 0; column < ColumnsNumber; column++)
                {
                    GemList chain = GetChainAt(row, column);
                    if (chain != null && chain.Count >= 3)
                    {
                        return chain;
                    }
                }
            }

            return null;
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
            Gem possibleBonus = null;
            GemList chain = RetriveChain();

            // Повторяем процесс пока находим цепочку на уровне
            while (chain != null)
            {
                // проверяем нет ли разрушителей в цепочке.
                possibleBonus = chain.GetBonus();

                if (possibleBonus != null)
                {
                    // Запоминаем разрушитель в списке разрушителей на анимацию
                    BonusesToAnimate.Add(possibleBonus);

                    // Очищаем цепочку и заносим в нее весь ряд илил столбец, 
                    // в зависимости от напрваленности разрушителя
                    chain.Clear();

                    if (possibleBonus.IsALineDestroyer)
                    {
                        if (possibleBonus.IsHorizontal)
                        {
                            for (int column = 0; column < ColumnsNumber; column++)
                            {
                                if (GemArray[possibleBonus.Row, column] != null)
                                {
                                    chain.Add(GemArray[possibleBonus.Row, column]);
                                }
                            }
                        }
                        else
                        {
                            for (int row = 0; row < RowsNumber; row++)
                            {
                                if (GemArray[row, possibleBonus.Column] != null)
                                {
                                    chain.Add(GemArray[row, possibleBonus.Column]);
                                }
                            }
                        }
                    }

                    if (possibleBonus.IsABomb)
                    {
                        int i;
                        int j;
                        if (possibleBonus.Row == 0)
                        {
                            i = possibleBonus.Row;
                        }
                        else
                        {
                            i = possibleBonus.Row - Properties.BombBlastRadius;
                        }
                        if (possibleBonus.Column == 0)
                        {
                            j = possibleBonus.Column;
                        }
                        else
                        {
                            j = possibleBonus.Column - Properties.BombBlastRadius;
                        }

                        for (int row = i; row <= possibleBonus.Row + Properties.BombBlastRadius && row <= RowsNumber - 1; row++)
                        {
                            for (int column = j; column <= possibleBonus.Column + Properties.BombBlastRadius && column <= ColumnsNumber - 1; column++)
                            {
                                if (GemArray[row, column] != null)
                                {
                                    chain.Add(GemArray[row, column]);
                                }
                            }
                        }
                    }

                }

                // подсчитываем очки за цепочку, добавляем их к общему счету
                int chainScore = (chain.Count - 2) * 10;
                Score += chainScore;

                // заносим цепочку в список на анимацию
                DestroyedChains.Add(chain);

                // удаляем из массива камешки в соответствии с цепью
                chain.ForEach(gem => GemArray[gem.Row, gem.Column] = null);

                // Получаем новую цепочку
                chain = RetriveChain();
            }

            //______ДЛЯ ДЕБАГА______
            PrintOutGemArrayToConsole(false);
        }

        /// <summary>
        /// Заполнение пустых ячеек в массиве модели.
        /// </summary>
        /// <returns>Список новых камешков на обогащение спрайтами</returns>
        public GemList FillBlanks ()
        {
            GemList newGems = new GemList();
            Random rnd = new Random();

            for (int row = 0; row < RowsNumber; row++)
            {
                for (int column = 0; column < ColumnsNumber; column++)
                {
                    if (GemArray[row, column] == null)
                    {
                        bool foundApropriateGemType = false;

                        // Создаем камешек, записываем его в массив, проверяем не создал ли он цепочку
                        // если создал - повторяем до те пор, пока не найдем камешек, который не создаст цепочку
                        while (!foundApropriateGemType)
                        {
                            GemType newGemType = (GemType)rnd.Next(Enum.GetNames(typeof(GemType)).Length);

                            GemArray[row, column] = new Gem(newGemType, row, column);

                            foundApropriateGemType = GetChainAt(row, column).Count < 3;
                        }

                        // Добавляем новый камешек в список камешков, которым позже добавят спрайты
                        newGems.Add(GemArray[row, column]);
                    }
                }
            }

            //______ДЛЯ ДЕБАГА______
            PrintOutGemArrayToConsole(false);

            return newGems;
        }

        /// <summary>
        /// Падение камешков на пустые места. Сканирует колонку на пустоту,
        /// найдя пустоту сканирует снова, на нличие камешка сверху. При наличии - 
        /// "роняет" его на пустое место. Упавшие амешки заносятся в колонки,
        /// Колонки заносятся в список на обновление спрайтов
        /// </summary>
        /// <returns>Список колонок камешков, которые переместились, 
        /// для обновления позиции спрайтов</returns>
        public List<GemList> DropGems()
        {
            List<GemList> columns = new List<GemList>();

            // пробегаем по столбцам
            for (int column = 0; column < ColumnsNumber; column++)
            {
                GemList columnOfFallenGems = new GemList();

                for (int row = 0; row < RowsNumber; row++)
                {
                    // Если текущая ячейка пустая - сканируем текущий столбец вверх
                    if (GemArray[row, column] == null)
                    {
                        bool foundNewGemAbove = false;

                        for (int aboveRow = row + 1; aboveRow < RowsNumber && !foundNewGemAbove; aboveRow++)
                        {
                            // находим непустую ячейку и переносим камешек оттуда в текущую ячейку
                            if (GemArray[aboveRow, column] != null)
                            {
                                GemArray[row, column] = GemArray[aboveRow, column];
                                // ячейку из которой спустили камешек зануляем
                                GemArray[aboveRow, column] = null;
                                // Обновляем координату ряда у камешка
                                GemArray[row, column].Row = row;

                                //запоминаем камешек для анимации спрайтов
                                columnOfFallenGems.Add(GemArray[row, column]);

                                foundNewGemAbove = true;
                            }
                        }

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
}
