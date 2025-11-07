using System;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Collider2D))]
public class TeleporteController : MonoBehaviour
{
    [Header("Progression séquentielle")] 
    [SerializeField]
    [Tooltip("ID du SpawnPoint à utiliser dans la scène suivante (doit exister dans la scène suivante)")]
    private string sequenceSpawnId;

    [Header("Fin de jeu")]
    [SerializeField]
    [Tooltip("Nom de la scène à charger quand on atteint la dernière scène (victoire)")]
    private string winSceneName = "SceneWin"; // Assure-toi d'ajouter la scène dans les Build Settings

    private void Reset()
    {
        // S'assurer que le collider est configuré en "Is Trigger" pour une zone de téléportation 2D
        // (ainsi, le joueur peut traverser la zone et déclencher l'événement sans collision physique)
        var col = GetComponent<Collider2D>();
        if (col != null)
        {
            col.isTrigger = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        StartCoroutine(DelayedTeleportCheck(other.gameObject));
    }

    private System.Collections.IEnumerator DelayedTeleportCheck(GameObject player)
    {
        // Attendre une frame pour laisser d'autres triggers/handlers s'exécuter
        yield return null;

        // if WinManager exists and a win is active (or already reached), skip teleport
        if (WinManager.Instance != null && WinManager.Instance.HasWon())
        {
            Debug.Log($"{nameof(TeleporteController)}: teleport skipped because Win active", this);
            yield break;
        }

        // Progression séquentielle uniquement: on passe à la scène suivante dans les Build Settings
        ChargerSceneSuivante(player);
    }

    private void ChargerSceneSuivante(GameObject player)
    {
        var active = SceneManager.GetActiveScene();
        int idx = active.buildIndex;

        if (idx < 0)
        {
            Debug.LogWarning($"{nameof(TeleporteController)} sur {name}: scène active non listée dans les Build Settings.", this);
            return;
        }

    int nextIndex = idx + 1; // scène suivante
        int total = SceneManager.sceneCountInBuildSettings;

        if (nextIndex >= total)
        {
            // Dernière scène atteinte -> charger la scène de victoire si configurée, sinon fallback WinManager
            if (!string.IsNullOrEmpty(winSceneName))
            {
                // Assurer le temps normal au cas où il aurait été mis en pause
                if (Time.timeScale == 0f) Time.timeScale = 1f;
                SceneManager.LoadScene(winSceneName);
            }
            else
            {
                // Fallback ancien comportement
                if (WinManager.Instance != null)
                {
                    WinManager.Instance.TriggerWin(player);
                }
                else
                {
                    Debug.Log($"{nameof(TeleporteController)}: dernière scène atteinte, WinManager absent et aucune winSceneName fournie.", this);
                }
            }
            return;
        }

        // Charger la scène suivante par index (et mémoriser un SpawnPoint si fourni)
        if (!string.IsNullOrEmpty(sequenceSpawnId))
            SceneSpawnManager.SetNext(sequenceSpawnId);

        SceneManager.LoadScene(nextIndex);
    }
}
