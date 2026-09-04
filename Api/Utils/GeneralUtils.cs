namespace Api.Utils;


public static class GeneralUtils
{

    public static NodaTime.Instant Now() => NodaTime.SystemClock.Instance.GetCurrentInstant();

    public static byte[] NewRowVersion()
    {
        var result = new byte[9];
        Random.Shared.NextBytes(result);
        return result;
    }

}