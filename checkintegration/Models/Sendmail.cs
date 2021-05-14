using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace checkintegration.Models
{
    public class Sendmail
    {
        public string From
        {
            get;
            set;
        }

         [Required(ErrorMessage = " Email Id is Required")]
        public string To
        {
            get;
            set;
        }
        public string Subject
        {
            get;
            set;
        }
        public string Body
        {
            get;
            set;
        }
    }
}