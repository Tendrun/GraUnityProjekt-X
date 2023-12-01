using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vera_Script : Player_Script
{
    #region Passive
    public override IEnumerator PlayerPassive()
    {
        StartCoroutine(PlayerPassive2());
        //AddReloadTimeDivider(10);

        yield return new WaitForSeconds(0);
    }

    public IEnumerator PlayerPassive2()
    {
        //AddReloadTimeDivider(1);

        yield return new WaitUntil(() => GameManager.GameManagerInstance.On1EnemiesDeaths());

        StartCoroutine(PlayerPassive2());
    }

    public void PlayerPassiveReset()
    {
        
    }

    #endregion
}
