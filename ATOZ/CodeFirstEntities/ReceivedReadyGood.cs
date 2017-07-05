using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace CodeFirstEntities
{
    [Serializable]
    public class ReceivedReadyGood
    {
        [Key]
        public int Id { get; set; }
        public string InwardCode { get; set; }
        public string OutwardForProductionCode { get; set; }
        public DateTime? Date { get; set; }
        public string ShopCode { get; set; }
        public string ShopName { get; set; }
        public double? TotalAmount { get; set; }
        public double? ActualLabourCost { get; set; }
        public double? OtherExpenses { get; set; }
        public double? ProductionGrandTotal { get; set; }
        public string Status { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string PreparedBy { get; set; }
        public string PreparedByEmail { get; set; }
        public string PreparedByDesignation { get; set; }
    }
}
