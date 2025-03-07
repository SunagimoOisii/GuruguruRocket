using DG.Tweening;
using UnityEngine;

public class Item_BarrierEquippingEffect : MonoBehaviour
{
    private Vector3 targetScale;

    [Header("�������̃G�t�F�N�g�ݒ�")]
    [SerializeField] private float duration;
    [SerializeField] private Ease  ease;

    private void Start()
    {
        targetScale = transform.localScale;

        transform.localScale = Vector3.zero;
        transform.DOScale(targetScale, duration).SetEase(ease);
    }
}