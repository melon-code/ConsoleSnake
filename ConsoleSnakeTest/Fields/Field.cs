using System;

namespace ConsoleSnake {
    public class Field {
        public static Field CreateField(int height, int width) {
            return new Field(height, width);
        }

        public static Field CreateField(int height, int width, bool borderless, bool allowPortalBorders, bool enableBigFood) {
            return borderless ? new BorderlessField(height, width, allowPortalBorders, enableBigFood) : new Field(height, width, allowPortalBorders, enableBigFood);
        }

        public static Field CreateField(int height, int width, bool borderless, bool allowPortalBorders, int bigFoodInterval) {
            return borderless ? new BorderlessField(height, width, allowPortalBorders, bigFoodInterval) : new Field(height, width, allowPortalBorders, bigFoodInterval);
        }

        public static Field CreateField(int height, int width, bool borderless, bool allowPortalBorders) {
            return CreateField(height, width, allowPortalBorders, false);
        }

        public static Field CreateField(int height, int width, bool borderless) {
            return CreateField(height, width, borderless, false);
        }

        public static Field CreateField(CustomGameGrid customGrid) {
            return customGrid.Borderless ? new BorderlessField(customGrid.Grid, customGrid.SnakeHeadX, customGrid.SnakeHeadY, customGrid.SnakeHeadDirection, customGrid.PortalBorders)
                : new Field(customGrid.Grid, customGrid.SnakeHeadX, customGrid.SnakeHeadY, customGrid.SnakeHeadDirection, customGrid.PortalBorders);
        }

        public static int GetFoodValue(FieldItem item) {
            if (item is FoodItem food)
                return food.Value;
            return 0;
        }

        const string cggExceptionParamName = "Grid";
        const string cggExceptionMessage = "CustomGameGrid can not be null";
        const int smallFoodValue = 1;
        const int bigFoodValue = 3;
        const int borderWidth = 1;
        const int defaultBigFoodInterval = 10;

        readonly int bigFoodInterval = defaultBigFoodInterval;
        readonly bool bigFoodEnabled;
        bool bigFoodSpawned = false;
        int currentBigFoodInterval;
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

        public Field(int height, int width, bool allowPortalBorders, bool enableBigFood) {
            Grid = new GameGrid(height, width, Borderless);
            snake = allowPortalBorders ? new PortalSnake(Height / 2, Width / 2, LimitX, LimitY, BorderWidth) : new Snake(Height / 2, Width / 2);
            bigFoodEnabled = enableBigFood;
            PrepareForStart();
        }

        public Field(int height, int width, bool allowPortalBorders, int bigFoodInterval) : this(height, width, allowPortalBorders, true) {
            this.bigFoodInterval = bigFoodInterval;
        }

        public Field(int height, int width, bool allowPortalBorders) : this(height, width, allowPortalBorders, false) {
        }

        public Field(int height, int width) : this(height, width, false) {
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
            GenerateSmallFood();
            if (bigFoodEnabled)
                currentBigFoodInterval = bigFoodInterval;
        }

        protected CollisionType IsCollision(int headX, int headY) {
            var item = Grid[headX, headY];
            var collision = item.GetCollision();
            if (collision == CollisionType.Snake)
                return item.GetSnakeItem().BodyPart == SnakeType.Tail ? CollisionType.No : CollisionType.Snake;
            return collision;
        }

        protected virtual CollisionType IsCollision(Point head) {
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

        void MoveSnake() {
            snake.Move();
            Grid.SetNewSnakeHead(snake.Head);
            Grid.RemoveSnakeTail(snake.Tail);
        }

        void MoveAndExtendSnake(int extendValue) {
            snake.Move(extendValue);
            Grid.SetNewSnakeHead(snake.Head);
        }

        protected virtual void ProcessBorderCollision() {
            MoveSnake();
            IsGameOver = true;
        }

        void ProcessFoodCollision(int foodValue) {
            if (foodValue == smallFoodValue) {
                GenerateSmallFood();
                if (bigFoodEnabled && !bigFoodSpawned) {
                    currentBigFoodInterval--;
                    if (currentBigFoodInterval == 0) {
                        GenerateBigFood();
                        bigFoodSpawned = true;
                    }
                }
            }
            else {
                currentBigFoodInterval = bigFoodInterval;
                bigFoodSpawned = false;
            }
        }

        public void Iterate() {
            snake.SetHeadDirection(currentlySetDirection);
            var nextHead = snake.GetNextHead();
            switch (IsCollision(nextHead)) {
                case CollisionType.No:
                    MoveSnake();
                    break;
                case CollisionType.Food:
                    int foodValue = GetFoodValue(Grid[nextHead.X, nextHead.Y]);
                    MoveAndExtendSnake(foodValue);
                    ProcessFoodCollision(foodValue);
                    break;
                case CollisionType.Snake:
                case CollisionType.Border:
                    ProcessBorderCollision();
                    break;
            }
        }

        public void GenerateFood(int foodValue) {
            if (snake.Length < PlayableArea) {
                int x, y;
                do {
                    x = rand.Next(Height);
                    y = rand.Next(Width);
                } while (Grid[x, y].Type != FieldItemType.Empty);
                Grid.AddFood(new Point(x, y), foodValue);
            }
        }

        public void GenerateSmallFood() {
            GenerateFood(smallFoodValue);
        }

        public void GenerateBigFood() {
            GenerateFood(bigFoodValue);
        }
    }
}
