using System.IO;

namespace MatchThreeMore;

public class LevelData
{
    public string[,] level;
    
    public static LevelData LoadFrom (string fileName)
    {
        string data = File.ReadAllText(fileName);

        LevelData levelData = Newtonsoft.Json.JsonConvert.DeserializeObject<LevelData>(data);

        return levelData;
    }

    public (GemType, bool, bool, bool) GetGemTypeFromLevelDataAt (int row, int column)
    {
        switch (level[row, column])
        {
            case "T":
                // треугольник, вертикальная линия
                return (GemType.triangle, true, false, false); 
            case "S":
                // квадрат, вертикальная линия
                return (GemType.square, true, false, false);
            case "D":
                // ромб, вертикальная линия
                return (GemType.diamond, true, false, false);
            case "P":
                // пентагон, вертикальная линия
                return (GemType.pentagon, true, false, false);
            case "H":
                // гексагон, вертикальная линия
                return (GemType.hexagon, true, false, false);
            case "T'":
                // треугольник, горизонтальная линия
                return (GemType.triangle, true, true, false);
            case "S'":
                // квадрат, горизонтальная линия
                return (GemType.square, true, true, false);
            case "D'":
                // ромб, горизонтальная линия
                return (GemType.diamond, true, true, false);
            case "P'":
                // пентагон, горизонтальная линия
                return (GemType.pentagon, true, true, false);
            case "H'":
                // гексагон, горизонтальная линия
                return (GemType.hexagon, true, true, false);
            case "t":
                // треугольник
                return (GemType.triangle, false, false, false);
            case "s":
                // квадрат
                return (GemType.square, false, false, false);
            case "d":
                // ромб
                return (GemType.diamond, false, false, false);
            case "p":
                // пентагон
                return (GemType.pentagon, false, false, false);
            case "h":
                // гексагон
                return (GemType.hexagon, false, false, false);
            case "t'":
                // треугольник, бомба
                return (GemType.triangle, false, false, true);
            case "s'":
                // квадрат, бомба
                return (GemType.square, false, false, true);
            case "d'":
                // ромб, бомба
                return (GemType.diamond, false, false, true);
            case "p'":
                // пентагон, бомба
                return (GemType.pentagon, false, false, true);
            case "h'":
                // гексагон, бомба
                return (GemType.hexagon, false, false, true);
            default:
                return (GemType.triangle, false, false, true);
        }


    }
}
