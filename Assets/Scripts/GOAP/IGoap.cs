using System.Collections.Generic;

public interface IGoap {

    /**
	 * Agent and world state.
	 */
    Dictionary<string, object> getWorldState();

    /**
     * Provides a new goal to the planner.
	 */
    Dictionary<string, object> createGoalState();

    /**
	 * No sequence of actions satisfy the goal.
	 */
    void planFailed(Dictionary<string, object> failedGoal);

    /**
     * Plan to reach goal has been found.
     * Actions are provided in the correct order.
	 */
    void planFound(Dictionary<string, object> goal, Queue<GoapAction> actions);

    /**
	 * Actions all completed and goal reached.
	 */
    void actionsFinished();

    /**
     * An action has caused the plan to abort.
	 */
    void planAborted(GoapAction aborter);

    /**
	 * Called during Update. Move the agent towards the target in order
	 * for the next action to be able to perform.
     * If the agent is near enough of the target returns true.
     * False if it is not the case.
	 */
    bool moveAgent(GoapAction nextAction);
}
