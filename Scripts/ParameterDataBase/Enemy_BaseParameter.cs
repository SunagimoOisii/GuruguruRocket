using DG.Tweening;
using UnityEngine;

/// <summary>
/// 敵の共通処理(Collision,ColorChanger)のパラメータを提供する
/// </summary>
[CreateAssetMenu(fileName = "NewEnemyBaseParameter", menuName = "ParameterDataBase/Enemy_BaseParameter")]
public class Enemy_BaseParameter : ScriptableObject
{
    [Header("以下Enemy_ColorChangerの設定")]
    [Header("通常時のカラー設定")]
    [SerializeField] private Color32  initColor = new(255, 75, 0, 255);
    [SerializeField] private Material initMaterial = null;
    public Color32  InitColor => initColor;
    public Material InitMaterial => initMaterial;

    [Header("ロケットがバリア装備状態時のカラー設定")]
    [SerializeField] private Color32  weakColor    = new(77, 196, 255, 255);
    [SerializeField] private Material weakMaterial = null;
    public Color32  WeakColor => weakColor;
    public Material WeakMaterial => weakMaterial;



    [Header("以下Enemy_Collisionの設定")]
    [Header("ふっとび度合いの設定")]
    [SerializeField] private float blowForce_min = 10f;
    [SerializeField] private float blowForce_max = 25f;
    [SerializeField] private float torque_min = -25f;
    [SerializeField] private float torque_max = 25f;
    public float BlowForceMin => blowForce_min;
    public float BlowForceMax => blowForce_max;
    public float TorqueMin => torque_min;
    public float TorqueMax => torque_max;

    [Header("拡縮アニメーションの設定")]
    [SerializeField] private Ease  punch_easing    = Ease.OutSine;
    [SerializeField] private float punch_magnitude = 1.01f;
    [SerializeField] private float punch_duration  = 0.75f;
    [SerializeField] private int   punch_vibrato   = 15;
    public Ease  PunchEasing => punch_easing;
    public float PunchMagnitude => punch_magnitude;
    public float PunchDuration => punch_duration;
    public int   PunchVibrato => punch_vibrato;

    [Header("撃破スコアボーナス")]
    [SerializeField] private int bonus_defeat;
    public int BonusDefeat => bonus_defeat;

    [Header("やられ時のカメラズーム,スローの設定")]
    [SerializeField] private float de_zoomIn           = 4.25f;
    [SerializeField] private float de_zoomDuration     = 0.2f;
    [SerializeField] private Ease  de_zoomEase         = Ease.OutCubic;
    [SerializeField] private float de_slowTimeScale    = 0.2f;
    [SerializeField] private int   de_slowTimeDuration = 750;
    public float DefeatZoomIn => de_zoomIn;
    public float DefeatZoomDuration => de_zoomDuration;
    public Ease  DefeatZoomEase => de_zoomEase;
    public float DefeatSlowTimeScale => de_slowTimeScale;
    public int   DefeatSlowTimeDuration => de_slowTimeDuration;
}