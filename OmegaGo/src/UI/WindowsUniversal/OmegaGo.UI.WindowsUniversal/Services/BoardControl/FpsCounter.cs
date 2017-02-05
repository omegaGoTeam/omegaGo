using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI;
using Microsoft.Graphics.Canvas.UI.Xaml;

namespace OmegaGo.UI.WindowsUniversal.Services.BoardControl
{
    class FpsCounter
    {
        private string fpsString = "";
        private int framesSinceLastSecond;
        private DateTime lastFpsEmit = DateTime.Now;

        public void Draw(CanvasAnimatedDrawEventArgs args, Rect rect)
        {
            args.DrawingSession.FillRectangle(rect, Colors.CornflowerBlue);
            args.DrawingSession.DrawText(fpsString, new Vector2((int)rect.X + 5, (int)rect.Y + 2), Colors.Black);
            framesSinceLastSecond++;
            if ((DateTime.Now - lastFpsEmit) > TimeSpan.FromSeconds(0.5))
            {
                fpsString = (framesSinceLastSecond*2) + " FPS";
                framesSinceLastSecond = 0;
                lastFpsEmit = DateTime.Now;
            }
        }
    }
}
