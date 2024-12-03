using CQRS.Core.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Post.Cmd.Api.Commands;
using Post.Common.Response;

namespace Post.Cmd.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PostController : ControllerBase
{
    private readonly ICommandDispatcher _commandDispatcher;
    private readonly ILogger<PostController> _logger;

    public PostController(ICommandDispatcher commandDispatcher, ILogger<PostController> logger)
    {
        _commandDispatcher = commandDispatcher;
        _logger = logger;
    }

    [HttpPost]
    public async Task<ActionResult<BaseResponse<Guid>>> CreateNewPost(NewPostCommand command)
    {
        var id = Guid.NewGuid();
        this._logger.LogInformation("Create new post with Id: {Id}", id);
        try
        {
            command.Id = id;
            await this._commandDispatcher.SendAsync(command);
            this._logger.LogInformation("Post created: {Id}", id);
            return BaseResponse<Guid>.OkResult(id);
        }
        catch (Exception ex)
        {
            this._logger.LogError("Error while create post, Message: {Message}", ex.Message);
            return BaseResponse<Guid>.Failure(ex);
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<BaseResponse<Guid>>> UpdateMessage(Guid id, EditMessageCommand command)
    {
        this._logger.LogInformation("Edit post with Id: {Id}", id);

        try
        {
            command.Id = id;
            await this._commandDispatcher.SendAsync(command);
            return BaseResponse<Guid>.OkResult(id);
        }
        catch (Exception ex)
        {
            this._logger.LogError("Error while edit post, Message: {Message}", ex.Message);
            return BaseResponse<Guid>.Failure(ex);
        }
    }

    [HttpPost("{id:guid}/like")]
    public async Task<ActionResult<BaseResponse<Guid>>> LikePost(Guid id)
    {
        this._logger.LogInformation("Like post with Id: {Id}", id);
        try
        {
            await this._commandDispatcher.SendAsync(new LikePostCommand()
            {
                Id = id
            });
            return BaseResponse<Guid>.OkResult(id);
        }
        catch (Exception ex)
        {
            this._logger.LogError("Error while like post, Message: {Message}", ex.Message);
            return BaseResponse<Guid>.Failure(ex);
        }
    }


    [HttpPost("{id:guid}/comment")]
    public async Task<ActionResult<BaseResponse<Guid>>> AddComment(Guid id, AddCommentCommand command)
    {
        this._logger.LogInformation("Create comment for post post with Id: {Id}", id);
        try
        {
            command.Id = id;
            await this._commandDispatcher.SendAsync(command);
            return BaseResponse<Guid>.OkResult(id);
        }
        catch (Exception ex)
        {
            this._logger.LogError("Error while create comment for post, Message: {Message}", ex.Message);
            return BaseResponse<Guid>.Failure(ex);
        }
    }

    [HttpPut("{id:guid}/comment")]
    public async Task<ActionResult<BaseResponse<Guid>>> UpdateComment(Guid id, EditCommentCommand command)
    {
        this._logger.LogInformation("Update comment for post with Id: {Id}", id);
        try
        {
            command.Id = id;
            await this._commandDispatcher.SendAsync(command);
            return BaseResponse<Guid>.OkResult(id);
        }
        catch (Exception ex)
        {
            this._logger.LogError("Error while edit comment post, Message: {Message}", ex.Message);
            return BaseResponse<Guid>.Failure(ex);
        }
    }

    [HttpDelete("{id:guid}/comment")]
    public async Task<ActionResult<BaseResponse<Guid>>> DeleteComment(Guid id, RemoveCommentCommand command)
    {
        this._logger.LogInformation("Delete post comment with Id: {Id}", id);
        try
        {
            command.Id = id;
            await this._commandDispatcher.SendAsync(command);
            return BaseResponse<Guid>.OkResult(id);
        }
        catch (Exception ex)
        {
            this._logger.LogError("Error while delete post comment, Message: {Message}", ex.Message);
            return BaseResponse<Guid>.Failure(ex);
        }
    }


    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<BaseResponse<Guid>>> DeletePost(Guid id, DeletePostCommand command)
    {
        this._logger.LogInformation("Delete post with Id: {Id}", id);
        try
        {
            command.Id = id;
            await this._commandDispatcher.SendAsync(command);
            return BaseResponse<Guid>.OkResult(id);
        }
        catch (Exception ex)
        {
            this._logger.LogError("Error while delete post, Message: {Message}", ex.Message);
            return BaseResponse<Guid>.Failure(ex);
        }
    }

    [HttpPost("restoreDb")]
    public async Task<ActionResult<BaseResponse<bool>>> RestoreDb()
    {
        this._logger.LogInformation("Start restoring DataBase");
        try
        {
            await this._commandDispatcher.SendAsync(new RestoreDataBaseCommand()
            {

            });
            return BaseResponse<bool>.OkResult(true, "Database restored Successful.");
        }
        catch (Exception ex)
        {
            this._logger.LogError("Error while restoring database, Message: {Message}", ex.Message);
            return BaseResponse<bool>.Failure(ex);
        }
    }
}