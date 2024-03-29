﻿using System;
using ConsoleMenuAPI;

namespace ConsoleSnake {
    public static class ConsoleSnakeGameProcessor {
        static public void Run() {
            MenuEndResult endscreenResult = MenuEndResult.Further;
            SnakeLocalization.SetRusLocalizationDictionary();
            MainMenu menu = new MainMenu();
            while (endscreenResult == MenuEndResult.Further && menu.ShowDialog() == MenuEndResult.Further) {
                EndscreenMenu endMenu = new EndscreenMenu();
                do {
                    var result = menu.Settings;
                    ConsoleSnakeGame snakeGame = result.IsCustomGrid ? new ConsoleSnakeGame(SettingsMenu.GetCustomGrid(result.CustomGridType.Value), result.SnakeSpeed)
                        : new ConsoleSnakeGame(result.Height, result.Width, result.Borderless, result.PortalBorders, result.BigFood, result.SnakeSpeed);
                    snakeGame.StartLoop();
                    endMenu.GameResults = snakeGame.Results;
                    endscreenResult = endMenu.ShowDialog();
                } while (endMenu.EndResult == MenuEndResult.Further && endMenu.Restart);
            }
        }
    }
}
