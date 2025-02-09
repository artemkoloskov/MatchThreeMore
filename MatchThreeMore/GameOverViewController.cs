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
        if (ScoreLabel.Text is null)
        {
            return NSUrl.FromFilename("veryLowScoreRus.wav");
        }

        return int.Parse(ScoreLabel.Text) switch
        {
            >= 3500 => NSUrl.FromFilename("greatScoreRus.wav"),
            >= 2000 and < 3500 => NSUrl.FromFilename("loserScoreRus.wav"),
            >= 500 and < 2000 => NSUrl.FromFilename("antScoreRus.wav"),
            _ => NSUrl.FromFilename("veryLowScoreRus.wav"),
        };
    }

    private void AddStartButton()
    {
        ArgumentNullException.ThrowIfNull(View);
        ArgumentNullException.ThrowIfNull(Storyboard);
        ArgumentNullException.ThrowIfNull(NavigationController);

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
        ArgumentNullException.ThrowIfNull(View);

        UILabel scoreTitle = new()
        {
            Frame = new CGRect(View.Bounds.Size.Width / 2 - 75, 170, 150, 50),
            Font = CommonFont,
            TextAlignment = UITextAlignment.Center,
            TextColor = UIColor.White,
            Text = "Ваш счёт:"
        };

        View.Add(scoreTitle);

        ScoreLabel.Frame = new CGRect(View.Bounds.Size.Width / 2 - 75, 185, 150, 50);
        ScoreLabel.TextAlignment = UITextAlignment.Center;
        ScoreLabel.Font = UIFont.FromName("GillSans-BoldItalic", 18f);
        ScoreLabel.TextColor = UIColor.White;

        View.Add(ScoreLabel);
    }

    private void AddGameOverLabel()
    {
        ArgumentNullException.ThrowIfNull(View);

        UILabel gameOverLabel = new()
        {
            Frame = new CGRect(View.Bounds.Size.Width / 2 - 75, 150, 150, 50),
            Font = CommonFont,
            TextAlignment = UITextAlignment.Center,
            TextColor = UIColor.White,
            Text = "Игра окончена!"
        };

        View.Add(gameOverLabel);
    }

    private void AddBackground()
    {
        var sourceImage = UIImage.FromFile("background.jpg");

        ArgumentNullException.ThrowIfNull(sourceImage);
        ArgumentNullException.ThrowIfNull(View);

        UIImageView background = new(
            ResizeUIImage(
                sourceImage,
                (float)View.Bounds.Size.Width,
                (float)View.Bounds.Size.Height));

        View.Add(background);
    }
}
