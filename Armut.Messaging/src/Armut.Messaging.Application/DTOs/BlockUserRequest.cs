using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Armut.Messaging.Application.DTOs
{
    public class BlockUserRequest
    {
        public BlockUserRequest(string targetUserName)
        {
            TargetUserName = targetUserName;
        }

        public string TargetUserName { get; }
    }
}
