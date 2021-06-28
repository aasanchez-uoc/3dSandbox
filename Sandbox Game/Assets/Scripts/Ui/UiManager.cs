using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UiManager : MonoBehaviour
{
    public UiBarController HealthBar;
    public Text AmmoText;
    public GameObject GameOverPanel;
    public PlayerController Player;
    public WeaponManager Weapon;

    void Start()
    {
        GameOverPanel.SetActive(false);
        Player.OnHealthChanged += UpdateHealth;

        Player.OnPlayerDeath += ShowGameOverPanel;

        Weapon.OnAmmoChanged += UpdateAmmoText;
        UpdateHealth();
        UpdateAmmoText();
    }


    void UpdateHealth()
    {
        HealthBar.Value = Player.Health;
    }


    void UpdateAmmoText()
    {
        AmmoText.text = Weapon.getCurrentAmmo().ToString();
    }

    void ShowGameOverPanel()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        GameOverPanel.SetActive(true);
    }

    public void ReloadScene()
    {
        var Drops = FindObjectsOfType<DropItem>();
        foreach (DropItem o in Drops)
        {
            o.dontDrop = true;
        }

        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }
}
