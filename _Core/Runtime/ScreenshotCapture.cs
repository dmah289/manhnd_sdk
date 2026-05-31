using manhnd_sdk.Runtime.SystemDesign;
using UnityEngine;

#if UNITY_EDITOR
using System.IO;
using UnityEditor;
#endif

namespace manhnd_sdk.Runtime
{
    public class ScreenshotCapture : MonoSingleton<ScreenshotCapture>
    {
        [SerializeField] private Camera myCamera;
        [SerializeField] private int resolutionWidth = 1080;
        [SerializeField] private int resolutionHeight = 1920;
        [SerializeField] private int scale = 1;
        [SerializeField] private bool isTransparent;

        [SerializeField] private KeyCode screenshotKey = KeyCode.S;
        [SerializeField] private string saveFolderName = "Screenshots";
        [SerializeField] private string savePath = "";

        protected override void Awake()
        {
            if (myCamera == null)
                myCamera = Camera.main;

            #if UNITY_EDITOR
            savePath = Path.Combine(Path.GetDirectoryName(Application.dataPath), saveFolderName);
            #endif
        }
    }
}
