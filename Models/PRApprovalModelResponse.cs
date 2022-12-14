using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WMS_FE.Models
{
    public class PRApprovalModelResponse
    {
        public string PRBoxGroupCode { get; set; }
        public string PRBoxGroupDescription { get; set; }
        public List<PRApprovalDetails> Details { get; set; }
        public int MyProperty { get; set; }
    }

    public class PRApprovalDetails
    {
        public int SequenceNo { get; set; }
        public string PRBoxName { get; set; }
        public string PRBoxTitle { get; set; }
    }
}