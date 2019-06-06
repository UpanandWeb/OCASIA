using System.ComponentModel.DataAnnotations;

namespace OCASIA.Meeting.DAL
{
    public  class Mst_NOC
    {
        [Key]
        public int NOCID { get; set; }
        public string Name { get; set; }
        public string Abbreviation { get; set; }
        public string Email { get; set; }

        public bool IsActive { get; set; }
        
    }
}
