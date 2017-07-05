using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace CodeFirstEntities
{
    [Serializable]
    public class OutwardForProduction
    {
        [Key]
        public int Id { get; set; }
        public string OutwardCode { get; set; }
        public DateTime Date { get; set; }
        [Required]
        public string EmpName { get; set; }
        public string EmpDesignation { get; set; }
        public string EmpContact { get; set; }
        [Required]
        public double? ExpectedLabourCost { get; set; }
        public string ExpectedManufacturingDays { get; set; }
        public double? OtherExpenses { get; set; }
        public double? TotalAmount { get; set; }
        public double? ProductionGrandTotal { get; set; }
        public string ShopCode { get; set; }
        public string ShopName { get; set; }
        public string Narration { get; set; }
        public string PreparedBy { get; set; }
        public string Status { get; set; }
        public DateTime? ModifiedOn { get; set; }
    }
}
