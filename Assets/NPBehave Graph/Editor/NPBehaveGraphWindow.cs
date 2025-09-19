using System;
using System.IO;
using System.Text;
using UnityEditor.BehaveGraph.Serialization;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace UnityEditor.BehaveGraph
{
    public class NPBehaveGraphEditorWindow : EditorWindow
    {
        [NonSerialized]
        bool m_FrameAllAfterLayout;
        [SerializeField]
        string m_Selected = "AAAA";
        [NonSerialized]
        bool m_HasError;
        
        [SerializeField]
        string m_LastSerializedFileContents;
        [SerializeField]
        GraphObject m_GraphObject;
        
        internal GraphObject graphObject
        {
            get { return m_GraphObject; }
            set
            {
                if (m_GraphObject != null)
                    DestroyImmediate(m_GraphObject);
                m_GraphObject = value;
            }
        }
        
        public string selectedGuid
        {
            get { return m_Selected; }
            private set
            {
                m_Selected = value;
            }
        }
        
        NPBehaveGraphEditorView m_GraphEditorView;
        internal NPBehaveGraphEditorView graphEditorView
        {
            get { return m_GraphEditorView; }
            
            private set
            {
                if (m_GraphEditorView != null)
                {
                    m_GraphEditorView.RemoveFromHierarchy();
                    m_GraphEditorView.Dispose();
                }
    
                m_GraphEditorView = value;
    
                if (m_GraphEditorView != null)
                {
                    m_GraphEditorView.saveRequested += () => SaveAsset();
                    m_GraphEditorView.RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);
                    m_FrameAllAfterLayout = true;
                    rootVisualElement.Add(m_GraphEditorView);
                }
            }
        }
        
        [MenuItem("Window/NPBehaveGraphWindow")]
        public static void OpenWindow()
        {
            var window = GetWindow<NPBehaveGraphEditorWindow>();
            window.titleContent = new GUIContent("NPBehaveGraph");
        }
    
        private void Update()
        {
            if (m_HasError)
                return;
            
            if (graphObject == null && selectedGuid != null)
            {
                var guid = selectedGuid;
                selectedGuid = null;
                Initialize(guid);
            }
            
            var materialGraph = graphObject.graph as GraphData;
            if (materialGraph == null)
                return;
            
            if (graphEditorView == null)
            {
                graphEditorView = new NPBehaveGraphEditorView(this, materialGraph);
            }
            graphEditorView.HandleGraphChanges(false);
            graphObject.graph.ClearChanges();
        }
        
        void OnGeometryChanged(GeometryChangedEvent evt)
        {
            if (graphEditorView == null)
            {
                return;
            }
            
            graphEditorView.UnregisterCallback<GeometryChangedEvent>(OnGeometryChanged);
            
            if (m_FrameAllAfterLayout)
            {
                graphEditorView.graphView.FrameAll();
            }
            
            m_FrameAllAfterLayout = false;
        }

        public void Initialize(string assetGuid)
        {
            var asset = AssetDatabase.LoadAssetAtPath<Object>(AssetDatabase.GUIDToAssetPath(assetGuid));
            if (asset == null)
                return;
            
            if (selectedGuid == assetGuid)
               return;
            
            selectedGuid = assetGuid;
           
           var path = AssetDatabase.GetAssetPath(asset);
            
            try
            {
                
                
                m_LastSerializedFileContents = File.ReadAllText(path, Encoding.UTF8);
                
                graphObject = CreateInstance<GraphObject>();
                graphObject.hideFlags = HideFlags.HideAndDontSave;
                graphObject.graph = new GraphData();
                
                MultiJson.Deserialize(graphObject.graph, m_LastSerializedFileContents);
                
                Repaint();
            }
            catch (Exception e)
            {
                Debug.LogError("Exception:" + e.ToString());
                m_HasError = true;
                m_GraphEditorView = null;
                graphObject = null;
                throw;
            }
        }

        void OnEnable()
        {
            this.SetAntiAliasing(4);
        }
    
        void OnDisable()
        {
            graphEditorView = null;
        }

        void OnDestroy()
        {
            graphObject = null;
            graphEditorView = null;
        }
        
        public bool SaveAsset()
        {
            bool saved = false;

            if (selectedGuid != null && graphObject != null)
            {
                var path = AssetDatabase.GUIDToAssetPath(selectedGuid);
                if (string.IsNullOrEmpty(path) || graphObject == null)
                    return false;

                var newFileContents = FileUtilities.WriteShaderGraphToDisk(path, graphObject.graph);
                if (newFileContents != null)
                {
                    saved = true;
                    m_LastSerializedFileContents = newFileContents;
                    AssetDatabase.ImportAsset(path);
                }

                OnSaveGraph(path);
                hasUnsavedChanges = false;
            }
            return saved;
        }
        
        void OnSaveGraph(string path)
        {

        }

    }
}

