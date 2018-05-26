using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ZombieAgentWindow : EditorWindow {

    [MenuItem("Window/ZombieAgent infos")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow<ZombieAgentWindow>("ZombieAgent infos");
    }

    public void OnInspectorUpdate()
    {
        Repaint();
    }

    public void OnGUI()
    {
        if (Selection.activeGameObject != null)
        {
            ZombieAgent zombieAgent = Selection.activeGameObject.GetComponent<ZombieAgent>();
            if (zombieAgent)
            {
                GUILayout.Label("Current position: (" + zombieAgent.transform.position.x + ", " +
                    zombieAgent.transform.position.y + ", " + zombieAgent.transform.position.z + ").");

                GUILayout.Label("Is currently buffed: " + zombieAgent.IsBuffed);

                Dictionary<string, object> worldState = zombieAgent.getWorldState();

                GUILayout.Label("Current World state:");

                foreach (KeyValuePair<string, object> state in worldState)
                {
                    GUILayout.Label("\t- " + state.Key + ": " + state.Value.ToString());
                }
            }
        }
    }
}
