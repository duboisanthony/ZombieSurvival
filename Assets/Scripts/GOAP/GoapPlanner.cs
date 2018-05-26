using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoapPlanner : MonoBehaviour {

    // Usage of List<T> over HashSet<T> for less than 20 items is better.

    public Queue<GoapAction> plan(GameObject agent, List<GoapAction> availableActions,
        Dictionary<string, object> worldState, Dictionary<string, object> goal)
    {
        // Reset every action.
        foreach (GoapAction act in availableActions)
            act.doReset();

        // Create a list of each runnable action (i.e: that satisfies preconditions).
        List<GoapAction> usableActions = new List<GoapAction>();
        foreach (GoapAction act in availableActions)
            if (act.checkProceduralPrecondition(agent))
                usableActions.Add(act);

        // Build a tree with leaf leading to the goal.
        List<Node> leaves = new List<Node>();
        Node start = new Node(null, 0, worldState, null);

        Debug.Log("STARTING BUILD GRAPH");
        if (!buildGraph(start, leaves, usableActions, goal))
        {
            Debug.Log("No plan found for agent: " + agent.name + " currently at position " + agent.transform.position);
            return null;
        }

        // Find the cheapest leaf
        Node cheapest = null;
        leaves.ForEach((leaf) =>
        {
            if (cheapest == null)
                cheapest = leaf;
            else if (leaf.runningCost < cheapest.runningCost)
                cheapest = leaf;
        });

        List<GoapAction> result = new List<GoapAction>();
        Node n = cheapest;
        while (n != null)
        {
            if (n.action != null)
                result.Insert(0, n.action);
            n = n.parent;
        }

        // Store plan actions in a new queue.
        Queue<GoapAction> queue = new Queue<GoapAction>();
        result.ForEach(res =>
        {
            queue.Enqueue(res);
        });

        return queue;
    }

    private bool buildGraph(Node parent, List<Node> leaves, List<GoapAction> actions, Dictionary<string, object> goal)
    {
        bool foundPath = false;

        foreach (GoapAction act in actions)
        {
            // If preconditions can be satisfied by the parent of this action.
            if (inState(act.Preconditions, parent.state))
            {
                // Apply action's effect to the parent.
                Dictionary<string, object> currentState = populateState(parent.state, act.Effects);
                Node node = new Node(parent, parent.runningCost + act.cost, currentState, act);

                // If goal is reached.
                if (inState(goal, currentState))
                {
                    leaves.Add(node);
                    foundPath = true;
                }
                else
                {
                    List<GoapAction> subset = actionSubset(actions, act);
                    if (buildGraph(node, leaves, subset, goal))
                        foundPath = true;
                }
            }
        }

        return foundPath;
    }


    /**
     * Create a subset of actions and remove one.
     * Used for tree branching.
     */
    private List<GoapAction> actionSubset(List<GoapAction> actions, GoapAction toRemove)
    {
        List<GoapAction> subset = new List<GoapAction>(actions);
        subset.Remove(toRemove);
        return subset;
    }

    /**
     * Check if all items in 'test' are in 'state'.
     * If one is missing or not matching, returns false.
     */
     private bool inState(Dictionary<string, object> test, Dictionary<string, object> state)
    {
        foreach (KeyValuePair<string, object> t in test)
        {
            bool match = false;
            foreach (KeyValuePair<string, object> s in state)
            {
                if (s.Equals(t))
                {
                    match = true;
                    break;
                }
            }
            if (!match)
                return false;
        }
        return true;
    }


    private Dictionary<string, object> populateState(Dictionary<string, object> currentState, Dictionary<string, object> stateChange)
    {
        Dictionary<string, object> state = new Dictionary<string, object>(currentState);

        foreach (KeyValuePair<string, object> change in stateChange)
        {
            bool exists = false;
            string keyString = "";

            foreach (KeyValuePair<string, object> s in state)
            {
                if (s.Equals(change))
                {
                    exists = true;
                    keyString = s.Key;
                    break;
                }
            }

            if (exists)
                state.Remove(keyString);
            state.Add(change.Key, change.Value);
        }

        return state;
    }


    /**
     * Graph node for the planning resolution
     */
    private class Node
    {
        public Node parent;
        public float runningCost;
        public Dictionary<string, object> state;
        public GoapAction action;

        public Node(Node parent, float runningCost, Dictionary<string, object> state, GoapAction action)
        {
            this.parent = parent;
            this.runningCost = runningCost;
            this.state = state;
            this.action = action;
        }
    }
}
