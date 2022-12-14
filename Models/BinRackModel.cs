using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WMS_FE.Models
{
    public class BinRackResponse
    {
        public BinRack data { get; set; }
        public bool status { get; set; }
        public string message { get; set; }
    }

    public class BinRack
    {
        public string ID { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string WarehouseCode { get; set; }
        public string WarehouseName { get; set; }
        public string BinRackAreaID { get; set; }
        public string BinRackAreaCode { get; set; }
        public string BinRackName { get; set; }
        public string BinRackAreaName { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedOn { get; set; }
        public string ModifiedBy { get; set; }
        public string ModifiedOn { get; set; }
    }

    public class BinRackList
    {
        public string ID { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string WarehouseCode { get; set; }
        public string WarehouseName { get; set; }
        public string BinRackAreaID { get; set; }
        public string BinRackAreaCode { get; set; }
        public string BinRackName { get; set; }
        public string BinRackAreaName { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedOn { get; set; }
        public string ModifiedBy { get; set; }
        public string ModifiedOn { get; set; }
    }
}