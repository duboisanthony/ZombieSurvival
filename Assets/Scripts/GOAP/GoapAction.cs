using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GoapAction : MonoBehaviour {


    private Dictionary<string, object> preconditions;
    private Dictionary<string, object> effects;

    public Dictionary<string, object> Preconditions
    {
        get
        {
            return preconditions;
        }
    }

    public Dictionary<string, object> Effects
    {
        get
        {
            return effects;
        }
    }

    private bool inRange = false;

    /**
	 * Is the agent in range of the target?
	 * MoveTo state will set this. This gets reset each time this action is performed.
	 */
    public bool isInRange
    {
        get { return inRange; }
        set { inRange = value; }
    }

    /** The cost of performing the action. 
	 * Figure out a weight that suits the action. 
	 * Changing it will affect what actions are chosen during planning.
     */
    public float cost = 1f;

    /**
	 * The object on which to perform the action. Can be null.
     */
    public GameObject target;

    public GoapAction()
    {
        preconditions = new Dictionary<string, object>();
        effects = new Dictionary<string, object>();
    }

    public void doReset()
    {
        reset();
        inRange = false;
        target = null;
    }

    /**
	 * Reset any variables that need to be reset before planning happens again.
	 */
    public abstract void reset();

    /**
	 * Is the action done?
	 */
    public abstract bool isDone();

    /**
	 * Procedurally check if this action can run. Not all actions
	 * will need this.
	 */
    public abstract bool checkProceduralPrecondition(GameObject agent);

    /**
	 * Run the action.
	 * Returns True if the action performed successfully or false
	 * if something happened and it can no longer perform.
     * In the later, the action queue should clear out and the goal cannot be reached.
	 */
    public abstract bool perform(GameObject agent);

    /**
	 * Does this action need to be within range of a target game object?
	 */
    public abstract bool requiresInRange();

    public void addPrecondition(string key, object value)
    {
        preconditions.Add(key, value);
    }


    public void removePrecondition(string key)
    {
        preconditions.Remove(key);
    }


    public void addEffect(string key, object value)
    {
        effects.Add(key, value);
    }


    public void removeEffect(string key)
    {
        effects.Remove(key);
    }
}
