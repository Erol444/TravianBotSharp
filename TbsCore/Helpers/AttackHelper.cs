using System;
using System.Collections.Generic;
using System.Text;
using TbsCore.Parsers;

namespace TbsCore.Helpers
{
    public static class AttackHelper
    {
        public static Classificator.ReportType GetReportType(string iReport)
        {
            int num = (int)Parser.RemoveNonNumeric(iReport);
            return (Classificator.ReportType)num;
        }
    }
}
