
using System;
using System.Collections.Generic;
using System.Linq;
using CoreGraphics;
using Foundation;
using SpriteKit;
using UIKit;

namespace MatchThreeMore
{
    public delegate void Del(Swap swap);

    public class GameScene : SKScene
    {
        public Level Level
        {
            get; set;
        }
        public Del SwipeHandler;
        public bool GameIsPaused;

        private SKNode _gameLayer = new SKNode();
        private SKNode _gemLayer = new SKNode();
        private SKSpriteNode _selectedSprite = new SKSpriteNode();
        private SKSpriteNode _background = new SKSpriteNode("background.jpg");

        private static readonly SKAction _playSwapSound = SKAction.PlaySoundFileNamed("swap.wav", false);
        private static readonly SKAction _playErrorSound = SKAction.PlaySoundFileNamed("error.wav", false);
        private static readonly SKAction _playDingSound = SKAction.PlaySoundFileNamed("ding.wav", false);
        private static readonly SKAction _playDestroySound = SKAction.PlaySoundFileNamed("destroyer.wav", false);
        private static readonly SKAction _playNewDestroyerAppearedSound = SKAction.PlaySoundFileNamed("new_destroyer.wav", false);
        private static readonly SKAction _playExplosionSound = SKAction.PlaySoundFileNamed("explosion.mp3", false);
        private static readonly SKAction _playNewBombAppearedSound = SKAction.PlaySoundFileNamed("new_bomb.mp3", false);

        private float _gemCellHeight;
        private float _gemCellWidth;
        private int _swipeStartColumn;
        private int _swipeStartRow;
        private bool _swipeIsValid;

        protected GameScene(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }

        public override void DidMoveToView(SKView view)
        {
            // Якорь на середину сцены
            AnchorPoint = new CGPoint(0.5f, 0.5f);

            // Подсчитываем рамеры клетки с камешком. Клетки квадратные
            _gemCellWidth = ((float)Size.Width - (Properties.GameFieldPadding * 2)) / Level.ColumnsNumber;
            _gemCellHeight = _gemCellWidth;

            AddBackground();
            AddGameLayer();
            AddGemLayer();
        }

        public override void TouchesBegan(NSSet touches, UIEvent evt)
        {
            if (GameIsPaused)
            {
                return;
            }

            // Определение валидности свайпа и столбца-ряда, с которого начинается свайп
            foreach (NSObject touch in touches)
            {
                CGPoint touchLocation = ((UITouch)touch).LocationInNode(_gemLayer);

                (bool gotRowAndColumn, int touchedColumn, int touchedRow) =
                    GetRowAndColumnFromLocation(touchLocation);

                if (gotRowAndColumn)
                {
                    _swipeStartRow = touchedRow;
                    _swipeStartColumn = touchedColumn;
                    _swipeIsValid = true;

                    // Подсвечиваем выбранный камешек
                    ShowSelectionIndicator(Level.GemArray[touchedRow, touchedColumn]);
                }
            }
        }

        public override void TouchesMoved(NSSet touches, UIEvent evt)
        {
            // определение направления свайпа
            if (_swipeIsValid && !GameIsPaused)
            {
                foreach (NSObject touch in touches)
                {
                    CGPoint touchLocation = ((UITouch)touch).LocationInNode(_gemLayer);

                    (bool gotRowAndColumn, int swipeEndColumn, int swipeEndRow) =
                        GetRowAndColumnFromLocation(touchLocation);

                    if (gotRowAndColumn)
                    {
                        int horizontalDelta =
                            GetDeltaDirection(_swipeStartColumn, swipeEndColumn);
                        int verticalDelta =
                            GetDeltaDirection(_swipeStartRow, swipeEndRow);

                        if (horizontalDelta != 0 || verticalDelta != 0)
                        {
                            TrySwap(horizontalDelta, verticalDelta);

                            _swipeIsValid = false;

                            HideSelectionIndicator();
                        }
                    }

                }
            }
        }

        public override void TouchesEnded(NSSet touches, UIEvent evt)
        {
            // когда прикосновение кончается снимаем подсветку камешка
            if (_selectedSprite.Parent != null && _swipeIsValid && !GameIsPaused)
            {
                HideSelectionIndicator();
            }
        }

        public override void Update(double currentTime)
        {
            // Called before each frame is rendered
        }

        public void SwitchBacgroundZPosition()
        {
            if (_background.ZPosition == 1)
            {
                _background.ZPosition = 150;
            }
            else
            {
                _background.ZPosition = 1;
            }
        }

        /// <summary>
        /// Прикрепление спрайтов к камешкам в списке
        /// </summary>
        /// <param name="gems">Список камешков на обогащение спрайтами</param>
        public void AttachSpritesToGems(GemList gems)
        {
            foreach (Gem gem in gems)
            {
                AttachSpriteToGem(gem);
            }
        }

        /// <summary>
        /// Scan for bonuses and play new bonus (bomb or destroyer) announcements
        /// </summary>
        /// <param name="gems">List of gems to scan</param>
        public void AnnounceBonusGems(GemList gems)
        {
            // маркер появления на игровом поле разрушителя, для проигрывания 
            // звука появления разрушителя
            bool hasDestroyers = gems.Any(g => g.IsALineDestroyer);
            bool hasBomb = gems.Any(g => g.IsABomb);

            // проигрывам звук появления разрушителя
            if (hasBomb)
            {
                RunAction(_playNewBombAppearedSound);
            }
            else if (hasDestroyers)
            {
                RunAction(_playNewDestroyerAppearedSound);
            }
        }

        /// <summary>
        /// Анимация перемещения камешков
        /// </summary>
        /// <param name="swap">Объект с камешками на обмен</param>
        /// <param name="swapIsValid">Индикатор того, что своп возможен</param>
        public void AnimateSwap(Swap swap, bool swapIsValid)
        {
            SKSpriteNode spriteA = swap.GemA.Sprite;
            SKSpriteNode spriteB = swap.GemB.Sprite;

            // Спрайт А "приподнимаем", чтобы создать впечатление, что он пролетает над камешком В
            spriteA.ZPosition = 100;
            spriteB.ZPosition = 90;

            // Анимация камешка А
            SKAction moveA = SKAction.MoveTo(spriteB.Position, Properties.SwapAnimationDuration / 1000f);
            moveA.TimingMode = SKActionTimingMode.EaseOut;

            //Анимация камешка B
            SKAction moveB = SKAction.MoveTo(spriteA.Position, Properties.SwapAnimationDuration / 1000f);
            moveB.TimingMode = SKActionTimingMode.EaseOut;

            if (swapIsValid)
            {
                spriteA.RunAction(moveA);
                spriteB.RunAction(moveB);

                // Проигрываем звук обмена
                RunAction(_playSwapSound);
            }
            else
            {
                spriteA.RunAction(SKAction.Sequence(moveA, moveB));
                spriteB.RunAction(SKAction.Sequence(moveB, moveA));

                // Проигрываем звук ошибки
                RunAction(_playErrorSound);
            }
        }

        /// <summary>
        /// Анимация разрушения цепочек 
        /// </summary>
        /// <param name="chains">Список цепочек на разрушение.</param>
        public void AnimateDestructionOfChains(List<GemList> chains)
        {
            // маркер проверки на наличие в разрушенных цепочках разрушителя
            // если был разрушитель - звук разрушения будет другой
            bool hadDestroyers = false;
            bool hadBombs = false;

            if (chains == null)
            {
                return;
            }

            foreach (GemList chain in chains)
            {
                foreach (Gem gem in chain)
                {
                    SKSpriteNode sprite = gem.Sprite;
                    SKAction sprtieAction = SKAction.FadeAlphaTo(0.0f, Properties.DestructionAnimationDuration / 1000f);
                    sprite.RunAction(SKAction.Sequence(sprtieAction, SKAction.RemoveFromParent()));

                    AnimateScore(chain);

                    hadDestroyers |= gem.IsALineDestroyer;
                    hadBombs |= gem.IsABomb;
                }
            }

            if (hadBombs)
            {
                RunAction(_playExplosionSound);
            }
            else if (hadDestroyers)
            {
                RunAction(_playDestroySound);
            }
            else
            {
                RunAction(_playDingSound);
            }
        }

        /// <summary>
        /// Анимация разрушителей. создает спрайт, на месте бонуса, которому придает анимацию
        /// перемещения к центру (зависит от активированного бонуса - вертикально
        /// или горизонтально), с одновременным растягиванием, иммитируя лазерныйй луч
        /// затем удаляет спрайт со сцены
        /// </summary>
        /// <param name="destroyer">Активированный бонус.</param>
        public void AnimateLineDestroyer(Gem destroyer)
        {
            SKSpriteNode sprite;
            CGPoint centerPoint;
            SKAction resizeSprite;

            // инициализация спрайта, подготовка координат для анимации, размеров
            if (destroyer.IsHorizontal)
            {
                float newWidth = _gemCellWidth * Level.ColumnsNumber;

                sprite = SKSpriteNode.FromImageNamed("destroyer_ray_horisontal");

                centerPoint = new CGPoint(_gemCellWidth * Level.ColumnsNumber / 2, destroyer.Sprite.Position.Y);

                resizeSprite = SKAction.ResizeToWidth(newWidth, Properties.LineDestructionDuration / 1000f);
            }
            else
            {
                float newHeight = _gemCellHeight * Level.RowsNumber;

                sprite = SKSpriteNode.FromImageNamed("destroyer_ray_vertical");

                centerPoint = new CGPoint(destroyer.Sprite.Position.X, _gemCellHeight * Level.RowsNumber / 2);

                resizeSprite = SKAction.ResizeToHeight(newHeight, Properties.LineDestructionDuration / 1000f);
            }

            SKAction moveToCenter = SKAction.MoveTo(centerPoint, Properties.LineDestructionDuration / 1000f);

            CGPoint initialPosition = GetPositionFromRowAndColumn(destroyer.Row, destroyer.Column);
            CGSize initialSize = new CGSize(_gemCellWidth, _gemCellHeight);

            sprite.Size = initialSize;
            sprite.Position = initialPosition;
            sprite.ZPosition = 110;

            _gemLayer.AddChild(sprite);

            sprite.RunAction(moveToCenter);
            sprite.RunAction(SKAction.Sequence(resizeSprite, SKAction.RemoveFromParent()));
        }

        /// <summary>
        /// Анимация бонуса Бомба
        /// </summary>
        /// <param name="bomb">Бомба.</param>
        public void AnimateBomb(Gem bomb)
        {
            CGSize initialSize = new CGSize(_gemCellWidth, _gemCellHeight);
            CGSize newSize = new CGSize(_gemCellWidth * (Properties.BombBlastRadius * 2 + 1), _gemCellHeight * (Properties.BombBlastRadius * 2 + 1));
            CGPoint initialPosition = GetPositionFromRowAndColumn(bomb.Row, bomb.Column);

            SKSpriteNode sprite = SKSpriteNode.FromImageNamed("bomb_blast");
            sprite.Size = initialSize;
            sprite.Position = initialPosition;
            sprite.ZPosition = 110;

            SKAction resizeSprite = SKAction.ResizeTo(newSize, Properties.LineDestructionDuration / 1000f);

            _gemLayer.AddChild(sprite);

            sprite.RunAction(SKAction.Sequence(resizeSprite, SKAction.RemoveFromParent()));
        }

        /// <summary>
        /// Изменение размера сцены
        /// </summary>
        /// <param name="size">Новый размер</param>
        public void SetSize(CGSize size)
        {
            Size = size;
        }

        //++++++++++ДОПОЛНИТЕЛЬНЫЕ МЕТОДЫ+++++++++++++

        private void AddGemLayer()
        {
            // Расчет позиции нода с камешками в зависиомсти от высоты и ширины клетки и колисчества клеток
            CGPoint layerPosition = new CGPoint(-_gemCellWidth * Level.ColumnsNumber / 2.0f, -_gemCellHeight * Level.RowsNumber / 2.0f);

            // добавляем в основной нод нод для камешков
            _gemLayer.Position = layerPosition;
            _gemLayer.ZPosition = 3;
            _gameLayer.AddChild(_gemLayer);
        }

        private void AddGameLayer()
        {
            // добавляем на сцену основной нод игры, в который будут добавлены остальные элементы уровня
            _gameLayer.ZPosition = 2;

            AddChild(_gameLayer);
        }

        private void AddBackground()
        {
            // Устанавливаем бэкграунд из текстуры
            _background.Size = Size;
            _background.ZPosition = 1;

            AddChild(_background);
        }

        private int GetDeltaDirection(int start, int end)
        {
            int delta = end - start;

            return delta == 0 ? 0 : delta < 0 ? -1 : 1;
        }

        /// <summary>
        /// Добававляем его спрайт на нод для камешка, с расчетом размера и позиции
        /// </summary>
        /// <param name="gem">Камешек которому добавляется спрайт.</param>
        private void AttachSpriteToGem(Gem gem)
        {
            SKSpriteNode sprite;

            // Если разрушитель - открепляем старый спрайт на этом месте от слоя камешков
            if ((gem.IsALineDestroyer || gem.IsABomb) && Level.GemArray[gem.Row, gem.Column] != null)
            {
                sprite = Level.GemArray[gem.Row, gem.Column].Sprite;

                if (sprite != null && sprite.Parent != null)
                {
                    sprite.RemoveFromParent();
                }
            }

            // подготовка спрайта
            sprite = SKSpriteNode.FromImageNamed(gem.GetSpriteName());
            sprite.Size = new CGSize(_gemCellWidth, _gemCellHeight);
            sprite.Position = GetPositionFromRowAndColumn(gem.Row, gem.Column);
            _gemLayer.AddChild(sprite);

            gem.Sprite = sprite;

            // подготовка к анимации
            sprite.Alpha = 0;
            sprite.XScale = 0.5f;
            sprite.YScale = 0.5f;

            // Анимация появления камешка
            sprite.RunAction(
              SKAction.Sequence(
                SKAction.WaitForDuration(0.25, 0.5),
                SKAction.Group(
                  SKAction.FadeInWithDuration(0.25),
                  SKAction.ScaleTo(1.0f, 0.25)
                )
            ));

            // если разрушитель - заменяем в массиве камешек
            if (gem.IsALineDestroyer || gem.IsABomb)
            {
                Level.GemArray[gem.Row, gem.Column] = gem;
            }
        }

        /// <summary>
        /// Расчет позиции камешка на сцене в зависимости от его положения в массиве камешков
        /// </summary>
        /// <returns>Точку на ноде</returns>
        /// <param name="row">Ряд.</param>
        /// <param name="column">Колонка.</param>
        private CGPoint GetPositionFromRowAndColumn(int row, int column)
        {
            return new CGPoint(column * _gemCellWidth + _gemCellWidth / 2.0f, row * _gemCellHeight + _gemCellHeight / 2.0f);
        }

        /// <summary>
        /// Расчет положения в массиве в зависимости от позиции на сцене
        /// </summary>
        /// <returns>Индикатор нахождения точки на одной из клеток игровго поля
        /// и координаты клетки</returns>
        /// <param name="point">Точка на игровом поле</param>
        private (bool, int, int) GetRowAndColumnFromLocation(CGPoint point)
        {
            // Проверяем находится ли точка в рамках нода с камешками
            if (point.X >= 0 && point.X < Level.ColumnsNumber * _gemCellWidth * 1.0f &&
                point.Y >= 0 && point.Y < Level.RowsNumber * _gemCellHeight * 1.0f)
            {
                return (true, (int)(point.X / _gemCellWidth), (int)(point.Y / _gemCellHeight));
            }

            return (false, 0, 0);
        }

        /// <summary>
        /// Попытка обмена местами камешков. Сначала подготавливает камешки для смены в класс Swap, 
        /// затем передаем их делегату SwipeHandler, который обрабатывет смену на уровне модели и представления
        /// </summary>
        /// <param name="horizontalDelta">Смещение по горизонтали</param>
        /// <param name="verticalDelta">Смещение по вертикали</param>
        private void TrySwap(int horizontalDelta, int verticalDelta)
        {
            int toColumn = _swipeStartColumn + horizontalDelta;
            int toRow = _swipeStartRow + verticalDelta;

            if (toColumn >= 0 && toColumn < Level.ColumnsNumber ||
                toRow >= 0 && toRow < Level.RowsNumber)
            {
                Gem fromGem = Level.GemArray[_swipeStartRow, _swipeStartColumn];
                Gem toGem = Level.GemArray[toRow, toColumn];

                Swap swap = new Swap(fromGem, toGem);

                SwipeHandler(swap);
            }
        }

        /// <summary>
        /// Анимация счета за уничтожение цепочки
        /// </summary>
        /// <param name="chain">Уничтожаемая цепочка.</param>
        private void AnimateScore(GemList chain)
        {
            SKSpriteNode firstGem = chain.GetFirstGem().Sprite;
            SKSpriteNode lastGem = chain.GetLastGem().Sprite;

            CGPoint centerPoint = new CGPoint(
                (firstGem.Position.X + lastGem.Position.X) / 2,
                (firstGem.Position.Y + lastGem.Position.Y) / 2 - 8);

            SKLabelNode scoreLabel = new SKLabelNode("GillSans-BoldItalic")
            {
                FontSize = 16,
                Text = chain.GetScore() + "",
                Position = centerPoint,
                ZPosition = 300
            };

            _gemLayer.AddChild(scoreLabel);

            SKAction moveAction = SKAction.MoveBy(0, 3, 0.7);
            moveAction.TimingMode = SKActionTimingMode.EaseOut;
            scoreLabel.RunAction(SKAction.Sequence(moveAction, SKAction.RemoveFromParent()));
        }

        /// <summary>
        /// Анимация падения камешков на пустые места
        /// </summary>
        /// <param name="gemLists">Списки камешков перемещенных в модели, столбцами</param>
        public void AnimateFallingGems(List<GemList> gemLists)
        {
            foreach (GemList gems in gemLists)
            {
                foreach (Gem gem in gems)
                {
                    CGPoint newPosition = GetPositionFromRowAndColumn(gem.Row, gem.Column);
                    SKSpriteNode sprite = gem.Sprite;

                    SKAction action = SKAction.MoveTo(newPosition, Properties.FallAnimationDuration / 1000f);
                    action.TimingMode = SKActionTimingMode.EaseOut;

                    sprite.RunAction(action);
                }
            }
        }

        /// <summary>
        /// Подсвечивание выбранного камешка заменой текстуры в спрайте
        /// </summary>
        /// <param name="gem">Камешек на подстветку.</param>
        private void ShowSelectionIndicator(Gem gem)
        {
            // Открепляем спрайт от родителя, если он есть
            if (_selectedSprite.Parent != null)
            {
                _selectedSprite.RemoveFromParent();
            }

            SKSpriteNode sprite = gem.Sprite;
            SKTexture texture = SKTexture.FromImageNamed(gem.GetSelectedSpriteName());

            _selectedSprite.Size = new CGSize(_gemCellWidth, _gemCellHeight);
            _selectedSprite.RunAction(SKAction.SetTexture(texture));

            // "Подсветка" добавляется в качестве потомка к основному спрайту камешка
            sprite.AddChild(_selectedSprite);
            _selectedSprite.Alpha = 1.0f;
        }

        /// <summary>
        /// Отключение подсветки выбранного камешка
        /// </summary>
        private void HideSelectionIndicator()
        {
            // открепляем спрайт "подсветки" 
            _selectedSprite.RunAction(
                SKAction.Sequence(
                    SKAction.FadeOutWithDuration(Properties.SelectedGemTextureFadeDuration / 1000f),
                    SKAction.RemoveFromParent()));
        }
    }
}
