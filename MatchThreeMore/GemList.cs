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
        public Gem GetDestroyer()
        {
            foreach (Gem gem in this)
            {
                if (gem.IsALineDestroyer)
                { 
                    return gem;
                }
            }

            return null;
        }

        public bool HasDestroyer()
        {
            return GetDestroyer() != null;
        }
    }
}
