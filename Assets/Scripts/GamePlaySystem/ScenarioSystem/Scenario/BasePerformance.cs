using System.Collections;
using UnityEngine;

namespace GamePlaySystem.ScenarioSystem.Scenario
{
    public abstract class BasePerformance : MonoBehaviour
    {
        public abstract IEnumerator StartPerformance();
    }
}