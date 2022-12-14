using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WMS_FE.Models
{
    public class MaterialRequestHeaderDTO
    {
        public string ID { get; set; }
        public string Code { get; set; }
        public string RefNumber { get; set; }
        public string SourceType { get; set; }
        public string SourceID { get; set; }
        public string SourceCode { get; set; }
        public string SourceName { get; set; }
        public string SourceAddress { get; set; }
        public string DestinationID { get; set; }
        public string DestinationCode { get; set; }
        public string DestinationName { get; set; }
        public string DestinationType { get; set; }
        public string TruckType { get; set; }
        public List<MaterialRequestDetailDTO> Details { get; set; }
        public string TransactionStatus { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedOn { get; set; }
        public string ModifiedBy { get; set; }
        public string ModifiedOn { get; set; }
        public string DeliveryDate { get; set; }
    }

    public class MaterialRequestDetailDTO
    {
        public string ID { get; set; }
        public string HeaderID { get; set; }
        public string MaterialCode { get; set; }
        public string MaterialName { get; set; }
        public string ETA { get; set; }
        public decimal Qty { get; set; }
        public string UoM { get; set; }
        public string Packaging { get; set; }
        public string Remarks { get; set; }
    }
}