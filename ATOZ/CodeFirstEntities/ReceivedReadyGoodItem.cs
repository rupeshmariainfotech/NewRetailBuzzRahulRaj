using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeFirstEntities
{
    [Serializable]
    public class ReceivedReadyGoodItem
    {
        [Key]
        public int Id { get; set; }
        public string InwardCode { get; set; }
        public string ItemName { get; set; }
        public double? Quantity { get; set; }
        public string Color { get; set; }
        public string Size { get; set; }
        public string Unit { get; set; }
        public string Description { get; set; }
        public string CostPrice { get; set; }
        public double? LabourCost { get; set; }
        public double? OtherExpenses { get; set; }
        public string Status { get; set; }
        public DateTime? ModifiedOn { get; set; }
    }
}
