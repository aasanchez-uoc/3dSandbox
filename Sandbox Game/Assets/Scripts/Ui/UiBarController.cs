using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiBarController : MonoBehaviour
{
    public Slider bar;
    public Text ValueText;

    public int Value
    {
        set
        {
            ValueText.text = value.ToString();
            bar.value = value / (100.0f);
        }
    }
}
