namespace SemsamECS.Core.Unity
{
    /// <summary>
    /// A provider for creating an entity.
    /// </summary>
    public sealed class EntityProvider : UnityEngine.MonoBehaviour
    {
        [UnityEngine.SerializeField] private int _worldIndex;
        [UnityEngine.SerializeField] private bool _isDestroyingAfter;

        private void Start()
        {
            if (WorldsInstance.TryGet(out var worlds))
                AttemptUseWorld((Worlds)worlds);
            else
                WorldsInstance.Constructed += OnWorldsConstructed;
        }

        private void OnWorldsConstructed(Worlds worlds)
        {
            WorldsInstance.Constructed -= OnWorldsConstructed;
            AttemptUseWorld(worlds);
        }

        private void AttemptUseWorld(Worlds worlds)
        {
            var worldsAsSpan = worlds.GetWorlds();
            if (_worldIndex < worldsAsSpan.Length)
                ConstructEntity(worldsAsSpan[_worldIndex]);
            else
                worlds.Created += OnWorldCreated;
        }

        private void OnWorldCreated(BoxedWorld world)
        {
            if (world.Index != _worldIndex)
                return;

            ((Worlds)WorldsInstance.Get()).Created -= OnWorldCreated;
            ConstructEntity(world.World);
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