using CQRS.Core.Events;
using Post.Common.Events;
using Post.Query.Domain.Entities;
using Post.Query.Domain.Repositories;

namespace Post.Query.Infrastructure.Handlers;

public class EventHandler : IEventHandler
{
    private readonly IPostRepository _postRepository;
    private readonly ICommentRepository _commentRepository;

    public EventHandler(IPostRepository postRepository, ICommentRepository commentRepository)
    {
        _postRepository = postRepository;
        _commentRepository = commentRepository;
    }
    public async Task On(PostCreatedEvent @event)
    {
        var post = new PostEntity()
        {
            Id = @event.Id,
            Author = @event.Author,
            DatePosted = @event.DatePosted,
            Message = @event.Message
        };
        await this._postRepository.CreateAsync(post);
    }

    public async Task On(MessageUpdatedEvent @event)
    {
        var post = await this._postRepository.GetByIdAsync(@event.Id);
        if (post == null)
        {
            return;
        }

        post.Message = @event.Message;

        await this._postRepository.UpdateAsync(post);
    }

    public async Task On(PostLikedEvent @event)
    {
        var post = await this._postRepository.GetByIdAsync(@event.Id);
        if (post == null)
        {
            return;
        }

        post.Likes++;
        await this._postRepository.UpdateAsync(post);
    }

    public async Task On(CommentAddedEvent @event)
    {
        var comment = new CommentEntity()
        {
            PostId = @event.Id,
            CommentId = @event.CommentId,
            CommentDate = @event.CommentDate,
            Comment = @event.Comment,
            UserName = @event.UserName
        };

        await this._commentRepository.CreateAsync(comment);
    }

    public async Task On(CommentRemovedEvent @event)
    {
        await this._commentRepository.DeleteAsync(@event.CommentId);
    }

    public async Task On(CommentUpdatedEvent @event)
    {
        var comment = await this._commentRepository.GetByIdAsync(@event.CommentId);
        if (comment == null)
        {
            return;
        }

        comment.Comment = @event.Comment;
        comment.Edited = true;
        comment.CommentDate = @event.EditDate;

        await this._commentRepository.UpdateAsync(comment);
    }

    public async Task On(PostRemovedEvent @event)
    {
        await this._postRepository.DeleteAsync(@event.Id);
    }

    public async Task On(Type type, BaseEvent @event)
    {
        var onMethod = this.GetType()
                .GetMethods().FirstOrDefault(m => m.Name == nameof(On) && m.GetParameters().FirstOrDefault(p => p.ParameterType == type)?.HasDefaultValue != null);
        await (Task)onMethod?.Invoke(this, new[] { @event });

    }
}