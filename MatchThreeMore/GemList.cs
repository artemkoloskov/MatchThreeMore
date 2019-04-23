using System.Collections.Generic;
using System.Linq;
namespace MatchThreeMore
{
    /// <summary>
    /// Класс для облегчения чтения кода
    /// </summary>
    public class GemList: List <Gem>
    {
        /// <summary>
        /// Проверяет цепочку на наличие в ней бонуса, возвращает бонус.
        /// </summary>
        /// <returns>The for destroyers in list.</returns>
        public Gem GetBonus()
        {
            foreach (Gem gem in this)
            {
                if (gem.IsALineDestroyer || gem.IsABomb)
                { 
                    return gem;
                }
            }

            return null;
        }

        /// <summary>
        /// Проверяет цепочку на наличие бонусов и возвращает список бонусов
        /// </summary>
        /// <returns>Список бонусов из цепочки.</returns>
        public GemList GetAllBonuses ()
        {
            GemList bonuses = new GemList();

            foreach (Gem gem in this)
            {
                if (gem.IsALineDestroyer || gem.IsABomb)
                {
                    bonuses.Add(gem);
                }
            }

            return bonuses;
        }

        /// <summary>
        /// Проверка на наличе бонуса в списке
        /// </summary>
        /// <returns><c>true</c>, если бонус есть, <c>false</c> если 
        /// бонуса нет.</returns>
        public bool HasBonus()
        {
            return GetBonus() != null;
        }

        /// <summary>
        /// Проеверка на наличие в списке заданного камешка
        /// </summary>
        /// <returns>Содержит/не содержит</returns>
        /// <param name="gem">Заданный камешек.</param>
        public new bool Contains (Gem gem)
        {
            foreach (Gem g in this)
            {
                if (gem.Equals(g))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Расчет количество очков, которое игрок получит, если этот список
        /// камешков будет уничтожен
        /// </summary>
        /// <returns>Счет за уничтожение.</returns>
        public int GetScore()
        {
            return (Count - 2) * 10;
        }

        public Gem GetFirstGem()
        {
            return this.First();
        }

        public Gem GetLastGem()
        {
            return this.Last();
        }
    }
}
