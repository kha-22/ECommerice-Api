using ECommerice.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerice.Core.DTOs
{
    public class QuestionWithReplayDTO
    {
        public int Id { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
        public DateTime CreateOn { get; set; }

        public List<ContactusReplayDTO> Replyes { get; set; }
    }
}
