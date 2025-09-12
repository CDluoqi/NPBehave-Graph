using System;
using System.Collections.Generic;
using UnityEditor.BehaveGraph.Serialization;
using UnityEngine;
using System.Linq;
using NPBehave;
using Unity.VisualScripting;

namespace UnityEditor.BehaveGraph
{
    sealed class GraphData : JsonObject
    {
        [SerializeField]
        List<JsonData<AbstractBehaveNode>> m_Nodes = new List<JsonData<AbstractBehaveNode>>();
        
        public IEnumerable<T> GetNodes<T>()
        {
            return m_Nodes.SelectValue().OfType<T>();
        }
        
        [NonSerialized]
        List<AbstractBehaveNode> m_AddedNodes = new List<AbstractBehaveNode>();

        public IEnumerable<AbstractBehaveNode> addedNodes
        {
            get { return m_AddedNodes; }
        }

        [NonSerialized]
        List<AbstractBehaveNode> m_RemovedNodes = new List<AbstractBehaveNode>();

        public IEnumerable<AbstractBehaveNode> removedNodes
        {
            get { return m_RemovedNodes; }
        }
        
        #region Edge data

        [SerializeField]
        List<Edge> m_Edges = new List<Edge>();

        public IEnumerable<Edge> edges => m_Edges;

        [NonSerialized]
        Dictionary<string, List<IEdge>> m_NodeEdges = new Dictionary<string, List<IEdge>>();

        [NonSerialized]
        List<IEdge> m_AddedEdges = new List<IEdge>();

        public IEnumerable<IEdge> addedEdges
        {
            get { return m_AddedEdges; }
        }

        [NonSerialized]
        List<IEdge> m_RemovedEdges = new List<IEdge>();

        public IEnumerable<IEdge> removedEdges
        {
            get { return m_RemovedEdges; }
        }

        #endregion
        
        public GraphObject owner { get; set; }
        
        static List<IEdge> s_TempEdges = new List<IEdge>();

        public override void OnAfterMultiDeserialize(string json)
        {
            var nodes = GetNodes<NPBehaveStackNode>();
            foreach (var node in nodes)
            {
                DeserializeContextData(node.stackData);
            }
            
            foreach (var node in m_Nodes.SelectValue())
            {
                node.UpdateNodeAfterDeserialization();
            }
            
            foreach (var edge in m_Edges)
                AddEdgeToNodeEdges(edge);
            
        }
        
        void DeserializeContextData(StackData stackData)
        {
            var blocks = stackData.blocks.SelectValue().ToList();
            var blockCount = blocks.Count;
            for (int i = 0; i < blockCount; i++)
            {
                var block = blocks[i];
                block.stackData = stackData;
            }
        }

        public void ClearChanges()
        {
            m_AddedNodes.Clear();
            m_RemovedNodes.Clear();
            m_AddedEdges.Clear();
            m_RemovedEdges.Clear();
        }

        public void AddNode(AbstractBehaveNode node)
        {
            m_Nodes.Add(node);
            m_AddedNodes.Add(node);
        }
        
        public void RemoveNode(AbstractBehaveNode node)
        {
            if (!node.canDeleteNode)
            {
                throw new InvalidOperationException($"Node {node.name} ({node.objectId}) cannot be deleted.");
            }
            RemoveNodeNoValidate(node);
            ValidateGraph();
        }
        
        void RemoveNodeNoValidate(AbstractBehaveNode node)
        {
            m_Nodes.Remove(node);
            m_RemovedNodes.Add(node);

            if (node is NPBehaveBlockNode blockNode && blockNode.stackData != null)
            {
                blockNode.stackData.blocks.Remove(blockNode);
            }
        }
        
        public void AddBlock(NPBehaveBlockNode blockNode, StackData stackData, int index)
        {
            AddNode(blockNode);
            
            blockNode.stackData = stackData;

            if (index == -1 || index >= stackData.blocks.Count())
            {
                stackData.blocks.Add(blockNode);
            }
            else
            {
                stackData.blocks.Insert(index, blockNode);
            }
        }
        
        public void GetEdges(SlotReference s, List<IEdge> foundEdges)
        {
            NPBehaveSlot slot = s.slot;

            List<IEdge> candidateEdges;
            if (!m_NodeEdges.TryGetValue(s.node.objectId, out candidateEdges))
                return;

            foreach (var edge in candidateEdges)
            {
                var cs = slot.isInputSlot ? edge.inputSlot : edge.outputSlot;
                if (cs.node == s.node && cs.slotId == s.slotId)
                    foundEdges.Add(edge);
            }
        }

        public IEnumerable<IEdge> GetEdges(SlotReference s)
        {
            var edges = new List<IEdge>();
            GetEdges(s, edges);
            return edges;
        }
        
        void AddEdgeToNodeEdges(IEdge edge)
        {
            List<IEdge> inputEdges;
            if (!m_NodeEdges.TryGetValue(edge.inputSlot.node.objectId, out inputEdges))
                m_NodeEdges[edge.inputSlot.node.objectId] = inputEdges = new List<IEdge>();
            inputEdges.Add(edge);

            List<IEdge> outputEdges;
            if (!m_NodeEdges.TryGetValue(edge.outputSlot.node.objectId, out outputEdges))
                m_NodeEdges[edge.outputSlot.node.objectId] = outputEdges = new List<IEdge>();
            outputEdges.Add(edge);
        }
        
        IEdge ConnectNoValidate(SlotReference fromSlotRef, SlotReference toSlotRef)
        {
            var fromNode = fromSlotRef.node;
            var toNode = toSlotRef.node;

            if (fromNode == null || toNode == null)
            {
                return null;
            }
                
            var fromSlot = fromNode.FindSlot<NPBehaveSlot>(fromSlotRef.slotId);
            var toSlot = toNode.FindSlot<NPBehaveSlot>(toSlotRef.slotId);
            if (fromSlot == null || toSlot == null)
                return null;
            if (fromSlot.isOutputSlot == toSlot.isOutputSlot)
                return null;
            var outputSlot = fromSlot.isOutputSlot ? fromSlotRef : toSlotRef;
            var inputSlot = fromSlot.isInputSlot ? fromSlotRef : toSlotRef;

            s_TempEdges.Clear();
            GetEdges(fromSlotRef, s_TempEdges);

            foreach (var edge in s_TempEdges)
            {
                RemoveEdgeNoValidate(edge);
            }
            
            s_TempEdges.Clear();
            GetEdges(toSlotRef, s_TempEdges);

            foreach (var edge in s_TempEdges)
            {
                RemoveEdgeNoValidate(edge);
            }

            var newEdge = new Edge(outputSlot, inputSlot);

            m_Edges.Add(newEdge);
            m_AddedEdges.Add(newEdge);
            AddEdgeToNodeEdges(newEdge);
            
            return newEdge;
        }
        
        public IEdge Connect(SlotReference fromSlotRef, SlotReference toSlotRef)
        {
            var newEdge = ConnectNoValidate(fromSlotRef, toSlotRef);
            ValidateGraph();
            return newEdge;
        }

        public void RemoveElements(AbstractBehaveNode[] nodes, IEdge[] edges)
        {
            foreach (var node in nodes)
            {
                if (!node.canDeleteNode)
                {
                    throw new InvalidOperationException($"Node {node.name} ({node.objectId}) cannot be deleted.");
                }
            }
            
            foreach (var edge in edges.ToArray())
            {
                RemoveEdgeNoValidate(edge);
            }
            
            foreach (var serializableNode in nodes)
            {
                RemoveNodeNoValidate(serializableNode);
            }
        }
        
        void RemoveEdgeNoValidate(IEdge e, bool reevaluateActivity = true)
        {
            e = m_Edges.FirstOrDefault(x => x.Equals(e));
            if (e == null)
                throw new ArgumentException("Trying to remove an edge that does not exist.", "e");
            m_Edges.Remove(e as Edge);

            AbstractBehaveNode input = e.inputSlot.node, output = e.outputSlot.node;

            List<IEdge> inputNodeEdges;
            if (m_NodeEdges.TryGetValue(input.objectId, out inputNodeEdges))
                inputNodeEdges.Remove(e);

            List<IEdge> outputNodeEdges;
            if (m_NodeEdges.TryGetValue(output.objectId, out outputNodeEdges))
                outputNodeEdges.Remove(e);

            m_AddedEdges.Remove(e);
            m_RemovedEdges.Add(e);
        }
        
        public void OnEnable()
        {

        }

        public void OnDisable()
        {
           
        }

        public void ValidateGraph()
        {
            
        }

        public string ConvertToConfig()
        {
            NPRoot root = null;
            foreach (var node in GetNodes<NPRoot>())
            {
                root = node;
                break;
            }

            if (root == null)
            {
                return "fuck";
            }

            NodeConfig config = GetNodeConfig(root);
            return JsonUtility.ToJson(config);
        }
        
        NodeConfig GetNodeConfig(AbstractBehaveNode node)
        {
            NodeConfig nodeConfig = new NodeConfig
            {
                nodeType = node.nodeType
            };
            if (nodeConfig.nodeType == NPBehaveNodeType.Unknown)
            {
                Debug.LogError("Unknown node type " + node.GetType());
            }
               
            NodeConfig[] childNodeConfigs = null;
            if (node is NPBehaveStackNode stackNode)
            {
                List<NodeConfig>  nodeConfigs = new List<NodeConfig>();
                var blockNodes = stackNode.stackData.blocks.SelectValue().ToList();
                foreach (var blockNode in blockNodes)
                {
                    NodeConfig childNodeConfig = GetConnectedNodeConfig(blockNode);
                    nodeConfigs.Add(childNodeConfig);
                }
                childNodeConfigs = nodeConfigs.ToArray();
            }
            else
            {
                NodeConfig childNodeConfig = GetConnectedNodeConfig(node);
                if (childNodeConfig != null)
                {
                    childNodeConfigs = new[] { childNodeConfig };
                }
            }

            nodeConfig.param = node.ParamToJson();
            nodeConfig.nodes = childNodeConfigs;
            return nodeConfig;
        }

        NodeConfig GetConnectedNodeConfig(AbstractBehaveNode node)
        {
            List<NPBehaveOutputSlot> foundSlot = new List<NPBehaveOutputSlot>();
            node.GetSlots(foundSlot);

            foreach (var outputSlot in foundSlot)
            {
                List<IEdge> foundEdges = new List<IEdge>();
                GetEdges(outputSlot.slotReference, foundEdges);
                foreach (var edge in foundEdges)
                {
                    return GetNodeConfig(edge.inputSlot.node);
                }
                break;
            }
            return null;
        }
        
    }
}


