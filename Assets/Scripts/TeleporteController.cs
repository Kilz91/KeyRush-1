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

    private void Reset()
    {
        // S'assurer que le collider est configuré en "Is Trigger" pour une zone de téléportation 2D
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
        // wait one frame so other trigger handlers can run
        yield return null;

        // if WinManager exists and a win is active (or already reached), skip teleport
        if (WinManager.Instance != null && WinManager.Instance.HasWon())
        {
            Debug.Log($"{nameof(TeleporteController)}: teleport skipped because Win active", this);
            yield break;
        }

        // progression séquentielle uniquement
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

        int nextIndex = idx + 1;
        int total = SceneManager.sceneCountInBuildSettings;

        if (nextIndex >= total)
        {
            // Dernière scène atteinte -> déléguer la victoire au WinManager
            if (WinManager.Instance != null)
            {
                WinManager.Instance.TriggerWin(player);
            }
            else
            {
                Debug.Log($"{nameof(TeleporteController)}: dernière scène atteinte, WinManager absent.", this);
            }
            return;
        }

        // Charger la scène suivante par index
        if (!string.IsNullOrEmpty(sequenceSpawnId))
            SceneSpawnManager.SetNext(sequenceSpawnId);

        SceneManager.LoadScene(nextIndex);
    }
}
