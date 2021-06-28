using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Esta clase se encarga de gestionar que los objetos que contienen este script pueda soltar objetos al morir (al destruirse el gameObject)
/// </summary>
public class DropItem : MonoBehaviour
{
    /// <summary>
    /// Una lista de los objetos que el gameObject puede soltar al morir.
    /// Al destruirse soltará un elemento al azar del array
    /// </summary>
    public PickableItem[] ItemsPrefabs;


    /// <summary>
    /// Indica la probabilidad con la que el enemigo soltará un item al morir
    /// Si es >=100 Soltará siempre un objeto y si es <=0 no lo soltará nunca
    /// </summary>
    public int DropChance
    {
        get
        {
            return chance;
        }
        set
        {
            chance = Mathf.Clamp( value, 0, 100);
        }
    }

    [SerializeField]
    private int chance = 100;

    [HideInInspector]
    public bool dontDrop = false;
    public void Drop()
    {
        if(ItemsPrefabs.Length > 0 && DropChance > 0)
        {
            int index = Random.Range(0, ItemsPrefabs.Length);

            int c = Random.Range(0, 100);
            if (c <= DropChance)
            {
                Instantiate(ItemsPrefabs[index], transform.position + new Vector3(0, 1.5f, 0), Quaternion.identity);
            }
        }        
    }
    void OnApplicationQuit()
    {
        dontDrop = true;
    }

    public void OnDisable()
    {
        if (!dontDrop)
        {
            Drop();
        }

    }
}
