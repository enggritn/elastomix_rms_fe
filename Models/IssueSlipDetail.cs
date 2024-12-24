using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WMS_FE.Models
{
    public class IssueSlipDTOReport
    {
        public string ID_Header { get; set; }
        public string ID_Order { get; set; }
        public string Header_Code { get; set; }
        public string Header_Name { get; set; }
        public string Header_ProductionDate { get; set; }
        public string RM_Code { get; set; }
        public string RM_Name { get; set; }
        public string RM_VendorName { get; set; }
        public string Wt_Request { get; set; }
        public string SupplyQty { get; set; }
        public string FromBinRackCode { get; set; }
        public string ExpDate { get; set; }
        public string PickedBy { get; set; }
        public string ReturnQty { get; set; }
        public string ToBinRackCode { get; set; }
        public string PutBy { get; set; }
    }

    public class DataInOutDTOReport
    {
        public string ItemCode { get; set; }
        public string Date { get; set; }
        public string UserHanheld { get; set; }
        public string Type { get; set; }
        public string ReceiveQty { get; set; }
        public string IssueSlipQty { get; set; }
        public string BalanceQty { get; set; }
    }

    public class ListTransactionDTOReport
    {
        public string RMCode { get; set; }
        public string RMName { get; set; }
        public string WHName { get; set; }
        public string InOut { get; set; }
        public string TransactionDate { get; set; }
        public string InQty { get; set; }
        public string OutQty { get; set; }
        public string InventoryQty { get; set; }
        public string InOutType { get; set; }
        public string CreateBy { get; set; }
        public string CreateOn { get; set; }
    }
}