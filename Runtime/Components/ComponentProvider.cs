using UnityEngine;

namespace SemsamECS.Core.Unity
{
    [RequireComponent(typeof(EntityProvider))]
    public abstract class ComponentProvider<TComponent> : MonoBehaviour where TComponent : struct
    {
        [SerializeField] private EntityProvider _entityProvider;
        [SerializeField] private TComponent _component;
        [SerializeField] private bool _isTag;

        private void Reset()
        {
            _entityProvider = GetComponent<EntityProvider>();
        }

        private void Start()
        {
            _entityProvider.AddComponent(_component, _isTag);
        }
    }
}