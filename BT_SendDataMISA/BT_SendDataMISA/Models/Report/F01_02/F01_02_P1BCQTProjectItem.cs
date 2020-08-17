using System;

namespace BT_SendDataMISA.Models.Report.F01_02
{
    public class F01_02_P1BCQTProjectItem
    {
        public string ProjectID { get; set; }
        public string ProjectNumber { get; set; }
        public string ProjectName { get; set; }
        public string ProgramName { get; set; }
        public int ObjectType { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime FinishDate { get; set; }
        public string ExecutionUnit { get; set; }
        public string ParentID { get; set; }
        public int IsDetail { get; set; }
    }
}
