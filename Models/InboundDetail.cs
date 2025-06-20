﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WMS_FE.Models
{
    public class OtherInboundDetailReportDTO
    {
        public string ReceiptDate { get; set; }
        public string ReceiptNo { get; set; }
        public string WarehouseCode { get; set; }
        public string WarehouseName { get; set; }
        public string MaterialCode { get; set; }
        public string MaterialName { get; set; }
        public string Uom { get; set; }
        public string QtyL { get; set; }
        public string Qty { get; set; }
        public string Memo { get; set; }
    }
    public class OtherInbound2DetailReportDTO
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
        public string QtyPutaway { get; set; }
        public string PutawayBy { get; set; }
        public string PutawayOn { get; set; }
        public string BinRack { get; set; }
        public string Status { get; set; }
        public string Memo { get; set; }
    }

}