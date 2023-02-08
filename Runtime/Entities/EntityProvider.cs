namespace SemsamECS.Core.Unity
{
    /// <summary>
    /// A provider for creating an entity.
    /// </summary>
    public sealed class EntityProvider : UnityEngine.MonoBehaviour
    {
        [UnityEngine.SerializeField] private PooledWorldProvider _worldProvider;
        [UnityEngine.SerializeField] private bool _isDestroyingAfter;

        private IPools _pools;
        private int _entity;

        private void Start()
        {
            var world = _worldProvider.GetWorld();
            _pools = world.Pools;
            _entity = world.Entities.Create();

            AddComponents();

            if (_isDestroyingAfter)
                Destroy(gameObject);
        }

        private void AddComponents()
        {
            var components = new System.Span<IComponentProvider>(GetComponents<IComponentProvider>());
            foreach (var component in components)
                component.CreateComponent(_pools, _entity);
        }
    }
}