using System;
using UnityEngine;

namespace manhnd_sdk.Scripts.Optimization.PoolingSystem
{
    public class classA : MonoBehaviour
    {
        private async void Awake()
        {
            Transform t = await ObjectPooler.GetFromPool<Transform>(PoolingType.Separator, this.destroyCancellationToken);
        }
    }
}