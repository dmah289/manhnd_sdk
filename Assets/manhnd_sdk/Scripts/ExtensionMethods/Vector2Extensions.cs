using UnityEngine;

namespace manhnd_sdk.Scripts.ExtensionMethods
{
    public static class Vector2Extensions
    {
        /// <summary>
        /// Get a random point within an annulus defined by minRadius and maxRadius from the origin point. <br></br>
        /// Apply annulus area-based distribution.
        /// </summary>
        /// <param name="origin">The origin point to consider</param>
        /// <param name="minRadius">Min radius</param>
        /// <param name="maxRadius">Max radius</param>
        /// <returns></returns>
        public static Vector2 RandomPointInAnnulus(this Vector2 origin, float minRadius, float maxRadius)
        {
            // Get vector direction by random angle
            float angle = 2f * Mathf.PI * Random.value;
            Vector2 direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
            
            float minRadiusSq = minRadius * minRadius;
            float maxRadiusSq = maxRadius * maxRadius;
            // Get radius with annulus area-based distribution
            float radius = Mathf.Sqrt(Random.value * (maxRadiusSq - minRadiusSq) + minRadiusSq);
            
            Vector2 position = direction * radius;
            return origin + position;
        }
    }
}