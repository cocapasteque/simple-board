using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
        public async Task<ActionResult<BoardEntry>> Post()
        {
            using var reader = new StreamReader(Request.Body, Encoding.UTF8);
            var request = await reader.ReadToEndAsync();
            
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
            await _context.SaveChangesAsync();
            
            return Ok(entry);
        }

        public class EncryptedEntry
        {
            [JsonProperty("entry")] public string Entry { get; set; }
        }
    }
}