using UnityEngine;

namespace manhnd_sdk.Scripts.ExtensionMethods
{
    public static class CameraExtensions
    {
        /// <summary>
        /// Get viewport size in world space at a given distance from camera to plane that need calculating
        /// </summary>
        /// <param name="distance">The distance from camera to plane that need calculating</param>
        /// <param name="viewportMargin">The addition margin offset to the viewport</param>
        /// <returns>Viewport size in world space</returns>
        public static Vector2 GetViewportSizeAtDistance(this Camera camera, float distance, Vector2? viewportMargin = null)
        {
            Vector2 margin = viewportMargin ?? Vector2.zero;
            
            float height = 2f * distance * Mathf.Tan(camera.fieldOfView * 0.5f * Mathf.Deg2Rad) + margin.y;
            float width = height * camera.aspect + margin.x;
            return new Vector2(width, height);
        }
    }
}