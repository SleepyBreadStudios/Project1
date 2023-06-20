using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    [SerializeField]
    private int vertices;

    [SerializeField]
    List<SkillBehaviour> skills = new();

    // add graph edge/prereq for skill
    // not really utilized since skills should be initialized with their prerequisites
    public void AddPrerequisite(SkillBehaviour skill, SkillBehaviour prereq)
    {
        var foundSkill = skills.Find(x => x == skill);
        if (foundSkill != null)
        {
            foundSkill.AddPrereq(prereq);
        }
    }

    void Start()
    {
    }
}
