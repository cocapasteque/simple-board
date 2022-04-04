using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace SimpleBoard.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LeaderboardController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

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

        [HttpGet("{name}")]
        public BoardEntry GetNamed(string name)
        {
            var index = _context.Entries.OrderByDescending(x => x.Score).ToList().FindIndex(x => x.Name.Equals(name));
            var entry = _context.Entries.FirstOrDefault(x => x.Name.Equals(name));
            if (entry == null)
            {
                return new BoardEntry { Name = name, Score = 0 };
            }
            entry.Id = index;
            return entry;
        }

        [HttpPost]
        public BoardEntry Post([FromBody] BoardEntry entry)
        {
            var previous = _context.Entries.FirstOrDefault(x => x.Name.Equals(entry.Name));
            if (previous != null)
            {
                if (previous.Score > entry.Score)
                {
                    return previous;
                }
                else
                {
                    _context.Remove(previous);
                }
            }

            _context.Entries.Add(entry);
            _context.SaveChanges();
            return entry;
        }
    }
}