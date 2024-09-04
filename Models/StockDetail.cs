using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WMS_FE.Models
{
    public class StockDetail
    {
        public string WarehouseCode { get; set; }
        public string WarehouseName { get; set; }
        public string BinRackAreaName { get; set; }
        public string BinRackName { get; set; }
        public string MaterialCode { get; set; }
        public string MaterialName { get; set; }
        public string LotNo { get; set; }
        public string InDate { get; set; }
        public string ExpDate { get; set; }
        public string BagQty { get; set; }
        public string QtyPerBag { get; set; }
        public string TotalQty { get; set; }
        public string IsExpired { get; set; }
    }
}