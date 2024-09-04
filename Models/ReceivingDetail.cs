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

    public class ReceivingDetailReportDTO
    {
        public string ID { get; set; }
        public string SourceName { get; set; }
        public string DocumentNo { get; set; }
        public string MaterialCode { get; set; }
        public string MaterialName { get; set; }
        public string InDate { get; set; }
        public string ExpDate { get; set; }
        public string QtyPerBag { get; set; }
        public string BagQty { get; set; }
        public string Qty { get; set; }
        public string DoNo { get; set; }
        public string LotNo { get; set; }
        public string LastSeries { get; set; }
        public string OKQty { get; set; }
        public string NGQty { get; set; }
        public string ReceivedBy { get; set; }
        public DateTime ReceivedOn { get; set; }
    }
    public class ReceivingDetailReportDTO2
    {
        public string DestinationName { get; set; }
        public string RefNumber { get; set; }
        public string SourceCode { get; set; }
        public string SourceType { get; set; }
        public string SourceName { get; set; }
        public string MaterialCode { get; set; }
        public string MaterialName { get; set; }
        public string InDate { get; set; }
        public string ExpDate { get; set; }
        public string LotNo { get; set; }
        public string PerBag { get; set; }
        public string FullBag { get; set; }
        public string Total { get; set; }
        public string Area { get; set; }
        public string RackNo { get; set; }
        public string DoNo { get; set; }
        public string ATA { get; set; }
        public string TransactionStatus { get; set; }
    }
    public class ReceivingDetailReportDTO3
    {
        public string RefNumber { get; set; }
        public string SourceCode { get; set; }
        public string SourceType { get; set; }
        public string SourceName { get; set; }
        public string MaterialCode { get; set; }
        public string MaterialName { get; set; }
        public string Schedule { get; set; }
        public string TotalQtyPo { get; set; }
        public string InDate { get; set; }
        public string ExpDate { get; set; }
        public string LotNo { get; set; }
        public string QtyPerBag { get; set; }
        public string QtyBag { get; set; }
        public string Total { get; set; }
        public string DoNo { get; set; }
        public string OK { get; set; }
        public string NgDamage { get; set; }
        public string NgWet { get; set; }
        public string NgContamination { get; set; }
        public string COA { get; set; }
        public string StatusPo { get; set; }
        public string ReceivedBy { get; set; }
        public DateTime ReceivedOn { get; set; }
        public string QtyPutaway { get; set; }
        public string Area { get; set; }
        public string RackNo { get; set; }
        public string Status { get; set; }
        public string Remarks { get; set; }
    }
}