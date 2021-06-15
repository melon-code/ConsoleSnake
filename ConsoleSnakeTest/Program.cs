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
            customGameGrid.CopyTo(gameGrid, 0);
            FieldItemType borderMark = borderless ? FieldItemType.Empty : FieldItemType.Border;
            for (int i = 0; i < Height; i++)
                for (int j = 0; j < Width; j++) {
                    if (gameGrid[i, j] != null) {
                        if (IsBorder(i, j) && gameGrid[i, j].Type != borderMark)
                            gameGrid[i, j] = GetBorderMark(borderless);
                    }
                    else
                        gameGrid[i, j] = new EmptyItem();
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

        public static Field CreateField(GameGrid customGameGrid, int initialSnakeHeadX, int initialSnakeHeadY, Direction initialSnakeDirection) {
            return new Field(customGameGrid, initialSnakeHeadX, initialSnakeHeadY, initialSnakeDirection);
        }

        const int smallFoodValue = 1;
        const int borderWidth = 1;

        protected Snake snake;
        Random rand;
        Direction currentlySetDirection = Direction.Right;

        public GameGrid Grid { get; protected set; }

        protected virtual int BorderWidth => borderWidth;
        protected virtual int PlayableArea { get { return Height * Width - 2 * Height - 2 * Width + 4; } }
        protected bool IsGameOver { get; set; } = false;
        public virtual bool Borderless => false;
        protected bool Win { get { return snake.Length == PlayableArea; } }
        public bool PortalBorders => snake is PortalSnake; //???
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
            snake = allowPortalBorders ? new PortalSnake(Height / 2, Width / 2, Height - BorderWidth, Width - BorderWidth, BorderWidth) : new Snake(Height / 2, Width / 2);
            PrepareForStart();
        }

        public Field(int height, int width) : this(height, width, false) {
        }

        public Field(int h, int w, int startLength) : this(h, w) {
            //todo
        }

        public Field(GameGrid customGameGrid, int initialSnakeHeadX, int initialSnakeHeadY, Direction initialSnakeDirection) {
            Grid = customGameGrid ?? throw new ArgumentNullException("Grid", "CustomGameGrid can not be null");
            snake = new Snake(initialSnakeHeadX, initialSnakeHeadY, initialSnakeDirection);
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
            if (drawingField.Borderless) { // WTF
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

    public class ConsoleGame { //not abstract from drawing
        const int defaultSnakeSpeed = 6;

        readonly int snakeSpeed;
        Field gameField;
        ConsoleSnakeDrawer drawer;
        bool isFirstTurn = true;

        public bool AnyKeyPressed { get; private set; }
        public SnakeGameStats Results { get { return new SnakeGameStats(gameField.State == GameState.Win ? true : false, gameField.SnakeLenght); } }

        public ConsoleGame(int height, int width, bool borderless, bool portalBorders, int speed) {
            snakeSpeed = 1100 - speed * 100;
            gameField = Field.CreateField(height, width, borderless, portalBorders);
            drawer = new ConsoleSnakeDrawer(gameField);
        }

        public ConsoleGame(int height, int width) : this(height, width, false, false, defaultSnakeSpeed) {
        }

        public ConsoleGame(int height, int width, bool portalBorders) : this(height, width, false, portalBorders, defaultSnakeSpeed) {
        }

        public ConsoleGame(int height, int width, bool portalBorders, int speed) : this(height, width, false, portalBorders, speed) {
        }

        public ConsoleGame(int height, int width, bool borderless, bool portalBorders) : this(height, width, borderless, portalBorders, defaultSnakeSpeed) {
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
            Renderer rend = new Renderer(snakeSpeed, RenderFrame); // time here
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
                    ProcessInput(info.Key);
                    break;
                case ConsoleKey.Escape:
                    EndResult = MenuEndResult.Exit;
                    return false;
                default:
                    ProcessInput(info.Key);
                    break;
            }
            return true;
        }

        public abstract void ProcessInput(ConsoleKey input);
    }

    public interface IMenuItem {
        bool Visible { get; set; }
        string Name { get; }

        void Draw();
        void ProcessInput(ConsoleKey input);
    }

    public interface IMenuValueItem<T> : IMenuItem {
        T Value { get; }
    }

    public class MenuItem : IMenuItem {
        public bool Visible { get; set; } = true;
        public string Name { get; }

        public MenuItem(string name) {
            Name = name;
        }

        public void Draw() {
            Console.Write("\t" + Name + "\n\n");
        }

        public void ProcessInput(ConsoleKey input) {
            //mb add inside menus
        }
    }

    public class ValueBasedItem<T> {
        public bool Visible { get; set; } = true;
        public string Name { get; }
        public T Value { get; protected set; }

        public ValueBasedItem(string name, T defaultValue) {
            Name = name;
            Value = defaultValue;
        }
    }

    public class BoolMenuItem : ValueBasedItem<bool>, IMenuValueItem<bool> {
        const string onTitle = "Да";
        const string offTitle = "Нет";

        public BoolMenuItem(string name, bool defaultValue) : base(name, defaultValue) {
        }

        public void Draw() {
            Console.WriteLine("\t" + Name + string.Format(" < {0} >", Value ? onTitle : offTitle) + "\n");
        }

        public virtual void ChangeValue() {
            Value = !Value;
        }

        public void ProcessInput(ConsoleKey input) {
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


        public void Draw() {
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
                Console.Write("\tВведите числовое значение: ");
                input = Console.ReadLine();
                if (ValidateStringInput(input) && ValidateInteger(Convert.ToInt32(input)))
                    isInputValid = true;
            } while (!isInputValid);
        }

        public void ProcessInput(ConsoleKey input) {
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
        const string exitString = "Выход";
        const int heightIndex = 0;
        const int widthIndex = 1;
        const int bigFoodIndex = 2;
        const int portalBorderIndex = 3;
        const int snakeSpeedIndex = 4;

        public int Height => GetInt(heightIndex);
        public int Width => GetInt(widthIndex);
        public bool BigFood => GetBool(bigFoodIndex);
        public bool PortalBorders => GetBool(portalBorderIndex);
        public int SnakeSpeed => GetInt(snakeSpeedIndex);

        public SettingsMenu(IList<IMenuItem> settingsItems) : base(settingsItems, exitString) {
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

        public override void ProcessInput(ConsoleKey input) {
            Items[CurrentPosition].ProcessInput(input);
        }
    }

    public struct SettingsResult {
        public int Height { get; }
        public int Width { get; }
        public bool BigFood { get; }
        public bool PortalBorders { get; }
        public int SnakeSpeed { get; }

        public SettingsResult(int height, int width, bool bigFood, bool portalBorders, int snakeSpeed) {
            Height = height;
            Width = width;
            BigFood = bigFood;
            PortalBorders = portalBorders;
            SnakeSpeed = snakeSpeed;
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
            IntMenuItem customTypes = new IntMenuItem("Тип поля", 1, 1, 3);
            BoolMenuItem portalBorders = new BoolMenuItem("Портальные границы", false);
            IList<DependencyItem> dependencies = new List<DependencyItem>() { new DependencyItem(portalBorders, true), new DependencyItem(customTypes) };
            return new List<IMenuItem> { new IntMenuItem("Высота", defaultHeight, minHeight, Console.LargestWindowHeight),
                new IntMenuItem("Ширина", defaultWidth, minWidth, Console.LargestWindowWidth), new BoolMenuItem("Большая еда (не робiт)", false),
                portalBorders, new IntMenuItem("Скорость", defaultSpeed, minSpeed, maxSpeed),
                new DependencyBoolMenuItem("Пользовательское поле", false, dependencies), customTypes };
        }

        public static IList<IMenuItem> GetMainMenuList() {
            return new IMenuItem[] { new MenuItem("Новая игра"), new MenuItem("Настройки"), new MenuItem("Выход") };
        }
    }

    public class MainMenu : ConsoleMenu {
        SettingsMenu sMenu;

        public SettingsResult Settings => new SettingsResult(sMenu.Height, sMenu.Width, sMenu.BigFood, sMenu.PortalBorders, sMenu.SnakeSpeed);

        public MainMenu() : base(ItemsListHelper.GetMainMenuList()) {
            sMenu = new SettingsMenu(ItemsListHelper.GetSettingsMenuList());
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

        public EndscreenMenu() : base(new List<IMenuItem>() { new MenuItem("Еще раз (с теми же параметрами)"), new MenuItem("В начальное меню") }, "Выход") {
        }

        public override void ProcessInput(ConsoleKey input) {
            if (input == ConsoleKey.Enter) {
                Restart = CurrentPosition == 0 ? true : false;
                IsEnd = true;
            }
        }

        protected override void Draw() {
            Console.Clear();
            Console.WriteLine(string.Format("\t{0}!\n", GameResults.Win ? "You Won" : "GAME OVER"));
            Console.WriteLine("\tSnake length is " + GameResults.SnakeLength + "\n");
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
                    //debug
                    Console.WriteLine("Height = " + result.Height);
                    Console.WriteLine("Width = " + result.Width);
                    Console.WriteLine("Big food = " + result.BigFood);
                    Console.WriteLine("Portal borders = " + result.PortalBorders);
                    // snake game starts
                    ConsoleGame snakeGame = new ConsoleGame(result.Height, result.Width, result.PortalBorders, result.SnakeSpeed);
                    snakeGame.StartLoop();
                    //results + end menu
                    endMenu.GameResults = snakeGame.Results;
                    endscreenResult = endMenu.ShowDialog();
                } while (endMenu.EndResult == MenuEndResult.Further && endMenu.Restart);
            }
            Console.Write("\nEXIT!");
        }
    }

    public static class CustomGameGridParser {
        public static GameGrid Parse(int height, int width, string grid) {
            if (height * width != grid.Length)
                return null;
            FieldItem[,] items = new FieldItem[height, width];
            for (int i = 0; i < height; i++)
                for (int j = 0; j < width; j++)
                    items[i, j] = ParseItem(grid[i * j]);
            return new GameGrid(height, width, false, items);
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

        public CustomGameGrid(int height, int width, string grid, int snakeHeadX, int snakeHeadY, Direction snakeHeadDirection) {
            Grid = CustomGameGridParser.Parse(height, width, grid);
            SnakeHeadX = snakeHeadX;
            SnakeHeadY = snakeHeadY;
            SnakeHeadDirection = snakeHeadDirection;
        }
    }

    public static class CustomGameGridTypes {
        public static CustomGameGrid TypeA => new CustomGameGrid(CustomGameGridTypeA.Height, CustomGameGridTypeA.Width, CustomGameGridTypeA.Grid, CustomGameGridTypeA.HeadX, 
            CustomGameGridTypeA.HeadY, CustomGameGridTypeA.HeadDirection);
        public static CustomGameGrid TypeB => new CustomGameGrid(CustomGameGridTypeB.Height, CustomGameGridTypeB.Width, CustomGameGridTypeB.Grid, CustomGameGridTypeB.HeadX,
            CustomGameGridTypeB.HeadY, CustomGameGridTypeB.HeadDirection);
        public static CustomGameGrid TypeC => new CustomGameGrid(CustomGameGridTypeC.Height, CustomGameGridTypeC.Width, CustomGameGridTypeC.Grid, CustomGameGridTypeC.HeadX,
            CustomGameGridTypeC.HeadY, CustomGameGridTypeC.HeadDirection);
    }

    public static class CustomGameGridTypeA {
        public const int Height = 10;
        public const int Width = 12;
        public const int HeadX = Height / 2;
        public const int HeadY = Width / 2;
        public const Direction HeadDirection = Direction.Right;
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
