using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WMS_FE.Models
{
    public class InspectionResponse
    {
        public bool status { get; set; }
        public string message { get; set; }
        public List<InspectionDetailReportDTO> list { get; set; }
        public List<ShelfLifeExtensionReportDTO> list2 { get; set; }
    }
}