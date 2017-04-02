using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
namespace FormsPrototype
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            OmegaGo.Core.AI.AISystems.RegisterFuegoBuilder(new FormsFuego.Win32FuegoBuilder());
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            // The IgsForm connects to IGS and allows playing local games.
            // The KgsForm connects to KGS only.
            // There is no hub from where you can access both, so it's in this source file that you pick what you want to prototype now.
            Application.Run(new KgsForm ());
            
        }
    }
}
