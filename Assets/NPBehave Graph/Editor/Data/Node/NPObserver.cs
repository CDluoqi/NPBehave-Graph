using NPBehave;
using UnityEditor.BehaveGraph.Drawing.Controls;
using UnityEngine;

namespace UnityEditor.BehaveGraph
{
    [Title("Decorator", "Observer")]
    class NPObserver : NPDecorator
    {
        public override NPBehaveNodeType nodeType => NPBehaveNodeType.Observer;

        public NPObserver()
        {
            name = "Observer";
            synonyms = new string[] { "observer" };
        }

        [SerializeField] 
        private string m_OnStartFunc = "";

        [FunctionControl("OnStart", FuncPurpose.ObserverOnStart)]
        public string OnStartFunc
        {
            get => m_OnStartFunc;
            set
            {
                if (m_OnStartFunc == value)
                {
                    return;
                }
                m_OnStartFunc = value;
                Dirty(ModificationScope.Graph);
            }
        }
        
        [SerializeField] 
        private string m_OnStopFunc = "";

        [FunctionControl("OnStop", FuncPurpose.ObserverOnStop)]
        public string OnStopFunc
        {
            get => m_OnStopFunc;
            set
            {
                if (m_OnStopFunc == value)
                {
                    return;
                }
                m_OnStopFunc = value;
                Dirty(ModificationScope.Graph);
            }
        }

        public override string ParamToJson()
        {
            NPObserverParam param = new NPObserverParam
            {
                OnStartFunc = OnStartFunc,
                OnStopFunc = OnStopFunc
            };
            return JsonUtility.ToJson(param);
        }
    }
        
}
