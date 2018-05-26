using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NecromancerZombie : ZombieAgent
{
    public float satisfactionRange = 1.5f;

    public override Dictionary<string, object> createGoalState()
    {
        Dictionary<string, object> goal = new Dictionary<string, object>();

        goal.Add("safeBoost", true);
        return goal;
    }

    public override bool moveAgent(GoapAction nextAction)
    {
        lastTarget = nextAction.target.transform;

        // gameObject.transform.position = Vector3.MoveTowards(gameObject.transform.position, nextAction.target.transform.position, step);
        Vector3 nextTargetPosition = nextAction.target.transform.position;
        Vector3 direction = new Vector3(nextTargetPosition.x - transform.position.x, 0.0f, nextTargetPosition.z - transform.position.z);
        direction = Vector3.ClampMagnitude(direction, 1.0f);

        float currAccel = Mathf.Min(1f, Mathf.Max(0f, (transform.position - nextTargetPosition).magnitude - satisfactionRange)) * acceleration;

        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(direction), rotationDegreesPerSecond * Time.deltaTime);
        transform.position += transform.forward * Time.deltaTime * currAccel;
        _anim.SetBool("isMoving", true);

        if ((nextAction.target.transform.position - gameObject.transform.position).magnitude < attackRange / 2)
        {
            // we are at the target location, we are done
            nextAction.isInRange = true;
            _anim.SetBool("isMoving", false);
            return true;
        }
        else
            return false;
    }
}
