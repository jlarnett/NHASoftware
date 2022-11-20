using Microsoft.AspNetCore.Mvc;
using NHASoftware.Data;
using NHASoftware.Models.ForumModels;
using NHASoftware.Services;
using NHASoftware.ViewModels;

namespace NHASoftware.Controllers
{
    public class ForumController : Controller
    {
        private readonly IForumRepository _forumRepository;

        public ForumController(ApplicationDbContext context, IForumRepository forumRepository)
        {
            _forumRepository = forumRepository;
        }

        /// <summary>
        /// Create Forum Section List and ForumTopic list
        /// Sends a KeyValuePair with mapping between topic & SectionId.
        /// Optimized to (0(n)) by using HashMap. (Not really needed, but I don't like O(n^2))
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Index()
        {
            var forumSections = await _forumRepository.GetForumSectionsAsync();
            var forumTopics = await _forumRepository.GetForumTopicsAsync();

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
