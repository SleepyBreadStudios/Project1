using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class PassiveSkill : SkillBehaviour
{
    [SerializeField]
    private int changeAmount = 5;

    [SerializeField]
    private string changedStat;

    // pass in player as parameter
    public void UpdatePlayerPassive(PlayerBehavior player)
    {
        player.UpdatePassive(changedStat, changeAmount);
    }
}
