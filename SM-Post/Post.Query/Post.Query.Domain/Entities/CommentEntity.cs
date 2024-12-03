﻿using System.Text.Json.Serialization;

namespace Post.Query.Domain.Entities;

public class CommentEntity
{
    public Guid CommentId { get; set; }

    public string UserName { get; set; }

    public DateTime CommentDate { get; set; }

    public string Comment { get; set; }

    public bool Edited { get; set; }

    public Guid PostId { get; set; }

    [JsonIgnore]
    public virtual PostEntity Post { get; set; }
}