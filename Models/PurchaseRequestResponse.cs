using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WMS_FE.Models
{
    public class MaterialRequestResponse
    {
        public MaterialRequestHeaderDTO data { get; set; }
        public bool status { get; set; }
        public string message { get; set; }
    }
}