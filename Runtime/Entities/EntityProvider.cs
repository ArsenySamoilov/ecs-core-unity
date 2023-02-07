namespace SemsamECS.Core.Unity
{
    public sealed class EntityProvider : UnityEngine.MonoBehaviour
    {
        [UnityEngine.SerializeField] private PooledWorldProvider _worldProvider;
        [UnityEngine.SerializeField] private bool _haveDestroyAfterConversion;

        private System.Action<IPools, int>[] _delayedConversions;
        private int _delayedConversionCount;

        private IPools _pools;
        private int _entity;
        private bool _isStarted;

        private void Start()
        {
            var world = _worldProvider.GetWorld();
            _pools = world.Pools;
            _entity = world.Entities.Create();
            _isStarted = true;

            ExecuteDelayedConversions();
            
            if (_haveDestroyAfterConversion)
                Destroy(gameObject, 1f);
        }

        public void AddComponent<TComponent>(TComponent component, bool isTag) where TComponent : struct
        {
            if (_isStarted)
            {
                ExecuteConversion(component, isTag);
                return;
            }

            if (_delayedConversions.Length == _delayedConversionCount)
                System.Array.Resize(ref _delayedConversions, _delayedConversionCount + 1);
            DelayConversion(component, isTag);
        }

        private void ExecuteDelayedConversions()
        {
            var delayedConversionsAsSpan = new System.Span<System.Action<IPools, int>>(_delayedConversions, 0, _delayedConversionCount);
            foreach (var conversion in delayedConversionsAsSpan)
                conversion.Invoke(_pools, _entity);
        }

        private void ExecuteConversion<TComponent>(TComponent component, bool isTag) where TComponent : struct
        {
            if (isTag)
                _pools.Get<TComponent>().Create(_entity);
            else
                _pools.Get<TComponent>().Create(_entity, component);
        }

        private void DelayConversion<TComponent>(TComponent component, bool isTag) where TComponent : struct
        {
            if (isTag)
                _delayedConversions[_delayedConversionCount++] = (world, entity) => world.Get<TComponent>().Create(entity);
            else
                _delayedConversions[_delayedConversionCount++] = (world, entity) => world.Get<TComponent>().Create(entity, component);
        }
    }
}