using System;
using System.Runtime.InteropServices;
using UnityEngine;
using System.Collections;

public class EnemyHealth : MonoBehaviour
{
    private bool isDead = false;
    private Animator animator;

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
    }

    public void Die()
    {
        if (isDead) return;
        isDead = true;

        if (animator != null)
            animator.SetLayerWeight(1, 0f);
            animator.SetTrigger("Die");

        Debug.Log("Враг уничтожен");

        var ai = GetComponent<EnemyAI>();
        if (ai != null) ai.enabled = false;

        StartCoroutine(ShowGameOverDelayed());

        //Destroy(gameObject, 2f); // Удалить после анимации
    }

    private IEnumerator ShowGameOverDelayed()
    {
        yield return new WaitForSeconds(2f);
        GameUIManager ui = UnityEngine.Object.FindFirstObjectByType<GameUIManager>();
        ui?.ShowGameOver();
    }
}
