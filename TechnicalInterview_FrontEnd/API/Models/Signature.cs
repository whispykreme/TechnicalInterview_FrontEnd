using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechnicalInterview_FrontEnd.API.Models
{
    public class Signature
    {
        public Guid SignatureId { get; set; }
        public byte[] SignatureImage { get; set; }
        public int VoterId { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}
