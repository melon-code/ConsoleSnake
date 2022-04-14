using System;

namespace ConsoleSnake {
    public class Renderer {
        System.Windows.Forms.Timer timer;

        public Renderer(int timeBetween, Action renderFunc) {
            timer = new System.Windows.Forms.Timer();
            timer.Interval = timeBetween;
            timer.Tick += new EventHandler((s, e) => { renderFunc(); });
        }

        public void Start() {
            timer.Start();
        }

        public void Stop() {
            timer.Stop();
        }
    }
}
