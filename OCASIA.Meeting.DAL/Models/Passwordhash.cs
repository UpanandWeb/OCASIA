using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCASIA.Meeting.DAL
{
    public class Passwordhash
    {
        public int PasswordhashID { get; set; }
        [Required]
        public virtual string UserId { get; set; }
        public string Password { get; set; }

        [ForeignKey("UserId")]
        public virtual ApplicationUser Users { get; set; }
    }
}
