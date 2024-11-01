using UnityEngine;

namespace Utility.Behaviour
{
    public class HoverAroundBehaviour : MonoBehaviour
    {
        private Transform _transform;
        
        [Header("参数")]
        [SerializeField] private float hoverHeight = 0.1f;
        [SerializeField] private float hoverSpeed = 1f;
        
        private void Awake()
        {
            _transform = transform;
        }
        
        private void Update()
        {
            _transform.localPosition = new Vector3( 0, Mathf.Sin(Time.time * hoverSpeed) * hoverHeight, 0);
        }
    }
}