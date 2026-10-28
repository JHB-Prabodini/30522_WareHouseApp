using System;
using System.Windows.Forms;
using _30522_WareHouseApp.Forms;

namespace _30522_WareHouseApp
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Sets the starting form to LoginForm under the Forms namespace
            Application.Run(new LoginForm());
        }
    }
}