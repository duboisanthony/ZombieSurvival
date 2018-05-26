using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GoapAgent), true)]
public class GoapAgentEditor : Editor {

    public float labelYOffset = 2f;

    private void OnSceneGUI()
    {
        GoapAgent ga = (GoapAgent)target;
        Queue<GoapAction> currentActions = ga.CurrentActions;

        if (currentActions != null)
            drawCurrentPlan(ga, currentActions);
        if (currentActions != null && currentActions.Count > 0)
        {
            drawCurrentAction(ga, currentActions.Peek());
        }
    }

    private void drawCurrentPlan(GoapAgent agent, Queue<GoapAction> actions)
    {
        GUIStyle style = new GUIStyle();
        style.normal.textColor = Color.green;

        Handles.BeginGUI();
        GUILayout.Label("Current plan:\r\n" + GoapAgent.prettyPrint(actions) + "\r\n\r\n" +
            "Currently in state: " + agent.CurrentFSMState + "\r\n\r\n", style);
        Handles.EndGUI();
    }

    private void drawCurrentAction (GoapAgent agent, GoapAction action)
    {
        Handles.color = Color.green;
        if (action.target != null)
        {
            GUIStyle style = new GUIStyle();
            style.normal.textColor = Color.red;

            Handles.BeginGUI();
            GUILayout.Label("Current action: " + GoapAgent.prettyPrint(action) + '\n' +
                "Current target: " + action.target.name + '\n' +
                "Currently in range? " + action.isInRange, style);
            Handles.EndGUI();

            Handles.DrawLine(agent.transform.position, action.target.transform.position);
            Handles.Label(action.target.transform.position + labelYOffset * Vector3.up, "Target", style);

            style.normal.textColor = Color.green;
            Handles.Label(agent.transform.position + labelYOffset * Vector3.up, "Distance to target: " +
                (action.target.transform.position - agent.transform.position).magnitude, style);
        }
    }
}
