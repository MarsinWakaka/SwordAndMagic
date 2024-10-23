using System.Collections;
using System.Collections.Generic;
using MyEventSystem;
using ScenarioSystem.Performance;
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
            EventCenter<GameStateEvent>.Instance.Invoke(GameStateEvent.GameStateScenarioEnd);
        }

        public void OnScenarioEnd()
        {
            EventCenter<GameStateEvent>.Instance.Invoke(GameStateEvent.GameStatePlayerDeployedStart);
        }
    }
}