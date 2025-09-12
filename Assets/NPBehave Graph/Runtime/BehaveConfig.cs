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
          public NPBehaveNodeType nodeType;
          public string param;
          public NodeConfig[]  nodes;
     }

     public class NodeConfigParam
     {
          
     }
}
