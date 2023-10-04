namespace NHASoftware.ConsumableEntities
{
    public class CookieTrackingObject
    {
        public int ObjectIdentifierId { get; set; }
        public string CookieGuid { get; set; } = string.Empty;

        public override bool Equals(object? obj)
        {
            if (obj is CookieTrackingObject other)
            {
                return other.CookieGuid == CookieGuid && other.ObjectIdentifierId == ObjectIdentifierId;
            }

            return false;
        }
    }
}
