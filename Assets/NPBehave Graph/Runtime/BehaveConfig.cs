using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace NPBehave
{
     [Serializable]
     public class NodeConfig
     {
          public int id;
          public NPBehaveNodeType nodeType;
          public string param;
          public int[]  nodes;
     }

     [Serializable]
     public class NPBehaveTreeConfig
     {
          public NodeConfig[] nodes;
     }
}
