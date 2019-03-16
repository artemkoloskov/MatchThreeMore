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

        public (GemType, bool, bool, bool) GetGemTypeFromLevelDataAt (int row, int column)
        {
            switch (level[row, column])
            {
                case "T":
                    return (GemType.triangle, true, false, false);
                case "S":
                    return (GemType.square, true, false, false);
                case "D":
                    return (GemType.diamond, true, false, false);
                case "P":
                    return (GemType.pentagon, true, false, false);
                case "H":
                    return (GemType.hexagon, true, false, false);
                case "T'":
                    return (GemType.triangle, true, true, false);
                case "S'":
                    return (GemType.square, true, true, false);
                case "D'":
                    return (GemType.diamond, true, true, false);
                case "P'":
                    return (GemType.pentagon, true, true, false);
                case "H'":
                    return (GemType.hexagon, true, true, false);
                case "t":
                    return (GemType.triangle, false, false, false);
                case "s":
                    return (GemType.square, false, false, false);
                case "d":
                    return (GemType.diamond, false, false, false);
                case "p":
                    return (GemType.pentagon, false, false, false);
                case "h":
                    return (GemType.hexagon, false, false, false);
                case "t'":
                    return (GemType.triangle, false, false, true);
                case "s'":
                    return (GemType.square, false, false, true);
                case "d'":
                    return (GemType.diamond, false, false, true);
                case "p'":
                    return (GemType.pentagon, false, false, true);
                case "h'":
                    return (GemType.hexagon, false, false, true);
                default:
                    return (GemType.triangle, false, false, true);
            }


        }
    }
}
