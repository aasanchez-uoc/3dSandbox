using UnityEngine;

/// <summary>
/// Esta clase implementa el estado de la FSM en el que perseguimos al objetivo
/// </summary>
public class ChaseState : State
{
    /// <summary>
    /// Campo privado el que actualizaremos la última posición conocida del enemigo
    /// </summary>
    private Vector3 lastKnownPosition;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="machine">La FSM que controla este estado</param>
    public ChaseState(EnemyStateMachine machine) : base(machine)
    {
    }
       
    /// <summary>
    /// La actualización del estado, que contiene toda la lógica para la persecución y el cambio de estado
    /// </summary>
    public override void Update()
    {
        //comprobamos si seguimos viendo al objetivo
        if (stateMachine.IsInFOV())
        {
            //actualizamos su ultima posicion conocida:
            lastKnownPosition = stateMachine.Target.transform.position;
            float distanceToTarget = (lastKnownPosition - stateMachine.transform.position).magnitude;

            //si estamos suficientemente cerca, cambiamos al estado de atacar:
            if (distanceToTarget <= stateMachine.AttackDistance)
            {
                stateMachine.ToAttackState();
            }
            //en caso contrario, actualizamos el punto de destino (seguimos persiguiendole):
            else
            {
                stateMachine.NavAgent.destination = lastKnownPosition;
            }

        }
        else
        {
            if(  (stateMachine.transform.position - lastKnownPosition).sqrMagnitude < 0.05)
            {
                //hemos llegado a la ultima posicion conocida y seguimos sin verle, volvemos al estado de wander:
                stateMachine.ToWanderState();
            }

        }
       
    }
    public override void Exit()
    {
    }

    public override void Start()
    {
        stateMachine.NavAgent.speed = stateMachine.ChaseSpeed;
    }
}
