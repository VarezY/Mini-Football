using UnityEngine;

namespace MiniFootball
{
    public class GetObjectLocalPosition : MonoBehaviour
    {
        private void Start()
        {
            Debug.Log($"Local: {transform.localPosition}");
            Debug.Log($"World: {transform.position}");
        }
    }
}