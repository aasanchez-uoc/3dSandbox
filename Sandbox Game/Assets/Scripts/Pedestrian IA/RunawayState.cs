using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunawayState : PedestrianState
{
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="machine">La FSM que controla este estado</param>
    public RunawayState(PedestrianStateMachine machine) : base(machine)
    {
    }

    /// <summary>
    /// La actualización del estado, que contiene toda la lógica para alejarnos del objetivo y el cambio de estado
    /// </summary>
    public override void Update()
    {

        var enemies = GameObject.FindGameObjectsWithTag("Enemy");
        bool enemiesInDistance = false;
        Vector3 runawayVector = Vector3.zero;

        foreach (GameObject enemy in enemies)
        {
            float distanceToTarget = (enemy.transform.position - stateMachine.transform.position).magnitude;
            if (distanceToTarget < 2 * stateMachine.RunAwayDistance)
            {
                enemiesInDistance = true;
                //calculamos la direccion haia el objetivo
                Vector3 dirToTarget = enemy.transform.position - stateMachine.transform.position;
                //y le sumamos el vector contraria a nuestro vector de huída
                runawayVector -= dirToTarget;
            }
        }

        //si hay enemigos demasiado cerca, actualizamos el punto de destino (seguimos huyendo):
        if (enemiesInDistance)
        {
            //nos movemos esa cantidad en la direccion opuesta al enemigo
            stateMachine.NavAgent.destination = stateMachine.transform.position + stateMachine.RunAwayDistance * runawayVector.normalized;
        }
        //en caso contrario, cambiamos al estado de wander:
        else
        {
            stateMachine.ToWanderState();
        }

        
    }

    public override void Exit()
    {
    }

    public override void Start()
    {
        stateMachine.NavAgent.speed = stateMachine.runSpeed;
    }


}
