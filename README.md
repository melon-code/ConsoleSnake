# Console snake

Console snake game supports:
- customizable game field size (height, width)
- borderless option
- portal border
- food with variable value (e.g. big food)
- customizable snake movement speed
- custom game field layouts

The game has console drawer for displaying game field. The drawer adjusts the console window size according to the game field size.

Implemented console menu for changing settings and starting/restarting the game via [ConsoleMenuAPI](https://github.com/melon-code/ConsoleMenuAPI).

Capturing keyboard input is made with external [KeystrokeAPI](https://github.com/fabriciorissetto/KeystrokeAPI).

## Configuring snake game

Game settings can be set through the `ConsoleSnakeGame` constructor:

`ConsoleSnakeGame(int height, int width, bool borderless, bool portalBorders, bool enableBigFood, int speed)`

`height`

Game field height

`width`

Game field width

`borderless`

Indicates whether the game field has border items.

If true the edge of the console window becomes the visual border of the game field.

If false border size is included in the `height` and `width` parameters.

`portalBorders`

If true the border teleports snake to other side of the game field

`enableBigFood`

Enables spawning of big food

`speed`

Sets snake speed movement from 1 to 10 (1000ms to 100ms)

## Custom game field

Custom game field layout can be set using the following `ConsoleSnakeGame` constructor:

`ConsoleSnakeGame(CustomGameGrid gameGrid, int speed)`

`CustomGameGrid` is a struct that includes custom game field data. Supports all standard game field options. Plus option for initial snake position and movement direction.

Layout data itself stores as a `string` value where `'B'` is border item and `' '`(empty space) is empty item.
