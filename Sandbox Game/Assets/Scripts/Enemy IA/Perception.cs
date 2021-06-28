using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Esta clase se encarga de comprobar si el objetivo se encuentra a la vista del agente (a menos de cierta distancia, en el angulo de visión y sin obstáculos entre medias)
/// </summary>
public class Perception : MonoBehaviour
{
    /// <summary>
    /// El objetivo que ela gente intenta ver.
    /// </summary>
    public GameObject Target;

    /// <summary>
    /// La distancia máxima a la que ve nuestro agente.
    /// </summary>
    public float ViewDistance;

    /// <summary>
    /// Si se asigna, este elemento se mostrará cuando el objetivo esté a la vista y lo oculta cuando no.
    /// </summary>
    public GameObject icon;

    /// <summary>
    /// El águnlo de vision del agente, centrado alrededor de forward.
    /// </summary>
    public float viewAngle = 30f;

    /// <summary>
    /// Propiedad de solo lectura que nos indica si el objetivo se encuentra a la vista
    /// </summary>
    public bool IsInFOV { get; private set; }

    /// <summary>
    /// Referencia al componente NavMeshAgent
    /// </summary>
    private NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        bool isInFOV = false;

        //comprobamos si el objetivo esta dentro de la distancia y del angulo de vision
        Vector3 distanceToTarget = Target.transform.position - transform.position;
        float angle = Vector3.Angle(transform.forward, distanceToTarget);
        if (distanceToTarget.magnitude < ViewDistance && angle < viewAngle/2) 
        {
            //hacemos raycast al objetivo para comprobar si hay obstáculos
            if(!agent.Raycast(Target.transform.position, out NavMeshHit hit))
            {
                isInFOV = true;
            }
        }

        //actualizamos la propiedad
        IsInFOV = isInFOV;

        if (icon != null)
        {
            //mostramos el icono
            icon.SetActive(isInFOV);
        }
    }
}
