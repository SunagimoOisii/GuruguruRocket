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

    //1�b���ɃX�R�A�����Z���Ă���
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
    /// ��ʒ����Ƀ��P�b�g���o��������<para></para>
    /// �C���Q�[���ɓ������ŏ��̂ݎg�p����
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

        //���ꉉ�o�p�[�e�B�N���̐���
        var deadP = Instantiate(param.DeadParticlePrefab, transform.position, Quaternion.identity);
        deadP.transform.SetParent(spawnListT);

        //�w�莞�ԃX���[�ɂ���
        Time.timeScale = param.DeadSlowTimeScale;
        await UniTask.Delay((int)param.DeadSlowTimeDuration * 1000, true);
        Time.timeScale = 1.0f;

        //�Q�[���X�e�[�g�������L���O�ɑJ��
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
        //�ړ��̗͂�������
        Vector2 direction = Quaternion.Euler(0, 0, -45) * transform.up;
        rocketRB.AddForce(direction * param.MoveForce, ForceMode2D.Impulse);

        camEffects.BeginShake(param.CameraShakeDuration, param.CameraShakeStrength, param.CameraShakeVibrato).Forget();
        ScoreSystem.instance.AddScore(param.BonusMove, ScoreSystem.TextAnimScale.Small);
        _ = soundSystem.SE.Play(soundParam.AddressSERocketMove);
    }

    public void EnableBarrier(GameObject barrierPrefab)
    {
        //�o���A�q�I�u�W�F�N�g�𐶐��A�o���A�L����
        param.HasBarrier = true;
        var addBarrier = Instantiate(barrierPrefab);
        addBarrier.transform.SetParent(transform);
        addBarrier.transform.localPosition = Vector3.zero;

        //�w�莞�Ԍ�Ƀo���A�I�u�W�F�N�g�j���ƃo���A��Ԗ��������s��
        HandleBarrierLifecycle(addBarrier, param.BarrierDuration).Forget();

        OnBarrierEquipped?.Invoke();

        //�o���A��������pBGM�ɕύX
        _ = soundSystem.BGM.Play(soundParam.AddressBGMIngameBarrier, 0.25f);
    }

    private async UniTaskVoid HandleBarrierLifecycle(GameObject barrierObj, float delay)
    {
        //�_�ŊJ�n���Ԃ�ݒ�
        float blinkStartTime = delay - param.BarrierBlinkTime;
        var   barrierSprRend = barrierObj.GetComponent<SpriteRenderer>();
        if (blinkStartTime > 0)
        {
            await UniTask.Delay((int)(blinkStartTime * 1000));
            BlinkBarrier(barrierSprRend, param.BarrierBlinkTime).Forget();
        }

        //�_�ŏI���܂őҋ@���Ă���o���A��j��
        await UniTask.Delay((int)param.BarrierBlinkTime * 1000);
        Destroy(barrierObj);
        param.HasBarrier = false;
        OnBarrierStripped?.Invoke();

        //�ʏ�BGM�ɖ߂�
        _ = soundSystem.BGM.Play(soundParam.AddressBGMIngameStandard, 0.25f);
    }

    private async UniTaskVoid BlinkBarrier(SpriteRenderer barrierSprRend, float duration)
    {
        //�o���A�_�ŏ���
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
        //��ʊO�Ɉʒu����ꍇ�A��������̉�ʒ[�ɍ��W��ݒ肷��
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

        //��ʒ[�ւ̈ړ��ɂ�蔭������O�Ղ��폜
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