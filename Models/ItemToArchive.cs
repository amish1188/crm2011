using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace KlpCrm.Filenet.Web.ReactApplication.Models
{
    public class ItemToArchive
    {
        [Required(AllowEmptyStrings = false)]
        public string KasseNummer { get; set; }
        [Required(AllowEmptyStrings = false)]
        public string ForsikringstakerNummer { get; set; }
        [Required(AllowEmptyStrings = false)]
        public string Department { get; set; }
        [Required(AllowEmptyStrings = false)]
        public string Description { get; set; }
        [Required]
        public List<EntityWithAttachments> Entities { get; set; }
    }
}