using DG.Tweening;
using UnityEngine;

public class Item_BarrierEquippingEffect : MonoBehaviour
{
    private Vector3 targetScale;

    [Header("装備時のエフェクト設定")]
    [SerializeField] private float duration;
    [SerializeField] private Ease  ease;

    private void Start()
    {
        targetScale = transform.localScale;

        transform.localScale = Vector3.zero;
        transform.DOScale(targetScale, duration).SetEase(ease);
    }
}