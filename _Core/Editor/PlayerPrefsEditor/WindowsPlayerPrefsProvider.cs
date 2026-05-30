using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Win32;
using UnityEditor;
using UnityEngine;

namespace manhnd_sdk.Editor.PlayerPrefsEditor
{
    public class WindowsPlayerPrefsProvider : IPlayerPrefsProvider
    {
        private static Encoding encoding = new UTF8Encoding();
        private static string PlayerPrefsPath = $@"Software\Unity\UnityEditor\{Application.companyName}\{Application.productName}";
        private static List<PlayerPrefsPair> cachedPlayerPrefsPairs = new();

        public List<PlayerPrefsPair> PlayerPrefsPairs
        {
            get
            {
                using(RegistryKey registryKey = Registry.CurrentUser.OpenSubKey(PlayerPrefsPath))
                {
                    if (registryKey != null)
                    {
                        string[] fileNames = registryKey.GetValueNames();
                        cachedPlayerPrefsPairs.Clear();
                    
                        for (int i = 0; i < fileNames.Length; i++)
                        {
                            // Remove the _h193410979 style suffix used on PlayerPref keys in Windows registry
                            int idx = fileNames[i].LastIndexOf("_", StringComparison.Ordinal);
                            string standardFileName = idx > 0 ? fileNames[i].Substring(0, idx) : fileNames[i];
                        
                            object fileContent = registryKey.GetValue(fileNames[i]);
                        
                            // Unfortunately floats will come back as an int (at least on 64 bit) because the float is stored as
                            // 64 bit but marked as 32 bit - which confuses the GetValue() method greatly!
                            if (fileContent is int)
                            {
                                if (PlayerPrefs.GetInt(standardFileName, -1) == -1 && PlayerPrefs.GetInt(standardFileName, 0) == 0)
                                    fileContent = PlayerPrefs.GetFloat(standardFileName, 0f);
                            }
                            else if (fileContent is byte[])
                                fileContent = encoding.GetString((byte[])fileContent).TrimEnd('\0');
                        
                            cachedPlayerPrefsPairs.Add(new PlayerPrefsPair{Key = standardFileName, Value = fileContent});
                        }
                        return cachedPlayerPrefsPairs;
                    }
                }

                return cachedPlayerPrefsPairs;
            }
        }
    }
}