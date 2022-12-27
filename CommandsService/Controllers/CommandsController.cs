using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CommandsService.Data;
using CommandsService.Dtos;
using CommandsService.Models;
using Microsoft.AspNetCore.Mvc;

namespace CommandsService.Controllers
{
    [Route("api/c/platforms/{platformId}/[controller]")]
    [ApiController]
    public class CommandsController : ControllerBase
    {
        private readonly ICommandRepo _repo;
        private readonly IMapper _mapper;
        public CommandsController(ICommandRepo repo, IMapper mapper)
        {
            _mapper = mapper;
            _repo = repo;
        }

        [HttpGet]
        public ActionResult<IEnumerable<CommandReadDto>> GetCommandsForPlatform(int platformId) 
        {
            Console.WriteLine($"--> Hit GetCommandsForPlatform {platformId}");

            if (!_repo.PlatformExists(platformId)) return NotFound();

            var commands = _repo.GetCommandsForPlatform(platformId);
            return Ok(_mapper.Map<IEnumerable<CommandReadDto>>(commands));
        }

        [HttpGet("{commandId}", Name = nameof(GetCommandForPlatform))] // additional route parameter
        public ActionResult<CommandReadDto> GetCommandForPlatform(int commandId, int platformId)
        {
            Console.WriteLine($"--> Hit GetCommandForPlatform platformId {platformId}, commandId {commandId}");

            if (!_repo.PlatformExists(platformId)) return NotFound();

            var command = _repo.GetCommand(platformId, commandId);
            if (command is null) return NotFound();

            return Ok(_mapper.Map<CommandReadDto>(command));
        }

        [HttpPost]
        public ActionResult<CommandReadDto> CreateCommandForPlatform(int platformId, CommandCreateDto commandCreateDto)
        {
            // we didn't check the command dto validity here because we've added field annotations in CommandCreateDto
            // and ApiController will check this for us
            Console.WriteLine($"--> Hit CreateCommandForPlatform platformId {platformId}");

            if (!_repo.PlatformExists(platformId)) return NotFound();
            
            var command = _mapper.Map<Command>(commandCreateDto);
            _repo.CreateCommand(platformId, command);
            _repo.SaveChanges();

            var commandReadDto = _mapper.Map<CommandReadDto>(command);

            return CreatedAtRoute(nameof(GetCommandForPlatform), 
                new {platformId = platformId, commandId = commandReadDto.Id}, commandReadDto);
        }
    }
}