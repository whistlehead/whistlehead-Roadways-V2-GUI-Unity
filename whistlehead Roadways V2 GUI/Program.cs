using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace whistlehead_Roadways_V2_GUI
{
    static class Program
    {
        [STAThreadAttribute]
        static void Main(string[] args)
        {
            // abort if dupicate program is running
            if (System.Diagnostics.Process.GetProcessesByName(System.IO.Path.GetFileNameWithoutExtension(System.Reflection.Assembly.GetEntryAssembly().Location)).Count() > 1) System.Diagnostics.Process.GetCurrentProcess().Kill();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            LoadingForm.ShowLoadingScreen();
            Form1 form1 = new Form1();
            LoadingForm.CloseForm();
            Application.Run(form1);
        }
    }
}
