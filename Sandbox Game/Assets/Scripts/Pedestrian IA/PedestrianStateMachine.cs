using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Este Monobehaviour implementa la FSM que controla los distintos estados del agente
/// </summary>
public class PedestrianStateMachine : MonoBehaviour
{
    #region Campos Privados

    /// <summary>
    /// Campo en el que almacenamos el estado actual de la máquina de estados.
    /// </summary>
    private PedestrianState currentState;

    /// <summary>
    /// El estado de la FSM en el que el agente se pasea por el escenario
    /// </summary>
    private PedestrianWanderState wanderState;

    /// <summary>
    /// El estado de la FSM en el que el agente 
    /// </summary>
    private RunawayState runawayState;

    /// <summary>
    /// Booleano que indica si este enemigo ha muerto
    /// </summary>
    private bool isDead = false;

    public Collider lastSidewalk {  get; private set; }
    #endregion

    #region Campos Públicos

    /// <summary>
    /// Parámetro para el estado de wander que representa la intensidad con la que varía su direccion de movimiento
    /// </summary>
    public float WanderStrength = 2f;

    /// <summary>
    /// Parámetro para el estado de wander
    /// </summary>
    public float SteeringDistance = 6f;

    /// <summary>
    /// La vida del enemigo
    /// </summary>
    public int Health = 100;
    
        /// <summary>
    /// La distancia a paritr de la cual empezaremos a alejarnos del enemigo
    /// </summary>
    public float RunAwayDistance;

    public float walkSpeed = 1;

    public float runSpeed = 3;

    /// <summary>
    /// Propiedad de solo lectura  para acceder al componente NavMeshAgent de este GameObject
    /// </summary>
    public NavMeshAgent NavAgent { get; private set; }

    /// <summary>
    /// Propiedad de solo lectura  para acceder al componente Animator de este GameObject
    /// </summary>
    public Animator m_Animator { get; private set; }

    /// <summary>
    /// Si se asigna, este elemento se mostrará cuando el objetivo esté a la vista y lo oculta cuando no.
    /// </summary>
    public GameObject ViewIcon;

    #endregion


    void Start()
    {
        //obtenemos e inicializamos los componentes
        NavAgent = GetComponent<NavMeshAgent>();
        m_Animator = GetComponent<Animator>();

        //creamos los estados
        wanderState = new PedestrianWanderState(this);
        runawayState = new RunawayState(this);

        //fijamos el estado inicial
        currentState = wanderState;
        currentState.Start();
    }

    void Update()
    {
        if (!isDead)
        {
            currentState.Update();
            var vel = NavAgent.velocity;
            m_Animator.SetFloat("Forward", vel.magnitude, 0.1f, Time.deltaTime);
        }

    }

    /// <summary>
    /// Método encargado de cambiar al estado de pasear por el escenario
    /// </summary>
    public void ToWanderState()
    {
        ChangeState(wanderState);
    }

    public void ToRunawayState()
    {
        ChangeState(runawayState);
    }


    /// <summary>
    /// Método al que llamaremos cuando el enemigo reciba on disparo o golpe
    /// </summary>
    /// <param name="damage"></param>
    public void Hit(int damage)
    {
        if (!isDead) //si el enemigo ya está muerto, no recibe más daño
        {
            //Indicamos al animator que reproduzca la animación de recibir daño
            m_Animator.SetTrigger("getHit");

            //disminuimos la vida
            Health = Math.Max(Health - damage, 0);

            //comprobamos si ha muerto
            if (Health <= 0)
            {
                isDead = true;
                m_Animator.SetBool("isDead", true);
            }
        }

    }

    /// <summary>
    /// Método encargado de cambiar entre estados
    /// </summary>
    /// <param name="newState">el nuevo estado al que pasamos</param>
    private void ChangeState(PedestrianState newState)
    {
        currentState.Exit();
        currentState = newState;
        newState.Start();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Sidewalk")
        {
            if (lastSidewalk != other)
            {
                lastSidewalk = other;
            }
        }
    }

}
