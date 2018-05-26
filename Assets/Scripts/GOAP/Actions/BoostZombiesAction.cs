using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoostZombiesAction : GoapAction
{
    private ZombieAgent targetZombie = null;
    private Transform targetPlayer = null;
    private float lastAttack = 0;

    public BoostZombiesAction()
    {
        addEffect("safeBoost", true);
        cost = 1;
    }

    public override void reset()
    {
        if (target != null)
            Destroy(target.gameObject);
        targetZombie = null;
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

        if (za.PlayerNear)
            return false;

        Collider col = za.nearbyEntities.Find(c => c ? c.gameObject.GetComponent<BasicZombie>() : null);
        if (col == null)
            return false;

        targetZombie = col.GetComponent<ZombieAgent>();

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
        target.transform.position = targetZombie.gameObject.transform.position + (targetZombie.transform.position - nearestPlayer.position).normalized * (za.attackRange / 4);
        cost = 1;
        return true;
    }

    public override bool perform(GameObject agent)
    {
        ZombieAgent za = agent.GetComponent<ZombieAgent>();

        if (za.PlayerNear || za.nearbyEntities.Find(c => c ? c.gameObject.GetComponent<BasicZombie>() : null) == null)
        {
            Destroy(target.gameObject);
            return false;
        }

        target.transform.position = targetZombie.gameObject.transform.position + (targetZombie.transform.position - targetPlayer.position).normalized * (za.attackRange / 4);
        za.moveAgent(this);

        if (Time.time - lastAttack > za.timeBetweenAttacks)
        {
            lastAttack = Time.time;
            agent.GetComponent<Animator>().SetBool("isAttacking", true);
            za.nearbyEntities.ForEach(c =>
            {
                if (c && c.GetComponent<BasicZombie>())
                    c.GetComponent<ZombieAgent>().Boost();
            });
        }
        else
        {
            agent.GetComponent<Animator>().SetBool("isAttacking", false);
        }

        return true;
    }
}
