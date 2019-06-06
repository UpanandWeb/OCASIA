using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCASIA.Meeting.DAL
{
  public  class Mst_InvitationCategory
    {
        [Key]
        public int InviationCategoryID { get; set; }
        public string InviationCategoryName { get; set; }
        public string DisplayText { get; set; }
        public string Abbreviation { get; set; }
        public string GroupName { get; set; }
        public string Description { get; set; }
        public string TemplatePath { get; set; }
        public string TemplateHtml { get; set; }
        public DateTime CreatedOn { get; set; }
        [Required]
        public string CreatedBy { get; set; }
        public DateTime? UpdateOn { get; set; }
        public string UpdatedBy { get; set; }
        public bool IsActive { get; set; }
        public bool IsPublish { get; set; } 

        [ForeignKey("CreatedBy")]
        public virtual ApplicationUser Users { get; set; }

    }
}
