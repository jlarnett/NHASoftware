using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NHASoftware.Data;
using NHASoftware.DTOs;
using NHASoftware.Models;

namespace NHASoftware.Controllers.WebAPIs
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookAuthorsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public BookAuthorsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/BookAuthors
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookAuthor>>> GetBookAuthors()
        {
            return await _context.BookAuthors.ToListAsync();
        }

        // GET: api/BookAuthors/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BookAuthor>> GetBookAuthor(int id)
        {
            var bookAuthor = await _context.BookAuthors.FindAsync(id);

            if (bookAuthor == null)
            {
                return NotFound();
            }

            return bookAuthor;
        }

        // PUT: api/BookAuthors/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBookAuthor(int id, BookAuthor bookAuthor)
        {
            if (id != bookAuthor.AuthorId)
            {
                return BadRequest();
            }

            _context.Entry(bookAuthor).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BookAuthorExists(id))
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

            // POST: api/BookAuthor
            [HttpPost]
            public JsonResult PostBookAuthor(BookAuthorsDTO bookAuthorDTO)
            {
                //Receives DTO with bookID and array of assigned authorIds
                //Checks if whether record exist between authors. If it doesn't it is added to database.

                foreach (var authorid in bookAuthorDTO.Authors)
                {
                    BookAuthor bookAuthor = _context.BookAuthors.Find(int.Parse(authorid), bookAuthorDTO.bookId);

                    if (bookAuthor == null)
                    {
                        BookAuthor book = new BookAuthor();
                        book.BookId=bookAuthorDTO.bookId;
                        book.AuthorId=int.Parse(authorid);

                        _context.BookAuthors.Add(book);
                        _context.SaveChanges();
                    }
                    else
                    {
                        return new JsonResult(new { success = false});
                    }
                }

                return new JsonResult(new { success = true});
            }

        // DELETE: api/BookAuthors/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBookAuthor(int id)
        {
            var bookAuthor = await _context.BookAuthors.FindAsync(id);
            if (bookAuthor == null)
            {
                return NotFound();
            }

            _context.BookAuthors.Remove(bookAuthor);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BookAuthorExists(int id)
        {
            return _context.BookAuthors.Any(e => e.AuthorId == id);
        }
    }
}
