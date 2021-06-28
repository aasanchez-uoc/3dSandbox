using UnityEngine;

/// <summary>
/// Esta clase implementa el estado de la FSM en el que perseguimos al objetivo
/// </summary>
public class AttackState : State
{
    /// <summary>
    /// Campo privado el que actualizaremos la última posición conocida del enemigo
    /// </summary>
    private Vector3 lastKnownPosition;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="machine">La FSM que controla este estado</param>
    public AttackState(EnemyStateMachine machine) : base(machine)
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

            //si el objetivo ya no está suficientemente cerca, le perseguimos
            if (distanceToTarget > stateMachine.AttackDistance)
            {
                stateMachine.ToChaseState();
            }

        }
        else
        {
            stateMachine.ToWanderState();

        }
       
    }
    public override void Exit()
    {
        stateMachine.m_Animator.SetBool("IsAttacking", false);
    }

    public override void Start()
    {
        stateMachine.m_Animator.SetBool("IsAttacking", true);
    }
}
