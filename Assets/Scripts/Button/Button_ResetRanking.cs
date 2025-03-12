using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Button_ResetRanking : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private SoundSystem soundSystem;
    [SerializeField] private SoundParameter param;

    [SerializeField] private Image frontIcon = null;
    [SerializeField] private float fillAmountBackDuration = 0.2f;
    private float holdTime = 0f;
    private bool isHolding = false;

    [Header("スコア消去が確定する押下時間")]
    [SerializeField] private float decideTimeToReset = 3f;

    public void OnPointerDown(PointerEventData ed)
    {
        holdTime  = 0f;
        isHolding = true;
    }

    public void OnPointerUp(PointerEventData ed)
    {
        isHolding = false;
        frontIcon.DOFillAmount(0, fillAmountBackDuration);
    }

    private void FixedUpdate()
    {
        if (isHolding)
        {
            holdTime += Time.fixedDeltaTime;
            frontIcon.fillAmount = Mathf.Clamp01(holdTime / decideTimeToReset);

            //アイコンが完全に表示されたらリセット実行
            if (frontIcon.fillAmount >= 1f)
            {
                ResetScore();
                _ = soundSystem.SE.Play(param.AddressSERocketDead);
            }
        }
    }

    private void ResetScore()
    {
        RankingSystem.instance.ResetRankings();

        //押下時間,アイコン表示状態の初期化
        holdTime  = 0f;
        isHolding = false;
        frontIcon.DOFillAmount(0, fillAmountBackDuration);
    }
}