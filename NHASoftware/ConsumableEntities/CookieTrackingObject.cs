namespace NHASoftware.ConsumableEntities
{
    public class CookieTrackingObject
    {
        public int ObjectIdentifierId { get; set; }
        public string CookieGuid { get; set; }

        public override bool Equals(object? obj)
        {
            if(obj == null) return false;
            var other = obj as CookieTrackingObject;
            return other.CookieGuid == CookieGuid && other.ObjectIdentifierId == ObjectIdentifierId;
        }
    }
}
