using System;
using Keystroke.API;
using System.Windows.Forms;

namespace ConsoleSnake {
    public class ConsoleSnakeGame {
        const int defaultSnakeSpeed = 6;

        readonly int snakeSpeed;
        Field gameField;
        ConsoleSnakeDrawer drawer;

        public bool AnyKeyPressed { get; private set; }
        public SnakeGameStats Results { get { return new SnakeGameStats(gameField.State == GameState.Win ? true : false, gameField.SnakeLenght); } }

        ConsoleSnakeGame(int speed, Func<Field> createField) {
            snakeSpeed = 1100 - speed * 100;
            gameField = createField();
            drawer = new ConsoleSnakeDrawer(gameField);
        }

        public ConsoleSnakeGame(int height, int width, bool borderless, bool portalBorders, bool enableBigFood, int speed) :
            this(speed, () => Field.CreateField(height, width, borderless, portalBorders, enableBigFood)) {
        }

        public ConsoleSnakeGame(int height, int width, bool borderless, bool portalBorders, int bigFoodInterval, int speed) :
            this(speed, () => Field.CreateField(height, width, borderless, portalBorders, bigFoodInterval)) {
        }

        public ConsoleSnakeGame(int height, int width, bool borderless, bool portalBorders, int speed) : this(height, width, borderless, portalBorders, false, defaultSnakeSpeed) {
        }

        public ConsoleSnakeGame(int height, int width) : this(height, width, false, false, defaultSnakeSpeed) {
        }

        public ConsoleSnakeGame(int height, int width, bool portalBorders) : this(height, width, false, portalBorders, defaultSnakeSpeed) {
        }

        public ConsoleSnakeGame(int height, int width, bool portalBorders, int speed) : this(height, width, false, portalBorders, speed) {
        }

        public ConsoleSnakeGame(int height, int width, bool borderless, bool portalBorders) : this(height, width, borderless, portalBorders, defaultSnakeSpeed) {
        }

        public ConsoleSnakeGame(CustomGameGrid gameGrid, int speed) : this(speed, () => Field.CreateField(gameGrid)) {
        }

        void RenderFrame() {
            gameField.Iterate();
            drawer.DrawGameField();
            if (gameField.State != GameState.InProgress)
                Application.Exit();
        }

        public void StartLoop() {
            Renderer rend = new Renderer(snakeSpeed, RenderFrame);
            drawer.SetConsoleWindow();
            drawer.DrawGameField();
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
}
