namespace Post.Cmd.Infrastructure.Config;

public class MongoDbConfig
{
    public const string Key = nameof(MongoDbConfig);

    public string ConnectionString { get; set; }

    public string DatabaseName { get; set; }

    public string CollectionName { get; set; }
}