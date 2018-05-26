using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvadePlayerAction : GoapAction {

    private Transform targetPlayer = null;
    private float lastAttack = 0;

    public EvadePlayerAction()
    {
        addEffect("safeBoost", true);
        cost = 1;
    }

    public override void reset()
    {
        if (target != null)
            Destroy(target.gameObject);
        targetPlayer = null;
        lastAttack = 0;
    }

    public override bool requiresInRange()
    {
        return true;
    }

    public override bool isDone()
    {
        return false;
    }

    public override bool checkProceduralPrecondition(GameObject agent)
    {
        ZombieAgent za = agent.GetComponent<ZombieAgent>();

        Transform nearestPlayer = null;
        float nearestPlayerDistance = 0;
        foreach (PlayerController2 pc in GameObject.FindObjectsOfType<PlayerController2>())
        {
            if (nearestPlayer == null)
            {
                nearestPlayer = pc.transform;
                nearestPlayerDistance = (nearestPlayer.position - za.transform.position).magnitude;
            }
            else if ((pc.transform.position - za.transform.position).magnitude < nearestPlayerDistance)
            {
                nearestPlayer = pc.transform;
                nearestPlayerDistance = (pc.transform.position - za.transform.position).magnitude;
            }
        }

        if (target != null)
            Destroy(target.gameObject);

        targetPlayer = nearestPlayer;
        target = new GameObject("Necro target");
        target.transform.position = za.transform.position + (za.transform.position - nearestPlayer.position).normalized * 2;
        cost = 1;
        return true;
    }

    public override bool perform(GameObject agent)
    {
        ZombieAgent za = agent.GetComponent<ZombieAgent>();

        if (!za.PlayerNear)
        {
            if (target)
                Destroy(target.gameObject);
            return false;
        }

        target.transform.position = za.transform.position + (za.transform.position - targetPlayer.position).normalized * 2;
        za.moveAgent(this);

        return true;
    }
}
