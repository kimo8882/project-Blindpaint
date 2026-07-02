using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class MainMenu : MonoBehaviour
{
    [Header("Menu Panels")]
    public GameObject settingsPanel;

    void Start()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }
    }

    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (settingsPanel != null && settingsPanel.activeInHierarchy)
            {
                settingsPanel.SetActive(false);
            }
        }
    }
    public void SetMasterVolume(float volume)
    {
        AudioListener.volume = volume;
    }
    public void StartDeathmatch()
    {
        Debug.Log("THE BUTTON WAS CLICKED!"); 
        SceneManager.LoadScene("SampleScene");
    }

    public void OpenSettings()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(true);
        }
    }

    public void QuitToDesktop()
    {
        Debug.Log("Game Quit!"); 
        Application.Quit(); 
    }
}