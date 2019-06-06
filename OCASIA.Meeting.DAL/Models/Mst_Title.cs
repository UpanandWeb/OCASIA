using System.ComponentModel.DataAnnotations;

namespace OCASIA.Meeting.DAL
{
    public   class Mst_Title
    {
        [Key]
        public int TitleID { get; set; }
        public string TitleName { get; set; }
        public bool IsActive { get; set; }
        
    }
}
