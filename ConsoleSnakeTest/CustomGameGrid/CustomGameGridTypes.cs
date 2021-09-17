namespace ConsoleSnake {
    public static class CustomGameGridTypes {
        public static CustomGameGrid TypeA => new CustomGameGrid(CustomGameGridTypeA.Height, CustomGameGridTypeA.Width, CustomGameGridTypeA.Grid, CustomGameGridTypeA.Borderless,
            CustomGameGridTypeA.HeadX, CustomGameGridTypeA.HeadY, CustomGameGridTypeA.HeadDirection, CustomGameGridTypeA.PortalBorders);
        public static CustomGameGrid TypeB => new CustomGameGrid(CustomGameGridTypeB.Height, CustomGameGridTypeB.Width, CustomGameGridTypeB.Grid, CustomGameGridTypeA.Borderless,
            CustomGameGridTypeB.HeadX, CustomGameGridTypeB.HeadY, CustomGameGridTypeB.HeadDirection, CustomGameGridTypeB.PortalBorders);
        public static CustomGameGrid TypeC => new CustomGameGrid(CustomGameGridTypeC.Height, CustomGameGridTypeC.Width, CustomGameGridTypeC.Grid, CustomGameGridTypeA.Borderless,
            CustomGameGridTypeC.HeadX, CustomGameGridTypeC.HeadY, CustomGameGridTypeC.HeadDirection, CustomGameGridTypeC.PortalBorders);
    }
}
