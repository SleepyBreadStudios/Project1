using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class DashShot : SkillBehaviour
{
    public void UpdatePlayerDash(PlayerBehavior player)
    {
        player.LearnDashShot();
    }
}
