using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsManager : MonoBehaviour
{
    public Slider VolumeSlider;
    public Dropdown ResolutionDropdown;
    public Toggle FullscreenToggle;

    void OnEnable()
    {   

        int vol = PlayerPrefs.GetInt("Volume", 100);
        VolumeSlider.value = vol;
        Resolution[] resolutions = Screen.resolutions;

        List<string> m_DropOptions = new List<string>();
        int i = 0;
        int index = 0;
        foreach(Resolution res in resolutions) 
        {
            m_DropOptions.Add(res.width + "x" + res.height + ", " + res.refreshRate + "Hz");
            if(Screen.currentResolution.height == res.height && Screen.currentResolution.width == res.width)
            {
                index = i;
            }
            i++;
        }
        
        ResolutionDropdown.ClearOptions();
        ResolutionDropdown.AddOptions(m_DropOptions);
        ResolutionDropdown.value = index;

        FullscreenToggle.isOn = Screen.fullScreen;
    }

    public void setVolumeText()
    {
        VolumeSlider.GetComponentInChildren<Text>().text = VolumeSlider.value.ToString();
    }


    public void SaveValues()
    {
        PlayerPrefs.SetInt("Volume", (int)VolumeSlider.value);
        Resolution res = Screen.resolutions[ResolutionDropdown.value];
        Screen.SetResolution(res.width, res.height, FullscreenToggle.isOn, res.refreshRate);
        HideOptions();
    }

    public void HideOptions()
    {
        gameObject.SetActive(false);
    }

    public void ShowOptions()
    {
        gameObject.SetActive(false);
    }
}
