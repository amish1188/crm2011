using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KlpCrm.Filenet.Web.ReactApplication.Models
{
    public class GetDescriptionInput
    {
        public string ForsikringstakerNummer { get; set; }
        public string Department { get; set; }
        public string Kassenummer { get; set; }
        public string Typed { get; set; }
    }
}