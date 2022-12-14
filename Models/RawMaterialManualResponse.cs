using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WMS_FE.Models
{
    public class RawMaterialManualResponse
    {
        public ManualStockViewModel data { get; set; }
        public bool status { get; set; }
        public string message { get; set; }
    }
}