﻿using Microsoft.AspNetCore.Mvc;
using NHASoftware.Data;
using NHASoftware.ViewModels;

namespace NHASoftware.Controllers
{
    public class AuthorBookController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AuthorBookController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult AddAuthors(int id)
        {
            var AssignVM = new AssignAuthorViewModel(id, _context.Authors.ToList());
            return View(AssignVM);
        }

        [HttpPost]
        public IActionResult AddAuthors()
        {
            return Ok();
        }

    }
}
