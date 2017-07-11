using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace CodeFirstEntities
{
    [Serializable]
     public class ClientLead
    {
        [Key]
        public int ClientLeadId { get; set; }
        [Required]
        [Display(Name = "Name")]
        public string ClientName { get; set; }
        [Required]
        [Display(Name="ContactNo1")]
        public string ContactNo1 { get; set; }
        public string ContactNo2 { get; set; }
        [Required]
        [Display(Name="Address")]
        public string Address { get; set; }
        [Required]
        [Display(Name = "Requriment")]
        public string Requriment { get; set; }
        [Required]
        public DateTime Date { get; set; }
        [Required]
        [DataType(DataType.Date)]
        public DateTime? ScheduleDate { get; set; }
      //  public DateTime? ScheduleDate { get; set; }

        public string Remark { get; set; }


        public int ClientId { get; set; }

    }
}
