using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace SimpleBoard.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LeaderboardController : ControllerBase
    {
        private readonly ILogger<LeaderboardController> _logger;
        private readonly BoardContext _context;
        
        public LeaderboardController(ILogger<LeaderboardController> logger, BoardContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpGet]
        public IEnumerable<BoardEntry> Get()
        {
            return _context.Entries.OrderByDescending(x => x.Score).Take(3);
        }

        [HttpGet("all")]
        public IEnumerable<BoardEntry> GetAll()
        {
            return _context.Entries.OrderByDescending(x => x.Score);
        }

        [HttpGet("{name}")]
        public BoardEntry GetNamed(string name)
        {
            var index = _context.Entries.OrderByDescending(x => x.Score).ToList().FindIndex(x => x.Name.Equals(name.ToLower()));
            var entry = _context.Entries.FirstOrDefault(x => x.Name.Equals(name.ToLower()));
            if (entry == null)
            {
                return new BoardEntry { Name = name.ToLower(), Score = 0 };
            }
            entry.Id = index;
            return entry;
        }

        [HttpPost]
        public ActionResult<BoardEntry> Post(string request)
        {
            Console.WriteLine("Got " + request);
            var stringEntry = CipherService.Decrypt(request, Environment.GetEnvironmentVariable("SECRET_KEY"));
            
            Console.WriteLine("Unencrypted: " + stringEntry);
            var entry = JsonConvert.DeserializeObject<BoardEntry>(stringEntry);

            if (entry == null)
            {
                return BadRequest("Cannot deserialize entry object");
            }
            
            entry.Name = entry.Name.ToLower();
            var previous = _context.Entries.FirstOrDefault(x => x.Name.Equals(entry.Name.ToLower()));
            if (previous != null)
            {
                if (previous.Score > entry.Score)
                {
                    return Ok(previous);
                }
                else
                {
                    _context.Remove(previous);
                }
            }

            _context.Entries.Add(entry);
            _context.SaveChanges();
            return Ok(entry);
        }

        public class EncryptedEntry
        {
            [JsonProperty("entry")] public string Entry { get; set; }
        }
    }
}