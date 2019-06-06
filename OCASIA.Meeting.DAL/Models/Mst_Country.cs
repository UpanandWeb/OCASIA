using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OCASIA.Meeting.DAL
{
    public class Mst_Country
    {
        [Key]
        public int CountryID { get; set; }
        public string CountryName { get; set; }
        public bool IsActive { get; set; }

        public ICollection<UserDetail> UserDetails { get; set; }
    }
}
