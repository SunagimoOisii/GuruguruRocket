using UnityEngine;

[CreateAssetMenu(fileName = "NewBlackholeParameter", menuName = "ParameterDataBase/Enemy_BlackholeParameter")]
public class Enemy_BlackholeParameter : ScriptableObject
{
    [Header("��ʂ��炢���痣�ꂽ���ʊO����ɂ��邩)")]
    [SerializeField] private float offScreenMargin_x = 1f;
    [SerializeField] private float offScreenMargin_y = 1f;
    public float OffScreenMarginX => offScreenMargin_x;
    public float OffScreenMarginY => offScreenMargin_y;

    [Header("�X�|�[������̃X�P�[���A�b�v�̐ݒ�")]
    [SerializeField] private float scaleUp_init     = 0.25f;
    [SerializeField] private float scaleUp_max      = 5f;
    [SerializeField] private float scaleUp_duration = 1.5f;
    public float ScaleUpInit => scaleUp_init;
    public float ScaleUpMax => scaleUp_max;
    public float ScaleUpDuration => scaleUp_duration;

    [Header("�ړ��̐ݒ�")]
    [SerializeField] private float move_speed = 3f;

    public float MoveSpeed => move_speed;
    public bool  CanMove { get; set; }
}