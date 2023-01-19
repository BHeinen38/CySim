using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CySim.Data;
using CySim.Models.ScoreBoardModels;
using Microsoft.AspNetCore.Mvc;

namespace LeaderBoard.Controllers
{

    [Route("api/[controller]")]
    public class LeaderBoardController : Controller
    {
        public ApplicationDbContext _context { get; }

        public LeaderBoardController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IEnumerable<ScoreBoard> All()
        {
            return _context.Leaders();
        }

        [HttpGet("top/{count}")]
        public IEnumerable<ScoreBoard> Top([FromRoute] int count)
        {
            return _context.Leaders(count);
        }

    }
}

