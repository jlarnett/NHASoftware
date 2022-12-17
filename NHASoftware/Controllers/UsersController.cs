using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NHASoftware.Controllers.WebAPIs;
using NHASoftware.Entities.Identity;
using NHASoftware.Services.RepositoryPatternFoundationals;
using NHASoftware.Views.ViewModels.SocialVMs;

namespace NHASoftware.Controllers
{
    public class UsersController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UsersController(UserManager<ApplicationUser> userManager, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _userManager = userManager;
            this._unitOfWork = unitOfWork;
            this._mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetProfiles(string? userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            var posts = _unitOfWork.PostRepository.Find(p => p.UserId.Equals(user.Id));

            var profileVM = new ProfileVM()
            {
                User = user,
                UserPosts = await new PostsController(_mapper, _unitOfWork).GetPosts()
            };

            return user != null ? View("Profiles", profileVM) : NotFound();
        }
    }
}
