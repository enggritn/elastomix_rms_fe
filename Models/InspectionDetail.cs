using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WMS_FE.Models
{
    public class InspectionDetailReportDTO
    {
        public string DocumentNo { get; set; }
        public string WHName { get; set; }
        public string RMCode { get; set; }
        public string RMName { get; set; }
        public string InDate { get; set; }
        public string ExpDate { get; set; }
        public string LotNo { get; set; }
        public string Bag { get; set; }
        public string FullBag { get; set; }
        public string Total { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedOn { get; set; }
        public string PickingBag { get; set; }
        public string PickingFullBag { get; set; }
        public string PickingTotal { get; set; }
        public string PickingBinRack { get; set; }
        public string PickingBy { get; set; }
        public string PickingOn { get; set; }
        public string ActionExtendDuration { get; set; }
        public string ActionExpDate { get; set; }
        public string ActionDispose { get; set; }
        public string ApproveBy { get; set; }
        public string ApproveOn { get; set; }
        public string PrintLabelBy { get; set; }
        public string PrintLabelOn { get; set; }
        public string PutawayBag { get; set; }
        public string PutawayFullBag { get; set; }
        public string PutawayTotal { get; set; }
        public string PutawayBinRack { get; set; }
        public string PutawayBy { get; set; }
        public string PutawayOn { get; set; }
        public string Status { get; set; }
        public string Memo { get; set; }
    }

    public class ShelfLifeExtensionReportDTO
    {
        public string RMCode { get; set; }
        public string RMName { get; set; }
        public string InDate { get; set; }
        public string LotNo { get; set; }
        public string Qty { get; set; }
        public string ExpiredDate { get; set; }
        public string Extension { get; set; }
        public string Remark { get; set; }
        public string ShelfLifeBaseOnCOA { get; set; }
        public string Note { get; set; }
        public string InspectedOn { get; set; }
    }
}