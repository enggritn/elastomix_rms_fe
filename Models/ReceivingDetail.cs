using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WMS_FE.Models
{
    public class ReceivingDetailDTO
    {
        public string ID { get; set; }
        public string HeaderID { get; set; }
        public string StockCode { get; set; }
        public string DoNo { get; set; }
        public string LotNo { get; set; }
        public string InDate { get; set; }
        public string ExpDate { get; set; }
        public string QtyPerBag { get; set; }
        public string Qty { get; set; }
        public string UoM { get; set; }
        public string COA { get; set; }
        public string LastSeries { get; set; }
        public string FotoCOA { get; set; }
        public string LastFotoCOA { get; set; }
        public List<RequestDetailFotoDTO> Details { get; set; }
    }

    public class RequestDetailFotoDTO
    {
        public string ID { get; set; }
        public string URL { get; set; }
    }
}