using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCASIA.Meeting.DAL
{
   public class TravelDetail
    {
        public int TravelDetailID { get; set; }
        public int UserDetailID { get; set; }
        public string ArrivalAirport { get; set; }
        public DateTime? ArrivalDate { get; set; }
        public string ArrivalTime { get; set; }
        public string ArrivalFlightNumber { get; set; }
        public string DeparturelAirport { get; set; }
        public DateTime? DepartureDate { get; set; }
        public string DepartureTime { get; set; }
        public int NoCheckInBages { get; set; }
        public string Comments { get; set; }
        public string Hotel { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public string Desc1 { get; set; }
        public string Desc2 { get; set; }
        public string Desc3 { get; set; }

        public DateTime CreatedOn { get; set; }
        [Required]
        public string CreatedBy { get; set; }
        public bool IsActive { get; set; }
        public DateTime? UpdateOn { get; set; }
        public string UpdatedBy { get; set; }
        public bool IsPublish { get; set; }

        [ForeignKey("CreatedBy")]     
        public virtual ApplicationUser Users { get; set; }

        [ForeignKey("UserDetailID")]
        public virtual UserDetail UserDetails { get; set; }


    }
}
