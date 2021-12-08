using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NHASoftware.Data;
using NHASoftware.Models;
using NHASoftware.ViewModels;

namespace NHASoftware.Controllers
{
    public class BookAuthorsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BookAuthorsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult AddBookAuthors(int id)
        {
            //Redirects the user to the AddBookAuthors View.
            //Creates a AddBookAuthorViewModel and sends author list + selected author list.

            List<BookAuthor> bookAuthors = _context.BookAuthors.ToList();
            List<Author> Authors = new List<Author>();

            foreach (var ba in bookAuthors)
            {
                if (ba.BookId == id)
                {
                    Authors.Add(_context.Authors.Find(ba.AuthorId));
                }
            }

            var assignVM = new AddBookAuthorViewModel(id, _context.Authors.ToList(), Authors);
            return View(assignVM);
        }

    }
}
