namespace PoetryGlobal.Session
{
    public class PermissiveCurrentSession : ICurrentSession
    {
        public Guid Guid { get; } = Guid.Parse("00000000-0000-0000-0000-000000000000");
    }
}