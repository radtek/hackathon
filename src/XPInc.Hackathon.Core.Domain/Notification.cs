using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XPInc.Hackathon.Core.Domain
{
    public class Notification
    {
        public Guid Id { get; set; }
        public Guid EventId { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public DateTimeOffset CreationDate { get; set; } = DateTime.Now;
        public object TeamMemberId { get; set; }
        public object TeamId { get; set; }
    }
}
