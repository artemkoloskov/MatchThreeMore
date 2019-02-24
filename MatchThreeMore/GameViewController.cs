using System;
using System.Threading.Tasks;
using System.Text;

using Foundation;
using SpriteKit;
using UIKit;
using System.IO;

namespace MatchThreeMore
{
    public partial class GameViewController : UIViewController
    {
        public bool DevModeIsOn { get; set; }

        private Level level;
        private GameScene scene;

        private NSTimer gameTimer;
        private int currentTime = Properties.LevelTime;

        private int highScore;
        private string highScoresFileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "High_scores.txt");

        private bool chainsHadDestroyer;

        private UILabel timerLabel;
        private UILabel scoreLabel;
        private UILabel highScoreLabel;

        protected GameViewController(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            // Configure the view.
            SKView skView = (SKView)View;
            skView.ShowsFPS = true;
            skView.ShowsNodeCount = true;

            /* Sprite Kit applies additional optimizations to improve rendering performance */
            skView.IgnoresSiblingOrder = true;

            // Create and configure the scene.
            scene = SKNode.FromFile<GameScene>("GameScene");
            scene.ScaleMode = SKSceneScaleMode.AspectFill;

            // Передаем сцене данные о размере вью
            scene.SetSize(skView.Bounds.Size);

            // Создаем игровой уровень, передаем его сцене
            level = new Level(DevModeIsOn);

            scene.Level = level;

            // инциализируем делегат обработки обмена местами камешков
            scene.SwipeHandler = HandleSwipeAsync;

            // Кнопка "В меню"
            UIButton StopButton = new UIButton
            {
                Frame = new CoreGraphics.CGRect(skView.Bounds.Size.Width/2 - 75, skView.Bounds.Size.Height - 100, 150, 50),
                BackgroundColor = UIColor.Gray
            };

            StopButton.SetTitle("В МЕНЮ", UIControlState.Normal);

            StopButton.TouchUpInside += (sender, e) => {
                gameTimer.Invalidate();
                UIViewController mainMenu = Storyboard.InstantiateViewController("MainMenu");
                NavigationController.PushViewController(mainMenu, true);
            };

            // получаем лучший счет
            highScore = GetSetHighScore();

            // лэйбл с лучшим счетом
            highScoreLabel = new UILabel
            {
                Frame = new CoreGraphics.CGRect(skView.Bounds.Size.Width / 2 - 75, 50, 150, 50),
                TextAlignment = UITextAlignment.Center,
                Text = "Лучший счет: " + highScore
            };

            // лэйбл с таймером
            timerLabel = new UILabel
            {
                Frame = new CoreGraphics.CGRect(skView.Bounds.Size.Width / 2 - 75, 75, 150, 50),
                TextAlignment = UITextAlignment.Center
            };

            // лэйбл с надписью Счет
            UILabel scoreTitle = new UILabel
            {
                Frame = new CoreGraphics.CGRect(skView.Bounds.Size.Width / 2 - 75, 90, 150, 50),
                TextAlignment = UITextAlignment.Center,
                Text = "Счёт:"
            };

            // лэйбл со счетом
            scoreLabel = new UILabel
            {
                Frame = new CoreGraphics.CGRect(skView.Bounds.Size.Width / 2 - 75, 105, 150, 50),
                TextAlignment = UITextAlignment.Center,
                Text = "0"
            };

            // добавляем элементы интерфейса на вью
            skView.Add(StopButton);
            skView.Add(highScoreLabel);
            skView.Add(timerLabel);
            skView.Add(scoreTitle);
            skView.Add(scoreLabel);

            // Present the scene.
            skView.PresentScene(scene);

            // Начало игры
            BeginGame();
        }

        /// <summary>
        /// Загружает из файла предыдущий лучший счет
        /// </summary>
        private int GetSetHighScore()
        {
            // счет для режима разработчика
            if (DevModeIsOn)
            {
                return 300;
            }

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
                return Convert.ToInt32(line[1]);
            }

            // лучший счет 0 если нет лучшего счета
            return 0;
        }

        public override bool ShouldAutorotate()
        {
            return true;
        }

        public override UIInterfaceOrientationMask GetSupportedInterfaceOrientations()
        {
            return UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone ? UIInterfaceOrientationMask.AllButUpsideDown : UIInterfaceOrientationMask.All;
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }

        public override bool PrefersStatusBarHidden()
        {
            return false;
        }

        //++++++++++++ДОПОЛНИТЕЛЬНЫЕ МЕТОДЫ+++++++++++++

        /// <summary>
        /// Начать игру, перемешав камешки на уровне и запустив таймер обратного
        /// отсчета
        /// </summary>
        private void BeginGame()
        {
            // перемешиваем камешки в модели и заполняем спрайтами сцену
            ShuffleGems();

            // запуск таймера
            gameTimer = NSTimer.CreateRepeatingScheduledTimer(TimeSpan.FromSeconds(1.0), delegate
            {
                // когда таймер дойдет до 0
                if (TimerAction() == 0)
                {
                    // деактивировать таймер
                    gameTimer.Invalidate();

                    // Записать в файл новый лучший счет
                    int newHighScore = Math.Max(highScore, Convert.ToInt32(scoreLabel.Text));
                    File.WriteAllText(highScoresFileName, "score, " + newHighScore);

                    // перейти к экрану конца игры
                    GameOverViewController gameOver = Storyboard.InstantiateViewController("GameOver") as GameOverViewController;

                    // Передаем счет игры экрану конца игры
                    gameOver.score.Text = scoreLabel.Text;
                    NavigationController.PushViewController(gameOver, true);
                }
            });
        }

        /// <summary>
        /// Поведение таймера.
        /// </summary>
        /// <returns>Текущее время таймера.</returns>
        private int TimerAction ()
        {
            currentTime--;

            int minutes = Math.Abs(currentTime / 60);
            int seconds = currentTime % 60;

            StringBuilder timerText = new StringBuilder();

            if (minutes < 10)
            {
                timerText.Append("0" + minutes);
            }
            else
            {
                timerText.Append(minutes);
            }

            timerText.Append(":");

            if (seconds < 10)
            {
                timerText.Append("0" + seconds);
            }
            else
            {
                timerText.Append(seconds);
            }

            timerLabel.Text = timerText.ToString();

            return currentTime;
        }

        /// <summary>
        /// Перемешать камешки, добавив спрайты камешков на сцену
        /// </summary>
        private void ShuffleGems()
        {
            level.Shuffle();
            scene.AttachSpritesTo(level.GemArray);
        }

        /// <summary>
        /// Обработчик обмена местами камешков, отключает интеракции с представлением,
        /// проводит обмен на уровне сцены и в массиве камешков, массив проверяет обмен
        /// на валидность, если обмен валидный включает интерактивность обратно,
        /// если обмен не валидный - возвращает массив в исходное состояние, проигрывает
        /// анимацию возвращения камешков на исходные позиции
        /// </summary>
        /// <param name="swap">Объект с камешками для обмена.</param>
        public async void HandleSwipeAsync(Swap swap)
        {
            // отключаем интерактивность
            View.UserInteractionEnabled = false;

            bool swapIsValid = level.Swaps.Contains(swap);

            if (swapIsValid)
            {
                // проводим обмен в модели
                level.Perform(swap);

                // анимируем обмен на сцене
                scene.Animate(swap, swapIsValid);
                await Task.Delay(Properties.SwapAnimationDuration);

                // обрабатываем полученные цепочки
                await HandleChains();

                // задержка, нужная для всех анимаций перед тем,как включать интерактивность
                int delay = Properties.DestructionAnimationDuration + Properties.FallAnimationDuration;

                // если был разрушитель - увеличиваем время задержки
                if (chainsHadDestroyer)
                {
                    delay += Properties.LineDestructionDuration;
                    chainsHadDestroyer = false;
                }

                await Task.Delay(delay);

                View.UserInteractionEnabled = true;
            } 
            else
            {
                scene.Animate(swap, swapIsValid);

                await Task.Delay(Properties.SwapAnimationDuration * 2);

                View.UserInteractionEnabled = true;
            }
        }

        /// <summary>
        /// Поиск и уничтожение цепей, проигрывание анимации уничтожения,
        /// обработка падения камешков на уровне модели, проигрывание анимации 
        /// падения, заполнение модели и сцены новыми камешками
        /// </summary>
        public async Task HandleChains ()
        {
            // Сканирование массива до тех пор, пока в модели остаются цепочки, после уничтожения
            // и спуска камешков
            while (level.RetriveChain() != null)
            {
                // уничтожаем цепочки
                level.DestroyChains();

                // если при уничтожении были найдены бонусы - анимируем сцену особым образом
                if (level.BonusesToAnimate.Count > 0)
                {
                    // Цикл проходит по всем бонусам, анимирует их и анимирует удаление найденных цепочек
                    foreach (Gem gem in level.BonusesToAnimate)
                    {
                        if (gem.IsALineDestroyer)
                        {
                            chainsHadDestroyer = true;

                            scene.AnimateLineDestroyer(gem);

                            scene.AnimateTheDstructionOf(level.DestroyedChains);

                            level.DestroyedChains.Clear();

                            await Task.Delay(Properties.LineDestructionDuration);
                        }
                    }

                    // очищаем список бнусами на анимацию
                    level.BonusesToAnimate.Clear();
                }
                else
                {
                    // бонусов нет, обычное удаление цепочек
                    scene.AnimateTheDstructionOf(level.DestroyedChains);

                    level.DestroyedChains.Clear();

                    await Task.Delay(Properties.DestructionAnimationDuration);
                }

                // анимируем падение камешков
                scene.AnimateFallingGemsIn(level.DropGems());
                await Task.Delay(Properties.FallAnimationDuration);

            }

            // обновлем лэйблы с счетом
            scoreLabel.Text = level.Score.ToString();
            highScoreLabel.Text = "Лучший счет: " + Math.Max(level.Score, highScore) + "";

            // вызываем метод заполнения пустот в модели, создаем для них спрайты
            scene.AttachSpritesTo(level.FillBlanks());

            // создаем спрайты для новых бонусов
            scene.AttachSpritesTo(level.BonusesToAddSpritesTo);

            // очищаем списки обработанных бонусов
            level.BonusesToAddSpritesTo.Clear();

            // загружаем список доступных обменов
            level.DetectPossibleSwaps();
        }
    }
}
