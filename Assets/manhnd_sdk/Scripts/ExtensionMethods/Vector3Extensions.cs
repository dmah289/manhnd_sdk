using UnityEngine;

namespace manhnd_sdk.Scripts.ExtensionMethods
{
    public static class Vector3Extensions
    {
        #region Element-based modification

        public static Vector3 With(this Vector3 v, float? x = null, float? y = null, float? z = null)
            => new Vector3(x ?? v.x, y ?? v.y, z ?? v.z);

        public static Vector3 Add(this Vector3 v, float? x = null, float? y = null, float? z = null)
            => new Vector3(v.x + (x ?? 0), v.y + (y ?? 0), v.z + (z ?? 0));
        
        public static Vector3 Multiply(this Vector3 v, float? mX = null, float? mY = null, float? mZ = null)
            => new Vector3(v.x * (mX ?? 1), v.y * (mY ?? 1), v.z * (mZ ?? 1));

        #endregion

        #region Object-based modification

        public static Vector3 Add(this Vector3 v, float amount)
            => v.Add(amount, amount, amount);

        #endregion


    }
}