using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CommandsService.Dtos
{
    public class CommandCreateDto
    {
        // the Required Annotation will be checked when hitting CommandsController>>CreateCommandForPlatform, 
        // if the required field is not provided, the ActionResult will go back with an error
        [Required]
        public string HowTo { get; set; }
        [Required]
        public string CommandLine { get; set; }
    }
}