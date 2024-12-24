using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WMS_FE.Models
{
    public class TransformDetailReportDTO
    {
        public string DocumentNo { get; set; }
        public string WHName { get; set; }
        public string SourceRMCode { get; set; }
        public string SourceRMName { get; set; }
        public string TransformQty { get; set; }
        public string TargetRMCode { get; set; }
        public string TargetRMName { get; set; }
        public string SourceBinRack { get; set; }
        public string SourceInDate { get; set; }
        public string SourceExpDate { get; set; }
        public string SourceLotNo { get; set; }
        public string SourceBag { get; set; }
        public string SourceFullBag { get; set; }
        public string SourceTotal { get; set; }
        public string PickingBy { get; set; }
        public string PickingOn { get; set; }
        public string TargetBinRack { get; set; }
        public string TargetInDate { get; set; }
        public string TargetExpDate { get; set; }
        public string TargetLotNo { get; set; }
        public string TargetBag { get; set; }
        public string TargetFullBag { get; set; }
        public string TargetTotal { get; set; }
        public string PutawayBy { get; set; }
        public string PutawayOn { get; set; }
        public string Status { get; set; }
        public string Memo { get; set; }
    }

}