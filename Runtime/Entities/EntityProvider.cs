namespace SemsamECS.Core.Unity
{
    /// <summary>
    /// A provider for creating an entity.
    /// </summary>
    public sealed class EntityProvider : UnityEngine.MonoBehaviour
    {
        [UnityEngine.SerializeField] private int _worldIndex;
        [UnityEngine.SerializeField] private bool _isDestroyingAfter;

        private Worlds _worlds;

        private void Start()
        {
            if (Worlds.IsCreated)
                AttemptUseWorld(Worlds.GetInstance());
            else
                Worlds.Constructed += OnWorldsConstructed;
        }

        private void OnWorldsConstructed(Worlds worlds)
        {
            Worlds.Constructed -= OnWorldsConstructed;
            AttemptUseWorld(worlds);
        }

        private void AttemptUseWorld(Worlds worlds)
        {
            _worlds = worlds;
            if (worlds.Have(_worldIndex))
                ConstructEntity(worlds.Get(_worldIndex));
            else
                worlds.Added += OnWorldAdded;
        }

        private void OnWorldAdded(IWorld world, int index)
        {
            if (index != _worldIndex)
                return;

            _worlds.Added -= OnWorldAdded;
            ConstructEntity(world);
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
            var components = new System.Span<IComponentProvider>(GetComponents<IComponentProvider>());
            foreach (var component in components)
                component.CreateComponent(pools, entity);
        }
    }
}