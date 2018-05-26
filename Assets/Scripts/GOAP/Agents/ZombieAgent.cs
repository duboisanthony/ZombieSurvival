using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * General zombie class.
 * Implement createGoalState() method  in subclasses
 * to populate the goal for the GOAP planner.
 */
public abstract class ZombieAgent : MonoBehaviour, IGoap {

    public struct TankNear
    {
        public bool isNear;
        public bool toConsider;

        public TankNear(bool near, bool consider)
        {
            isNear = near;
            toConsider = consider;
        }

        public override string ToString()
        {
            return ("isNear: " + isNear + " | toConsider: " + toConsider);
        }
    }

    public float moveSpeed = 1;

    public float acceleration;
    public float rotationDegreesPerSecond = 360;

    public float hiddenByTankRange = 8f;

    public float closeAttackRange = 1.2f;
    public float attackRange = 2.2f;

    private bool isBuffed = false;

    public bool IsBuffed { get { return isBuffed; } }

    public float buffDuration = 3f;
    public int buffBoost = 2;

    private ParticleSystem particleSystem;

    public float timeBetweenAttacks = 0.5f;
    [SerializeField]
    private int damages = 10;

    public int Damages { get { return isBuffed ? damages * buffBoost : damages; } }

    public float nearbyTankArea = 10.0f;
    public float nearbyPlayerArea = 3f;

    public float checkNearbyEntityFrequency = 1.5f;
    private bool playerNear = false;

    protected Animator _anim;

    protected Transform lastTarget = null;

    public Transform LastTarget { get { return lastTarget; } }

    public bool PlayerNear { get { return playerNear; } }

    public List<Collider> nearbyEntities = new List<Collider>();

    void Awake()
    {
        _anim = GetComponent<Animator>();
        particleSystem = GetComponent<ParticleSystem>();
        StartCoroutine("checkNearbyEntities");
    }

    void Update()
    {
    }

    public void Boost()
    {
        if (!isBuffed)
        {
            particleSystem.Play();
            isBuffed = true;
            StartCoroutine("buffZombie");
        }
    }

    IEnumerator buffZombie()
    {
        yield return new WaitForSeconds(buffDuration);
        isBuffed = false;
        particleSystem.Stop();
    }

    IEnumerator checkNearbyEntities()
    {
        Collider[] cols;
        bool foundPlayerEntity;

        while (true)
        {
            cols = Physics.OverlapSphere(transform.position, nearbyTankArea);
            nearbyEntities = new List<Collider>(cols);
            foundPlayerEntity = false;
            foreach (Collider col in cols)
                if (col.GetComponent<PlayerController2>() != null && (col.transform.position - transform.position).magnitude < nearbyPlayerArea)
                {
                    playerNear = true;
                    foundPlayerEntity = true;
                    break;
                }
            if (!foundPlayerEntity)
                playerNear = false;
            yield return new WaitForSeconds(checkNearbyEntityFrequency);
        }
    }

    /**
	 * Key/Value data that will feed the GOAP actions and system while planning.
	 */
    public Dictionary<string, object> getWorldState()
    {
        Dictionary<string, object> worldData = new Dictionary<string, object>();

        TankNear foundOne = new TankNear(false, false);
        Collider[] cols = Physics.OverlapSphere(transform.position, nearbyTankArea);
        foreach (Collider col in cols)
            if (col.GetComponent<TankZombie>() != null)
            {
                foundOne.isNear = true;
                if (col.GetComponent<ZombieAgent>().PlayerNear == false ||
                    (GetComponent<Com.MyCompany.MyGame.EnemyHealth>() && !GetComponent<Com.MyCompany.MyGame.EnemyHealth>().IsDead) ||
                    (Com.MyCompany.MyGame.GameManager.Instance._payload.transform.position - transform.position).magnitude < (col.transform.position - transform.position).magnitude)
                {
                    foundOne.toConsider = true;
                    break;
                }
            }
        worldData.Add("isTankNearby", foundOne);

        return worldData;
    }

    /**
	 * Implement in subclasses
	 */
    public abstract Dictionary<string, object> createGoalState();


    public void planFailed(Dictionary<string, object> failedGoal)
    {
        // Not handling this here since we are making sure our goals will always succeed.
        // But normally you want to make sure the world state has changed before running
        // the same goal again, or else it will just fail.
    }

    public void planFound(Dictionary<string, object> goal, Queue<GoapAction> actions)
    {
        Debug.Log("<color=green>Plan found</color> " + GoapAgent.prettyPrint(actions));
    }

    public void actionsFinished()
    {
        // Goal has been reached, every action succeded.
        Debug.Log("<color=blue>Actions completed</color>");
    }

    public void planAborted(GoapAction aborter)
    {
        // An action failed and made the plan abort. State has been reset to plan again.
        Debug.Log("<color=red>Plan Aborted</color> " + GoapAgent.prettyPrint(aborter));
    }

    public virtual bool moveAgent(GoapAction nextAction)
    {
        lastTarget = nextAction.target.transform;

        // move towards the NextAction's target
        float step = moveSpeed * Time.deltaTime;

        // gameObject.transform.position = Vector3.MoveTowards(gameObject.transform.position, nextAction.target.transform.position, step);
        Vector3 nextTargetPosition = nextAction.target.transform.position;
        Vector3 direction = new Vector3(nextTargetPosition.x - transform.position.x, 0.0f, nextTargetPosition.z - transform.position.z);
        direction = Vector3.ClampMagnitude(direction, 1.0f);

        float currAccel = Mathf.Min(1f, (transform.position - nextTargetPosition).magnitude) * acceleration;

        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(direction), rotationDegreesPerSecond * Time.deltaTime);
        transform.position += transform.forward * Time.deltaTime * currAccel;
        _anim.SetBool("isWalking", true);

        if (nextAction.target.GetComponentInParent<TankZombie>() != null &&
            (nextAction.target.GetComponentInParent<Transform>().position - gameObject.transform.position).magnitude < hiddenByTankRange)
        {
            // we are behind the zombie
            nextAction.isInRange = true;
          //  _anim.SetBool("isWalking", false);
            return true;
        }
        else if ((nextAction.target.transform.position - gameObject.transform.position).magnitude < closeAttackRange)
        {
            // we are at the target location, we are done
            nextAction.isInRange = true;
           // _anim.SetBool("isWalking", false);
            return true;
        }
        else
            return false;
    }
}