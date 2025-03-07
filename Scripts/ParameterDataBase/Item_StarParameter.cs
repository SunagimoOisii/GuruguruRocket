using UnityEngine;

[CreateAssetMenu(fileName = "NewStarParameter", menuName = "ParameterDataBase/Item_StarParameter")]
public class Item_StarParameter : ScriptableObject
{
    [Header("�擾���̃X�R�A�{�[�i�X")]
    [SerializeField] private int bonus_small = 50;
    [SerializeField] private int bonus_big   = 350;
    public int BonusSmall => bonus_small;
    public int BonusBig => bonus_big;

    [Header("Star�擾�A�j���[�V�����̐ݒ�")]
    [SerializeField] private float collect_moveUpDistance = 1.0f;
    [SerializeField] private float collect_rotationSpeed  = 800f;
    [SerializeField] private float collect_animDuration   = 0.75f;
    public float CollectMoveUpDistance => collect_moveUpDistance;
    public float CollectRotationSpeed => collect_rotationSpeed;
    public float CollectAnimDuration => collect_animDuration;
}