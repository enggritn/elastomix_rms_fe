using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WMS_FE.Models
{
    public class MovementResponse
    {
        public bool status { get; set; }
        public string message { get; set; }
        public List<MovementDetailReportDTO> list { get; set; }
    }
}