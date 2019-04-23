using CoreGraphics;
using Foundation;
using SpriteKit;
using System;
using System.IO;
using UIKit;

using static MatchThreeMore.AdditionalMethods;
using static MatchThreeMore.Properties;

namespace MatchThreeMore
{
    public partial class MainMenuViewController : UIViewController
    {
        private string highScoresFileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "High_scores.txt");
        public static bool IsDebugRelease =>
            #if DEBUG
                true;
            #else
                false;
            #endif

        public MainMenuViewController (IntPtr handle) : base (handle)
        {
        }



        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            // Устанавливаем бэкграунд из текстуры
            UIImageView background = new UIImageView(
                ResizeUIImage(
                    UIImage.FromFile("background.jpg"), (float)View.Bounds.Size.Width, (float)View.Bounds.Size.Height));

            View.Add(background);

            UIButton startButton = new UIButton
            {
                Frame = new CGRect(View.Bounds.Size.Width / 2 - 75, View.Bounds.Size.Height - 100, 150, 50),
                Font = CommonFont,
                BackgroundColor = ButtonColor
            };

            startButton.SetTitle("НАЧАТЬ ИГРУ", UIControlState.Normal);

            // лэйбл с лучшим счетом
            UILabel highScoreLabel = new UILabel
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
                Text = "Лучший счёт: " + GetHighScore()
            };

            View.Add(startButton);

            View.Add(highScoreLabel);


            startButton.TouchUpInside += (sender, e) => {
                GameViewController gameView = Storyboard.InstantiateViewController("GameView") as GameViewController;
                NavigationController.PushViewController(gameView, true);
            };
            if (IsDebugRelease)
            {
                UIButton startInDevModeButton = new UIButton
                {
                    Frame = new CGRect(View.Bounds.Size.Width / 2 - 75, View.Bounds.Size.Height - 175, 150, 50),
                    Font = CommonFont,
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