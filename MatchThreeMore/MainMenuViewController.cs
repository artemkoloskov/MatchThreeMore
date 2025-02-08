using static MatchThreeMore.AdditionalMethods;
using static MatchThreeMore.Properties;

namespace MatchThreeMore;

public partial class MainMenuViewController(IntPtr handle) : UIViewController(handle)
{
    private readonly string highScoresFileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "High_scores.txt");
    public static bool IsDebugRelease =>
        #if DEBUG
            true;

#else
#endif



    public override void ViewDidLoad()
    {
        base.ViewDidLoad();

        // Устанавливаем бэкграунд из текстуры
        UIImageView background = new(
            ResizeUIImage(
                UIImage.FromFile("background.jpg"), (float)View.Bounds.Size.Width, (float)View.Bounds.Size.Height));

        View.Add(background);

        UIButton startButton = new()
        {
            Frame = new CGRect(View.Bounds.Size.Width / 2 - 75, View.Bounds.Size.Height - 100, 150, 50),
            BackgroundColor = ButtonColor
        };

        startButton.SetTitle("НАЧАТЬ ИГРУ", UIControlState.Normal);

        // лэйбл с лучшим счетом
        UILabel highScoreLabel = new()
        {
            Frame = new CGRect
            (
                View.Bounds.Size.Width / 2 - HighScoreLabelWidth / 2,
                HighScoreLabelY + 100,
                HighScoreLabelWidth,
                CommonLabelHeight
            ),
            TextAlignment = UITextAlignment.Center,
            Font = CommonFont,
            Text = "Лучший счёт: " + GetHighScore(),
            TextColor = UIColor.White
        };

        View.Add(startButton);

        View.Add(highScoreLabel);


        startButton.TouchUpInside += (sender, e) => {
            GameViewController gameView = Storyboard.InstantiateViewController("GameView") as GameViewController;
            NavigationController.PushViewController(gameView, true);
        };
        if (IsDebugRelease)
        {
            UIButton startInDevModeButton = new()
            {
                Frame = new CGRect(View.Bounds.Size.Width / 2 - 75, View.Bounds.Size.Height - 175, 150, 50),
                
                BackgroundColor = ButtonColor
            };

            startInDevModeButton.SetTitle("DEV MODE", UIControlState.Normal);

            View.Add(startInDevModeButton);

            startInDevModeButton.TouchUpInside += (sender, e) =>
            {
                GameViewController gameView = Storyboard.InstantiateViewController("GameView") as GameViewController;
                gameView.DevModeIsOn = true;
                NavigationController.PushViewController(gameView, true);
            };
        }
    }

    /// <summary>
    /// Загружает из файла предыдущий лучший счет
    /// </summary>
    private string GetHighScore()
    {
        string highScoreText = "";

        // загрузка из файла
        try
        {
            highScoreText = File.ReadAllText(highScoresFileName);
        }
        catch (Exception e)
        {
            Console.Write("File " + highScoresFileName + " not found. \n" + e);
        }

        // парсим счет из строки загруженной из файла
        string[] line = highScoreText.Split(',');

        if (line.GetLength(0) != 1)
        {
            return line[1];
        }

        // лучший счет 0 если нет лучшего счета
        return "0";
    }

}
