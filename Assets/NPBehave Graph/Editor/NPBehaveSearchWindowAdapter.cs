using System.Collections;
using System.Collections.Generic;
using UnityEditor.Searcher;
using UnityEngine;

namespace UnityEditor.BehaveGraph
{
    public class NPBehaveSearchWindowAdapter : SearcherAdapter
    {
        public NPBehaveSearchWindowAdapter(string title) : base(title)
        {
            
        }
    }
    
    internal class SearchNodeItem : SearcherItem
    {
        public NodeEntry NodeGUID;

        public SearchNodeItem(string name, NodeEntry nodeGUID, string[] synonyms,
            string help = " ", List<SearchNodeItem> children = null) : base(name)
        {
            NodeGUID = nodeGUID;
            Synonyms = synonyms;
        }
    }
}

