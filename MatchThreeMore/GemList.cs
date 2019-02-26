using System.Collections.Generic;
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

        public bool HasBonus()
        {
            return GetBonus() != null;
        }
    }
}
