using System.Collections.Generic;
using FSM2;
using UnityEngine;

public class GOAPAction {
    public Dictionary<string, bool> preconditions { get; private set; }
    public Dictionary<string, bool> effects       { get; private set; }
    public string                   name          { get; private set; }
    public float                    cost          { get; private set; }
    public IState                   linkedState   { get; private set; }

    public GOAPAction(string name) {
        this.name     = name;
        cost          = 1f;
        preconditions = new Dictionary<string, bool>();
        effects       = new Dictionary<string, bool>();
    }
    /// <summary>
    /// Sets the cost of the Action.
    /// </summary>
    /// <param name="cost">The cost of the action as a floating point.</param>
    /// <returns>A reference to self</returns>
    public GOAPAction SetCost(float cost = 1) {
        if (cost < 1f) {
            //Costs < 1f make the heuristic non-admissible. h() could overestimate and create sub-optimal results.
            //https://en.wikipedia.org/wiki/A*_search_algorithm#Properties
            Debug.Log(string.Format("Warning: Using cost < 1f for '{0}' could yield sub-optimal results", name));
        }

        this.cost = cost;
        return this;
    }
    /// <summary>
    /// Sets a Precondition for this action.
    /// </summary>
    /// <param name="s">The identifier/name of the precondition</param>
    /// <param name="value">The value of the precondition</param>
    /// <returns>A reference to self</returns>
    public GOAPAction SetPre(string s, bool value) {
        preconditions[s] = value;
        return this;
    }
    /// <summary>
    /// Sets an Effect for this action.
    /// </summary>
    /// <param name="s">Name of the Effect</param>
    /// <param name="value">Value of the Effect</param>
    /// <returns>A reference to self</returns>
    public GOAPAction SetEffect(string s, bool value) {
        effects[s] = value;
        return this;
    }
    /// <summary>
    /// Sets a fsm state assosiated to this particular action.
    /// </summary>
    /// <param name="state">A reference to the linked state.</param>
    /// <returns>A reference to self</returns>
    public GOAPAction SetLinkedState(IState state) {
        linkedState = state;
        return this;
    }
}
