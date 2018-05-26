using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankZombie : ZombieAgent {

    public override Dictionary<string, object> createGoalState()
    {
        Dictionary<string, object> goal = new Dictionary<string, object>();

        goal.Add("playerLose", true);
        return goal;
    }
}
