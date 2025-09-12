using NPBehave;
using UnityEditor.BehaveGraph.Drawing.Controls;
using UnityEngine;

namespace UnityEditor.BehaveGraph
{
    [Title("Decorator", "Random")]
    class NPRandom : NPDecorator
    {
        public override NPBehaveNodeType nodeType => NPBehaveNodeType.Random;
        public NPRandom()
        {
            name = "Random";
            synonyms = new string[] { "random" };
        }

        [SerializeField]
        private float m_Probability = 0.5f;

        [FloatControl("Probability")]
        public float Probability
        {
            get => m_Probability;
            set
            {
                m_Probability = value; 
                Dirty(ModificationScope.Graph);
            }
        }

        public override string ParamToJson()
        {
            NPRandomParam param = new NPRandomParam()
            {
                probability = Probability
            };
            return JsonUtility.ToJson(param);
        }
    }
}
