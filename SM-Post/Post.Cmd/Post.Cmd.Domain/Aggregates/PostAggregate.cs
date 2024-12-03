using CQRS.Core.Domain;
using Post.Common.Events;

namespace Post.Cmd.Domain.Aggregates
{
    public class PostAggregate : AggregateRoot
    {
        private bool _active;
        private string _author;
        private readonly Dictionary<Guid, (string, string)> _comments = new();

        public bool Active
        {
            get => _active;
            set => _active = value;
        }

        public PostAggregate()
        {

        }

        public PostAggregate(Guid id, string author, string message)
        {
            RaiseEvent(new PostCreatedEvent()
            {
                Id = id,
                Author = author,
                Message = message,
                DatePosted = DateTime.Now
            });
        }

        public void Apply(PostCreatedEvent @event)
        {
            _id = @event.Id;
            _author = @event.Author;
            _active = true;
        }


        public void EditMessage(string message)
        {
            if (!_active)
            {
                throw new InvalidOperationException("You can not edit message of inactive post!");
            }

            if (string.IsNullOrWhiteSpace(message))
            {
                throw new InvalidOperationException($"The value of {nameof(message)} can not be null or empty. Please provide a valid {nameof(message)}");
            }

            RaiseEvent(new MessageUpdatedEvent()
            {
                Id = _id,
                Message = message
            });
        }

        public void Apply(MessageUpdatedEvent @event)
        {
            _id = @event.Id;
        }

        public void LikePost()
        {
            if (!_active)
            {
                throw new InvalidOperationException("You can not like inactive post!");
            }

            RaiseEvent(new PostLikedEvent()
            {
                Id = _id
            });
        }

        public void Apply(PostLikedEvent @event)
        {
            _id = @event.Id;
        }



        public void AddComment(string comment, string userName)
        {
            if (!_active)
            {
                throw new InvalidOperationException("You can not add comment to inactive post!");
            }

            if (string.IsNullOrWhiteSpace(comment))
            {
                throw new InvalidOperationException($"The value of {nameof(comment)} can not be null or empty. Please provide a valid {nameof(comment)}");
            }

            RaiseEvent(new CommentAddedEvent()
            {
                Id = _id,
                Comment = comment,
                UserName = userName,
                CommentId = Guid.NewGuid(),
                CommentDate = DateTime.Now,
            });
        }

        public void Apply(CommentAddedEvent @event)
        {
            _id = @event.Id;
            _comments.Add(@event.CommentId, (@event.Comment, @event.UserName));
        }


        public void EditComment(Guid commentId, string comment, string userName)
        {
            if (!_active)
            {
                throw new InvalidOperationException("You can not edit comment to inactive post!");
            }

            if (string.IsNullOrWhiteSpace(comment))
            {
                throw new InvalidOperationException($"The value of {nameof(comment)} can not be null or empty. Please provide a valid {nameof(comment)}");
            }

            var oldComment = _comments[commentId];
            if (oldComment.Item2.Equals(userName, StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    "You are not allowed to edit a comment that was made by another user!");
            }

            RaiseEvent(new CommentUpdatedEvent()
            {
                Id = _id,
                Comment = comment,
                CommentId = commentId,
                EditDate = DateTime.Now,
                UserName = userName
            });
        }

        public void Apply(CommentUpdatedEvent @event)
        {
            _id = @event.Id;
            _comments[@event.CommentId] = (@event.Comment, @event.UserName);
        }


        public void RemoveComment(Guid commentId, string userName)
        {
            if (!_active)
            {
                throw new InvalidOperationException("You can not edit comment to inactive post!");
            }


            var oldComment = _comments[commentId];
            if (oldComment.Item2.Equals(userName, StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    "You are not allowed to remove a comment that was made by another user!");
            }

            RaiseEvent(new CommentRemovedEvent()
            {
                Id = _id,
                CommentId = commentId
            });
        }

        public void Apply(CommentRemovedEvent @event)
        {
            _id = @event.Id;
            _comments.Remove(@event.CommentId);
        }

        public void DeletePost(string userName)
        {
            if (!_active)
            {
                throw new InvalidOperationException("You can not remove post to inactive post!");
            }

            if (!_author.Equals(userName, StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    "You are not allowed to remove post that was made by another user!");
            }

            RaiseEvent(new PostRemovedEvent()
            {
                Id = _id
            });
        }

        public void Apply(PostRemovedEvent @event)
        {
            _id = @event.Id;
            _active = false;
        }
    }
}
