using Microsoft.AspNetCore.Mvc;
using NHASoftware.DBContext;
using NHASoftware.Entities.Forums;
using NHASoftware.Services.Forums;
using NHASoftware.Services.RepositoryPatternFoundationals;
using NHASoftware.ViewModels;

namespace NHASoftware.Controllers
{
    public class ForumController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public ForumController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Create Forum Section List and ForumTopic list
        /// Sends a KeyValuePair with mapping between topic & SectionId.
        /// Optimized to (0(n)) by using HashMap. (Not really needed, but I don't like O(n^2))
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Index()
        {
            var forumSections = await _unitOfWork.ForumSectionRepository.GetAllAsync();
            var forumTopics = await _unitOfWork.ForumTopicRepository.GetAllAsync();

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
