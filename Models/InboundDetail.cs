using System;
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

}