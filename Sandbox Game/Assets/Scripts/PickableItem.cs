using UnityEngine;

/// <summary>
/// Enum que indica los distintos tipos de objetos que podemos recoger
/// </summary>
public enum ItemType
{
    health,
    ammo
}

/// <summary>
/// Clase muy simple que contiene los valores de los objetos que el jugador puede recoger. La lógica se maneja en la clase PlayerController
/// </summary>
public class PickableItem : MonoBehaviour
{
    /// <summary>
    /// Propiedad que indica el tipo de objeto
    /// </summary>
    public ItemType Type;

    /// <summary>
    /// El valor asociado a este objeto, depende de su tipo (vida, municion...)
    /// </summary>
    public int Value;



}
