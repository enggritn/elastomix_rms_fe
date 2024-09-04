using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WMS_FE.Models
{
    public class InboundResponse
    {
        public bool status { get; set; }
        public string message { get; set; }
        public List<OtherInboundDetailReportDTO> list { get; set; }
    }
}