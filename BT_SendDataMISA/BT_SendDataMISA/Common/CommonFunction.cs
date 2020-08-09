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
                    ToDate = endDate.ToString("yyyy-MM-dd HH:mm:ss")
                };
                listStartEndDateOYear.Add(startEndDateOfMonth);
            }
            return listStartEndDateOYear;
        }

        public static string GetStartDateMisa(out string startDate)
        {
            startDate = null;
            string msg = Exec.ExecQueryStringOne("SELECT OptionValue StartDate FROM DBOption WHERE OptionID = 'DBStartDate'", out StartDateDbOption startDateDbOption);
            if (msg.Length > 0) startDate = new DateTime(DateTime.Now.Year, 1, 1).ToString("yyyy-MM-dd HH:mm:ss");
            if (string.IsNullOrEmpty(startDateDbOption.StartDate)) return "Không Get được thông tin DBStartDate Cơ sở dữ liệu MISA";

            DateTime startDateConvert = Convert.ToDateTime(startDateDbOption.StartDate);
            startDate = startDateConvert.ToString("yyyy-MM-dd HH:mm:ss");

            return "";
        }
    }
}
