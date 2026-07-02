using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseMenuUI : NetworkBehaviour
{
    [Header("UI Panels")]
    public GameObject pausePanel;
    public GameObject settingsPanel;
    public GameObject hudCanvas;

    private PlayerController _playerController;
    private CameraController _cameraController;
    private PlayerGun _playerGun; 

    void Start()
    {
        _playerController = GetComponent<PlayerController>();
        _playerGun = GetComponentInChildren<PlayerGun>();
    }

    void Update()
    {
        if (!IsOwner) return;

        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (settingsPanel != null && settingsPanel.activeInHierarchy)
            {
                settingsPanel.SetActive(false);
                pausePanel.SetActive(true);
            }
            else
            {
                TogglePauseMenu();
            }
        }
    }

    private void TogglePauseMenu()
    {
        bool isPaused = !pausePanel.activeInHierarchy;
        pausePanel.SetActive(isPaused);

        if (hudCanvas != null) hudCanvas.SetActive(!isPaused);

        if (_cameraController == null && Camera.main != null)
        {
            _cameraController = Camera.main.GetComponent<CameraController>();
        }

        if (isPaused)
        {
            if (_playerController != null) _playerController.enabled = false;
            if (_cameraController != null) _cameraController.enabled = false;
            if (_playerGun != null) _playerGun.enabled = false;
        }
        else
        {
            if (_playerController != null) _playerController.enabled = true;
            if (_cameraController != null) _cameraController.enabled = true;
            if (_playerGun != null) _playerGun.enabled = true;
        }
    }

    public void ClickContinue()
    {
        TogglePauseMenu();
    }

    public void ClickSettings()
    {
        pausePanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(true);
    }

    public void ClickExitToMainMenu()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.Shutdown();
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        SceneManager.LoadScene(0);
    }

    public void SetMasterVolume(float volume)
    {
        AudioListener.volume = volume;
    }
}