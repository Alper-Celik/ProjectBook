public interface IEntityMetadata
{
    public Guid Id { get; }
    public int RowVersion { get; }
    public NodaTime.Instant MetadataAddedAt { get; }
    public NodaTime.Instant MetadataUpdatedAt { get; }
}