namespace sevaLK_service_auth.Infra.Config;

public class AwsS3Options
{
    public const string SectionName = "AwsS3";

    public string BucketName { get; set; } = string.Empty;
    public string Region { get; set; } = string.Empty;
    public int DefaultSignedUrlExpiryMinutes { get; set; } = 10;
}
