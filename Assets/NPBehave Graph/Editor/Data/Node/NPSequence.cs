using System;
using System.Collections;
using System.Collections.Generic;
using NPBehave;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace UnityEditor.BehaveGraph
{
    [Title("Composite", "Sequence")]
    class NPSequence : NPComposite
    {
        public override NPBehaveNodeType nodeType => NPBehaveNodeType.Sequence;
        public NPSequence()
        {
            name = "Sequence";
            synonyms = new string[] { "sequence", "arr" };
        }
    }
}

