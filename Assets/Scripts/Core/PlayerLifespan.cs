using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerLifespan : MonoBehaviour
{
    public float initialCooldown = 10f;
    public float cooldownStep = 0.7f;
    public float minCooldown = 3f;
    public int linesPerLevel = 10;

    [Header("UI")]
    public Image cooldownUI;

    public Color initialColor = Color.white;
    public Color emptyColor = Color.red;

    private Coroutine lifespanCoroutine;

    public void StartPieceLifespan(PlayerPiece player, Grid grid)
    {
        if (lifespanCoroutine != null)
        {
            StopCoroutine(lifespanCoroutine);
        }
        lifespanCoroutine = StartCoroutine(PieceLifespanCoroutine(player, grid));
    }

    private IEnumerator PieceLifespanCoroutine(PlayerPiece player, Grid grid)
    {
        float currentCooldown = Mathf.Max(initialCooldown - grid.totalLines / linesPerLevel * cooldownStep, minCooldown);
        cooldownUI.DOKill();
        cooldownUI.fillAmount = 1;
        cooldownUI.color = initialColor;
        cooldownUI.DOColor(emptyColor, currentCooldown).SetEase(Ease.Linear);
        cooldownUI.DOFillAmount(0f, currentCooldown).SetEase(Ease.Linear);
        yield return new WaitForSeconds(currentCooldown);
        player.OnPlace();
    }
}