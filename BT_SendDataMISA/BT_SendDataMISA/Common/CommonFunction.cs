using BT_SendDataMISA.Common;
using BT_SendDataMISA.Models;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace BT_SendDataMISA.Function
{
    public static class CommonFunction
    {
        public static List<StartEndDateOfMonth> GetStartEndDateAllMonthInYear()
        {
            List<StartEndDateOfMonth> listStartEndDateOYear = new List<StartEndDateOfMonth>();
            for (int month = 1; month <= 12; month++)
            {
                var startDate = new DateTime(DateTime.Now.Year, month, 1);
                var endDate = startDate.AddMonths(1).AddDays(-1);
                StartEndDateOfMonth startEndDateOfMonth = new StartEndDateOfMonth
                {
                    FromDate = startDate.ToString("yyyy-MM-dd HH:mm:ss"),
                    ToDate = endDate.ToString("yyyy-MM-dd HH:mm:ss"),
                    Month = month,
                    Year = DateTime.Now.Year
                };
                listStartEndDateOYear.Add(startEndDateOfMonth);
            }
            return listStartEndDateOYear;
        }

        public static string GetDBInfoMisa(out DbMisaInfo oMisaInfo)
        {
            string msg = Exec.ExecQueryStringOne(string.Format(@"SELECT 
                                                                (SELECT OptionValue FROM dbo.DBOption WHERE OptionID = 'CompanyCode') AS CompanyID ,
                                                                (SELECT  OptionValue FROM dbo.DBOption WHERE  OptionID = 'CompanyName') AS CompanyName,
                                                                (SELECT Version FROM DBInfo) AS ExportVersion,
                                                                0 AS ParticularID,
                                                                (SELECT Application + ' ' + Version FROM DBInfo) AS ProductID,
                                                                (SELECT MISAVersionControl FROM DBInfo) AS Version,
                                                                (SELECT OptionValue FROM dbo.DBOption WHERE OptionID='DefaultBudgetChapterCode') AS BudgetChapterID,
                                                                (SELECT OptionValue FROM dbo.DBOption WHERE OptionID='DefaultBudgetKindItemCode') AS BudgetKindItemID, 
                                                                (SELECT OptionValue FROM dbo.DBOption WHERE OptionID='DefaultBudgetSubKindItemCode') AS BudgetSubKindItemID,
                                                                (SELECT OptionValue FROM DBOption WHERE OptionID = 'DBStartDate') AS StartDate,
                                                                (SELECT OptionValue FROM DBOption WHERE OptionID = 'AccountSystem') AS AccountSystem"), out oMisaInfo);

            if (msg.Length > 0) return Msg.Exec_GetDBInfoMisa_Err;
            if (string.IsNullOrEmpty(oMisaInfo.StartDate)) oMisaInfo.StartDate = new DateTime(DateTime.Now.Year, 1, 1).ToString("yyyy-MM-dd HH:mm:ss");

            DateTime startDateConvert = Convert.ToDateTime(oMisaInfo.StartDate);
            oMisaInfo.StartDate = startDateConvert.ToString("yyyy-MM-dd HH:mm:ss");

            return "";
        }
    }
}
