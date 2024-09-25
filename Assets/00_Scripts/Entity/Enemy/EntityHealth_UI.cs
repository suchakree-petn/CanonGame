using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class EntityHealth_UI : MonoBehaviour
{
    [FoldoutGroup("Config", true), SerializeField] float delayAnimateDecrease = 2;
    [FoldoutGroup("Config", true), SerializeField] float durationAnimateDecrease = 1;

    [FoldoutGroup("Reference", true), Required]
    [SerializeField] private Image hpFill;

    [FoldoutGroup("Reference", true), Required]
    [SerializeField] private Image hpFillDecrease;

    Sequence hpDecreaseSequence;
    bool isHpDecreaseSequencePlaying = false;

    public void SetHpBar(float hpPercentage)
    {
        hpFill.fillAmount = hpPercentage;
        AnimateHpDecrease();
    }

    private void AnimateHpDecrease()
    {
        if (isHpDecreaseSequencePlaying) return;
        
        isHpDecreaseSequencePlaying = true;
        float to = hpFill.fillAmount;

        hpDecreaseSequence = DOTween.Sequence();

        hpDecreaseSequence.AppendInterval(delayAnimateDecrease)
            .OnUpdate(() =>
            {
                if (to != hpFill.fillAmount)
                {
                    hpDecreaseSequence.Kill();
                    isHpDecreaseSequencePlaying = false;

                    AnimateHpDecrease();
                }

            })
            .SetUpdate(true);

        hpDecreaseSequence.Append(hpFillDecrease.DOFillAmount(to, durationAnimateDecrease))
        .OnComplete(() =>
        {
            isHpDecreaseSequencePlaying = false;
        });

        hpDecreaseSequence.Play();
    }
}
