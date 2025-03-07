using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class GameStateSystem : MonoBehaviour
{
    public static GameStateSystem instance;

    [SerializeField] private SoundSystem   soundSystem;
    [SerializeField] private SoundParameter soundDB;

    [Header("各BGMのクロスフェードが完了する時間")]
    [SerializeField] private float fadeTimeToTitleFromRanking  = 0.5f;
    [SerializeField] private float fadeTimeToIngameFromTitle   = 0.5f;
    [SerializeField] private float fadeTimeToRankingFromIngame = 0.5f;

    [SerializeField] private Image fadeImage;
    [SerializeField] private Background background;

    [SerializeField] private Transform spawnListT;

    [Header("暗幕の設定")]
    [SerializeField] private float fadeInAlpha;
    [SerializeField] private float fadeDuration;

    [Header("各ステートで操作するUIのRectTransform")]
    [SerializeField] private RectTransform rectT_title;
    [SerializeField] private RectTransform rectT_ingame;
    [SerializeField] private RectTransform rectT_ranking;
    //各ステートUI入退室の基準点
    private readonly int enterBeginPos_x = -1920;
    private readonly int exitEndPos_x    = 1920;

    [SerializeField] private Rocket rocket;

    public enum State
    {
        Title,
        Ingame,
        Ranking
    }

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.LogWarning("同シーンにGameStateSystemが複数存在する");
            Destroy(gameObject);
        }

        soundSystem.BGM.Play(soundDB.AddressBGMTitle).Forget();
    }

    public void ChangeState(State s)
    {
        switch(s)
        {
            case State.Title:
                EnterTitleState();
                break;

            case State.Ingame:
                EnterIngameState();
                break;

            case State.Ranking:
                EnterRankingState();
                break;

            default:
                Debug.LogError("不明なステートが指定されている");
                break;
        }
    }

    private void EnterTitleState()
    {
        RankingSystem.instance.InitRankings();

        //RankingステートのUIを画面右に流す
        rectT_ranking.DOLocalMoveX(exitEndPos_x, 1)
            .OnComplete(() =>
            {
                rectT_ranking.localPosition = new(enterBeginPos_x, 0, 0);
            });

        //TitleステートのUIを画面左から入れる
        rectT_title.DOLocalMoveX(0, 1);

        soundSystem.BGM.CrossFade(soundDB.AddressBGMTitle, fadeTimeToTitleFromRanking).Forget();
    }

    private void EnterIngameState()
    {
        //TitleステートのUIを画面右から退出させる
        rectT_title.DOLocalMoveX(exitEndPos_x, 1)
            .OnComplete(() =>
            {
                rectT_title.localPosition = new(enterBeginPos_x, 0, 0);
            });

        //暗幕を消す
        fadeImage.raycastTarget = false;
        fadeImage.DOFade(0, fadeDuration);

        //IngameステートのUIを画面左から入れる
        rectT_ingame.DOLocalMoveX(0, 1);

        //スコア,配置された敵やアイテムをリセット
        ScoreSystem.instance.InitScore();
        for (int i = 0; i < spawnListT.childCount; i++)
        {
            var obj = spawnListT.GetChild(i).gameObject;
            Destroy(obj);
        }

        //自機を配置し、敵,アイテムスポーン開始
        rocket.Spawn();
        SpawnSystem.instance.StartSpawning();

        soundSystem.BGM.CrossFade(soundDB.AddressBGMIngameStandard, fadeTimeToIngameFromTitle, 0.25f).Forget();
    }

    private void EnterRankingState()
    {
        //敵,アイテムスポーン停止
        SpawnSystem.instance.StopSpawning();

        RankingSystem.instance.AdjustRankings();
        background.AnimateColor_Gameover();

        //IngameステートのUIを画面右から退出させる
        rectT_ingame.DOLocalMoveX(exitEndPos_x, 1)
            .OnComplete(() =>
            {
                rectT_ingame.localPosition = new(enterBeginPos_x, 0, 0);
            });

        //暗幕出現後、RankingステートのUIを表示
        fadeImage.raycastTarget = true;
        fadeImage.DOFade(fadeInAlpha, fadeDuration);
        rectT_ranking.DOLocalMoveX(0, 1);

        soundSystem.BGM.CrossFade(soundDB.AddressBGMRanking, fadeTimeToRankingFromIngame).Forget();
    }
}