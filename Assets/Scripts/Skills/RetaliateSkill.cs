using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class RetaliateSkill : SkillBehaviour
{
    public void UpdatePlayerRetaliate(PlayerBehavior player)
    {
        player.LearnRetaliate();
    }
}
