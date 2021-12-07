using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Build.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using NHASoftware.Data;
using NHASoftware.DTOs;
using NHASoftware.Models;
using NHASoftware.ViewModels;

namespace NHASoftware.Controllers.WebAPIs
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorBooksController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public AuthorBooksController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/AuthorBooks
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AuthorBook>>> GetAuthorBooks()
        {
            return await _context.AuthorBooks.ToListAsync();
        }

        // GET: api/AuthorBooks/5
        [HttpGet("{id}")]
        public async Task<ActionResult<AuthorBook>> GetAuthorBook(int id)
        {
            var authorBook = await _context.AuthorBooks.FindAsync(id);

            if (authorBook == null)
            {
                return NotFound();
            }

            return authorBook;
        }

        // PUT: api/AuthorBooks/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAuthorBook(int id, AuthorBook authorBook)
        {
            if (id != authorBook.AuthorId)
            {
                return BadRequest();
            }

            _context.Entry(authorBook).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AuthorBookExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/AuthorBooks
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public JsonResult PostAuthorBook(BookAuthorsDTO bookAuthorDTO)
        {
            //Receives DTO with bookID and array of assigned authorIds
            //Checks if whether record exist between authors. If it doesn't it is added to database.

            foreach (var authorid in bookAuthorDTO.Authors)
            {
                AuthorBook authorBook = _context.AuthorBooks.Find(int.Parse(authorid), bookAuthorDTO.bookId);

                if (authorBook == null)
                {
                    AuthorBook book = new AuthorBook();
                    book.BookId=bookAuthorDTO.bookId;
                    book.AuthorId=int.Parse(authorid);

                    _context.AuthorBooks.Add(book);
                    _context.SaveChanges();
                }
                else
                {
                    return new JsonResult(new { success = false});
                }
            }

            return new JsonResult(new { success = true});
        }

        // DELETE: api/AuthorBooks/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAuthorBook(int id)
        {
            var authorBook = await _context.AuthorBooks.FindAsync(id);
            if (authorBook == null)
            {
                return NotFound();
            }

            _context.AuthorBooks.Remove(authorBook);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool AuthorBookExists(int id)
        {
            return _context.AuthorBooks.Any(e => e.AuthorId == id);
        }
    }
}
