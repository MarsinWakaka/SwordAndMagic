using System.Collections;
using System.Collections.Generic;
using GamePlaySystem.ScenarioSystem.Scenario;
using MyEventSystem;
using UnityEngine;

namespace BattleSystem.ScenarioSystem
{
    // 演出考虑做成Prefab，每场演出组合都是一个Prefab
    public class ScenarioManager : MonoBehaviour
    {
        public List<BasePerformance> performanceList = new();
        private int _curPfmIndex = 0;

        public void OnScenarioStart()
        {
            StartCoroutine(StartScenario());
        }
        
        private IEnumerator StartScenario()
        {
            while (_curPfmIndex < performanceList.Count)
            {
                yield return performanceList[_curPfmIndex].StartPerformance();
                _curPfmIndex++;
            }
            EventCenter<GameEvent>.Instance.Invoke(GameEvent.ScenarioEnd);
        }

        public void OnScenarioEnd()
        {
            EventCenter<GameEvent>.Instance.Invoke(GameEvent.PlayerDeployedStart);
        }
    }
}