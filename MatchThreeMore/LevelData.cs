using System.IO;

namespace MatchThreeMore
{
    public class LevelData
    {
        public string[,] level;
        
        public static LevelData LoadFrom (string fileName)
        {
            string data = File.ReadAllText(fileName);

            LevelData levelData = Newtonsoft.Json.JsonConvert.DeserializeObject<LevelData>(data);

            return levelData;
        }

        public (GemType, bool, bool) GetGemTypeFromLevelDataAt (int row, int column)
        {
            switch (level[row, column])
            {
                case "T":
                    return (GemType.triangle, true, false);
                case "S":
                    return (GemType.square, true, false);
                case "D":
                    return (GemType.diamond, true, false);
                case "P":
                    return (GemType.pentagon, true, false);
                case "H":
                    return (GemType.hexagon, true, false);
                case "T'":
                    return (GemType.triangle, true, true);
                case "S'":
                    return (GemType.square, true, true);
                case "D'":
                    return (GemType.diamond, true, true);
                case "P'":
                    return (GemType.pentagon, true, true);
                case "H'":
                    return (GemType.hexagon, true, true);
                case "t":
                    return (GemType.triangle, false, false);
                case "s":
                    return (GemType.square, false, false);
                case "d":
                    return (GemType.diamond, false, false);
                case "p":
                    return (GemType.pentagon, false, false);
                case "h":
                    return (GemType.hexagon, false, false);
                default:
                    return (GemType.triangle, false, false);
            }


        }
    }
}
