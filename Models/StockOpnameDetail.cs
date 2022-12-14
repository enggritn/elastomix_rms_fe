using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WMS_FE.Models
{
    public class StockOpnameDetailDTO
    {
        public string ID { get; set; }
        public string BinRackCode { get; set; }
        public string MaterialCode { get; set; }
        public string MaterialName { get; set; }
        public string LotNo { get; set; }
        public DateTime InDate { get; set; }
        public DateTime ExpDate { get; set; }
        public string BagQty { get; set; }
        public string QtyPerBag { get; set; }
        public string TotalQty { get; set; }
        public string MaterialType { get; set; }
        public string ScannedBy { get; set; }
        public DateTime ScannedOn { get; set; }
    }
}