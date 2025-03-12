using TMPro;
using UnityEngine;

/// <summary>
/// インゲーム終了後のスコアランキング計算を行う
/// </summary>
public class RankingSystem : MonoBehaviour
{
    public static RankingSystem instance;

    [SerializeField] private TextMeshProUGUI[] best3ScoreTexts;

    [Header("スコアテキストの色設定")]
    [SerializeField] private Color32 initColor    = Color.white;
    [SerializeField] private Color32 updatedColor = new(255, 241, 0, 255);
    private readonly string best3ScoresKeyBase = "best";

    private void Start()
    {
        if (instance == null)
        {
            instance = this;
            InitRankings();
        }
        else
        {
            Debug.LogError("RankingSystemがシーン上に複数存在している");
            Destroy(instance.gameObject);
        }        
    }

    public void InitRankings()
    {
        //ランキングの各スコアテキストの初期化(色と値)
        int[] best3Scores = new int[3];
        for (int i = 0; i < 3; i++)
        {
            string key = best3ScoresKeyBase + "_" + i;
            best3Scores[i] = SerializedPlayerPrefs.GetObject<int>(key);
            best3ScoreTexts[i].text  = best3Scores[i].ToString("D7");
            best3ScoreTexts[i].color = initColor;
        }
    }

    /// <summary>
    /// プレイスコアの値を元にランキングを更新する
    /// </summary>
    public void AdjustRankings()
    {
        //スコアベスト3を読み込む
        int[] best3Scores = new int[3];
        for (int i = 0; i < 3; i++)
        {
            string key = best3ScoresKeyBase + "_" + i;
            best3Scores[i] = SerializedPlayerPrefs.GetObject<int>(key);
        }

        //各ベストスコアとプレイスコアを比較し
        //必要に応じてデータとテキスト共にランキング更新
        int nowScore = ScoreSystem.instance.Score;
        for (int i = 0; i < best3Scores.Length; i++)
        {
            if (nowScore > best3Scores[i])
            {
                string key = best3ScoresKeyBase + "_" + i;
                SerializedPlayerPrefs.SetObject(key, nowScore);
                best3ScoreTexts[i].text  = nowScore.ToString("D7");
                best3ScoreTexts[i].color = updatedColor;
                return;
            }
        }
    }

    public void ResetRankings()
    {
        //ベスト3スコアを全て0にする
        for (int i = 0; i < best3ScoreTexts.Length; i++)
        {
            int resetNum = 0;
            string key = best3ScoresKeyBase + "_" + i;
            SerializedPlayerPrefs.SetObject(key, resetNum);
            best3ScoreTexts[i].text = resetNum.ToString("D7");
        }
    }
}