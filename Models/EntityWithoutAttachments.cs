using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace KlpCrm.Filenet.Web.ReactApplication.Models
{
    public class EntityWithoutAttachments
    {
        [Required]
        public Guid Id { get; set; }
        public string LeadingText { get; set; }
        public string Subject { get; set; }
        [Required(AllowEmptyStrings = false)]
        public string Description { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string ActualEnd { get; set; }
        public bool ArchivingEnabled { get; set; }
        public Guid? ActivityId { get; set; } = null;
    }
}