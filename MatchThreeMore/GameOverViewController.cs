using Foundation;
using System;
using UIKit;

using static MatchThreeMore.AdditionalMethods;
using static MatchThreeMore.Properties;

namespace MatchThreeMore
{
    public partial class GameOverViewController : UIViewController
    {
        public GameOverViewController (IntPtr handle) : base (handle)
        {
        }

        public UILabel score = new UILabel();

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            // Устанавливаем бэкграунд из текстуры
            UIImageView background = new UIImageView(
                ResizeUIImage(
                    UIImage.FromFile("background.jpg"), (float)View.Bounds.Size.Width, (float)View.Bounds.Size.Height));

            View.Add(background);

            UILabel gameOverLabel = new UILabel
            {
                Frame = new CoreGraphics.CGRect(View.Bounds.Size.Width / 2 - 75, 150, 150, 50),
                Font = CommonFont,
                TextAlignment = UITextAlignment.Center
            };
            gameOverLabel.Text = "Игра окончена!";
            View.Add(gameOverLabel);

            UILabel scoreTitle = new UILabel
            {
                Frame = new CoreGraphics.CGRect(View.Bounds.Size.Width / 2 - 75, 170, 150, 50),
                Font = CommonFont,
                TextAlignment = UITextAlignment.Center
            };
            scoreTitle.Text = "Ваш счёт:";
            View.Add(scoreTitle);

            score.Frame = new CoreGraphics.CGRect(View.Bounds.Size.Width / 2 - 75, 185, 150, 50);
            score.TextAlignment = UITextAlignment.Center;
            score.Font = UIFont.FromName("GillSans-BoldItalic", 18f);
            View.Add(score);

            UIButton startButton = new UIButton
            {
                Frame = new CoreGraphics.CGRect(View.Bounds.Size.Width / 2 - 75, View.Bounds.Size.Height - 100, 150, 50),
                Font = CommonFont,
                BackgroundColor = ButtonColor
            };

            startButton.SetTitle("В МЕНЮ", UIControlState.Normal);

            View.Add(startButton);


            startButton.TouchUpInside += (sender, e) => {
                UIViewController mainMenu = Storyboard.InstantiateViewController("MainMenu");
                NavigationController.PushViewController(mainMenu, true);
            };
        }
    }
}