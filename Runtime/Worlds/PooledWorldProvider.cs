namespace SemsamECS.Core.Unity
{
    public abstract class PooledWorldProvider : UnityEngine.MonoBehaviour
    {
        public abstract PooledWorld GetWorld();
    }
}