using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Armut.Messaging.Application.DTOs
{
    public class UserMessageRequest
    {
        [Required]
        public string TargetUserName { get; set; }

        [Required]
        public string Message { get; set; }
    }
}
