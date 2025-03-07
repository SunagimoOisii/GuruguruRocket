using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using UnityEngine;

public class Rocket : MonoBehaviour
{
    public RocketParameter param;
    public event Action OnBarrierEquipped;
    public event Action OnBarrierStripped;

    [SerializeField] private SoundSystem soundSystem;
    [SerializeField] private SoundParameter soundParam;
    [SerializeField] private Transform spawnListT;
    [SerializeField] private CameraEffects camEffects;
    [SerializeField] private Rigidbody2D rocketRB;
    private TrailRenderer trailRenderer;

    private void Start()
    {
        param.IsDead = false;
        param.HasBarrier = false;
        trailRenderer = GetComponent<TrailRenderer>();
    }

    //1秒毎にスコアを加算していく
    private async UniTask StartSurviveScoreBonus()
    {
        while (param.IsDead == false)
        {
            await UniTask.Delay(1000);
            if (param.IsDead == false)
            {
                ScoreSystem.instance.AddScore(param.BonusSurvive, ScoreSystem.TextAnimScale.Small);
            }
        }
    }

    /// <summary>
    /// 画面中央にロケットを出現させる<para></para>
    /// インゲームに入った最初のみ使用する
    /// </summary>
    public void Spawn()
    {
        param.IsDead = false;
        transform.position   = Vector3.zero;
        transform.localScale = Vector3.zero;
        trailRenderer.enabled = true;
        GetComponent<SpriteRenderer>().enabled = true;

        transform.DOScale(param.SpawnScale, param.SpawnDuration).SetEase(param.SpawnEase);
        StartSurviveScoreBonus().Forget();
    }

    public async UniTaskVoid BeDead()
    {
        param.IsDead = true;
        _ = soundSystem.SE.Play(soundParam.AddressSERocketDead);

        GetComponent<SpriteRenderer>().enabled = false;
        trailRenderer.enabled = false;

        //やられ演出パーティクルの生成
        var deadP = Instantiate(param.DeadParticlePrefab, transform.position, Quaternion.identity);
        deadP.transform.SetParent(spawnListT);

        //指定時間スローにする
        Time.timeScale = param.DeadSlowTimeScale;
        await UniTask.Delay((int)param.DeadSlowTimeDuration * 1000, true);
        Time.timeScale = 1.0f;

        //ゲームステートをランキングに遷移
        GameStateSystem.instance.ChangeState(GameStateSystem.State.Ranking);
    }

    public void BeSmallScale()
    {
        transform.DOScale(param.RotationScale, param.RotationScalingDuration)
            .SetEase(param.RotationScalingSmallEase);
    }

    public void BeInitScale()
    {
        transform.DOScale(param.RotationInitialScale, param.RotationScalingDuration)
            .SetEase(param.RotationScalingInitEase);
    }

    public void Rotate()
    {
        transform.Rotate(0, 0, param.RotationSpeed * Time.deltaTime);
    }

    public void Move()
    {
        //移動の力を加える
        Vector2 direction = Quaternion.Euler(0, 0, -45) * transform.up;
        rocketRB.AddForce(direction * param.MoveForce, ForceMode2D.Impulse);

        camEffects.BeginShake(param.CameraShakeDuration, param.CameraShakeStrength, param.CameraShakeVibrato).Forget();
        ScoreSystem.instance.AddScore(param.BonusMove, ScoreSystem.TextAnimScale.Small);
        _ = soundSystem.SE.Play(soundParam.AddressSERocketMove);
    }

    public void EnableBarrier(GameObject barrierPrefab)
    {
        //バリア子オブジェクトを生成、バリア有効化
        param.HasBarrier = true;
        var addBarrier = Instantiate(barrierPrefab);
        addBarrier.transform.SetParent(transform);
        addBarrier.transform.localPosition = Vector3.zero;

        //指定時間後にバリアオブジェクト破棄とバリア状態無効化を行う
        HandleBarrierLifecycle(addBarrier, param.BarrierDuration).Forget();

        OnBarrierEquipped?.Invoke();

        //バリア装備時専用BGMに変更
        _ = soundSystem.BGM.Play(soundParam.AddressBGMIngameBarrier, 0.25f);
    }

    private async UniTaskVoid HandleBarrierLifecycle(GameObject barrierObj, float delay)
    {
        //点滅開始時間を設定
        float blinkStartTime = delay - param.BarrierBlinkTime;
        var   barrierSprRend = barrierObj.GetComponent<SpriteRenderer>();
        if (blinkStartTime > 0)
        {
            await UniTask.Delay((int)(blinkStartTime * 1000));
            BlinkBarrier(barrierSprRend, param.BarrierBlinkTime).Forget();
        }

        //点滅終了まで待機してからバリアを破棄
        await UniTask.Delay((int)param.BarrierBlinkTime * 1000);
        Destroy(barrierObj);
        param.HasBarrier = false;
        OnBarrierStripped?.Invoke();

        //通常BGMに戻す
        _ = soundSystem.BGM.Play(soundParam.AddressBGMIngameStandard, 0.25f);
    }

    private async UniTaskVoid BlinkBarrier(SpriteRenderer barrierSprRend, float duration)
    {
        //バリア点滅処理
        float elapsedTime = 0f;
        while (elapsedTime < duration &&
               barrierSprRend != null)
        {
            barrierSprRend.enabled = !barrierSprRend.enabled;
            await UniTask.Delay((int)(param.BarrierBlinkInterval * 1000));
            elapsedTime += param.BarrierBlinkInterval;
        }
    }

    private void FixedUpdate()
    {
        TeleportRocketFromOffScreen();
    }

    private void TeleportRocketFromOffScreen()
    {
        //画面外に位置する場合、もう一方の画面端に座標を設定する
        bool isTeleported = false;
        Vector2 teleportPos = transform.position;
        if (teleportPos.x <= param.MinPosition.x)
        {
            teleportPos.x = param.MaxPosition.x;
            isTeleported = true;
        }
        else if (teleportPos.x > param.MaxPosition.x)
        {
            teleportPos.x = param.MinPosition.x;
            isTeleported = true;
        }
        if (teleportPos.y <= param.MinPosition.y)
        {
            teleportPos.y = param.MaxPosition.y;
            isTeleported = true;
        }
        else if (teleportPos.y > param.MaxPosition.y)
        {
            teleportPos.y = param.MinPosition.y;
            isTeleported = true;
        }
        transform.position = teleportPos;

        //画面端への移動により発生する軌跡を削除
        if (isTeleported) trailRenderer.Clear();
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (param.IsDead) return;

        if (col.gameObject.TryGetComponent(out ICollide iCol))
        {
            iCol.OnCollide(this);
        }
    }
}