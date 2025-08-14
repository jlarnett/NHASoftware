using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement.Mvc;
using NHA.Website.Software.Entities.Forums;
using NHA.Website.Software.Services.RepositoryPatternFoundationals;
namespace NHA.Website.Software.Controllers.WebAPIs.Forums;
[Route("api/[controller]")]
[ApiController]
[FeatureGate("ForumsEnabled")]
public class ForumsController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;

    public ForumsController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    [HttpGet("Sections")]
    public async Task<ActionResult<ForumSection>> GetForumSections()
    {
        return Ok(await _unitOfWork.ForumSectionRepository.GetAllAsync());
    }

    [HttpGet("Topics")]
    public async Task<ActionResult<ForumSection>> GetForumTopics()
    {
        return Ok(await _unitOfWork.ForumTopicRepository.GetAllAsync());
    }

    [HttpGet("Posts")]
    public async Task<ActionResult<ForumSection>> GetForumPosts()
    {
        return Ok(await _unitOfWork.ForumPostRepository.GetAllAsync());
    }

    [HttpGet("Comments")]
    public async Task<ActionResult<ForumSection>> GetForumComments()
    {
        return Ok(await _unitOfWork.ForumCommentRepository.GetAllAsync());
    }

}
