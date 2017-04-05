using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO;
namespace NewMideaProgram
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            if (!System.IO.File.Exists(cMain.AppPath+"\\SystemInfo.txt"))
            {
                frmSys.DataClassToFile(cMain.mSysSet);
            }
            else
            {
                if (!frmSys.DataFileToClass((cMain.AppPath+"\\SystemInfo.txt"), out cMain.mSysSet, true))
                {
                    frmSys.DataClassToFile(cMain.mSysSet);
                }
            }
            cMain.IndexLanguage = 1;
            Application.Run(new frmMain());
        }
    }
}