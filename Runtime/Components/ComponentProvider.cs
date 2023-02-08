namespace SemsamECS.Core.Unity
{
    /// <summary>
    /// An abstract provider for creating a component of type <typeparamref name="TComponent"/> for an entity.
    /// </summary>
    public abstract class ComponentProvider<TComponent> : UnityEngine.MonoBehaviour, IComponentProvider where TComponent : struct
    {
        [UnityEngine.SerializeField] private TComponent _component;

        /// <summary>
        /// Creates a component for the entity using the pools.
        /// </summary>
        public void CreateComponent(IPools pools, int entity)
        {
            pools.Get<TComponent>().Create(entity, _component);
        }
    }
}