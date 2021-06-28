using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Este Monobehaviour implementa la FSM que controla los distintos estados del agente
/// </summary>
public class EnemyStateMachine : MonoBehaviour
{
    #region Campos Privados

    /// <summary>
    /// Campo en el que almacenamos el estado actual de la máquina de estados.
    /// </summary>
    private State currentState;

    /// <summary>
    /// Componente encargado de comprobar si estamos viendo al objetivo
    /// </summary>
    private Perception agentPerception;

    /// <summary>
    /// El estado de la FSM en el que el agente se pasea por el escenario
    /// </summary>
    private WanderState wanderState;

    /// <summary>
    /// El estado de la FSM en el que el agente ataca al objetivo
    /// </summary>
    private AttackState attackState;

    /// <summary>
    /// El estado de la FSM en el que el agente persigue al objetivo
    /// </summary>
    private ChaseState chaseState;

    /// <summary>
    /// Booleano que indica si este enemigo ha muerto
    /// </summary>
    private bool isDead = false;


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
    /// La distancia hasta donde ve nuestro agente
    /// </summary>
    public float ViewDistance;

    /// <summary>
    /// La distancia a paritr de la cual el agente atacará al objetivo
    /// </summary>
    public float AttackDistance;
    
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

    /// <summary>
    /// El objetivo al que buscamos y al que perseguiremos
    public GameObject Target;

    public float WalkSpeed;

    public float ChaseSpeed;
    #endregion


    void Start()
    {
        //obtenemos e inicializamos los componentes
        if (this.Target == null )
        {
            Target = GameObject.FindGameObjectWithTag("Player");
        }
        agentPerception = gameObject.AddComponent<Perception>();
        agentPerception.Target = this.Target;
        agentPerception.ViewDistance = this.ViewDistance;
        agentPerception.icon = ViewIcon;
        NavAgent = GetComponent<NavMeshAgent>();
        m_Animator = GetComponent<Animator>();

        //creamos los estados
        wanderState = new WanderState(this);
        chaseState = new ChaseState(this);
        attackState = new AttackState(this);

        //fijamos el estado inicial
        currentState = wanderState;
        currentState.Start();
    }

    void Update()
    {
        if (!isDead)
        {
            currentState.Update();
            var vel = NavAgent.velocity.normalized;
            m_Animator.SetFloat("Forward", vel.magnitude, 0.1f, Time.deltaTime);
        }

    }

    /// <summary>
    /// Método que indica si el objetivo se encuentra a la vista del agente
    /// </summary>
    /// <returns></returns>
    public bool IsInFOV() {
        return agentPerception.IsInFOV;
    }

    /// <summary>
    /// Método encargado de cambiar al estado de pasear por el escenario
    /// </summary>
    public void ToWanderState()
    {
        ChangeState(wanderState);
    }

    /// <summary>
    /// Método encargado de cambiar al estado de perseguir al objetivo
    /// </summary>
    public void ToChaseState()
    {
        ChangeState(chaseState);
    }

    /// <summary>
    /// Método encargado de cambiar al estado de atacar al objetivo
    /// </summary>
    public void ToAttackState()
    {
        ChangeState(attackState);
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
                //m_manager.OnZombieDead();
            }
        }

    }

    /// <summary>
    /// Método encargado de cambiar entre estados
    /// </summary>
    /// <param name="newState">el nuevo estado al que pasamos</param>
    private void ChangeState(State newState)
    {
        currentState.Exit();
        currentState = newState;
        newState.Start();
    }



}
