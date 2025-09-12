using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEditor.Searcher;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityEditor.BehaveGraph
{
    internal struct NodeEntry
    {
        public string[] title;
        public AbstractBehaveNode node;
        public int compatibleSlotId;
        public string slotName;
    }
    
    class SearchWindowProvider : ScriptableObject
    {
        internal EditorWindow m_EditorWindow;
        internal GraphData m_Graph;
        internal GraphView m_GraphView;
        
        public List<NodeEntry> currentNodeEntries;
        
        public bool regenerateEntries { get; set; }
        
        public VisualElement target { get; internal set; }
        
        public void Initialize(EditorWindow editorWindow, GraphData graph, GraphView graphView)
        {
            m_EditorWindow = editorWindow;
            m_Graph = graph;
            m_GraphView = graphView;
            GenerateNodeEntries();
        }

        public void GenerateNodeEntries()
        {
            List<NodeEntry> nodeEntries = new List<NodeEntry>();
            foreach (var type in NPBehaveNodeClassCache.knownNodeTypes)
            {
                TitleAttribute titleAttribute = NPBehaveNodeClassCache.GetAttributeOnNodeType<TitleAttribute>(type);
                if (titleAttribute != null)
                {
                    var node = (AbstractBehaveNode)Activator.CreateInstance(type);
                    
                    AddEntries(node, titleAttribute.title, nodeEntries);
                }
            }
            SortEntries(nodeEntries);
            currentNodeEntries = nodeEntries;
        }
        
        void AddEntries(AbstractBehaveNode node, string[] title, List<NodeEntry> addNodeEntries)
        {
            addNodeEntries.Add(new NodeEntry
            {
                node = node,
                title = title,
                compatibleSlotId = -1
            });
        }
        
        void SortEntries(List<NodeEntry> nodeEntries)
        {
            nodeEntries.Sort((entry1, entry2) =>
            {
                for (var i = 0; i < entry1.title.Length; i++)
                {
                    if (i >= entry2.title.Length)
                        return 1;
                    var value = entry1.title[i].CompareTo(entry2.title[i]);
                    if (value != 0)
                    {
                        if (entry1.title.Length != entry2.title.Length && (i == entry1.title.Length - 1 || i == entry2.title.Length - 1))
                        {
                            var alphaOrder = entry1.title.Length < entry2.title.Length ? -1 : 1;
                            var slotOrder = entry1.compatibleSlotId.CompareTo(entry2.compatibleSlotId);
                            return alphaOrder.CompareTo(slotOrder);
                        }
                        return value;
                    }
                }
                return 0;
            });
        }
        
    }

    class NPBehaveSearchProvider : SearchWindowProvider
    {
        public Searcher.Searcher LoadSearchWindow()
        {
            if (regenerateEntries)
            {
                GenerateNodeEntries();
                regenerateEntries = false;
            }
            var root = new List<SearcherItem>();
            var dummyEntry = new NodeEntry();
            
            foreach (var nodeEntry in currentNodeEntries)
            {
                SearcherItem item = null;
                SearcherItem parent = null;
                for (int i = 0; i < nodeEntry.title.Length; i++)
                {
                    var pathEntry = nodeEntry.title[i];
                    List<SearcherItem> children = parent != null ? parent.Children : root;
                    item = children.Find(x => x.Name == pathEntry);

                    if (item == null)
                    {
                        if (i == nodeEntry.title.Length - 1)
                        {
                            item = new SearchNodeItem(pathEntry, nodeEntry, nodeEntry.node.synonyms);
                        }
                        else
                        {
                            item = new SearchNodeItem(pathEntry, dummyEntry, null);
                        }
                        
                        
                        if (parent != null)
                        {
                            parent.AddChild(item);
                        }
                        else
                        {
                            children.Add(item);
                        }
                    }
                    parent = item;

                    if (parent.Depth == 0 && !root.Contains(parent))
                        root.Add(parent);
                }
            }
            
            var nodeDatabase = SearcherDatabase.Create(root, string.Empty, false);

            return new Searcher.Searcher(nodeDatabase, new NPBehaveSearchWindowAdapter("Create Node"));
        }

        public bool OnSearcherSelectEntry(SearcherItem entry, Vector2 screenMousePosition, NPBehaveStackNodeView stackNodeView = null)
        {
            if (entry == null || (entry as SearchNodeItem).NodeGUID.node == null)
                return true;
            
            var nodeEntry = (entry as SearchNodeItem).NodeGUID;
            
            var node = CopyNodeForGraph(nodeEntry.node);
            
            var windowRoot = m_EditorWindow.rootVisualElement;
            var windowMousePosition = windowRoot.ChangeCoordinatesTo(windowRoot.parent, screenMousePosition); //- m_EditorWindow.position.position);
            var graphMousePosition = m_GraphView.contentViewContainer.WorldToLocal(windowMousePosition);
            var drawState = node.drawState;
            
            if (stackNodeView != null)
            {
                var blockNode = new NPBehaveBlockNode() { stackData = stackNodeView.stackData };
                int index = stackNodeView.GetInsertionIndex(screenMousePosition);
                m_Graph.AddBlock(blockNode, stackNodeView.stackData, index);
                
                var fromReference = blockNode.GetMainSlotReference();
                var toReference = node.GetMainSlotReference();
                m_Graph.Connect(fromReference, toReference);
                
                var stackPos = stackNodeView.parent.ChangeCoordinatesTo(m_GraphView.contentViewContainer, stackNodeView.GetPosition());
                graphMousePosition.x = stackPos.x + stackNodeView.contentRect.width + 50;
                drawState.position = new Rect(graphMousePosition, Vector2.zero);
            }

            drawState.position = new Rect(graphMousePosition, Vector2.zero);
            node.drawState = drawState;
            
            m_Graph.AddNode(node);
            return true;
        }
        
        public AbstractBehaveNode CopyNodeForGraph(AbstractBehaveNode oldNode)
        {
            var newNode = (AbstractBehaveNode)Activator.CreateInstance(oldNode.GetType());
            return newNode;
        }
    }
}

