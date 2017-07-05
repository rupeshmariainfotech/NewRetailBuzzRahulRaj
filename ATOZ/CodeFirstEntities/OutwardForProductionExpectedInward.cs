using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace CodeFirstEntities
{
    [Serializable]
    public class OutwardForProductionExpectedInward
    {
        [Key]
        public int Id { get; set; }
        public string OutwardCode { get; set; }
        public string ItemName { get; set; }
        public string Quantity { get; set; }
        public string Unit { get; set; }
        public string Color { get; set; }
        public string Material { get; set; }
        public string ProductionItem { get; set; }
        public string Status { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string InwardStatus { get; set; }
    }
}
