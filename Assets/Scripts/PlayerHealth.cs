using System;
using UnityEngine;
using System.Collections;

public class PlayerHealth : MonoBehaviour
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

        var move = GetComponent<PlayerMovement>();
        if (move != null) move.enabled = false;

        var thrower = GetComponent<KnifeThrowController>();
        if (thrower != null) thrower.enabled = false;

        var controller = GetComponent<CharacterController>();
        if (controller != null) controller.enabled = false;

        StartCoroutine(ShowGameOverDelayed());

        Debug.Log("Игрок погиб");
    }

    private IEnumerator ShowGameOverDelayed()
    {
        yield return new WaitForSeconds(2f);
        GameUIManager ui = UnityEngine.Object.FindFirstObjectByType<GameUIManager>();
        ui?.ShowGameOver();
    }
}
