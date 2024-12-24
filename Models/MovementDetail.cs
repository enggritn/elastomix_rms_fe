using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WMS_FE.Models
{
    public class MovementDetailReportDTO
    {
        public string WHName { get; set; }
        public string RMCode { get; set; }
        public string RMName { get; set; }
        public string InDate { get; set; }
        public string ExpDate { get; set; }
        public string LotNo { get; set; }
        public string OriginBag { get; set; }
        public string OriginFullBag { get; set; }
        public string OriginTotal { get; set; }
        public string OriginBinRack { get; set; }
        public string PutawayBy { get; set; }
        public string PutawayOn { get; set; }
        public string DestinationBag { get; set; }
        public string DestinationFullBag { get; set; }
        public string DestinationTotal { get; set; }
        public string DestinationBinRack { get; set; }
        public string Status { get; set; }
        public string Memo { get; set; }
    }

}