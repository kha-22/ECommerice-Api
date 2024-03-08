using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerice.Core.Entities
{
    public class Contactus : BaseEntity
    {
        public string Subject { get; set; }
        public string Message { get; set; }
        public DateTime? SendDate { get; set; }
        public string Replay { get; set; }
        public DateTime? RepayDate { get; set; }

        public string UserId { get; set; }
    }
}
