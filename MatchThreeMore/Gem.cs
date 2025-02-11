using SpriteKit;
using System.Text;

namespace MatchThreeMore;

/// <summary>
/// Перечисление типов камешков
/// </summary>
public enum GemType
{
    triangle,
    square,
    diamond,
    pentagon,
    hexagon
}

/// <summary>
/// Класс Камешек, содержит тип камешка, его позицию в модели и спрайт
/// </summary>
public class Gem : IEquatable<Gem>
{
    public GemType GemType { get; }
    public int Row { get; set; }
    public int Column { get; set; }
    public SKSpriteNode? Sprite { get; set; }
    public bool IsALineDestroyer { get; set; }
    public bool IsHorizontal { get; set; }
    public bool IsABomb { get; set; }

    /// <summary>
    /// Конструктор создания камешка определенного типа в определенной позиции
    /// </summary>
    /// <param name="gemType">Тип камешка</param>
    /// <param name="row">Ряд</param>
    /// <param name="column">Колонка</param>
    public Gem(int gemType, int row, int column)
    {
        GemType = (GemType)gemType;
        Row = row;
        Column = column;
    }

    /// <summary>
    /// Конструктор создания камешка определенного типа в определенной позиции
    /// </summary>
    /// <param name="gemType">Тип камешка</param>
    /// <param name="row">Ряд</param>
    /// <param name="column">Колонка</param>
    public Gem(GemType gemType, int row, int column)
    {
        GemType = gemType;
        Row = row;
        Column = column;
    }

    /// <summary>
    /// Инициализация разрушителя линии
    /// </summary>
    /// <param name="gemType">Gem type.</param>
    /// <param name="row">Row.</param>
    /// <param name="column">Column.</param>
    public Gem(bool isALineDestroyer, bool isHorizontal, GemType gemType, int row, int column)
    {
        GemType = gemType;
        IsALineDestroyer = isALineDestroyer;
        IsHorizontal = isHorizontal;
        Row = row;
        Column = column;
    }

    /// <summary>
    /// Инициализация разрушителя линии
    /// </summary>
    /// <param name="gemType">Gem type.</param>
    /// <param name="row">Row.</param>
    /// <param name="column">Column.</param>
    public Gem(bool isABomb, GemType gemType, int row, int column)
    {
        GemType = gemType;
        Row = row;
        Column = column;
        IsABomb = isABomb;
    }

    /// <summary>
    /// Имя текстуры для камешка
    /// </summary>
    /// <returns>Имя файла</returns>
    public string GetSpriteName()
    {
        if (IsALineDestroyer)
        {
            if (IsHorizontal)
            {
                return GemType + "_horizontal";
            }

            return GemType + "_vertical";
        }

        if (IsABomb)
        {
            return GemType + "_bomb";
        }

        return GemType + "";
    }

    /// <summary>
    /// Имя текстуры для выбранного камешка. Проверяет на статус бонуса,
    /// возвращает имена соответствующих текстур для бонусов.
    /// </summary>
    /// <returns>Имя файла</returns>
    public string GetSelectedSpriteName()
    {
        if (IsALineDestroyer)
        {
            if (IsHorizontal)
            {
                return GemType + "_horizontal_selected";
            }

            return GemType + "_vertical_selected";
        }

        if (IsABomb)
        {
            return GemType + "_bomb_selected";
        }

        return GemType + "_selected";
    }

    public override string ToString()
    {
        StringBuilder str = new();
        _ = str.Append(GemType + "; (" + Row + ", " + Column + ")");

        if (IsALineDestroyer)
        {
            if (IsHorizontal)
            {
                _ = str.Append("; гор. разр.");
            }
            else
            {
                _ = str.Append("; верт. разр.");
            }
        }

        if (IsABomb)
        {
            _ = str.Append("; бомба");
        }

        return str.ToString();
    }

    public bool Equals(Gem? other)
    {
        return other is not null
            && GemType == other.GemType
            && Row == other.Row
            && Column == other.Column
            && IsALineDestroyer == other.IsALineDestroyer
            && IsHorizontal == other.IsHorizontal;
    }

    public override bool Equals(object? obj)
    {
        return obj is not null && (obj is Swap objAsSwap) && Equals(objAsSwap);
    }

    public override int GetHashCode()
    {
        return (int)GemType + Row + Column;
    }
}
