namespace SemsamECS.Core.Unity
{
    /// <summary>
    /// An interface of a component provider for creating a component for an entity.
    /// </summary>
    public interface IComponentProvider
    {
        /// <summary>
        /// Creates a component for the entity.
        /// </summary>
        void CreateComponent(IPools pools, int entity);
    }
}