using AudioToolbox;

using static MatchThreeMore.AdditionalMethods;
using static MatchThreeMore.Properties;

namespace MatchThreeMore;

public partial class GameOverViewController(IntPtr handle) : UIViewController(handle)
{
    public UILabel ScoreLabel = [];

    public override void ViewDidLoad()
    {
        base.ViewDidLoad();

        AddBackground();
        AddGameOverLabel();
        AddScore();
        AddStartButton();
        AnnounceScore();
    }

    private void AnnounceScore()
    {
        NSUrl nsUrl = GetScoreAnnouncerNsUrl();

        SystemSound ss = new(nsUrl);
        ss.PlayAlertSound();
    }

    private NSUrl GetScoreAnnouncerNsUrl()
    {
        switch (int.Parse(ScoreLabel.Text))
        {
            case >= 3500:
                return NSUrl.FromFilename("greatScoreRus.wav");
            case >= 2000 and < 3500:
                return NSUrl.FromFilename("loserScoreRus.wav");
            case >= 500 and < 2000:
                return NSUrl.FromFilename("antScoreRus.wav");
            default:
                return NSUrl.FromFilename("veryLowScoreRus.wav");
        }
    }

    private void AddStartButton()
    {
        UIButton startButton = new()
        {
            Frame = new CGRect(View.Bounds.Size.Width / 2 - 75, View.Bounds.Size.Height - 100, 150, 50),
            BackgroundColor = ButtonColor
        };

        startButton.SetTitle("В МЕНЮ", UIControlState.Normal);

        View.Add(startButton);

        startButton.TouchUpInside += (sender, e) =>
        {
            UIViewController mainMenu = Storyboard.InstantiateViewController("MainMenu");
            NavigationController.PushViewController(mainMenu, true);
        };
    }

    private void AddScore()
    {
        UILabel scoreTitle = new()
        {
            Frame = new CGRect(View.Bounds.Size.Width / 2 - 75, 170, 150, 50),
            Font = CommonFont,
            TextAlignment = UITextAlignment.Center,
            TextColor = UIColor.White
        };

        scoreTitle.Text = "Ваш счёт:";
        View.Add(scoreTitle);

        ScoreLabel.Frame = new CGRect(View.Bounds.Size.Width / 2 - 75, 185, 150, 50);
        ScoreLabel.TextAlignment = UITextAlignment.Center;
        ScoreLabel.Font = UIFont.FromName("GillSans-BoldItalic", 18f);
        ScoreLabel.TextColor = UIColor.White;

        View.Add(ScoreLabel);
    }

    private void AddGameOverLabel()
    {
        UILabel gameOverLabel = new()
        {
            Frame = new CGRect(View.Bounds.Size.Width / 2 - 75, 150, 150, 50),
            Font = CommonFont,
            TextAlignment = UITextAlignment.Center,
            TextColor = UIColor.White
        };

        gameOverLabel.Text = "Игра окончена!";
        View.Add(gameOverLabel);
    }

    private void AddBackground()
    {
        UIImageView background = new(
            ResizeUIImage(
                UIImage.FromFile("background.jpg"), (float)View.Bounds.Size.Width, (float)View.Bounds.Size.Height));

        View.Add(background);
    }
}
