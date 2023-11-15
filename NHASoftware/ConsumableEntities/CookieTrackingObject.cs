namespace NHA.Website.Software.ConsumableEntities;
public class CookieTrackingObject
{
    public readonly int ObjectIdentifierId;
    public readonly string CookieGuid;

    public CookieTrackingObject(int objectIdentifierId, string cookieGuid)
    {
        ObjectIdentifierId = objectIdentifierId;
        CookieGuid = cookieGuid;
    }

    public override bool Equals(object? obj)
    {
        if (obj is CookieTrackingObject other)
        {
            return other.CookieGuid == CookieGuid && other.ObjectIdentifierId == ObjectIdentifierId;
        }

        return false;
    }

    public override int GetHashCode()
    {
        int hash = 13;
        hash = (hash * 7) + ObjectIdentifierId.GetHashCode();
        hash = (hash * 7) + CookieGuid.GetHashCode();
        return hash;
    }
}
