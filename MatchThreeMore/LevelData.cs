using Newtonsoft.Json;

namespace MatchThreeMore;

public class LevelData
{
    public string[,] Level { get; set; } = new string[9, 9];

    public static LevelData LoadFrom (string fileName)
    {
        string data = File.ReadAllText(fileName);

        LevelData? levelData = JsonConvert.DeserializeObject<LevelData>(data)
            ?? throw new Exception("Не удалось загрузить уровень");

        return levelData;
    }

    public (GemType, bool, bool, bool) GetGemTypeFromLevelDataAt (int row, int column)
    {
        return Level[row, column] switch
        {
            "T" => (GemType.triangle, true, false, false),// треугольник, вертикальная линия
            "S" => (GemType.square, true, false, false),// квадрат, вертикальная линия
            "D" => (GemType.diamond, true, false, false),// ромб, вертикальная линия
            "P" => (GemType.pentagon, true, false, false),// пентагон, вертикальная линия
            "H" => (GemType.hexagon, true, false, false),// гексагон, вертикальная линия
            "T'" => (GemType.triangle, true, true, false),// треугольник, горизонтальная линия
            "S'" => (GemType.square, true, true, false),// квадрат, горизонтальная линия
            "D'" => (GemType.diamond, true, true, false),// ромб, горизонтальная линия
            "P'" => (GemType.pentagon, true, true, false),// пентагон, горизонтальная линия
            "H'" => (GemType.hexagon, true, true, false),// гексагон, горизонтальная линия
            "t" => (GemType.triangle, false, false, false),// треугольник
            "s" => (GemType.square, false, false, false),// квадрат
            "d" => (GemType.diamond, false, false, false),// ромб
            "p" => (GemType.pentagon, false, false, false),// пентагон
            "h" => (GemType.hexagon, false, false, false),// гексагон
            "t'" => (GemType.triangle, false, false, true),// треугольник, бомба
            "s'" => (GemType.square, false, false, true),// квадрат, бомба
            "d'" => (GemType.diamond, false, false, true),// ромб, бомба
            "p'" => (GemType.pentagon, false, false, true),// пентагон, бомба
            "h'" => (GemType.hexagon, false, false, true),// гексагон, бомба
            _ => (GemType.triangle, false, false, true),
        };
    }
}
