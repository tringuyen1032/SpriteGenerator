using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace SpriteGenerator
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new SpritesForm());
            if (args.Length == 1 && args[0] == "INSTALLER") { System.Diagnostics.Process.Start(Application.ExecutablePath); return; }
        }
    }
}
