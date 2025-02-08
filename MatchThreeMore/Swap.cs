namespace MatchThreeMore;

/// <summary>
/// Вспомогательный объект, для обработки обмена местами камешков
/// </summary>
public class Swap(Gem gemA, Gem gemB) : IEquatable<Swap>
{
    public Gem GemA
    {
        get;
    } = gemA;
    public Gem GemB
    {
        get;
    } = gemB;

    public bool Equals(Swap other)
    {
        return GemA.Equals(other.GemA) && GemB.Equals(other.GemB);
    }

    public override bool Equals(object obj)
    {
        return obj != null && (obj is Swap objAsSwap) && Equals(objAsSwap);
    }

    public override string ToString()
    {
        return "Смена А: " + GemA + " на В: " + GemB + "\n";
    }

    public override int GetHashCode() => GemA.GetHashCode() + GemB.GetHashCode();
}
