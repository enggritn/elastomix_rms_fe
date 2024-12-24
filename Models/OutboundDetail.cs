using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WMS_FE.Models
{
    public class OutboundDetailReportDTO
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
        public string CreateBy { get; set; }
        public string CreateOn { get; set; }
        public string PickingBag { get; set; }
        public string PickingFullBag { get; set; }
        public string PickingTotal { get; set; }
        public string PickingBinRack { get; set; }
        public string PickingBy { get; set; }
        public string PickingOn { get; set; }
        public string PutawayBag { get; set; }
        public string PutawayFullBag { get; set; }
        public string PutawayTotal { get; set; }
        public string PutawayBinRack { get; set; }
        public string PutawayBy { get; set; }
        public string PutawayOn { get; set; }
        public string Status { get; set; }
        public string Memo { get; set; }
    }

}