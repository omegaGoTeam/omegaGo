using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI.Xaml;

namespace OmegaGo.UI.WindowsUniversal.Services.BoardControl
{
    /// <summary>
    /// Shows how many frames per seconds we have. Updates twice per second.
    /// </summary>
    class FpsCounter
    {
        private string fpsString = "";
        private int framesSinceLastSecond;
        private DateTime lastFpsEmit = DateTime.Now;

        /// <summary>
        /// Draws the FPS counter on a <see cref="CanvasAnimatedControl"/>. 
        /// </summary>
        /// <param name="session">We use this to draw.</param>
        /// <param name="rect">The rectangle (size about 100x35) where this component should display.</param>
        public void Draw(CanvasDrawingSession session, Rect rect)
        {
            session.FillRectangle(rect, Colors.CornflowerBlue);
            session.DrawText(fpsString, new Vector2((int)rect.X + 5, (int)rect.Y + 2), Colors.Black);
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
