namespace SemsamECS.Core.Unity
{
    /// <summary>
    /// A provider for creating an entity.
    /// </summary>
    public sealed class EntityProvider : UnityEngine.MonoBehaviour
    {
        [UnityEngine.SerializeField] private int _worldId;
        [UnityEngine.SerializeField] private bool _isDestroyingAfter;

        private IWorlds _worldContainer;

        private void Start()
        {
            if (WorldsInstance.TryGet(out var worldContainer))
            {
                _worldContainer = worldContainer;
                AttemptUseWorld();
            }
            else
                WorldsInstance.Constructed += OnWorldsConstructed;
        }

        private void OnWorldsConstructed(IWorlds worldContainer)
        {
            WorldsInstance.Constructed -= OnWorldsConstructed;
            _worldContainer = worldContainer;
            AttemptUseWorld();
        }

        private void AttemptUseWorld()
        {
            if (_worldContainer.Have(_worldId))
                ConstructEntity(_worldContainer.Get(_worldId));
            else
            {
                _worldContainer.Created += OnWorldCreated;
                WorldsInstance.Disposed += OnWorldsDisposed;
            }
        }

        private void OnWorldCreated(IWorld world)
        {
            if (world.Id != _worldId)
                return;
            _worldContainer.Created -= OnWorldCreated;
            WorldsInstance.Disposed -= OnWorldsDisposed;
            ConstructEntity(world);
        }

        private void OnWorldsDisposed(IWorlds worldContainer)
        {
            _worldContainer.Created -= OnWorldCreated;
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

        private void AddComponents(IPools poolContainer, int entity)
        {
            var components = new System.ReadOnlySpan<IComponentProvider>(GetComponents<IComponentProvider>());
            foreach (var component in components)
                component.CreateComponent(poolContainer, entity);
        }
    }
}