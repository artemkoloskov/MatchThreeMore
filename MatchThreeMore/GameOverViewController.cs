using Foundation;
using System;
using UIKit;

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

            UILabel gameOverLabel = new UILabel
            {
                Frame = new CoreGraphics.CGRect(View.Bounds.Size.Width / 2 - 75, 150, 150, 50),
                TextAlignment = UITextAlignment.Center
            };
            gameOverLabel.Text = "Игра окончена!";
            View.Add(gameOverLabel);

            UILabel scoreTitle = new UILabel
            {
                Frame = new CoreGraphics.CGRect(View.Bounds.Size.Width / 2 - 75, 170, 150, 50),
                TextAlignment = UITextAlignment.Center
            };
            scoreTitle.Text = "Ваш счёт:";
            View.Add(scoreTitle);

            score.Frame = new CoreGraphics.CGRect(View.Bounds.Size.Width / 2 - 75, 185, 150, 50);
            score.TextAlignment = UITextAlignment.Center;
            View.Add(score);

            UIButton startButton = new UIButton
            {
                Frame = new CoreGraphics.CGRect(View.Bounds.Size.Width / 2 - 75, View.Bounds.Size.Height - 100, 150, 50),
                BackgroundColor = UIColor.Gray
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