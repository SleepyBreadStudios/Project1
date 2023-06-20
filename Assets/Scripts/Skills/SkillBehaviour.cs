using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillBehaviour : ScriptableObject
{

    [SerializeField]
    private string skillName = null;

    [SerializeField]
    public Sprite icon = null;

    //[SerializeField]
    //private int numPrereqs = 0;

    [SerializeField]
    private List<SkillBehaviour> skillPrereq = new();

    // getter method
    public string GetName()
    {
        return skillName;
    }

    // Set entire prereq list
    public void SetSkillPrereqList(List<SkillBehaviour> newList)
    {
        skillPrereq = newList;
    }


    // Add prereq to end of prereq list
    public void AddPrereq(SkillBehaviour prereq)
    {
        skillPrereq.Add(prereq);
    }
}
