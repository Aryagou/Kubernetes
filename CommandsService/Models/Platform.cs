using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CommandsService.Models
{
    public class Platform
    {
        [Key]
        [Required] 
        public int Id { get; set; } // Id can be automatically generated if not specified by the user
        [Required] 
        public int ExternalId { get; set; }
        [Required]
        public string Name { get; set; }
        public ICollection<Command> Commands { get; set; } = new List<Command>();
        // arya [todo] 
        // 1. what is navigation reference
        // 2. why do we have to initialise the list in the first place?
    }
}