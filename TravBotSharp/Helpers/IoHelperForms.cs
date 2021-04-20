﻿using System;
using System.IO;
using System.Windows.Forms;

namespace TravBotSharp.Files.Helpers
{
    public static class IoHelperForms
    {
        public static void ExportBuildTasks(string tasks)
        {
            var svd = new SaveFileDialog();
            svd.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory + "data\\";
            svd.Filter = "tbs files (*.tbs)|*.tbs|All files (*.*)|*.*";
            svd.FilterIndex = 1;
            svd.RestoreDirectory = true;
            svd.FileName = "myBuildTasks";

            if (svd.ShowDialog() == DialogResult.OK) File.WriteAllText(svd.FileName, tasks);
        }

        public static string PromptUserForBuidTasksLocation()
        {
            using (var ofd = new OpenFileDialog())
            {
                var initFolder = AppDomain.CurrentDomain.BaseDirectory;
                ofd.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory + "data\\";
                ofd.Filter = "TBS or TRBC files (*.tbs;*.trbc)|*.tbs;*.trbc";
                ofd.FilterIndex = 1;
                ofd.RestoreDirectory = true;

                if (ofd.ShowDialog() == DialogResult.OK)
                    //Get the path of specified file
                    return ofd.FileName;
            }

            return null;
        }

        public static bool AlertUser(string msg)
        {
            var alert = new AlertForm(msg);
            alert.ShowDialog();
            return true;
        }
    }
}