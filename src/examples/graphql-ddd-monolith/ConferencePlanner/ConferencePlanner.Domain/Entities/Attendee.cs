using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ConferencePlanner.Domain.Entities
{
    public class Attendee : SoftDeleteEntity
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string? FirstName { get; set; }

        [Required]
        [StringLength(200)]
        public string? LastName { get; set; }

        [Required]
        [StringLength(200)]
        public string? UserName { get; set; }

        [StringLength(256)]
        public string? EmailAddress { get; set; }

        [StringLength(256)]
        public string? Country { get; set; }

        public ICollection<SessionAttendee> SessionsAttendees { get; set; } =
            new List<SessionAttendee>();
    }
}
