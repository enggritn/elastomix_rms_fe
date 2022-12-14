using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WMS_FE.Models
{
    public class Receiving
    {
        public string MaterialCode { get; set; }
        public string MaterialName { get; set; }
        public string RawMaterialMaker { get; set; }
        public string Barcode { get; set; }
        public string LotNo { get; set; }
        public string InDate { get; set; }
        public string ExpDate { get; set; }
        public string QtyPerBag { get; set; }
        public string BagQty { get; set; }
        public string Qty { get; set; }
        public string UoM { get; set; }
        public string StartSeries { get; set; }
    }
}