using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CommandsService.Models;
using Grpc.Net.Client;
using PlatformService;

namespace CommandsService.SyncDataServices.Grpc
{
    public class PlatformDataClient : IPlatformDataClient
    {
        private readonly IConfiguration _config;
        private readonly IMapper _mapper;
        public PlatformDataClient(IConfiguration config, IMapper mapper)
        {
            _mapper = mapper;
            _config = config;
        }
        public IEnumerable<Platform> ReturnAllPlatforms()
        {
            var endpoint = _config["GrpcPlatform"];

            Console.WriteLine($"--> Calling GRPC Service {endpoint}");

            var channel = GrpcChannel.ForAddress(endpoint);
            var client = new GrpcPlatform.GrpcPlatformClient(channel);
            var request = new GetAllRequest();

            try
            {
                var reply = client.GetAllPlatforms(request);
                return _mapper.Map<IEnumerable<Platform>>(reply.Platform);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"--> Couldn't call GRPC Server {ex.Message}");
                return null;
            }
        }
    }
}