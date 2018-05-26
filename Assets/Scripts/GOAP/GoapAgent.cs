using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GoapAgent : MonoBehaviour {

    private FSM stateMachine;

    // Basic states for the GOAP
    private FSM.FSMState idleState;
    private FSM.FSMState moveToState;
    private FSM.FSMState performActionState;

    private List<GoapAction> availableActions;
    private Queue<GoapAction> currentActions;

    private int currentMoveToAttempts = 0;
    private int maximumMoveToAttempts = 100;

    // World to agent interface. Feeds world data and listens feedback.
    private IGoap dataProvider;
    private GoapPlanner planner;

    private bool hasActionPlan()
    {
        return currentActions.Count > 0;
    }

    public Queue<GoapAction> CurrentActions
    {
        get {
            if (currentActions != null)
                return new Queue<GoapAction>(currentActions);
            return null;
        }
    }

    public String CurrentFSMState
    {
        get
        {
            if (stateMachine != null)
            {
                FSM.FSMState state = stateMachine.getState();
                if (state.Equals(idleState))
                    return "Idle";
                else if (state.Equals(moveToState))
                    return "Move to";
                else if (state.Equals(performActionState))
                    return "Perform action";
            }
            return "NONE";
        }
    }

    // Available action managing.
    /**
     * Add an action to the available actions list.
     */
    public void addAction(GoapAction action)
    {
        availableActions.Add(action);
    }

    /**
     * Remove an action from the available actions list.
     */
    public void removeAction(GoapAction action)
    {
        availableActions.Remove(action);
    }

    /**
     * Get an action from the available actions list.
     */
    public GoapAction getAction(Type action)
    {
        return availableActions.Find(a => a.GetType().Equals(action));
    }


    private void Start()
    {
        stateMachine = new FSM();
        availableActions = new List<GoapAction>();
        currentActions = new Queue<GoapAction>();
        planner = new GoapPlanner();
        currentMoveToAttempts = 0;
        findDataProvider();

        // Create GOAP states
        createIdleState();
        createMoveToState();
        createPerformActionState();
        stateMachine.pushState(idleState);

        // Load every available actions
        loadActions();
    }

    private void Update()
    {
        stateMachine.Update(gameObject);
    }

    private void findDataProvider()
    {
        foreach (Component comp in gameObject.GetComponents<Component>())
        {
            if (typeof(IGoap).IsAssignableFrom(comp.GetType()))
            {
                dataProvider = (IGoap)comp;
                return;
            }
        }
    }

    // Creating states for the GOAP FSM.

    private void createIdleState()
    {
        idleState = (fsm, gameObj) =>
        {
            // Get world state and goal from data provider.
            Dictionary<string, object> worldState = dataProvider.getWorldState();
            Dictionary<string, object> goal = dataProvider.createGoalState();

            Queue<GoapAction> plan = planner.plan(gameObj, availableActions, worldState, goal);
            if (plan != null)
            {
                // Found a plan. Providing it to the data provider.
                currentActions = plan;
                dataProvider.planFound(goal, plan);

                // Move FSM to performAction state.
                fsm.popState();
                fsm.pushState(performActionState);
            }
            else
            {
                // No plan found for goal.
                Debug.Log("<color=orange>Failed plan:</color> " + prettyPrint(goal));
                dataProvider.planFailed(goal);

                // Going back to idleState.
                fsm.popState();
                fsm.pushState(idleState);
            }
        };
    }

    private void createMoveToState()
    {
        moveToState = (fsm, gameObj) =>
        {
            GoapAction action = currentActions.Peek();
            if (action.requiresInRange() && action.target == null)
            {
                // Action needs agent to be in range but there is no target.
                Debug.Log("<color=red>Fatal error:</color> Action requires a target but has none.\n" +
                    "Planning failed. You did not assign the target in your Action.checkProceduralPrecondition()\n" +
                    "Action failed: " + prettyPrint(action));

                currentMoveToAttempts = 0;

                // Remove 'move' and 'perform' states from the FSM and go back to idle.
                fsm.popState();
                fsm.popState();
                fsm.pushState(idleState);
                return;
            }

            // If the agent moved and is now in range remove the 'move' state from the FSM.
            if (dataProvider.moveAgent(action))
            {
                currentMoveToAttempts = 0;
                fsm.popState();
            }

            if (++currentMoveToAttempts > maximumMoveToAttempts)
            {
                currentMoveToAttempts = 0;
                fsm.popState();
                fsm.popState();
                fsm.pushState(idleState);
                return;
            }
        };
    }

    private void createPerformActionState()
    {
        performActionState = (fsm, gameObj) =>
        {
            // If no action to perform.
            if (!hasActionPlan())
            {
                Debug.Log("<color=green>Done actions</color>");

                // Remove the 'perform' state from the FSM and go back to idle.
                fsm.popState();
                fsm.pushState(idleState);

                // Inform the data provider we finished every actions.
                dataProvider.actionsFinished();
                return;
            }

            GoapAction action = currentActions.Peek();

            // If the current action is done, dequeue it.
            if (action.isDone())
                currentActions.Dequeue();

            // If we still have actions to do.
            if (hasActionPlan())
            {
                // Get the next action to perform.
                action = currentActions.Peek();
                bool inRange = action.requiresInRange() ? action.isInRange : true;

                // If we are in range, try performing the action.
                if (inRange)
                {
                    bool success = action.perform(gameObj);

                    // If the action failed, plan again.
                    if (!success)
                    {
                        // Remove the 'perform' state and go back to 'idle'.
                        fsm.popState();
                        fsm.pushState(idleState);

                        // Tell the data provider plan has been aborted because of action.
                        dataProvider.planAborted(action);
                    }
                }
                else
                {
                    // If we are not in range, use 'moveTo' state.
                    fsm.pushState(moveToState);
                }
            }
            else
            {
                // No more action to perform.

                // Remove the 'perform' state and go back to 'idle'.
                fsm.popState();
                fsm.pushState(idleState);

                // Tell the data provider we finished every actions.
                dataProvider.actionsFinished();
            }
        };
    }

    /*
     * Find every GoapAction on the gameObject in the availableActions list.
     */
    private void loadActions()
    {
        GoapAction[] actions = gameObject.GetComponents<GoapAction>();
        foreach (GoapAction a in actions)
            availableActions.Add(a);
        Debug.Log(availableActions.Count + " actions found.\nList: " + prettyPrint(actions));
    }

    public static string prettyPrint(Dictionary<string, object> state)
    {
        string s = "";
        foreach (KeyValuePair<string, object> kvp in state)
            s += kvp.Key + ':' + kvp.Value.ToString() + ", ";
        return s;
    }

    public static string prettyPrint(Queue<GoapAction> actions)
    {
        string s = "";
        foreach (GoapAction a in actions)
            s += a.GetType().Name + " -> ";
        s += "GOAL";
        return s;
    }

    public static string prettyPrint(GoapAction[] actions)
    {
        string s = "";
        foreach (GoapAction a in actions)
            s += a.GetType().Name + ", ";
        return s;
    }

    public static string prettyPrint(GoapAction action)
    {
        return action.GetType().Name;
    }
}
