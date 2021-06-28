using BigRookGames.Weapons;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    #region Eventos
    public delegate void WeaponChangedHandler();
    public WeaponChangedHandler OnAmmoChanged;
    #endregion

    float timeFromLastShot;
    float FireRate = 11.1f;
    float Dispersion = 0.05f;
    int Damage = 10;
    int ammo = 100;
    GunfireController weapon;

    /// <summary>
    /// El prefab con el sistema de partículas que instanciaremos cuando nuestros disparos acierten a un enemigo
    /// </summary>
    public GameObject EnemyBloodParticlesPrefab;

    void Start()
    {
        weapon = GetComponentInChildren<GunfireController>();
    }

    void Update()
    {
        timeFromLastShot += Time.deltaTime;
        if (Input.GetMouseButton(0) && timeFromLastShot >= (1 / FireRate))
        {

            if (ammo > 0)
            {
                RaycastHit hit;
                Vector2 v1 = (new Vector2(Random.Range(-1, 1), Random.Range(-1, 1))).normalized;
                float k = (Random.Range(0f, Dispersion));
                Vector2 disp = v1 * k;

                float radius = 0.5f;
                if (Physics.SphereCast(weapon.transform.position, radius, -weapon.transform.right, out hit))
                {
                    if (hit.collider.gameObject.tag == "Enemy")
                    {
                        Vector3 dir = hit.point - transform.position;
                        hit.collider.gameObject.GetComponentInParent<EnemyStateMachine>().Hit(Damage);
                        Instantiate(EnemyBloodParticlesPrefab, hit.point + (radius/2) * dir, Quaternion.LookRotation(dir));
                    }

                }
                weapon.FireWeapon();
                timeFromLastShot = 0;
                ammo = ammo - 1;
                OnAmmoChanged?.Invoke();
            }
        }
    }

    public int getCurrentAmmo()
    {
        return ammo;
    }

    public void AddAmmo(int value)
    {
        ammo += value;
        OnAmmoChanged?.Invoke();
    }
}
