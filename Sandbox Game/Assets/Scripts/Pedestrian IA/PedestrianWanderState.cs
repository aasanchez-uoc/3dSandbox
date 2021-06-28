using UnityEngine;
/// <summary>
/// Esta clase implementa el estado de la FSM en el que el agente pasea por el escenario
/// </summary>
public class PedestrianWanderState : PedestrianState
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
    public PedestrianWanderState(PedestrianStateMachine machine) : base(machine) { }
    #endregion

    #region Métodos Públicos

    /// <summary>
    /// La actualización del estado, que contiene toda la lógica para deambular por el escenario  y el cambio de estado
    /// </summary>
    public override void Update()
    {

        var enemies = GameObject.FindGameObjectsWithTag("Enemy");
        bool enemiesInDistance = false;

        foreach (GameObject enemy in enemies)
        {
            float distanceToTarget = (enemy.transform.position - stateMachine.transform.position).magnitude;
            if (distanceToTarget < stateMachine.RunAwayDistance)
            {
                enemiesInDistance = true;
                break;
            }
        }

        if (enemiesInDistance)
        {
            stateMachine.ToRunawayState();
        }
        //si el objetivo no está a la vista, seguimos deambulando
        else
        {
            //if (!stateMachine.lastSidewalk.bounds.Contains(stateMachine.transform.position))
            //{
            //    if (!stateMachine.lastSidewalk.bounds.Contains(lastDestination))
            //    {
            //        lastDestination = RandomPointInBounds(stateMachine.lastSidewalk.bounds);
            //    }
            //}
            //else
            //{
            //    lastDestination = calculateDestination();
            //}
            if ((stateMachine.transform.position - lastDestination).magnitude < 0.1f)
            {
                lastDestination = RandomPointInBounds(stateMachine.lastSidewalk.bounds);
            }

        }
        stateMachine.NavAgent.SetDestination(lastDestination);

    }

    public override void Exit()
    {
    }

    public override void Start()
    {
        stateMachine.NavAgent.speed = stateMachine.walkSpeed;
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

    public static Vector3 RandomPointInBounds(Bounds bounds)
    {
        return new Vector3(
            Random.Range(bounds.min.x, bounds.max.x),
            0,
            Random.Range(bounds.min.z, bounds.max.z)
        );
    }

    /// <summary>
    /// Método que calcula el siguiente punto de destino del agente
    /// </summary>
    /// <returns></returns>
    private Vector3 calculateDestination()
    {

        Vector3 futureLocation = stateMachine.gameObject.transform.position + stateMachine.NavAgent.velocity.normalized * stateMachine.SteeringDistance;
        futureLocation += getRandomDirection() * stateMachine.WanderStrength;
        return futureLocation;
    }
    #endregion

}
