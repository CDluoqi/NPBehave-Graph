using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor.AssetImporters;
using UnityEditor.Callbacks;
using UnityEngine;

namespace UnityEditor.BehaveGraph
{
    public class BehaveGraphImporterEditor : ScriptedImporterEditor
    {
        internal static bool ShowGraphEditWindow(string path)
        {
            var guid = AssetDatabase.AssetPathToGUID(path);
            
            var extension = Path.GetExtension(path);
            if (string.IsNullOrEmpty(extension))
                return false;
            // Path.GetExtension returns the extension prefixed with ".", so we remove it. We force lower case such that
            // the comparison will be case-insensitive.
            extension = extension.Substring(1).ToLowerInvariant();
            
            if (extension != BehaveGraphImporter.Extension)
                return false;
            
            foreach (var w in Resources.FindObjectsOfTypeAll<NPBehaveGraphEditorWindow>())
            {
                if (w.selectedGuid == guid)
                {
                    w.Focus();
                    return true;
                }
            }
            
            
            var window = EditorWindow.CreateWindow<NPBehaveGraphEditorWindow>(typeof(NPBehaveGraphEditorWindow), typeof(SceneView));
            window.Initialize(guid);
            window.Focus();
            return true;
        }

        [OnOpenAsset(0)]
        public static bool OnOpenAsset(int instanceID, int line)
        {
            var path = AssetDatabase.GetAssetPath(instanceID);
            return ShowGraphEditWindow(path);
        }
    }
}


