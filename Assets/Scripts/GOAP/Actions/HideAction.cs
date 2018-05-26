using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideAction : GoapAction
{

    private bool IsHidden = false;
    public GameObject targetTank = null;
    public GameObject payload = null;
    //private GameObject BehindTank = null;

    public float tankToPlayerShiftRange = 10f;
    public float agentToPayloadShiftRange = 10f;    

    public HideAction()
    {
        addPrecondition("isTankNearby", new ZombieAgent.TankNear(true, true));
        addEffect("isHidden", true);
        cost = 1;
    }
    // Reset every variable about the action.
    public override void reset()
    {
        IsHidden = false;
        targetTank = null;
        payload = null;
       // BehindTank = null;
    }

    // Action is done if the player is dead.
    public override bool isDone()
    {
        return IsHidden;
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
        GameObject[] Tanks = GameObject.FindGameObjectsWithTag("BehindTank");

        GameObject closestTank = null;
        
        payload = GameObject.FindGameObjectWithTag("Payload");

        float closestDistance = 0;
        float closestDistanceTank = 0;

        foreach (PlayerController2 player in players)
        {
            if (closestPlayer == null)
            {
                closestPlayer = player;
                closestDistanceTank = (player.transform.position - agent.transform.position).magnitude;
            }
            else
            {
                float distance = (player.transform.position - agent.transform.position).magnitude;
                if (distance < closestDistance)
                {
                    closestPlayer = player;
                    closestDistanceTank = distance;
                }
            }
        }
        foreach (GameObject tank in Tanks)
        {
            if (closestTank == null && !tank.transform.parent.GetComponent<Com.MyCompany.MyGame.EnemyHealth>().IsDead)
            {
                closestTank = tank;
                closestDistanceTank = (tank.transform.position - agent.transform.position).magnitude;
            }
            else if (!tank.transform.parent.GetComponent<Com.MyCompany.MyGame.EnemyHealth>().IsDead)
            {
                float distance = (tank.transform.position - agent.transform.position).magnitude;
                if (distance < closestDistance)
                {
                    closestTank = tank;
                    closestDistanceTank = distance;
                }
            }
        }

        if (closestPlayer == null || closestTank == null)
            return false;

        targetTank = closestTank;
       // targetPlayerHealth = targetPlayer.GetComponent<PlayerHealth>();
        target = targetTank.gameObject;
        cost = 10 / (closestDistanceTank);
        return true;
    }

    // Realise the action.
    public override bool perform(GameObject agent)
    {
        ZombieAgent zombieAgent = GetComponent<ZombieAgent>();
        zombieAgent.moveAgent(this);

        // if tank dead abort the plan
        if (target.GetComponentInParent<Com.MyCompany.MyGame.EnemyHealth>().IsDead)
            return false;

        // If we are near the payload or if the tank is near the player, go to next action (isDone true)
        if ((payload != null && (agent.transform.position - payload.transform.position).magnitude <= agentToPayloadShiftRange) ||
            ((target.transform.position - target.GetComponentInParent<ZombieAgent>().LastTarget.position).magnitude <= tankToPlayerShiftRange))
            IsHidden = true;

        return true;
    }


    
}
