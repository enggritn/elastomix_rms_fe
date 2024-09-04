using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WMS_FE.Models
{
    public class ReceivingResponse
    {
        public Receiving data { get; set; }
        public ReceivingDetailDTO datadetail { get; set; }
        public bool status { get; set; }
        public string message { get; set; }
        public List<ReceivingDetailReportDTO> list { get; set; }
        public List<ReceivingDetailReportDTO2> list2 { get; set; }
        public List<ReceivingDetailReportDTO3> list3 { get; set; }
    }
}