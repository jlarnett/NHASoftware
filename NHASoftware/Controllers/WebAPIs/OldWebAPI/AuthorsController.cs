﻿//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using AutoMapper;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using NHASoftware.Data;
//using NHASoftware.DTOs;
//using NHASoftware.Models;

//namespace NHASoftware.Controllers.WebAPIs
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class AuthorsController : ControllerBase
//    {
//        private readonly ApplicationDbContext _context;
//        private readonly IMapper _mapper;

//        public AuthorsController(ApplicationDbContext context, IMapper mapper)
//        {
//            _context = context;
//            _mapper = mapper;
//        }

//        // GET: api/Authors
//        [HttpGet]
//        public async Task<ActionResult<IEnumerable<AuthorDTO>>> GetAuthors()
//        { 

//            var authors = await _context.Authors.ToListAsync();
//            var authorDto = authors.ToList().Select(_mapper.Map<Author, AuthorDTO>);
//            return Ok(authorDto);
//        }

//        // GET: api/Authors/5
//        [HttpGet("{id}")]
//        public async Task<ActionResult<Author>> GetAuthor(int id)
//        {
//            var author = await _context.Authors.FindAsync(id);

//            if (author == null)
//            {
//                return NotFound();
//            }

//            return author;
//        }

//        // PUT: api/Authors/5
//        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
//        [HttpPut("{id}")]
//        public async Task<IActionResult> PutAuthor(int id, Author author)
//        {
//            if (id != author.Id)
//            {
//                return BadRequest();
//            }

//            _context.Entry(author).State = EntityState.Modified;

//            try
//            {
//                await _context.SaveChangesAsync();
//            }
//            catch (DbUpdateConcurrencyException)
//            {
//                if (!AuthorExists(id))
//                {
//                    return NotFound();
//                }
//                else
//                {
//                    throw;
//                }
//            }

//            return NoContent();
//        }

//        // POST: api/Authors
//        [HttpPost]
//        public JsonResult PostAuthor([Bind("Name", "Address")]AuthorDTO authorDTO)
//        {
//            //Create new author. User automapper to change dto into Author.
//            //Adds author & saves changes.
//            //Returns JsonResult

//            Author newAuthor = _mapper.Map<AuthorDTO, Author>(authorDTO);

//            var author = _context.Authors.Add(newAuthor);
//            _context.SaveChanges();

//            return new JsonResult(new { success = true});
//        }

//        // DELETE: api/Authors/5
//        [HttpDelete("{id}")]
//        public async Task<IActionResult> DeleteAuthor(int id)
//        {
//            var author = await _context.Authors.FindAsync(id);
//            if (author == null)
//            {
//                return NotFound();
//            }

//            _context.Authors.Remove(author);
//            await _context.SaveChangesAsync();

//            return NoContent();
//        }

//        private bool AuthorExists(int id)
//        {
//            return _context.Authors.Any(e => e.Id == id);
//        }
//    }
//}