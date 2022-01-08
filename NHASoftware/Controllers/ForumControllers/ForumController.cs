using Microsoft.AspNetCore.Mvc;
using Microsoft.Rest;
using NHASoftware.Data;
using NHASoftware.Models.ForumModels;
using NHASoftware.ViewModels;

namespace NHASoftware.Controllers
{
    public class ForumController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ForumController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            /************************************************************************************************************
             *  Create Forum Section List and ForumTopic list
             *  Sends a KeyValuePair with mapping between topic & SectionId.
             *  Optimized to (0(n)) by using HashMap. (Not really needed, but I dont like O(n^2))
             ************************************************************************************************************/


            var forumSections = _context.ForumSections.ToList();
            var forumTopics = _context.ForumTopics.ToList();


            List<KeyValuePair<int, ForumTopic>> topicSectionMap = new List<KeyValuePair<int, ForumTopic>>();

            foreach (var topic in forumTopics)
            {
                topicSectionMap.Add(new KeyValuePair<int, ForumTopic>(topic.ForumSectionId, topic));
            }

            var indexModel = new ForumIndexViewModel()
            {
                ForumSections = forumSections,
                ForumTopics = forumTopics,
                Forums = topicSectionMap
            };

            return View(indexModel);
        }
    }
}
