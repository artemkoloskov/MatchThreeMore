using System.Text;
using SpriteKit;

using static MatchThreeMore.Properties;

namespace MatchThreeMore;

public partial class GameViewController : UIViewController
{
    public bool DevModeIsOn { get; set; }

    private Level _level = default!;
    private GameScene _scene = default!;

    private NSTimer? _gameTimer;
    private int _currentTime = LEVEL_TIME;

    private int _highScore;
    private readonly string _highScoresFileName =
        Path.Combine(Environment.GetFolderPath(
            Environment.SpecialFolder.MyDocuments),
            "High_scores.txt");

    private bool _chainsHadBonuses;

    private UILabel _timerLabel = default!;
    private UILabel _scoreLabel = default!;
    private UILabel _highScoreLabel = default!;
    private UILabel _pauseLabel = default!;

    protected GameViewController(IntPtr handle) : base(handle)
    {
        // Note: this .ctor should not contain any initialization logic.
    }

    public override void ViewDidLoad()
    {
        ArgumentNullException.ThrowIfNull(View);
        ArgumentNullException.ThrowIfNull(Storyboard);
        ArgumentNullException.ThrowIfNull(NavigationController);

        base.ViewDidLoad();

        if (DevModeIsOn)
        {
            _currentTime = DEV_LEVEL_TIME;
        }

        // Configure the view.
        SKView skView = (SKView)View
            ?? throw new Exception("View is not SKView");
        skView.ShowsFPS = DevModeIsOn;
        skView.ShowsNodeCount = DevModeIsOn;

        /* Sprite Kit applies additional optimizations to improve rendering performance */
        skView.IgnoresSiblingOrder = true;

        // Create and configure the scene.
        _scene = SKNode.FromFile<GameScene>("GameScene")
            ?? throw new Exception("GameScene not found");
        _scene.ScaleMode = SKSceneScaleMode.AspectFill;
sssx
        // Передаем сцене данные о размере вью
        _scene.SetSize(skView.Bounds.Size);

        // Создаем игровой уровень, передаем его сцене
        _level = new Level(DevModeIsOn);

        _scene.Level = _level;

        // инициализируем делегат обработки обмена местами камешков
        _scene.SwipeHandler = HandleSwipeAsync;

        // Кнопка "В меню"
        UIButton stopButton = new()
        {
            Frame = new CGRect(30, View.Bounds.Size.Height - 100, 120, 45),
            BackgroundColor = ButtonColor
        };≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈≈

        stopButton.SetTitle("В МЕНЮ", UIControlState.Normal);

        stopButton.TouchUpInside += (sender, e) =>
        {
            _gameTimer?.Invalidate();
            UIViewController mainMenu = Storyboard.InstantiateViewController("MainMenu");
            NavigationController.PushViewController(mainMenu, true);
        };

        // получаем лучший счет
        _highScore = GetHighScore();

        // лэйбл с лучшим счетом
        _highScoreLabel = new UILabel
        {
            Frame = new CGRect
            (
                skView.Bounds.Size.Width / 2 - HIGH_SCORE_LABEL_WIDTH / 2,
                HIGH_SCORE_LABEL_Y,
                HIGH_SCORE_LABEL_WIDTH,
                COMMON_LABEL_HEIGHT
            ),
            TextAlignment = UITextAlignment.Center,
            Font = CommonFont,
            Text = "Лучший счёт: " + _highScore,
            TextColor = UIColor.White
        };

        // лэйбл с таймером
        _timerLabel = new UILabel
        {
            Frame = new CGRect
            (
                skView.Bounds.Size.Width / 2 - COMMON_LABEL_WIDTH / 2,
                TIMER_LABEL_Y,
                COMMON_LABEL_WIDTH,
                COMMON_LABEL_HEIGHT
            ),
            Font = CommonFont,
            TextAlignment = UITextAlignment.Center,
            TextColor = UIColor.White
        };

        // лэйбл с надписью Счет
        UILabel scoreTitle = new()
        {
            Frame = new CGRect
            (
                skView.Bounds.Size.Width / 2 - COMMON_LABEL_WIDTH / 2,
                SCORE_TITLE_LABEL_Y,
                COMMON_LABEL_WIDTH,
                COMMON_LABEL_HEIGHT
            ),
            TextAlignment = UITextAlignment.Center,
            Font = CommonFont,
            Text = "Счёт:",
            TextColor = UIColor.White
        };

        // лэйбл со счетом
        _scoreLabel = new UILabel
        {
            Frame = new CGRect
            (
                skView.Bounds.Size.Width / 2 - COMMON_LABEL_WIDTH / 2,
                SCORE_LABEL_Y,
                COMMON_LABEL_WIDTH,
                COMMON_LABEL_HEIGHT
            ),
            TextAlignment = UITextAlignment.Center,
            Font = CommonFont,
            Text = "0",
            TextColor = UIColor.White
        };

        // лэйбл с надписью Пауза
        _pauseLabel = new UILabel
        {
            Hidden = true,
            Frame = new CGRect
            (
                skView.Bounds.Size.Width / 2 - COMMON_LABEL_WIDTH / 2,
                skView.Bounds.Size.Height / 2 - COMMON_LABEL_HEIGHT / 2,
                COMMON_LABEL_WIDTH,
                COMMON_LABEL_HEIGHT
            ),
            TextAlignment = UITextAlignment.Center,
            Font = CommonFont,
            Text = "ПАУЗА",
            TextColor = UIColor.White
        };

        // Кнопка паузы
        UIButton pauseButton = new()
        {
            Frame = new CGRect(skView.Bounds.Size.Width - 150, View.Bounds.Size.Height - 100, 120, 45),
            BackgroundColor = ButtonColor
        };

        pauseButton.SetTitle("||", UIControlState.Normal);

        pauseButton.TouchUpInside += (sender, e) =>
        {
            _pauseLabel.Hidden = _scene.GameIsPaused;
            _scene.GameIsPaused = !_scene.GameIsPaused;
            _scene.SwitchBackgroundZPosition();
        };

        // добавляем элементы интерфейса на вью
        skView.Add(stopButton);
        skView.Add(pauseButton);
        skView.Add(_highScoreLabel);
        skView.Add(_timerLabel);
        skView.Add(scoreTitle);
        skView.Add(_scoreLabel);
        skView.Add(_pauseLabel);

        // Present the scene.
        skView.PresentScene(_scene);

        // Начало игры
        BeginGame();
    }

    /// <summary>
    /// Загружает из файла предыдущий лучший счет
    /// </summary>
    private int GetHighScore()
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
            highScoreText = File.ReadAllText(_highScoresFileName);
        }
        catch (Exception e)
        {
            Console.Write("File " + _highScoresFileName + " not found. \n" + e);
        }

        // парсим счет из строки загруженной из файла
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
        return UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone
            ? UIInterfaceOrientationMask.AllButUpsideDown
            : UIInterfaceOrientationMask.All;
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

    /// <summary>
    /// Начать игру, перемешав камешки на уровне и запустив таймер обратного
    /// отсчета
    /// </summary>
    private void BeginGame()
    {
        ArgumentNullException.ThrowIfNull(Storyboard);
        ArgumentNullException.ThrowIfNull(NavigationController);

        // перемешиваем камешки в модели и заполняем спрайтами сцену
        ShuffleGems();

        // запуск таймера
        _gameTimer = NSTimer.CreateRepeatingScheduledTimer(TimeSpan.FromSeconds(1.0), delegate
        {
            if (TimerAction() != 0)
            {
                return;
            }

            // деактивировать таймер
            _gameTimer?.Invalidate();

            // Записать в файл новый лучший счет
            int newHighScore = Math.Max(_highScore, Convert.ToInt32(_scoreLabel.Text));
            File.WriteAllText(_highScoresFileName, "score, " + newHighScore);

            // перейти к экрану конца игры
            GameOverViewController gameOver = Storyboard.InstantiateViewController("GameOver") as GameOverViewController
                ?? throw new Exception("GameOverViewController not found");

            // Передаем счет игры экрану конца игры
            gameOver.ScoreLabel.Text = _scoreLabel.Text;
            NavigationController.PushViewController(gameOver, true);
        });
    }

    /// <summary>
    /// Поведение таймера.
    /// </summary>
    /// <returns>Текущее время таймера.</returns>
    private int TimerAction()
    {
        if (!_scene.GameIsPaused)
        {
            _currentTime--;
        }

        int minutes = Math.Abs(_currentTime / 60);
        int seconds = _currentTime % 60;

        StringBuilder timerText = new();

        if (minutes < 10)
        {
            _ = timerText.Append("0" + minutes);
        }
        else
        {
            _ = timerText.Append(minutes);
        }

        _ = timerText.Append(':');

        if (seconds < 10)
        {
            _ = timerText.Append("0" + seconds);
        }
        else
        {
            _ = timerText.Append(seconds);
        }

        _timerLabel.Text = timerText.ToString();

        return _currentTime;
    }

    /// <summary>
    /// Перемешать камешки, добавив спрайты камешков на сцену
    /// </summary>
    private void ShuffleGems()
    {
        _level.Shuffle();
        _scene.AttachSpritesToGems(_level.GemList);
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
        ArgumentNullException.ThrowIfNull(View);

        // отключаем интерактивность
        View.UserInteractionEnabled = false;

        bool swapIsValid = _level.Swaps.Contains(swap);

        if (swapIsValid)
        {
            // проводим обмен в модели
            _level.Perform(swap);

            // анимируем обмен на сцене
            _scene.AnimateSwap(swap, swapIsValid);
            await Task.Delay(SWAP_ANIMATION_DURATION);

            // обрабатываем полученные цепочки
            await HandleChainsAsync();

            // задержка, нужная для всех анимаций перед тем,как включать интерактивность
            int delay = DESTRUCTION_ANIMATION_DURATION + FALL_ANIMATION_DURATION;

            // если был разрушитель - увеличиваем время задержки
            if (_chainsHadBonuses)
            {
                delay += LINE_DESTRUCTION_DURATION;
                _chainsHadBonuses = false;
            }

            await Task.Delay(delay);

            View.UserInteractionEnabled = true;
        }
        else
        {
            _scene.AnimateSwap(swap, swapIsValid);

            await Task.Delay(SWAP_ANIMATION_DURATION * 2);

            View.UserInteractionEnabled = true;
        }
    }

    /// <summary>
    /// Поиск и уничтожение цепей, проигрывание анимации уничтожения,
    /// обработка падения камешков на уровне модели, проигрывание анимации
    /// падения, заполнение модели и сцены новыми камешками
    /// </summary>
    public async Task HandleChainsAsync()
    {
        // Сканирование массива до тех пор, пока в модели остаются цепочки, после уничтожения
        // и спуска камешков
        while (_level.RetrieveChain() is not null)
        {
            // уничтожаем цепочки
            _level.DestroyChains();

            // если при уничтожении были найдены бонусы - анимируем сцену особым образом
            if (_level.BonusesToAnimate.Count <= 0)
            {
                // бонусов нет, обычное удаление цепочек
                _scene.AnimateDestructionOfChains(_level.DestroyedChains);

                _level.DestroyedChains.Clear();

                await Task.Delay(DESTRUCTION_ANIMATION_DURATION);
            }
            else
            {
                // Цикл проходит по всем бонусам, анимирует их и анимирует удаление найденных цепочек
                foreach (Gem gem in _level.BonusesToAnimate)
                {
                    if (gem.IsALineDestroyer)
                    {
                        _chainsHadBonuses = true;

                        _scene.AnimateLineDestroyer(gem);

                        _scene.AnimateDestructionOfChains(_level.DestroyedChains);

                        _level.DestroyedChains.Clear();

                        await Task.Delay(LINE_DESTRUCTION_DURATION);
                    }

                    if (gem.IsABomb)
                    {
                        _chainsHadBonuses = true;

                        _scene.AnimateBomb(gem);

                        _scene.AnimateDestructionOfChains(_level.DestroyedChains);

                        _level.DestroyedChains.Clear();

                        await Task.Delay(LINE_DESTRUCTION_DURATION);
                    }
                }

                // очищаем список бонусами на анимацию
                _level.BonusesToAnimate.Clear();
            }

            // анимируем падение камешков
            _scene.AnimateFallingGems(_level.DropGems());
            await Task.Delay(FALL_ANIMATION_DURATION);

        }

        // обновляем лэйблы с счетом
        _scoreLabel.Text = _level.Score.ToString();
        _highScoreLabel.Text = "Лучший счет: " + Math.Max(_level.Score, _highScore) + "";

        // вызываем метод заполнения пустот в модели, создаем для них спрайты
        _scene.AttachSpritesToGems(_level.FillBlanks());

        // создаем спрайты для новых бонусов
        _scene.AttachSpritesToGems(_level.BonusesToAddSpritesTo);

        // очищаем списки обработанных бонусов
        _level.BonusesToAddSpritesTo.Clear();

        // загружаем список доступных обменов
        _level.DetectPossibleSwaps();
    }
}
