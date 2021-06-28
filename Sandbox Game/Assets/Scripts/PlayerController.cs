using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Cameras;
using UnityStandardAssets.Characters.ThirdPerson;

public class PlayerController : MonoBehaviour
{
    #region Eventos
    public delegate void DataChangedHandler();

    public DataChangedHandler OnHealthChanged;
    public DataChangedHandler OnPlayerDeath;
    #endregion

    #region Propiedades públicas
    /// <summary>
    /// Propiedad que representa la vida del jugador
    /// </summary>
    public int Health { get; private set; } = 100;

    /// <summary>
    /// El tiempo en segundos que seremos invulnerables cuando recibamos un daño hasta el siguiente
    /// </summary>
    public float InvulnerabilityTime = 5f;

    /// <summary>
    /// El prefab con el sistema de partículas que instanciaremos cuando nos ataque un enemigo
    /// </summary>
    public GameObject PlayerBloodParticlesPrefab;
    #endregion

    #region Atributos privados
    /// <summary>
    /// Referencia al script WeaponManager del jugador, encargado de gestionar las armas y la munición.
    /// </summary>
    private WeaponManager weaponManager;

    /// <summary>
    /// EL tiempo desde la útlima vez que el personaje recibió daño
    /// </summary>
    private float timeFromLastHit = 5;

    /// <summary>
    /// Referencia al componente animator del jugador
    /// </summary>
    Animator m_Animator;

    private List<CarManager> carsInRange = new List<CarManager>();
    #endregion

    void Start()
    {
        weaponManager = GetComponentInChildren<WeaponManager>();
        m_Animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (Input.GetButtonDown("Action"))
        {
            if (carsInRange.Count > 0)
            {
                CarManager nearestCar = carsInRange[0];
                float nearestDist = Vector3.Distance(transform.position, nearestCar.transform.position);
                foreach (CarManager car in carsInRange)
                {
                    float dist2 = Vector3.Distance(transform.position, car.transform.position);
                    if (dist2 < nearestDist)
                    {
                        nearestCar = car;
                        nearestDist = dist2;
                    }
                }
                FreeLookCam cam = FindObjectOfType<FreeLookCam>();
                cam.SetTarget(nearestCar.transform);
                nearestCar.StartDriving(gameObject);
            }

        }

        if (Input.GetButtonDown("Cancel"))
        {
            Application.Quit();
        }

        if (timeFromLastHit < InvulnerabilityTime)
        {
            timeFromLastHit += Time.deltaTime;
        }
    }



    /// <summary>
    /// Método al que llamaremos cuando recibamos un ataque de un enemigo
    /// </summary>
    /// <param name="damage"></param>
    public void takeDamage(int damage)
    {
        m_Animator.SetTrigger("getHit");
        Health = Math.Max(Health - damage, 0);
        OnHealthChanged?.Invoke();

        if (Health <= 0)
        {
            weaponManager.enabled = false;
            GetComponent<ThirdPersonUserControl>().enabled = false;
            FindObjectOfType<FreeLookCam>().enabled = false;
            m_Animator.SetBool("isDead", true);
            OnPlayerDeath?.Invoke();
        }
    }

    /// <summary>
    /// Método que aumenta la vida del jugador en la cantidad indicada sin superar el máximo (100)
    /// </summary>
    /// <param name="value"></param>
    void addHealth(int value)
    {
        Health = Math.Min(100, Health + value);
        OnHealthChanged?.Invoke();
    }

    void OnTriggerEnter(Collider other)
    {
        //si el trigger es un item, miramos el tipo de item y actuamos en consecuencia
        if (other.tag == "Item")
        {
            PickableItem item = other.GetComponentInParent<PickableItem>();
            if (item != null)
            {
                switch (item.Type)
                {
                    //si es un itme de vida, recuperamos la cantidad indicada
                    case ItemType.health:
                        addHealth(item.Value);
                        break;
                    // si es un itme de munción, aumentamos nuestra munición la cantidad indicada
                    case ItemType.ammo:
                        weaponManager.AddAmmo( item.Value);
                        break;
                    default:
                        break;
                }
                Destroy(other.gameObject);
            }
        }
        //si se trata del enemigo, recibimos dañor:
        if(other.tag == "EnemyHitbox" && timeFromLastHit >= InvulnerabilityTime)
        {
            Vector3 dir = transform.position - other.transform.position;
            Instantiate(PlayerBloodParticlesPrefab, other.transform.position, Quaternion.LookRotation(dir));
            takeDamage(10);
            InvulnerabilityTime = 0;
        }

        if(other.tag == "Car")
        {
            CarManager car = other.GetComponentInParent<CarManager>();
            if (!carsInRange.Contains(car))
            {
                carsInRange.Add(car);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Car")
        {
            CarManager car = other.GetComponentInParent<CarManager>();
            carsInRange.Remove(car);
        }
    }
}
