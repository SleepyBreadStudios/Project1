using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ShieldSkill : SkillBehaviour
{
    public void UpdatePlayerShield(PlayerBehavior player)
    {
        player.LearnShield();
    }
}
