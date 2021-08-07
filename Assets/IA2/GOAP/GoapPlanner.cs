using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FSM2;
using UnityEngine;

public class GoapPlanner
{

    private const int _WATCHDOG_MAX = 200;

    private int _watchdog;

    private Func<IEnumerator, Coroutine> _GOAPCorroutine;
    public event Action<IEnumerable<GOAPAction>> OnPathCreated;



    public GoapPlanner(Func<IEnumerator, Coroutine> coroutine)
    {
        _GOAPCorroutine = coroutine;
    }

    public void Run(GOAPState from, GOAPState to, IEnumerable<GOAPAction> actions)
    {
        _watchdog = _WATCHDOG_MAX;

        var astar = new AStar<GOAPState>();

        astar.OnPathCreation += PathCreatedHandler;
        _GOAPCorroutine(astar.Run(from,
                                     state => Satisfies(state, to),
                                     node => Explode(node, actions, ref _watchdog),
                                     state => GetHeuristic(state, to)));

   
    }

    public void PathCreatedHandler(IEnumerable<GOAPState> states)
    {
        var IEstates = states.Skip(1).Select(state => state.generatingAction);
        Debug.Log("WATCHDOG " + _watchdog);
        OnPathCreated?.Invoke(IEstates);
    }

    public static FiniteStateMachine ConfigureFSM(IEnumerable<GOAPAction> plan, Func<IEnumerator, Coroutine> startCoroutine, FiniteStateMachine fsm)
    {
        var prevState = plan.First().linkedState;

        if (fsm != null)
        {
            fsm.Active = false;
            fsm.Clear();
        }

        fsm = new FiniteStateMachine(prevState, startCoroutine);

        foreach (var action in plan.Skip(1))
        {
            if (prevState == action.linkedState) continue;
            fsm.AddTransition("On" + action.linkedState.Name, prevState, action.linkedState);

            prevState = action.linkedState;
        }

        return fsm;
    }

    private IEnumerable<GOAPAction> CalculateGoap(IEnumerable<GOAPState> sequence)
    {
        foreach (var act in sequence.Skip(1))
        {
            Debug.Log(act);
        }

        Debug.Log("WATCHDOG " + _watchdog);

     
        return sequence.Skip(1).Select(x => x.generatingAction);
    }

  
    private static float GetHeuristic(GOAPState from, GOAPState goal) => goal.values.Count(kv => !kv.In(from.values));

 
    private static bool Satisfies(GOAPState state, GOAPState to) => to.values.All(kv => kv.In(state.values));


    private static IEnumerable<WeightedNode<GOAPState>> Explode(GOAPState node, IEnumerable<GOAPAction> actions,
                                                                ref int watchdog)
    {
        if (watchdog == 0) return Enumerable.Empty<WeightedNode<GOAPState>>();
        watchdog--;

        return actions.Where(action => action.preconditions.All(kv => kv.In(node.values)))
                      .Aggregate(new List<WeightedNode<GOAPState>>(), (possibleList, action) => {
                          var newState = new GOAPState(node); 
                          newState.values.UpdateWith(action.effects); 
                          newState.generatingAction = action; 
                          newState.step = node.step + 1; 

                          possibleList.Add(new WeightedNode<GOAPState>(newState, action.cost)); 
                          return possibleList;
                      });
    }
}