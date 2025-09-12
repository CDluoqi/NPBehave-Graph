using System.Collections;
using System.Collections.Generic;
using NPBehave;
using UnityEngine;


namespace UnityEditor.BehaveGraph
{
    [Title("Composite", "Selector")]
    class NPSelector : NPComposite
    {
        public override NPBehaveNodeType nodeType => NPBehaveNodeType.Selector;
        public NPSelector()
        {
            name = "Selector";
            synonyms = new string[] { "selector"};
        }
    }
}