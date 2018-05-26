using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ZombieAgent), true)]
public class ZombieAgentEditor : Editor
{

    public float labelYOffset = 2f;

    private void OnSceneGUI()
    {
        ZombieAgent za = (ZombieAgent)target;

        if (za)
            drawRanges(za);
    }

    private void drawRanges(ZombieAgent agent)
    {
        Handles.color = Color.blue;
        Handles.DrawWireArc(agent.transform.position, Vector3.up, agent.transform.position + Vector3.right, 360, agent.hiddenByTankRange);

        Handles.color = Color.red;
        Handles.DrawWireArc(agent.transform.position, Vector3.up, agent.transform.position + Vector3.right, 360, agent.attackRange);
        Handles.color = new Color(1, 0.2f, 0.2f);
        Handles.DrawWireArc(agent.transform.position, Vector3.up, agent.transform.position + Vector3.right, 360, agent.closeAttackRange);

        Handles.color = Color.yellow;
        Handles.DrawWireArc(agent.transform.position, Vector3.up, agent.transform.position + Vector3.right, 360, agent.nearbyTankArea);

        Handles.color = Color.cyan;
        Handles.DrawWireArc(agent.transform.position, Vector3.up, agent.transform.position + Vector3.right, 360, agent.nearbyPlayerArea);
    }
}
