namespace SemsamECS.Core.Unity
{
    /// <summary>
    /// An abstract component provider for creating a component for an entity.
    /// </summary>
    /// <typeparam name="TComponent">The type of the component.</typeparam>
    public abstract class ComponentProvider<TComponent> : UnityEngine.MonoBehaviour, IComponentProvider where TComponent : struct
    {
        [UnityEngine.SerializeField] private TComponent _component;

        public TComponent Component => _component;

        /// <summary>
        /// Creates a component for the entity.
        /// </summary>
        public void CreateComponent(IPools poolContainer, int entity)
        {
            if (poolContainer.Have<TComponent>())
                poolContainer.Get<TComponent>().Create(entity, _component);
            else
                poolContainer.Create<TComponent>().Create(entity, _component);
        }
    }
}