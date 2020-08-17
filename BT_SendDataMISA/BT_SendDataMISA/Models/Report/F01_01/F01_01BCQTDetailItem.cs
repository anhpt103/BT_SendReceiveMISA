using System;

namespace BT_SendDataMISA.Models.Report.F01_01
{
    public class F01_01BCQTDetailItem
    {
        public Guid RefID { get; set; }
        public int ExpenseType { get; set; }
        public string BudgetSourceID { get; set; }
        public string BudgetKindItemID { get; set; }
        public string BudgetKindItemName { get; set; }
        public string BudgetSubKindItemID { get; set; }
        public string BudgetSubKindItemName { get; set; }
        public string BudgetItemID { get; set; }
        public string BudgetItemName { get; set; }
        public string BudgetSubItemID { get; set; }
        public string BudgetSubItemName { get; set; }
        public decimal BudgetSourceAmount { get; set; }
        public decimal AidAmount { get; set; }
        public decimal DebitAmount { get; set; }
        public decimal DeductAmount { get; set; }
        public decimal OtherAmount { get; set; }
    }
}
