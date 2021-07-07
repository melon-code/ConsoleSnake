using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using Keystroke.API;
using System.Windows.Forms;
using System.IO;

namespace ConsoleSnake {
    public enum Direction {
        Up, Down, Left, Right
    }

    public enum FieldItemType {
        Snake, Head, Tail, Border, Food, Empty
    }

    public enum Collision {
        No, Food, Snake, Border
    }

    public enum SnakeType {
        Head, Body, Tail
    }

    public abstract class FieldItem {
        public FieldItemType Type { get; }

        protected FieldItem(FieldItemType type) {
            Type = type;
        }

        public SnakeItem GetSnakeItem() {
            return this as SnakeItem;
        }

        public abstract Collision GetCollision();
    }

    public class EmptyItem : FieldItem {
        public EmptyItem() : base(FieldItemType.Empty) {
        }

        public override Collision GetCollision() {
            return Collision.No;
        }
    }

    public class BorderItem : FieldItem {
        public BorderItem() : base(FieldItemType.Border) {
        }

        public override Collision GetCollision() {
            return Collision.Border;
        }
    }

    public class FoodItem : FieldItem {
        public int Value { get; }

        public FoodItem(int value) : base(FieldItemType.Food) {
            Value = value;
        }

        public override Collision GetCollision() {
            return Collision.Food;
        }
    }

    public class SnakeItem : FieldItem {
        SnakeItem next;
        SnakeItem prev;

        public Direction Direction { get; }
        public SnakeType BodyPart { get; private set; }

        public SnakeItem(Direction direction, SnakeType bodyPart) : base(FieldItemType.Snake) {
            Direction = direction;
            BodyPart = bodyPart;
        }

        public override Collision GetCollision() {
            return Collision.Snake;
        }

        public void AddHead(SnakeItem newHead) {
            if (BodyPart == SnakeType.Head) {
                prev = newHead;
                BodyPart = next != null ? SnakeType.Body : SnakeType.Tail;
                newHead.next = this;
            }
        }

        public void BecomeTail() {
            if (BodyPart != SnakeType.Head)
                BodyPart = SnakeType.Tail;
            next = null;
        }
    }

    public class Point {
        public int X { get; set; }
        public int Y { get; set; }

        public Point(int x, int y) {
            X = x;
            Y = y;
        }
    }

    public class SnakePoint : Point {
        protected const Direction defaultDirection = Direction.Right;

        public Direction Direction { get; set; }

        public SnakePoint(int x, int y) : base(x, y) {
            Direction = defaultDirection;
        }

        public SnakePoint(int x, int y, Direction direction) : base(x, y) {
            Direction = direction;
        }

        public virtual SnakePoint GetPointAfterMove() {
            SnakePoint point = new SnakePoint(X, Y, Direction);
            switch (Direction) {
                case Direction.Up:
                    point.X -= 1;
                    break;
                case Direction.Down:
                    point.X += 1;
                    break;
                case Direction.Left:
                    point.Y -= 1;
                    break;
                case Direction.Right:
                    point.Y += 1;
                    break;
            }
            return point;
        }
    }

    public class PortalSnakePoint : SnakePoint {
        const int borderlessMinValue = 0;

        readonly int minValue;
        readonly int limitX;
        readonly int limitY;

        public PortalSnakePoint(int x, int y, int limitX, int limitY, int minimum, Direction direction) : base(x, y, direction) {
            this.limitX = limitX;
            this.limitY = limitY;
            minValue = minimum;
        }

        public PortalSnakePoint(int x, int y, int limitX, int limitY, int minimum) : this(x, y, limitX, limitY, minimum, defaultDirection) {
        }

        public override SnakePoint GetPointAfterMove() {
            SnakePoint point = base.GetPointAfterMove();
            if (point.X < minValue)
                point.X += limitX - minValue;
            if (point.Y < minValue)
                point.Y += limitY - minValue;
            point.X %= limitX;
            point.Y %= limitY;
            if (minValue != borderlessMinValue) {
                if (point.X == 0)
                    point.X += minValue;
                if (point.Y == 0)
                    point.Y += minValue;
            }
            return new PortalSnakePoint(point.X, point.Y, limitX, limitY, minValue, point.Direction);
        }
    }

    public class Snake {
        static bool IsOpposite(Direction direction1, Direction direction2) {
            switch (direction1) {
                case Direction.Up:
                    if (direction2 == Direction.Down)
                        return true;
                    break;
                case Direction.Down:
                    if (direction2 == Direction.Up)
                        return true;
                    break;
                case Direction.Left:
                    if (direction2 == Direction.Right)
                        return true;
                    break;
                case Direction.Right:
                    if (direction2 == Direction.Left)
                        return true;
                    break;
            }
            return false;
        }

        protected LinkedList<SnakePoint> snake;

        public bool NeedExtend { get; set; } = false;
        public int Length { get { return snake.Count; } }
        public SnakePoint Head { get { return HeadNode.Value; } }
        public SnakePoint Tail { get { return TailNode.Value; } }
        public LinkedListNode<SnakePoint> HeadNode { get { return snake.First; } }
        public LinkedListNode<SnakePoint> TailNode { get { return snake.Last; } }

        protected Snake() {
            snake = new LinkedList<SnakePoint>();
        }

        public Snake(int headX, int headY) : this() {
            snake.AddFirst(new SnakePoint(headX, headY));
        }

        public Snake(int headX, int headY, Direction direction) : this() {
            snake.AddFirst(new SnakePoint(headX, headY, direction));
        }

        public void SetHeadDirection(Direction direction) {
            var currentDirection = Head.Direction;
            if ((currentDirection != direction) && !IsOpposite(currentDirection, direction))
                Head.Direction = direction;
        }

        public SnakePoint GetNextHead() {
            return Head.GetPointAfterMove();
        }

        public void Move() {
            snake.AddFirst(Head.GetPointAfterMove());
            if (NeedExtend)
                NeedExtend = false;
            else
                snake.RemoveLast();
        }
    }

    public class PortalSnake : Snake {
        public PortalSnake(int headX, int headY, int limitX, int limitY, int minimum) : base() {
            snake.AddFirst(new PortalSnakePoint(headX, headY, limitX, limitY, minimum));
        }

        public PortalSnake(int headX, int headY, int limitX, int limitY, int minimum, Direction direction) : base() {
            snake.AddFirst(new PortalSnakePoint(headX, headY, limitX, limitY, minimum, direction));
        }
    }

    public enum GameState {
        GameOver, InProgress, Win
    }

    public class GameGrid {
        static FieldItem GetBorderMark(bool borderless) {
            if (borderless)
                return new EmptyItem();
            return new BorderItem();
        }

        readonly FieldItem[,] gameGrid;
        Point tailCoordinates;
        SnakeItem snakeHead;

        public int Height { get; }
        public int Width { get; }
        public FieldItem this[int i, int j] { get { return gameGrid[i, j]; } }

        GameGrid(int height, int width) {
            Height = height;
            Width = width;
            gameGrid = new FieldItem[height, width];
        }

        public GameGrid(int height, int width, bool borderless) : this(height, width) {
            for (int i = 0; i < Height; i++)
                for (int j = 0; j < Width; j++)
                    if (!borderless && IsBorder(i, j))
                        gameGrid[i, j] = new BorderItem();
                    else
                        gameGrid[i, j] = new EmptyItem();
        }

        public GameGrid(int height, int width, bool borderless, FieldItem[,] customGameGrid) : this(height, width) {
            if (customGameGrid.Length < height * width)
                throw new ArgumentOutOfRangeException();
            FieldItemType borderMark = borderless ? FieldItemType.Empty : FieldItemType.Border;
            for (int i = 0; i < Height; i++)
                for (int j = 0; j < Width; j++) {
                    if (customGameGrid[i, j] == null)
                        gameGrid[i, j] = new EmptyItem();
                    if (IsBorder(i, j))
                        gameGrid[i, j] = GetBorderMark(borderless);
                    else
                        gameGrid[i, j] = customGameGrid[i, j];
                }
        }

        bool IsBorder(int indX, int indY) {
            if (indX == 0 || indY == 0 || indX == Height - 1 || indY == Width - 1)
                return true;
            return false;
        }

        void AddNewItem(Point point, FieldItem item) {
            gameGrid[point.X, point.Y] = item;
        }

        SnakeItem GetSnakeItem(Point point) {
            return gameGrid[point.X, point.Y].GetSnakeItem();
        }

        public void SetNewSnakeHead(SnakePoint newHead) {
            SnakeItem headItem = new SnakeItem(newHead.Direction, SnakeType.Head);
            if (snakeHead != null) {
                snakeHead.AddHead(headItem);
                snakeHead = headItem;
            }
            else {
                snakeHead = headItem;
                tailCoordinates = newHead;
            }
            AddNewItem(newHead, snakeHead);
        }

        public void RemoveSnakeTail(Point newTail) {
            GetSnakeItem(newTail).BecomeTail();
            if (GetSnakeItem(tailCoordinates).BodyPart != SnakeType.Head)
                AddNewItem(tailCoordinates, new EmptyItem());
            tailCoordinates = newTail;
        }

        public void AddFood(Point food, int value) {
            AddNewItem(food, new FoodItem(value));
        }
    }

    public class Field {
        public static Field CreateField(int height, int width) {
            return new Field(height, width);
        }

        public static Field CreateField(int height, int width, bool borderless, bool allowPortalBorders) {
            return borderless ? new BorderlessField(height, width, allowPortalBorders) : new Field(height, width, allowPortalBorders);
        }

        public static Field CreateField(int height, int width, bool borderless) {
            return CreateField(height, width, borderless, false);
        }

        public static Field CreateField(CustomGameGrid customGrid) {
            return customGrid.Borderless ? new BorderlessField(customGrid.Grid, customGrid.SnakeHeadX, customGrid.SnakeHeadY, customGrid.SnakeHeadDirection, customGrid.PortalBorders)
                : new Field(customGrid.Grid, customGrid.SnakeHeadX, customGrid.SnakeHeadY, customGrid.SnakeHeadDirection, customGrid.PortalBorders);
        }

        const string cggExceptionParamName = "Grid";
        const string cggExceptionMessage = "CustomGameGrid can not be null";
        const int smallFoodValue = 1;
        const int borderWidth = 1;

        protected Snake snake;
        Random rand;
        Direction currentlySetDirection = Direction.Right;

        public GameGrid Grid { get; protected set; }

        int LimitX => Height - BorderWidth;
        int LimitY => Width - BorderWidth;
        protected virtual int BorderWidth => borderWidth;
        protected virtual int PlayableArea { get { return Height * Width - 2 * Height - 2 * Width + 4; } }
        protected bool IsGameOver { get; set; } = false;
        public virtual bool Borderless => false;
        protected bool Win { get { return snake.Length == PlayableArea; } }
        public bool PortalBorders => snake is PortalSnake;
        public int SnakeLenght { get { return snake.Length; } }
        public int Height => Grid.Height;
        public int Width => Grid.Width;
        public GameState State {
            get {
                if (IsGameOver)
                    return GameState.GameOver;
                if (Win)
                    return GameState.Win;
                return GameState.InProgress;
            }
        }

        public Field(int height, int width, bool allowPortalBorders) {
            Grid = new GameGrid(height, width, Borderless);
            snake = allowPortalBorders ? new PortalSnake(Height / 2, Width / 2, LimitX, LimitY, BorderWidth) : new Snake(Height / 2, Width / 2);
            PrepareForStart();
        }

        public Field(int height, int width) : this(height, width, false) {
        }

        public Field(int h, int w, int startLength) : this(h, w) {
            //todo
        }

        public Field(GameGrid customGameGrid, int initialSnakeHeadX, int initialSnakeHeadY, Direction initialSnakeDirection, bool allowPortalBorders) {
            Grid = customGameGrid ?? throw new ArgumentNullException(cggExceptionParamName, cggExceptionMessage);
            snake = allowPortalBorders ? new PortalSnake(initialSnakeHeadX, initialSnakeHeadY, LimitX, LimitY, BorderWidth, initialSnakeDirection)
                : new Snake(initialSnakeHeadX, initialSnakeHeadY, initialSnakeDirection);
            PrepareForStart();
        }

        void PrepareForStart() {
            Grid.SetNewSnakeHead(snake.Head);
            rand = new Random();
            GenerateFood();
        }

        protected Collision IsCollision(int headX, int headY) {
            var item = Grid[headX, headY];
            var collision = item.GetCollision();
            if (collision == Collision.Snake)
                return item.GetSnakeItem().BodyPart == SnakeType.Tail ? Collision.No : Collision.Snake;
            return collision;
        }

        protected virtual Collision IsCollision(Point head) {
            return IsCollision(head.X, head.Y);
        }

        public void SetSnakeDirection(Direction direction) {
            currentlySetDirection = direction;
        }

        bool IsBorder(int indX, int indY) {
            if (indX == 0 || indY == 0 || indX == Height - 1 || indY == Width - 1)
                return true;
            return false;
        }

        void MoveSnakeHead() {
            snake.Move();
            Grid.SetNewSnakeHead(snake.Head);
        }

        void MoveSnake() {
            MoveSnakeHead();
            Grid.RemoveSnakeTail(snake.Tail);
        }

        void MoveAndExtendSnake() {
            snake.NeedExtend = true;
            MoveSnakeHead();
        }

        protected virtual void ProcessBorderCollision() {
            MoveSnake();
            IsGameOver = true;
        }

        public void Iterate() {
            snake.SetHeadDirection(currentlySetDirection);
            var nextHead = snake.GetNextHead();
            switch (IsCollision(nextHead)) {
                case Collision.No:
                    MoveSnake();
                    break;
                case Collision.Food:
                    MoveAndExtendSnake();
                    GenerateFood();
                    break;
                case Collision.Snake:
                case Collision.Border:
                    ProcessBorderCollision();
                    break;
            }
        }

        public void GenerateFood() {
            if (snake.Length < PlayableArea) {
                int x, y;
                do {
                    x = rand.Next(Height);
                    y = rand.Next(Width);
                } while (Grid[x, y].Type != FieldItemType.Empty);
                Grid.AddFood(new Point(x, y), smallFoodValue);
            }
        }
    }

    public class BorderlessField : Field {
        const int borderWidth = 0;

        protected override int BorderWidth => borderWidth;
        protected override int PlayableArea => Height * Width;
        public override bool Borderless => true;

        public BorderlessField(int h, int w, bool allowPortalBorders) : base(h, w, allowPortalBorders) {
        }

        public BorderlessField(int h, int w) : this(h, w, false) {
        }

        public BorderlessField(GameGrid customGameGrid, int initialSnakeHeadX, int initialSnakeHeadY, Direction initialSnakeDirection, bool allowPortalBorders)
            : base(customGameGrid, initialSnakeHeadX, initialSnakeHeadY, initialSnakeDirection, allowPortalBorders) {
        }

        protected override Collision IsCollision(Point head) {
            int headX = head.X;
            int headY = head.Y;
            if (headX < 0 || headY < 0 || headX > Height - 1 || headY > Width - 1) // do not need with portal snake
                return Collision.Border;
            return IsCollision(headX, headY);
        }

        protected override void ProcessBorderCollision() {
            IsGameOver = true;
        }
    }

    public class ConsoleSnakeDrawer {
        readonly int initialWindowHeight = Console.WindowHeight;
        readonly int initialWindowWidth = Console.WindowWidth;
        readonly int initialBufferHeight = Console.BufferHeight;
        readonly int initialBufferWidth = Console.BufferWidth;
        Field drawingField;

        public ConsoleSnakeDrawer(Field gameField) {
            drawingField = gameField;
        }

        public void SetConsoleWindow() {
            Console.Clear();
            Console.CursorVisible = false;
            if (drawingField.Borderless) {
                Console.WindowHeight = 1;
                //Console.SetWindowSize(1, 1);
                //var left = Console.WindowLeft;
                //var top = Console.WindowTop;
                //Console.SetBufferSize(gameField.Width, gameField.Height);
                //Console.BufferHeight = gameField.Height + 1;
                //Console.WindowHeight = gameField.Height;
                //Console.BufferWidth = 10;
                //Console.SetWindowSize(gameField.Width, gameField.Height);
                var left = Console.WindowLeft;
                var top = Console.WindowTop;
            }
        }

        public void RestoreConsoleWindow() {
            //Console.SetWindowSize(1, 1);
            //Console.SetBufferSize(initialBufferHeight, initialBufferWidth);
            //Console.SetWindowSize(initialWindowHeight, initialWindowWidth);
        }

        string GetHeadChar(Direction direction) {
            switch (direction) {
                case Direction.Up:
                    return "^";
                case Direction.Down:
                    return "v";
                case Direction.Left:
                    return "<";
                case Direction.Right:
                    return ">";
                default:
                    return ">";
            }
        }

        string GetDirectionSymbol(Direction direction) { //debug
            switch (direction) {
                case Direction.Up:
                    return "U";
                case Direction.Down:
                    return "D";
                case Direction.Left:
                    return "L";
                case Direction.Right:
                    return "R";
                default:
                    return "?";
            }
        }

        string DrawSnakeItem(SnakeItem item) {
            Direction direction = item.Direction;
            switch (item.BodyPart) {
                case SnakeType.Head:
                    return GetHeadChar(direction);
                case SnakeType.Body:
                    return GetDirectionSymbol(direction);
                case SnakeType.Tail:
                    return "*";
                default:
                    return "?";
            }
        }

        void DrawItem(FieldItem item) {
            if (item != null) {
                switch (item.Type) {
                    case FieldItemType.Snake:
                        //Console.Write("#");
                        Console.Write(DrawSnakeItem(item.GetSnakeItem()));
                        break;
                    case FieldItemType.Border:
                        Console.Write("B");
                        break;
                    case FieldItemType.Food:
                        Console.Write("o");
                        break;
                    default:
                        Console.Write(" ");
                        break;
                }
            }
            else
                Console.Write("?");
        }

        public void DrawGameField() {
            for (int i = 0; i < drawingField.Height; i++) {
                for (int j = 0; j < drawingField.Width; j++) {
                    DrawItem(drawingField.Grid[i, j]);
                }
                Console.WriteLine();
            }
            Console.SetCursorPosition(0, 0);
        }
    }

    public class ConsoleGame {
        const int defaultSnakeSpeed = 6;

        readonly int snakeSpeed;
        Field gameField;
        ConsoleSnakeDrawer drawer;
        bool isFirstTurn = true;

        public bool AnyKeyPressed { get; private set; }
        public SnakeGameStats Results { get { return new SnakeGameStats(gameField.State == GameState.Win ? true : false, gameField.SnakeLenght); } }

        ConsoleGame(int speed, Func<Field> createField) {
            snakeSpeed = 1100 - speed * 100;
            gameField = createField();
            drawer = new ConsoleSnakeDrawer(gameField);
        }

        public ConsoleGame(int height, int width, bool borderless, bool portalBorders, int speed) : this(speed, () => Field.CreateField(height, width, borderless, portalBorders)) {
        }

        public ConsoleGame(int height, int width) : this(height, width, false, false, defaultSnakeSpeed) {
        }

        public ConsoleGame(int height, int width, bool portalBorders) : this(height, width, false, portalBorders, defaultSnakeSpeed) {
        }

        public ConsoleGame(int height, int width, bool portalBorders, int speed) : this(height, width, false, portalBorders, speed) {
        }

        public ConsoleGame(int height, int width, bool borderless, bool portalBorders) : this(height, width, borderless, portalBorders, defaultSnakeSpeed) {
        }

        public ConsoleGame(CustomGameGrid gameGrid, int speed) : this(speed, () => Field.CreateField(gameGrid)) {
        }

        void RenderFrame() {
            if (isFirstTurn)
                isFirstTurn = false;
            else
                gameField.Iterate();
            drawer.DrawGameField();
            if (gameField.State != GameState.InProgress)
                Application.Exit();
        }

        public void StartLoop() {
            Renderer rend = new Renderer(snakeSpeed, RenderFrame);
            drawer.SetConsoleWindow();
            using (var api = new KeystrokeAPI()) {
                api.CreateKeyboardHook((character) => {
                    if (character.KeyCode != KeyCode.Escape) {
                        switch (character.KeyCode) {
                            case KeyCode.Up:
                                gameField.SetSnakeDirection(Direction.Up);
                                break;
                            case KeyCode.Down:
                                gameField.SetSnakeDirection(Direction.Down);
                                break;
                            case KeyCode.Left:
                                gameField.SetSnakeDirection(Direction.Left);
                                break;
                            case KeyCode.Right:
                                gameField.SetSnakeDirection(Direction.Right);
                                break;
                        }
                    }
                    else Application.Exit();
                });
                rend.Start();
                Application.Run();
                rend.Stop();
            }
            drawer.RestoreConsoleWindow();
        }
    }

    public class Renderer {
        //System.Windows.Forms.Timer timer;
        System.Threading.Timer timer;
        int time;
        TimerCallback func;

        public Renderer(int timeBetween, Action renderFunc) {
            //timer = new System.Windows.Forms.Timer();
            //timer.Interval = timeBetween;
            //timer.Tick += new EventHandler((s, e) => { renderFunc(); });
            func = new TimerCallback((obj) => renderFunc());
            time = timeBetween;
        }

        public void Start() {
            //timer.Start();
            timer = new System.Threading.Timer(func, null, 0, time);
        }

        public void Stop() {
            //timer.Stop();
            timer.Dispose();
        }
    }

    public class LocalizationDictionary {
        const string errorString = "NULL";
        public const int OnTitleKey = 1;
        public const int OffTitleKey = 2;
        public const int InputNumberKey = 3;
        public const int ExitStringKey = 4;
        public const int HeightKey = 5;
        public const int WidthKey = 6;
        public const int BigFoodKey = 7;
        public const int PortalBorderKey = 8;
        public const int SpeedKey = 9;
        public const int CustomFieldKey = 10;
        public const int CustomFieldTypeKey = 11;
        public const int NewGameKey = 20;
        public const int SettingsKey = 21;
        public const int SameRepeatKey = 22;
        public const int ToMainMenuKey = 23;
        public const int DisplaySnakeLengthKey = 25;
        public const int WinKey = 30;
        public const int GameOverKey = 31;

        readonly protected Dictionary<int, string> dictionary;

        public string this[int key] => GetItem(key); 

        public LocalizationDictionary() {
            dictionary = new Dictionary<int, string>();
        }

        public string GetItem(int key) {
            if (dictionary.TryGetValue(key, out string value))
                return value;
            return errorString;
        }
    }

    public class EngLangDictionary : LocalizationDictionary {
        const string onTitle = "On";
        const string offTitle = "Off";
        const string inputNumber = "Input integer value: ";
        const string exitString = "Exit";
        const string height = "Height";
        const string width = "Width";
        const string bigFood = "Big food";
        const string portalBorder = "Portal borders";
        const string speed = "Speed";
        const string customField = "Custom field";
        const string customFieldType = "Custom field type";
        const string newGame = "New Game";
        const string settings = "Settings";
        const string sameRepeat = "Start again";
        const string toMainMenu = "Go to Main Menu";
        const string displaySnakeLength = "Snake length is ";
        const string win = "Congratulations! You WON!";
        const string gameOver = "GAME OVER Try fortune next time!";

        public EngLangDictionary() : base() {
            dictionary.Add(OnTitleKey, onTitle);
            dictionary.Add(OffTitleKey, offTitle);
            dictionary.Add(InputNumberKey, inputNumber);
            dictionary.Add(ExitStringKey, exitString);
            dictionary.Add(HeightKey, height);
            dictionary.Add(WidthKey, width);
            dictionary.Add(BigFoodKey, bigFood);
            dictionary.Add(PortalBorderKey, portalBorder);
            dictionary.Add(SpeedKey, speed);
            dictionary.Add(CustomFieldKey, width);
            dictionary.Add(CustomFieldTypeKey, customFieldType);
            dictionary.Add(NewGameKey, newGame);
            dictionary.Add(SettingsKey, settings);
            dictionary.Add(SameRepeatKey, sameRepeat);
            dictionary.Add(ToMainMenuKey, toMainMenu);
            dictionary.Add(DisplaySnakeLengthKey, displaySnakeLength);
            dictionary.Add(WinKey, win);
            dictionary.Add(GameOverKey, gameOver);
        }
    }

    public class RusLangDictionary : LocalizationDictionary {
        const string onTitle = "Да";
        const string offTitle = "Нет";
        const string inputNumber = "Введите числовое значение: ";
        const string exitString = "Выход";
        const string height = "Высота";
        const string width = "Ширина";
        const string bigFood = "Большая еда";
        const string portalBorder = "Портальные границы";
        const string speed = "Скорость";
        const string customField = "Пользовательское поле";
        const string customFieldType = "Тип пользовательского поля";
        const string newGame = "Новая игра";
        const string settings = "Настройки";
        const string sameRepeat = "Повторить снова";
        const string toMainMenu = "В главное меню";
        const string displaySnakeLength = "Длина змейки: ";
        const string win = "Победа!";
        const string gameOver = "Вы проиграли, попробуйте снова!";

        public RusLangDictionary() : base() {
            dictionary.Add(OnTitleKey, onTitle);
            dictionary.Add(OffTitleKey, offTitle);
            dictionary.Add(InputNumberKey, inputNumber);
            dictionary.Add(ExitStringKey, exitString);
            dictionary.Add(HeightKey, height);
            dictionary.Add(WidthKey, width);
            dictionary.Add(BigFoodKey, bigFood);
            dictionary.Add(PortalBorderKey, portalBorder);
            dictionary.Add(SpeedKey, speed);
            dictionary.Add(CustomFieldKey, width);
            dictionary.Add(CustomFieldTypeKey, customFieldType);
            dictionary.Add(NewGameKey, newGame);
            dictionary.Add(SettingsKey, settings);
            dictionary.Add(SameRepeatKey, sameRepeat);
            dictionary.Add(ToMainMenuKey, toMainMenu);
            dictionary.Add(DisplaySnakeLengthKey, displaySnakeLength);
            dictionary.Add(WinKey, win);
            dictionary.Add(GameOverKey, gameOver);
        }
    }

    public static class Localization {
        static LocalizationDictionary dictionary = new RusLangDictionary();

        public static string OnTitle => dictionary.GetItem(LocalizationDictionary.OnTitleKey);
        public static string OffTitle => dictionary.GetItem(LocalizationDictionary.OffTitleKey);
        public static string InputNumber => dictionary.GetItem(LocalizationDictionary.InputNumberKey);
        public static string ExitString => dictionary.GetItem(LocalizationDictionary.ExitStringKey);
        public static string Height => dictionary.GetItem(LocalizationDictionary.HeightKey);
        public static string Width => dictionary.GetItem(LocalizationDictionary.WidthKey);
        public static string BigFood => dictionary.GetItem(LocalizationDictionary.BigFoodKey);
        public static string PortalBorders => dictionary.GetItem(LocalizationDictionary.PortalBorderKey);
        public static string Speed => dictionary.GetItem(LocalizationDictionary.SpeedKey);
        public static string CustomField => dictionary.GetItem(LocalizationDictionary.CustomFieldKey);
        public static string CustomFieldType => dictionary.GetItem(LocalizationDictionary.CustomFieldTypeKey);
        public static string NewGame => dictionary.GetItem(LocalizationDictionary.NewGameKey);
        public static string Settings => dictionary.GetItem(LocalizationDictionary.SettingsKey);
        public static string SameRepeat => dictionary.GetItem(LocalizationDictionary.SameRepeatKey);
        public static string ToMainMenu => dictionary.GetItem(LocalizationDictionary.ToMainMenuKey);
        public static string DisplaySnakeLength => dictionary.GetItem(LocalizationDictionary.DisplaySnakeLengthKey);
        public static string Win => dictionary.GetItem(LocalizationDictionary.WinKey);
        public static string GameOver => dictionary.GetItem(LocalizationDictionary.GameOverKey);

        public static void ChangeLanguage() {
            if (dictionary is RusLangDictionary)
                dictionary = new EngLangDictionary();
            else
                dictionary = new RusLangDictionary();
        }
    }

    public enum MenuEndResult {
        Further, Exit
    }

    public abstract class ConsoleMenu {
        readonly bool hasExitItem = false;

        bool IsExitSelected { get { return hasExitItem && CurrentPosition == ItemCount - 1; } }
        int ItemCount { get { return Items.Count; } }
        protected IList<IMenuItem> Items { get; }
        protected bool IsEnd { get; set; }
        protected int CurrentPosition { get; private set; } = 0;
        protected IMenuItem CurrentItem => Items[CurrentPosition];
        public MenuEndResult EndResult { get; protected set; }

        public ConsoleMenu(IList<IMenuItem> menuItems) {
            Items = menuItems;
        }

        public ConsoleMenu(IList<IMenuItem> menuItems, string exitTitle) : this(menuItems) {
            Items.Add(new MenuItem(exitTitle));
            hasExitItem = true;
        }

        public MenuEndResult ShowDialog() {
            IsEnd = false;
            EndResult = MenuEndResult.Further;
            if (Console.KeyAvailable)
                Console.ReadKey(true);
            do {
                Draw();
            } while (Navigation(Console.ReadKey(true)) && !IsEnd);
            return EndResult;
        }

        public void DrawMenu() {
            for (int i = 0; i < ItemCount; i++) {
                if (i == CurrentPosition)
                    Console.Write("\t---> ");
                if (Items[i].Visible)
                    Items[i].Draw();
            }
        }

        protected virtual void Draw() {
            Console.Clear();
            DrawMenu();
        }

        void ChangeCurrentPosition(bool increase) {
            int iterations = 0;
            do {
                CurrentPosition = increase ? CurrentPosition + 1 : ItemCount + CurrentPosition - 1;
                CurrentPosition %= ItemCount;
                iterations++;
            } while (iterations != ItemCount && !CurrentItem.Visible);
        }

        void IncreaseCurrentPosition() {
            ChangeCurrentPosition(true);
        }

        void DecreaseCurrentPosition() {
            ChangeCurrentPosition(false);
        }

        void CheckInteractivityAndProcessInput(ConsoleKey input) {
            if (CurrentItem.Interactive)
                ProcessInput(input);
        }

        public bool Navigation(ConsoleKeyInfo info) {
            switch (info.Key) {
                case ConsoleKey.DownArrow:
                    IncreaseCurrentPosition();
                    break;
                case ConsoleKey.UpArrow:
                    DecreaseCurrentPosition();
                    break;
                case ConsoleKey.Enter:
                    if (IsExitSelected) {
                        EndResult = MenuEndResult.Exit;
                        return false;
                    }
                    CheckInteractivityAndProcessInput(info.Key);
                    break;
                case ConsoleKey.Escape:
                    EndResult = MenuEndResult.Exit;
                    return false;
                default:
                    CheckInteractivityAndProcessInput(info.Key);
                    break;
            }
            return true;
        }

        public abstract void ProcessInput(ConsoleKey input);
    }

    public interface IMenuItem {
        bool Visible { get; set; }
        bool Interactive { get; set; }
        string Name { get; }

        void Draw();
        void ProcessInput(ConsoleKey input);
        void ChangeName(string newName);
    }

    public interface IMenuValueItem<T> : IMenuItem {
        T Value { get; }
    }

    public abstract class MenuItemBase : IMenuItem {
        public bool Visible { get; set; } = true;
        public bool Interactive { get; set; } = true;
        public string Name { get; private set; }

        protected MenuItemBase(string name) {
            Name = name;
        }

        public void ChangeName(string newName) {
            Name = newName;
        }

        public abstract void Draw();
        public abstract void ProcessInput(ConsoleKey input);
    }

    public class MenuItem : MenuItemBase {
        public MenuItem(string name) : base(name) {
        }

        public override void Draw() {
            Console.Write("\t" + Name + "\n\n");
        }

        public override void ProcessInput(ConsoleKey input) {
            //mb add inside menus
        }
    }

    public abstract class ValueBasedItem<T> : MenuItemBase {
        public T Value { get; protected set; }

        public ValueBasedItem(string name, T defaultValue) : base(name) {
            Value = defaultValue;
        }
    }

    public class BoolMenuItem : ValueBasedItem<bool>, IMenuValueItem<bool> {
        public BoolMenuItem(string name, bool defaultValue) : base(name, defaultValue) {
        }

        public override void Draw() {
            Console.WriteLine("\t" + Name + string.Format(" < {0} >", Value ? Localization.OnTitle : Localization.OffTitle) + "\n");
        }

        public virtual void ChangeValue() {
            Value = !Value;
        }

        public override void ProcessInput(ConsoleKey input) {
            if (input == ConsoleKey.Enter || input == ConsoleKey.LeftArrow || input == ConsoleKey.RightArrow)
                ChangeValue();
        }
    }

    public class DependencyItem {
        readonly IMenuItem item;
        readonly bool inverted;

        public DependencyItem(IMenuItem menuItem, bool invertedLogic) {
            item = menuItem;
            inverted = invertedLogic;
        }

        public DependencyItem(IMenuItem menuItem) : this(menuItem, false) {
        }

        public void ChangeVisibility(bool value) {
            item.Visible = value ^ inverted;
        }
    }

    public class DependencyBoolMenuItem : BoolMenuItem {
        readonly IList<DependencyItem> items;

        public DependencyBoolMenuItem(string name, bool defaultValue, IList<DependencyItem> dependencyItems) : base(name, defaultValue) {
            items = dependencyItems;
            SyncDependency(defaultValue);
        }

        void SyncDependency(bool value) {
            foreach (var item in items)
                item.ChangeVisibility(value);
        }

        public override void ChangeValue() {
            base.ChangeValue();
            SyncDependency(Value);
        }
    }

    public class IntMenuItem : ValueBasedItem<int>, IMenuValueItem<int> {
        static bool ValidateStringInput(string str) {
            if (string.IsNullOrEmpty(str))
                return false;
            bool valid = true;
            foreach (var item in str)
                if (!char.IsNumber(item))
                    valid = false;
            return valid;
        }

        readonly int minValue;
        readonly int maxValue;

        public IntMenuItem(string name, int defaultValue, int minLimit, int maxLimit) : base(name, defaultValue) {
            minValue = minLimit;
            maxValue = maxLimit;
        }

        public IntMenuItem(string name, int defaultValue) : this(name, defaultValue, int.MinValue, int.MaxValue) {
        }


        public override void Draw() {
            Console.WriteLine("\t" + Name + string.Format(" < {0} >", Value) + "\n");
        }

        public void IncrementValue() {
            if (Value < maxValue)
                Value++;
        }

        public void DecrementValue() {
            if (Value > minValue)
                Value--;
        }

        bool ValidateInteger(int number) {
            if (number >= minValue && number <= maxValue) {
                Value = number;
                return true;
            }
            return false;
        }

        public void InputValue() {
            Console.Clear();
            string input;
            bool isInputValid = false;
            do {
                Console.Write(Localization.InputNumber);
                input = Console.ReadLine();
                if (ValidateStringInput(input) && ValidateInteger(Convert.ToInt32(input)))
                    isInputValid = true;
            } while (!isInputValid);
        }

        public override void ProcessInput(ConsoleKey input) {
            switch (input) {
                case ConsoleKey.Enter:
                    InputValue();
                    Draw();
                    break;
                case ConsoleKey.LeftArrow:
                    DecrementValue();
                    break;
                case ConsoleKey.RightArrow:
                    IncrementValue();
                    break;
            }
        }
    }

    public class SettingsMenu : ConsoleMenu {
        public static CustomGameGrid GetCustomGrid(int type) {
            switch (type) {
                case 1:
                    return CustomGameGridTypes.TypeA;
                case 2:
                    return CustomGameGridTypes.TypeB;
                case 3:
                    return CustomGameGridTypes.TypeC;
                default:
                    return CustomGameGridTypes.TypeA;
            }
        }

        const int heightIndex = 0;
        const int widthIndex = 1;
        const int bigFoodIndex = 2;
        const int portalBorderIndex = 3;
        const int snakeSpeedIndex = 4;
        const int isCustomGridIndex = 5;
        const int customGridTypeIndex = 6;

        public bool LanguageChanged { get; private set; } = false;
        public int Height => GetInt(heightIndex);
        public int Width => GetInt(widthIndex);
        public bool BigFood => GetBool(bigFoodIndex);
        public bool PortalBorders => GetBool(portalBorderIndex);
        public int SnakeSpeed => GetInt(snakeSpeedIndex);
        public bool IsCustomGrid => GetBool(isCustomGridIndex);
        public int? CustomGridType {
            get {
                if (IsCustomGrid)
                    return GetInt(customGridTypeIndex);
                return null;
            }
        }

        public SettingsMenu(IList<IMenuItem> settingsItems) : base(settingsItems, Localization.ExitString) {
        }

        T GetValue<T, Type>(int index) where Type : IMenuValueItem<T> {
            var item = Items[index];
            if (item is Type)
                return ((Type)item).Value;
            throw new ArgumentException();
        }

        int GetInt(int index) {
            return GetValue<int, IntMenuItem>(index);
        }

        bool GetBool(int index) {
            return GetValue<bool, BoolMenuItem>(index);
        }

        public void UpdateNames() {
            var updatedItems = ItemsListHelper.GetSettingsMenuList();
            for (int i = 0; i < updatedItems.Count; i++)
                Items[i].ChangeName(updatedItems[i].Name);
            Items.Last().ChangeName(Localization.ExitString);
        }

        public override void ProcessInput(ConsoleKey input) {
            CurrentItem.ProcessInput(input);
        }
    }

    public struct SettingsResult {
        public int Height { get; }
        public int Width { get; }
        public bool BigFood { get; }
        public bool PortalBorders { get; }
        public int SnakeSpeed { get; }
        public bool IsCustomGrid { get; }
        public int? CustomGridType { get; }

        public SettingsResult(int height, int width, bool bigFood, bool portalBorders, int snakeSpeed, bool isCustomGrid, int? customGridType) {
            Height = height;
            Width = width;
            BigFood = bigFood;
            PortalBorders = portalBorders;
            SnakeSpeed = snakeSpeed;
            IsCustomGrid = isCustomGrid;
            CustomGridType = customGridType;
        }
    }

    public static class ItemsListHelper {
        const int defaultHeight = 10;
        const int defaultWidth = 10;
        const int minHeight = 10;
        const int minWidth = 10;
        const int defaultSpeed = 5; //500 ms
        const int minSpeed = 1;
        const int maxSpeed = 10;

        public static IList<IMenuItem> GetSettingsMenuList() {
            IntMenuItem customTypes = new IntMenuItem(Localization.CustomFieldType, 1, 1, 3);
            IList<DependencyItem> dependencies = new List<DependencyItem>() { new DependencyItem(customTypes) };
            return new List<IMenuItem> { new IntMenuItem(Localization.Height, defaultHeight, minHeight, Console.LargestWindowHeight),
                new IntMenuItem(Localization.Width, defaultWidth, minWidth, Console.LargestWindowWidth), new BoolMenuItem(Localization.BigFood, false),
                new BoolMenuItem(Localization.PortalBorders, false), new IntMenuItem(Localization.Speed, defaultSpeed, minSpeed, maxSpeed),
                new DependencyBoolMenuItem(Localization.CustomField, false, dependencies), customTypes };
        }

        public static IList<IMenuItem> GetMainMenuList() {
            return new IMenuItem[] { new MenuItem(Localization.NewGame), new MenuItem(Localization.Settings), new MenuItem(Localization.ExitString) };
        }

        public static IList<IMenuItem> GetEndscreenMenuList() {
            return new List<IMenuItem>() { new MenuItem(Localization.SameRepeat), new MenuItem(Localization.ToMainMenu) };
        }
    }

    public class MainMenu : ConsoleMenu {
        SettingsMenu sMenu;

        public SettingsResult Settings => new SettingsResult(sMenu.Height, sMenu.Width, sMenu.BigFood, sMenu.PortalBorders, sMenu.SnakeSpeed, sMenu.IsCustomGrid, sMenu.CustomGridType);

        public MainMenu() : base(ItemsListHelper.GetMainMenuList()) {
            sMenu = new SettingsMenu(ItemsListHelper.GetSettingsMenuList());
        }

        void UpdateNames() {
            var updatedItems = ItemsListHelper.GetMainMenuList();
            for (int i = 0; i < Items.Count; i++)
                Items[i].ChangeName(updatedItems[i].Name);
        }

        public override void ProcessInput(ConsoleKey input) {
            if (input == ConsoleKey.Enter) {
                if (CurrentPosition == 0) {
                    EndResult = MenuEndResult.Further;
                    IsEnd = true;
                }
                if (CurrentPosition == 1) {
                    sMenu.ShowDialog();
                }
                if (CurrentPosition == 2) {
                    EndResult = MenuEndResult.Exit;
                    IsEnd = true;
                }
            }
            if (input == ConsoleKey.L) {
                Localization.ChangeLanguage();
                UpdateNames();
                sMenu.UpdateNames();
            }
        }

        protected override void Draw() {
            base.Draw();
            Console.WriteLine("\n\tChange languege -> L");
        }
    }

    public struct SnakeGameStats {
        public bool Win { get; }
        public int SnakeLength { get; }

        public SnakeGameStats(bool win, int snakeLenght) {
            Win = win;
            SnakeLength = snakeLenght;
        }
    }

    public class EndscreenMenu : ConsoleMenu {
        public SnakeGameStats GameResults { get; set; }
        public bool Restart { get; private set; } = false;

        public EndscreenMenu() : base(ItemsListHelper.GetEndscreenMenuList(), Localization.ExitString) {
        }

        public override void ProcessInput(ConsoleKey input) {
            if (input == ConsoleKey.Enter) {
                Restart = CurrentPosition == 0 ? true : false;
                IsEnd = true;
            }
        }

        protected override void Draw() {
            Console.Clear();
            Console.WriteLine(string.Format("\t{0}\n", GameResults.Win ? Localization.Win : Localization.GameOver));
            Console.WriteLine(string.Format("\t{0}" + GameResults.SnakeLength + "\n", Localization.DisplaySnakeLength));
            DrawMenu();
        }
    }

    public class ConsoleGameProcessor {
        public void Run() {
            MenuEndResult endscreenResult = MenuEndResult.Further;
            MainMenu menu = new MainMenu();
            while (endscreenResult == MenuEndResult.Further && menu.ShowDialog() == MenuEndResult.Further) {
                EndscreenMenu endMenu = new EndscreenMenu();
                do {
                    var result = menu.Settings;
                    ConsoleGame snakeGame = result.IsCustomGrid ? new ConsoleGame(SettingsMenu.GetCustomGrid(result.CustomGridType.Value), result.SnakeSpeed)
                        : new ConsoleGame(result.Height, result.Width, result.PortalBorders, result.SnakeSpeed);
                    snakeGame.StartLoop();
                    endMenu.GameResults = snakeGame.Results;
                    endscreenResult = endMenu.ShowDialog();
                } while (endMenu.EndResult == MenuEndResult.Further && endMenu.Restart);
            }
            Console.Write("\nEXIT!");
        }
    }

    public static class CustomGameGridParser {
        public static GameGrid Parse(int height, int width, string grid, bool borderless) {
            if (height * width != grid.Length)
                return null;
            FieldItem[,] items = new FieldItem[height, width];
            for (int i = 0; i < height; i++)
                for (int j = 0; j < width; j++)
                    items[i, j] = ParseItem(grid[i * width + j]);
            return new GameGrid(height, width, borderless, items);
        }

        static FieldItem ParseItem(char item) {
            switch (item) {
                case ' ':
                    return new EmptyItem();
                case 'B':
                    return new BorderItem();
                default:
                    return new EmptyItem();
            }
        }
    }

    public struct CustomGameGrid {
        public GameGrid Grid { get; }
        public int SnakeHeadX { get; }
        public int SnakeHeadY { get; }
        public Direction SnakeHeadDirection { get; }
        public bool PortalBorders { get; }
        public bool Borderless { get; }

        public CustomGameGrid(int height, int width, string grid, bool borderless, int snakeHeadX, int snakeHeadY, Direction snakeHeadDirection, bool portalBorders) {
            Grid = CustomGameGridParser.Parse(height, width, grid, borderless);
            SnakeHeadX = snakeHeadX;
            SnakeHeadY = snakeHeadY;
            SnakeHeadDirection = snakeHeadDirection;
            PortalBorders = portalBorders;
            Borderless = borderless;
        }
    }

    public static class CustomGameGridTypes {
        public static CustomGameGrid TypeA => new CustomGameGrid(CustomGameGridTypeA.Height, CustomGameGridTypeA.Width, CustomGameGridTypeA.Grid, CustomGameGridTypeA.Borderless,
            CustomGameGridTypeA.HeadX, CustomGameGridTypeA.HeadY, CustomGameGridTypeA.HeadDirection, CustomGameGridTypeA.PortalBorders);
        public static CustomGameGrid TypeB => new CustomGameGrid(CustomGameGridTypeB.Height, CustomGameGridTypeB.Width, CustomGameGridTypeB.Grid, CustomGameGridTypeA.Borderless,
            CustomGameGridTypeB.HeadX, CustomGameGridTypeB.HeadY, CustomGameGridTypeB.HeadDirection, CustomGameGridTypeB.PortalBorders);
        public static CustomGameGrid TypeC => new CustomGameGrid(CustomGameGridTypeC.Height, CustomGameGridTypeC.Width, CustomGameGridTypeC.Grid, CustomGameGridTypeA.Borderless,
            CustomGameGridTypeC.HeadX, CustomGameGridTypeC.HeadY, CustomGameGridTypeC.HeadDirection, CustomGameGridTypeC.PortalBorders);
    }

    public static class CustomGameGridTypeA {
        public const int Height = 10;
        public const int Width = 12;
        public const int HeadX = Height / 2;
        public const int HeadY = Width / 2;
        public const Direction HeadDirection = Direction.Right;
        public const bool PortalBorders = false;
        public const bool Borderless = false;
        public const string Grid = "BBBBBBBBBBBB" +
                                   "B          B" +
                                   "B          B" +
                                   "B   BBBB   B" +
                                   "B          B" +
                                   "B          B" +
                                   "B   BBBB   B" +
                                   "B          B" +
                                   "B          B" +
                                   "BBBBBBBBBBBB";
    }

    public static class CustomGameGridTypeB {
        public const int Height = 3;
        public const int Width = 10;
        public const int HeadX = 1;
        public const int HeadY = 1;
        public const Direction HeadDirection = Direction.Right;
        public const bool PortalBorders = true;
        public const bool Borderless = false;
        public const string Grid = "BBBBBBBBBB" +
                                   "B        B" +
                                   "BBBBBBBBBB";
    }

    public static class CustomGameGridTypeC {
        public const int Height = 10;
        public const int Width = 4;
        public const int HeadX = 1;
        public const int HeadY = 1;
        public const Direction HeadDirection = Direction.Down;
        public const bool PortalBorders = false;
        public const bool Borderless = false;
        public const string Grid = "BBBB" +
                                   "B  B" +
                                   "B  B" +
                                   "B  B" +
                                   "B  B" +
                                   "B  B" +
                                   "B  B" +
                                   "B  B" +
                                   "B  B" +
                                   "B  B" +
                                   "BBBB";
    }

    public class Modif {
        public void Mod(int[] m) {
            m[0] = 11;
            m[1] = 22;
            m = new int[] { 5, 7, 8 };
        }

        public void ModInt(int v) {
            v = 66;
        }
    }

    public class Test {
        public int[] mass = new int[] { 1, 2, 3, 4 };
        int t = 2;
        Modif modif = new Modif();
        public int[] GetMass { get { return mass; } }

        public void TestMod() {
            modif.Mod(mass);
            modif.ModInt(t);
        }
    }

    public class Sample {
        public static int saveBufferWidth;
        public static int saveBufferHeight;
        public static int saveWindowHeight;
        public static int saveWindowWidth;
        public static bool saveCursorVisible;

        public static void Go() {
            string m1 = "1) Press the cursor keys to move the console window.\n" +
                "2) Press any key to begin. When you're finished...\n" +
                "3) Press the Escape key to quit.";
            string g1 = "+----";
            string g2 = "|    ";
            string grid1;
            string grid2;
            StringBuilder sbG1 = new StringBuilder();
            StringBuilder sbG2 = new StringBuilder();
            ConsoleKeyInfo cki;
            int y;
            //
            try {
                saveBufferWidth = Console.BufferWidth;
                saveBufferHeight = Console.BufferHeight;
                saveWindowHeight = Console.WindowHeight;
                saveWindowWidth = Console.WindowWidth;
                saveCursorVisible = Console.CursorVisible;
                //
                Console.Clear();
                Console.WriteLine(m1);
                Console.ReadKey(true);

                // Set the smallest possible window size before setting the buffer size.
                Console.SetWindowSize(1, 1);
                Console.SetBufferSize(80, 80);
                Console.SetWindowSize(40, 20);

                // Create grid lines to fit the buffer. (The buffer width is 80, but
                // this same technique could be used with an arbitrary buffer width.)
                for (y = 0; y < Console.BufferWidth / g1.Length; y++) {
                    sbG1.Append(g1);
                    sbG2.Append(g2);
                }
                sbG1.Append(g1, 0, Console.BufferWidth % g1.Length);
                sbG2.Append(g2, 0, Console.BufferWidth % g2.Length);
                grid1 = sbG1.ToString();
                grid2 = sbG2.ToString();

                Console.CursorVisible = false;
                Console.Clear();
                for (y = 0; y < Console.BufferHeight - 1; y++) {
                    if (y % 3 == 0)
                        Console.Write(grid1);
                    else
                        Console.Write(grid2);
                }

                Console.SetWindowPosition(0, 0);
                do {
                    cki = Console.ReadKey(true);
                    switch (cki.Key) {
                        case ConsoleKey.LeftArrow:
                            if (Console.WindowLeft > 0)
                                Console.SetWindowPosition(
                                        Console.WindowLeft - 1, Console.WindowTop);
                            break;
                        case ConsoleKey.UpArrow:
                            if (Console.WindowTop > 0)
                                Console.SetWindowPosition(
                                        Console.WindowLeft, Console.WindowTop - 1);
                            break;
                        case ConsoleKey.RightArrow:
                            if (Console.WindowLeft < (Console.BufferWidth - Console.WindowWidth))
                                Console.SetWindowPosition(
                                        Console.WindowLeft + 1, Console.WindowTop);
                            break;
                        case ConsoleKey.DownArrow:
                            if (Console.WindowTop < (Console.BufferHeight - Console.WindowHeight))
                                Console.SetWindowPosition(
                                        Console.WindowLeft, Console.WindowTop + 1);
                            break;
                    }
                }
                while (cki.Key != ConsoleKey.Escape);  // end do-while
            } // end try
            catch (IOException e) {
                Console.WriteLine(e.Message);
            }
            finally {
                Console.Clear();
                Console.SetWindowSize(1, 1);
                Console.SetBufferSize(saveBufferWidth, saveBufferHeight);
                Console.SetWindowSize(saveWindowWidth, saveWindowHeight);
                Console.CursorVisible = saveCursorVisible;
            }
        }
    }

    public static class TableDrawer {
        public static void Draw(FieldItem[,] table, int height, int width) {
            for (int i = 0; i < height; i++) {
                for (int j = 0; j < width; j++) {
                    Console.Write(table[i, j] is BorderItem ? "#" : " ");
                }
                Console.WriteLine();
            }
        }
    }

    class Program {
        static void Main(string[] args) {
            //Sample.Go();
            //Console.Write(Console.LargestWindowWidth);
            //Console.Write("\n");
            Console.Write("GO!\n");
            //ConsoleGame snake = new ConsoleGame(11, 11);
            //snake.StartLoop();
            ConsoleGameProcessor pr = new ConsoleGameProcessor();
            pr.Run();
            Console.Write("END!\n");
            Console.Read();
        }
    }
}
