using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackPayloadAction : GoapAction {
    private bool payloadIsDead = false;
    private Payload targetPayload = null;

    private float lastAttack = 0;

    public AttackPayloadAction()
    {
        addPrecondition("isTankNearby", new ZombieAgent.TankNear(false, false));
        addPrecondition("isHidden", false);
        addEffect("playerLose", true);
    }

    // Reset every variable about the action.
    public override void reset()
    {
        payloadIsDead = false;
        targetPayload = null;
    }

    // Action is done if the player is dead.
    public override bool isDone()
    {
        return payloadIsDead;
    }

    // Action requires the player to be near.
    public override bool requiresInRange()
    {
        return true;
    }

    // Check the preconditions for the action to be realised.
    public override bool checkProceduralPrecondition(GameObject agent)
    {
        targetPayload = GameObject.FindObjectOfType<Payload>();

        if (targetPayload == null)
            return false;

        Dictionary<string, object> worldState = GetComponent<IGoap>().getWorldState();

        if (Preconditions.ContainsKey("isHidden"))
            removePrecondition("isHidden");
        if (((ZombieAgent.TankNear)worldState["isTankNearby"]).isNear)
        {
            if (((ZombieAgent.TankNear)worldState["isTankNearby"]).toConsider)
            {
                cost = (targetPayload.transform.position - agent.transform.position).magnitude;
                Preconditions["isTankNearby"] = new ZombieAgent.TankNear(true, true);
                addPrecondition("isHidden", true);
            }
            else
            {
                cost = (targetPayload.transform.position - agent.transform.position).magnitude * 1f;
                Preconditions["isTankNearby"] = new ZombieAgent.TankNear(true, false);
            }
        }
        else
        {
            cost = (targetPayload.transform.position - agent.transform.position).magnitude * 1.5f;
            Preconditions["isTankNearby"] = new ZombieAgent.TankNear(false, false);
        }
        target = targetPayload.gameObject;
        return true;
    }

    // Realise the action.
    public override bool perform(GameObject agent)
    {
        ZombieAgent zombieAgent = GetComponent<ZombieAgent>();

        if (target != null && (agent.transform.position - target.transform.position).magnitude > (zombieAgent.attackRange))
            return false;

        // If the last attack was realised more than timeBetweenAttacks secs ago.
        if (Time.time - lastAttack > zombieAgent.timeBetweenAttacks)
        {
            // Do a new attack.
            targetPayload.TakeDamage(zombieAgent.Damages);
            agent.GetComponent<Animator>().SetBool("isAttacking", true);
            lastAttack = Time.time;

            if (targetPayload.currentHealth <= 0)
                payloadIsDead = true;
        }
        else
        {
            agent.GetComponent<Animator>().SetBool("isAttacking", false);
        }
        return true;
    }
}
