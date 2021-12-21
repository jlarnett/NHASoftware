//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
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
//    public class BookAuthorsController : ControllerBase
//    {
//        private readonly ApplicationDbContext _context;

//        public BookAuthorsController(ApplicationDbContext context)
//        {
//            _context = context;
//        }

//        // GET: api/BookAuthors
//        [HttpGet]
//        public async Task<ActionResult<IEnumerable<BookAuthor>>> GetBookAuthors()
//        {
//            //NOT IMPLEMENTED YET
//            //NOT IMPLEMENTED YET

//            return await _context.BookAuthors.ToListAsync();
//        }

//        // GET: api/BookAuthors/5
//        [HttpGet("{id}")]
//        public async Task<ActionResult<BookAuthor>> GetBookAuthor(int id)
//        {
//            //NOT IMPLEMENTED YET
//            //NOT IMPLEMENTED YET

//            var bookAuthor = await _context.BookAuthors.FindAsync(id);

//            if (bookAuthor == null)
//            {
//                return NotFound();
//            }

//            return bookAuthor;
//        }

//        // PUT: api/BookAuthors/5
//        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
//        [HttpPut("{id}")]
//        public async Task<IActionResult> PutBookAuthor(int id, BookAuthor bookAuthor)
//        {
//            //NOT IMPLEMENTED YET
//            //NOT IMPLEMENTED YET

//            if (id != bookAuthor.AuthorId)
//            {
//                return BadRequest();
//            }

//            _context.Entry(bookAuthor).State = EntityState.Modified;

//            try
//            {
//                await _context.SaveChangesAsync();
//            }
//            catch (DbUpdateConcurrencyException)
//            {
//                if (!BookAuthorExists(id))
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

//            // POST: api/BookAuthor
//            [HttpPost]
//            public JsonResult PostBookAuthor(BookAuthorsDTO bookAuthorDTO)
//            {
//                //Receives DTO with bookID and array of assigned authorIds
//                //Checks if whether record exist between authors. If it doesn't it is added to database.

//                List<BookAuthor> currentBookAuthors = _context.BookAuthors.Where(ba => ba.BookId == bookAuthorDTO.bookId).ToList();
//                List<int> currentBookAuthorsIds = new List<int>();

//                foreach (var bc in currentBookAuthors)
//                {
//                    currentBookAuthorsIds.Add(bc.AuthorId);
//                }

//                foreach (var authorid in bookAuthorDTO.Authors)
//                {
//                    BookAuthor bookAuthor = _context.BookAuthors.Find(int.Parse(authorid), bookAuthorDTO.bookId);

//                    if (currentBookAuthorsIds.Contains(int.Parse(authorid)))
//                    {
//                        currentBookAuthorsIds.Remove(int.Parse(authorid));
//                    }
                    
//                    if (bookAuthor == null)
//                    {
//                        BookAuthor book = new BookAuthor();
//                        book.BookId=bookAuthorDTO.bookId;
//                        book.AuthorId=int.Parse(authorid);

//                        _context.BookAuthors.Add(book);
//                        _context.SaveChanges();
//                    }
//                    else
//                    {
//                    }
//                }

//                foreach (var deletedItems in currentBookAuthorsIds)
//                {
//                    var book = _context.BookAuthors.Find(deletedItems, bookAuthorDTO.bookId);
//                    _context.BookAuthors.Remove(book);
//                    _context.SaveChanges();
//                }

//                //Returns json bool results
//                return new JsonResult(new { success = true});
//            }



//        // DELETE: api/BookAuthors/5
//        [HttpDelete("{id}")]
//        public async Task<IActionResult> DeleteBookAuthor(int id)
//        {
//            //NOT IMPLEMENTED YET
//            //NOT IMPLEMENTED YET

//            var bookAuthor = await _context.BookAuthors.FindAsync(id);
//            if (bookAuthor == null)
//            {
//                return NotFound();
//            }

//            _context.BookAuthors.Remove(bookAuthor);
//            await _context.SaveChangesAsync();

//            return NoContent();
//        }

//        private bool BookAuthorExists(int id)
//        {
//            //NOT IMPLEMENTED YET
//            //NOT IMPLEMENTED YET


//            return _context.BookAuthors.Any(e => e.AuthorId == id);
//        }
//    }
//}
