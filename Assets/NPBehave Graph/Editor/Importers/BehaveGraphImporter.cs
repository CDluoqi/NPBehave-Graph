using System.IO;
using System.Text;
using UnityEditor.AssetImporters;
using UnityEditor.BehaveGraph.Serialization;
using UnityEngine;
using NPBehave;

namespace UnityEditor.BehaveGraph
{
    [ScriptedImporter(1, Extension, -900)]
    public class BehaveGraphImporter : ScriptedImporter
    {
        public const string Extension = "behavegraph";
        
        public override void OnImportAsset(AssetImportContext ctx)
        {
            string path = ctx.assetPath;
            TextAsset mainAsset = new TextAsset(File.ReadAllText(ctx.assetPath));
            Texture2D texture = Resources.Load<Texture2D>("Icons/ase64");
            if (texture == null)
            {
                Debug.LogError("No Icon");
            }

            var graph = new GraphData();
            MultiJson.Deserialize(graph, mainAsset.text);
            graph.OnEnable();
            graph.ValidateGraph();
            
            string config = graph.ConvertToConfig();
            NPBehaveTreeAsset behaveTreeAsset = NPBehaveTreeAsset.Create(config);
            behaveTreeAsset.name = Path.GetFileNameWithoutExtension(ctx.assetPath);
            Debug.Log(config);
            ctx.AddObjectToAsset("MainAsset", mainAsset, texture);
            ctx.AddObjectToAsset("BehaveTreeAsset", behaveTreeAsset);
            ctx.SetMainObject(mainAsset);
        }
    }
    

}

