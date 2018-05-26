using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Com.MyCompany.MyGame;

public class AttackPlayerAction : GoapAction {

    private bool playerIsDead = false;
    private PlayerController2 targetPlayer = null;
    private PlayerHealth targetPlayerHealth = null;

    private float lastAttack = 0;

    public AttackPlayerAction()
    {
        addEffect("playerLose", true);
        cost = 1;
    }

    // Reset every variable about the action.
    public override void reset()
    {
        playerIsDead = false;
        targetPlayer = null;
        targetPlayerHealth = null;
        lastAttack = 0;
    }

    // Action is done if the player is dead.
    public override bool isDone()
    {
        return playerIsDead;
    }

    // Action requires the player to be near.
    public override bool requiresInRange()
    {
        return true;
    }

    // Check the preconditions for the action to be realised.
    public override bool checkProceduralPrecondition(GameObject agent)
    {
        // Find the nearest player to attack
        PlayerController2[] players = GameObject.FindObjectsOfType<PlayerController2>();
        PlayerController2 closestPlayer = null;
        float closestDistance = 0;

        foreach (PlayerController2 player in players)
        {
            if (closestPlayer == null)
            {
                closestPlayer = player;
                closestDistance = (player.transform.position - agent.transform.position).magnitude;
            }
            else
            {
                float distance = (player.transform.position - agent.transform.position).magnitude;
                if (distance < closestDistance)
                {
                    closestPlayer = player;
                    closestDistance = distance;
                }
            }
        }

        if (closestPlayer == null)
            return false;

        targetPlayer = closestPlayer;
        targetPlayerHealth = targetPlayer.GetComponent<PlayerHealth>();
        target = targetPlayer.gameObject;
        cost = closestDistance * (((ZombieAgent.TankNear)GetComponent<IGoap>().getWorldState()["isTankNearby"]).isNear ? 3 : 1);
        return true;
    }

    // Realise the action.
    public override bool perform(GameObject agent)
    {
        ZombieAgent zombieAgent = GetComponent<ZombieAgent>();

        if ((target.transform.position - agent.transform.position).magnitude > zombieAgent.attackRange)
            return false;

        // If the last attack was realised more than timeBetweenAttacks secs ago.
        if (Time.time - lastAttack > zombieAgent.timeBetweenAttacks)
        {
            // Do a new attack.
            targetPlayerHealth.TakeDamage(zombieAgent.Damages);
            agent.GetComponent<Animator>().SetBool("isAttacking", true);
            lastAttack = Time.time;

            if (targetPlayerHealth.currentHealth <= 0)
                playerIsDead = true;
        }
        else if (Time.time - lastAttack > 0.5f)
        {
            agent.GetComponent<Animator>().SetBool("isAttacking", false);
        }
        return true;
    }
}
