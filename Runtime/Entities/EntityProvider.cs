namespace SemsamECS.Core.Unity
{
    /// <summary>
    /// A provider for creating an entity.
    /// </summary>
    public sealed class EntityProvider : UnityEngine.MonoBehaviour
    {
        [UnityEngine.SerializeField] private int _worldId;
        [UnityEngine.SerializeField] private bool _isDestroyingAfter;

        private IWorlds _worlds;

        private void Start()
        {
            if (WorldsInstance.TryGet(out var worlds))
            {
                _worlds = worlds;
                AttemptUseWorld();
            }
            else
                WorldsInstance.Constructed += OnWorldsConstructed;
        }

        private void OnWorldsConstructed(IWorlds worlds)
        {
            WorldsInstance.Constructed -= OnWorldsConstructed;
            _worlds = worlds;
            AttemptUseWorld();
        }

        private void AttemptUseWorld()
        {
            if (_worlds.Have(_worldId))
                ConstructEntity(_worlds.Get(_worldId));
            else
            {
                _worlds.Created += OnWorldCreated;
                WorldsInstance.Disposed += OnWorldsDisposed;
            }
        }

        private void OnWorldCreated(IWorld world)
        {
            if (world.Id != _worldId)
                return;
            _worlds.Created -= OnWorldCreated;
            WorldsInstance.Disposed -= OnWorldsDisposed;
            ConstructEntity(world);
        }

        private void OnWorldsDisposed(IWorlds worlds)
        {
            _worlds.Created -= OnWorldCreated;
            WorldsInstance.Disposed -= OnWorldsDisposed;
            WorldsInstance.Constructed += OnWorldsConstructed;
        }

        private void ConstructEntity(IWorld world)
        {
            var pools = world.Pools;
            var entity = world.Entities.Create();
            AddComponents(pools, entity);
            if (_isDestroyingAfter)
                Destroy(gameObject);
        }

        private void AddComponents(IPools pools, int entity)
        {
            var components = new System.ReadOnlySpan<IComponentProvider>(GetComponents<IComponentProvider>());
            foreach (var component in components)
                component.CreateComponent(pools, entity);
        }
    }
}