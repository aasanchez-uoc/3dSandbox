using UnityEngine;
/// <summary>
/// Esta clase implementa el estado de la FSM en el que el agente pasea por el escenario
/// </summary>
public class WanderState : State
{
    #region Campos Privados
    /// <summary>
    /// Campo privado el en que almacenamos la última posicion establecida como destino
    /// </summary>
    private Vector3 lastDestination;
    #endregion

    #region Constructor
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="machine">La FSM que controla este estado</param>
    public WanderState(EnemyStateMachine machine) : base(machine) { }
    #endregion

    #region Métodos Públicos

    /// <summary>
    /// La actualización del estado, que contiene toda la lógica para deambular por el escenario  y el cambio de estado
    /// </summary>
    public override void Update()
    {
        //comprobamos si el objetivo está a la vista:
        if (stateMachine.IsInFOV())
        {
            //calculamos la distancia al objetivo
            float distanceToTarget = ( stateMachine.Target.transform.position - stateMachine.transform.position).magnitude;
            if(distanceToTarget <= stateMachine.AttackDistance)
            {
                stateMachine.ToAttackState();
            }
            else
            {
                //en caso contrario cambiamos al estado de perseguir
                stateMachine.ToChaseState();
            }
        }
        //si el objetivo no está a la vista, seguimos deambulando
        else
        {
            lastDestination = calculateDestination();
            stateMachine.NavAgent.SetDestination(lastDestination);
        }

    }

    public override void Exit()
    {
    }

    public override void Start()
    {
        stateMachine.NavAgent.speed = stateMachine.WalkSpeed;
    }
    #endregion

    #region Métodos Privados
    /// <summary>
    /// Método que calcula un punto al azar en el círculo con centro en el origen y radio 1 (en el plano XZ)
    /// </summary>
    /// <returns></returns>
    private Vector3 getRandomDirection()
    {
        float randomAngle = Random.value * 2 * Mathf.PI;
        return new Vector3(Mathf.Sin(randomAngle), 0, Mathf.Cos(randomAngle));
    }

    /// <summary>
    /// Método que calcula el siguiente punto de destino del agente
    /// </summary>
    /// <returns></returns>
    private Vector3 calculateDestination()
    {
        Vector3 futureLocation = stateMachine.gameObject.transform.position + stateMachine.NavAgent.velocity.normalized * stateMachine.SteeringDistance;
        return futureLocation + getRandomDirection() * stateMachine.WanderStrength;
    }
    #endregion

}
