using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NHA.Website.Software.DBContext;
using NHA.Website.Software.Entities.Anime;
using NHA.Website.Software.Services.RepositoryPatternFoundationals;

namespace NHA.Website.Software.Controllers.WebAPIs.Anime
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnimePagesController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public AnimePagesController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: api/AnimePages
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AnimePage>>> GetAnimePages(int pageNumber)
        {
            const int pageSize = 50;
            var totalItems = await _unitOfWork.AnimePageRepository.CountAsync(c => c.Id != null);
            var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            // Fix invalid page numbers
            if (pageNumber < 1) pageNumber = 1;
            if (pageNumber > totalPages) pageNumber = 1;

            var anime = await _unitOfWork.AnimePageRepository.GetResultPageAsync(pageNumber, 50);
            return Ok(anime);
        }
    }
}
