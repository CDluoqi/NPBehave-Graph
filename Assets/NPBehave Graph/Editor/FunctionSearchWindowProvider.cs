using System;
using System.Collections.Generic;
using NPBehave;
using UnityEditor.Searcher;

namespace UnityEditor.BehaveGraph
{
    class FunctionSearchWindowProvider
    {
        public Searcher.Searcher LoadSearchWindow(FuncPurpose purpose = FuncPurpose.Any)
        {
            var root = new List<SearcherItem>();

            foreach (var pair in NPBehaveFunctionCache.KnownFunctionLookupTable)
            {
                if (pair.Value.Count == 0)
                {
                    continue;
                }
                var classItem =  new SearcherItem(pair.Key.ToString());
                foreach (var functionNameAttribute in pair.Value)
                {
                    if (purpose == FuncPurpose.Any || functionNameAttribute.Purpose == FuncPurpose.Any || purpose == functionNameAttribute.Purpose)
                    {
                        classItem.AddChild(new SearcherItem(functionNameAttribute.Name, functionNameAttribute.Help));
                    }
                }
                if (classItem.HasChildren)
                {
                    root.Add(classItem);
                }
            }
            var nodeDatabase = SearcherDatabase.Create(root, string.Empty, false);
            
            return new Searcher.Searcher(nodeDatabase, new SearcherAdapter("Find Function"));
        }

        public bool OnSearcherSelectEntry(SearcherItem entry, Action<string> selectedAction)
        {
            if (entry == null || selectedAction == null)
            {
                return false;
            }
            selectedAction.Invoke(entry.Name);
            return true;
        }
    }
}

