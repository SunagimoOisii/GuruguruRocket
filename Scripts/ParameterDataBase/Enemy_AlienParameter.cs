using UnityEngine;

[CreateAssetMenu(fileName = "NewAlienParameter", menuName = "ParameterDataBase/Enemy_AlienParameter")]
public class Enemy_AlienParameter : ScriptableObject
{
    [Header("画面からいくら離れたら画面外判定にするか)")]
    [SerializeField] private float offScreenMargin_x = 1f;
    [SerializeField] private float offScreenMargin_y = 1f;
    public float OffScreenMarginX => offScreenMargin_x;
    public float OffScreenMarginY => offScreenMargin_y;

    [Header("スポーン時のフェードインの設定")]
    [SerializeField] private float fade_Duration = 1f;
    public float FadeDuration => fade_Duration;

    [Header("移動の設定")]
    [SerializeField] private float move_speed         = 5f;
    [SerializeField] private float move_waveAmplitude = 2f;
    [SerializeField] private float move_waveFrequency = 2f;
    public float MoveSpeed => move_speed;
    public float MoveWaveAmplitude => move_waveAmplitude;
    public float MoveWaveFrequency => move_waveFrequency;
}