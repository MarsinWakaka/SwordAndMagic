using UnityEngine;
using DeprecatedBattleSystem;

public class InputHandler : MonoBehaviour
{
    private BattleManager _battleSystem;
    
    private void Start()
    {
        _battleSystem = FindObjectOfType<BattleManager>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            _battleSystem.BtnClickedMove();
        }
        else if(Input.GetKeyDown(KeyCode.Alpha2))
        {
            _battleSystem.BtnClickedAttack();
        }
        else if(Input.GetKeyDown(KeyCode.Space))
        {
            _battleSystem.BtnClickedEndTurn();
        }
    }
}