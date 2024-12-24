using System.Collections.Generic;

namespace WMS_FE.Models
{
    public class IssueSlipResponse
    {
        public bool status { get; set; }
        public string message { get; set; }
        public List<IssueSlipDTOReport> list { get; set; }
        public List<ListTransactionDTOReport> list2 { get; set; }
    }
}