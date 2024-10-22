using System.Collections;
using UnityEngine;

namespace ScenarioSystem.Performance
{
    public abstract class BasePerformance : MonoBehaviour
    {
        public abstract IEnumerator StartPerformance();
    }
}