using Foundation;
using System;
using System.IO;
using UIKit;

namespace MatchThreeMore
{
    public partial class MainMenuViewController : UIViewController
    {
        private string highScoresFileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "High_scores.txt");

        public MainMenuViewController (IntPtr handle) : base (handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            UIButton startButton = new UIButton
            {
                Frame = new CoreGraphics.CGRect(View.Bounds.Size.Width / 2 - 75, View.Bounds.Size.Height - 100, 150, 50),
                Font = UIFont.FromName("Segoe UI", 18f),
                BackgroundColor = UIColor.Gray
            };

            startButton.SetTitle("НАЧАТЬ ИГРУ", UIControlState.Normal);

            UIButton startInDevModeButton = new UIButton
            {
                Frame = new CoreGraphics.CGRect(View.Bounds.Size.Width / 2 - 75, View.Bounds.Size.Height - 200, 150, 50),
                Font = UIFont.FromName("Segoe UI", 18f),
                BackgroundColor = UIColor.Gray
            };

            startInDevModeButton.SetTitle("DEV MODE", UIControlState.Normal);

            // лэйбл с лучшим счетом
            UILabel highScoreLabel = new UILabel
            {
                Frame = new CoreGraphics.CGRect
                (
                    View.Bounds.Size.Width / 2 - Properties.HighScoreLabelWidth / 2,
                    Properties.HighScoreLabelY + 100,
                    Properties.HighScoreLabelWidth,
                    Properties.CommonLabelHeight
                ),
                TextAlignment = UITextAlignment.Center,
                Font = UIFont.FromName("Segoe UI", 18f),
                Text = "Лучший счёт: " + GetHighScore()
            };

            View.Add(startButton);
            View.Add(startInDevModeButton);
            View.Add(highScoreLabel);


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

            // парсим счет из строки загруженрной из файла
            string[] line = highScoreText.Split(',');

            if (line.GetLength(0) != 1)
            {
                return line[1];
            }

            // лучший счет 0 если нет лучшего счета
            return "0";
        }

    }
}