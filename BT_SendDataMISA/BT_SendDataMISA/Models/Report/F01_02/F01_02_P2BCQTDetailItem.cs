using System;

namespace BT_SendDataMISA.Models.Report.F01_02
{
    public class F01_02_P2BCQTDetailItem
    {
        public Guid RefID { get; set; }
        public string ProjectID { get; set; }
        public int MethodDistributeID { get; set; }
        public string BudgetSourceID { get; set; }
        public string BudgetKindItemID { get; set; }
        public string BudgetSubKindItemID { get; set; }
        public string BudgetSubKindItemName { get; set; }
        public string BudgetItemID { get; set; }
        public string BudgetItemName { get; set; }
        public string BudgetSubItemID { get; set; }
        public string BudgetSubItemName { get; set; }
        public decimal TotalAmountApproved { get; set; }
        public decimal Amount { get; set; }
        public decimal AidAmount { get; set; }
        public decimal DebitAmount { get; set; }
        public decimal RegularAmount { get; set; }
        public decimal IrRegularAmount { get; set; }
        public decimal AccAmount { get; set; }
        public decimal AccAidAmount { get; set; }
        public decimal AccDebitAmount { get; set; }
        public decimal AccRegularAmount { get; set; }
        public decimal AccIrRegularAmount { get; set; }
    }
}
