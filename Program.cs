// language: C#, file: Program.cs, target: net48
using System;
using System.Windows.Forms;

namespace RobloxDirect
{
    internal static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }

    }
}