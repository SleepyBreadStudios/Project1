using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class DashSkill : SkillBehaviour
{
    public void UpdatePlayerDash(PlayerBehavior player)
    {
        player.LearnDash();
    }
}
