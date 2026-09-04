namespace Api.Works.QueryTypes;

public class Work : IEntityMetadata
{
    public static byte IdPostfix => Models.Work.IdPostfix;

    public Guid Id { get; set; }
    public int RowVersion { get; set; }
    public NodaTime.Instant MetadataAddedAt { get; set; }
    public NodaTime.Instant MetadataUpdatedAt { get; set; }

    public required string Title { get; set; }

    public string? Description { get; set; }

    public NodaTime.ZonedDateTime? WorkPublishedAt { get; set; }
    public NodaTime.ZonedDateTime? WorkUpdatedAt { get; set; }
    public List<WorkIdentifier> WorkIdentifiers { get; set; } = [];

    public record WorkIdentifier(
            string WorkIdentifierType,
            string WorkIdentifierValue);
}