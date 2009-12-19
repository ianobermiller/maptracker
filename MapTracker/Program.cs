using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace MapTracker
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            bool useHookProxy = args.Length > 0 && args[0].Equals("--useHookProxy", StringComparison.InvariantCultureIgnoreCase);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm(useHookProxy));
        }
    }
}
