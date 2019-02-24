using Foundation;
using System;
using UIKit;

namespace MatchThreeMore
{
    public partial class MainMenuViewController : UIViewController
    {
        public MainMenuViewController (IntPtr handle) : base (handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            UIButton startButton = new UIButton
            {
                Frame = new CoreGraphics.CGRect(View.Bounds.Size.Width / 2 - 75, View.Bounds.Size.Height - 100, 150, 50),
                BackgroundColor = UIColor.Gray
            };

            startButton.SetTitle("НАЧАТЬ ИГРУ", UIControlState.Normal);

            UIButton startInDevModeButton = new UIButton
            {
                Frame = new CoreGraphics.CGRect(View.Bounds.Size.Width / 2 - 75, View.Bounds.Size.Height - 200, 150, 50),
                BackgroundColor = UIColor.Gray
            };

            startInDevModeButton.SetTitle("DEV MODE", UIControlState.Normal);

            View.Add(startButton);
            View.Add(startInDevModeButton);


            startButton.TouchUpInside += (sender, e) => {
                GameViewController gameView = Storyboard.InstantiateViewController("GameView") as GameViewController;
                NavigationController.PushViewController(gameView, true);
            };

            startInDevModeButton.TouchUpInside += (sender, e) => {
                GameViewController gameView = Storyboard.InstantiateViewController("GameView") as GameViewController;
                gameView.DevModeIsOn = true;
                NavigationController.PushViewController(gameView, true);
            };
        }

    }
}