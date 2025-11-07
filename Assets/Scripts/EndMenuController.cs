using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

// Contrôle pour les scènes de fin (victoire ou défaite)
public class EndMenuController : MonoBehaviour
{
    [Header("Configuration")]
    [Tooltip("True si cette scène représente une victoire, false si défaite")]
    public bool isWinScene = true;

    [Tooltip("Nom de la scène de démarrage (menu principal)")]
    public string startSceneName = "SceneStart";

    [Tooltip("Nom de la première scène de gameplay à relancer directement")]
    public string firstGameplaySceneName = "SceneChateau"; // adapte selon ton build

    [Tooltip("Relancer en allant directement à la première scène de gameplay (sinon retour menu start)")]
    public bool replayDirectToGameplay = false;

    [Header("Raccourcis clavier")]
    public bool enableKeyboardShortcuts = true;

    [Tooltip("Touche pour relancer (Enter / Space)")]
    public bool allowEnterOrSpace = true;
    [Tooltip("Touche pour quitter (Escape)")]
    public bool allowEscapeQuit = true;

    private bool _actionLock = false;

    private void Awake()
    {
        // S'assurer que le temps reprend (au cas où une ancienne scène l'avait mis en pause)
        if (Time.timeScale == 0f) Time.timeScale = 1f;
    }

    private void Update()
    {
        if (!enableKeyboardShortcuts || _actionLock) return;
        var kb = Keyboard.current;
        if (kb == null) return;

        if (allowEnterOrSpace && (kb.enterKey.wasPressedThisFrame || kb.numpadEnterKey.wasPressedThisFrame || kb.spaceKey.wasPressedThisFrame))
        {
            OnReplayClicked();
        }
        if (allowEscapeQuit && kb.escapeKey.wasPressedThisFrame)
        {
            OnQuitClicked();
        }
    }

    // Bouton UI: Relancer
    public void OnReplayClicked()
    {
        if (_actionLock) return;
        _actionLock = true;

        string target = replayDirectToGameplay ? firstGameplaySceneName : startSceneName;
        if (string.IsNullOrEmpty(target))
        {
            Debug.LogError("EndMenuController: target replay scene non défini", this);
            _actionLock = false;
            return;
        }
        SceneManager.LoadScene(target);
    }

    // Bouton UI: Quitter
    public void OnQuitClicked()
    {
        if (_actionLock) return;
        _actionLock = true;
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
