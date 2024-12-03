using CQRS.Core.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Post.Common.Response;
using Post.Query.Api.Queries;
using Post.Query.Domain.Entities;

namespace Post.Query.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostsController : ControllerBase
    {

        private readonly ILogger<PostsController> _logger;
        private readonly IQueryDispatcher<PostEntity> _queryDispatcher;

        public PostsController(IQueryDispatcher<PostEntity> queryDispatcher, ILogger<PostsController> logger)
        {
            _queryDispatcher = queryDispatcher;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<BaseResponse<List<PostEntity>>>> GetAll()
        {
            try
            {
                var posts = await this._queryDispatcher.SendAsync(new FindAllPostsQuery());
                if (posts == null || !posts.Any())
                {
                    return Ok(BaseResponse<List<PostEntity>>.OkResult(new List<PostEntity>(), "Empty list"));
                }

                return Ok(BaseResponse<List<PostEntity>>.OkResult(posts));
            }
            catch (Exception ex)
            {
                this._logger.LogError($"Error while retrieving posts. Error: {ex.Message}", ex);
                return StatusCode(StatusCodes.Status500InternalServerError, BaseResponse<List<PostEntity>>.Failure(ex));
            }
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<BaseResponse<List<PostEntity>>>> GetById(Guid id)
        {
            try
            {
                var posts = await this._queryDispatcher.SendAsync(new FindPostByIdQuery()
                {
                    PostId = id
                });

                if (posts == null || !posts.Any())
                {
                    return Ok(BaseResponse<List<PostEntity>>.OkResult(new List<PostEntity>(), $"Entity with Id: {id} not found"));
                }

                return Ok(BaseResponse<List<PostEntity>>.OkResult(posts));
            }
            catch (Exception ex)
            {
                this._logger.LogError($"Error while retrieving post by Id: {id}. Error: {ex.Message}", ex);
                return StatusCode(StatusCodes.Status500InternalServerError, BaseResponse<List<PostEntity>>.Failure(ex));
            }
        }

        [HttpGet("author/{authorName}")]
        public async Task<ActionResult<BaseResponse<List<PostEntity>>>> GetByAuthor(string authorName)
        {
            try
            {
                var posts = await this._queryDispatcher.SendAsync(new FindPostsByAuthorQuery()
                {
                    AuthorName = authorName
                });

                if (posts == null || !posts.Any())
                {
                    return Ok(BaseResponse<List<PostEntity>>.OkResult(new List<PostEntity>(), $"Post create by {authorName} not found"));
                }

                return Ok(BaseResponse<List<PostEntity>>.OkResult(posts));
            }
            catch (Exception ex)
            {
                this._logger.LogError($"Error while retrieving post created by: {authorName}. Error: {ex.Message}", ex);
                return StatusCode(StatusCodes.Status500InternalServerError, BaseResponse<List<PostEntity>>.Failure(ex));
            }
        }




        [HttpGet("/withComments")]
        public async Task<ActionResult<BaseResponse<List<PostEntity>>>> GetWithComments()
        {
            try
            {
                var posts = await this._queryDispatcher.SendAsync(new FindPostsWithCommentsQuery());

                if (posts == null || !posts.Any())
                {
                    return Ok(BaseResponse<List<PostEntity>>.OkResult(new List<PostEntity>(), $"Post with comments not found!"));
                }

                return Ok(BaseResponse<List<PostEntity>>.OkResult(posts));
            }
            catch (Exception ex)
            {
                this._logger.LogError($"Error while retrieving post with comments. Error: {ex.Message}", ex);
                return StatusCode(StatusCodes.Status500InternalServerError, BaseResponse<List<PostEntity>>.Failure(ex));
            }
        }


        [HttpGet("/withLikes/{likesCount:int}")]
        public async Task<ActionResult<BaseResponse<List<PostEntity>>>> GetWithLikes(int likesCount = 0)
        {
            try
            {
                var posts = await this._queryDispatcher.SendAsync(new FindPostsWithLikesQuery()
                {
                    NumberOfLikes = likesCount
                });

                if (posts == null || !posts.Any())
                {
                    return Ok(BaseResponse<List<PostEntity>>.OkResult(new List<PostEntity>(), $"Post with more than {likesCount} not found"));
                }

                return Ok(BaseResponse<List<PostEntity>>.OkResult(posts));
            }
            catch (Exception ex)
            {
                this._logger.LogError($"Error while retrieving post with more than {likesCount} not found. Error: {ex.Message}", ex);
                return StatusCode(StatusCodes.Status500InternalServerError, BaseResponse<List<PostEntity>>.Failure(ex));
            }
        }
    }
}
