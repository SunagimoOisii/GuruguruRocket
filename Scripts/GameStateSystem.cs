using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class GameStateSystem : MonoBehaviour
{
    public static GameStateSystem instance;

    [SerializeField] private SoundSystem   soundSystem;
    [SerializeField] private SoundParameter soundDB;

    [Header("�eBGM�̃N���X�t�F�[�h���������鎞��")]
    [SerializeField] private float fadeTimeToTitleFromRanking  = 0.5f;
    [SerializeField] private float fadeTimeToIngameFromTitle   = 0.5f;
    [SerializeField] private float fadeTimeToRankingFromIngame = 0.5f;

    [SerializeField] private Image fadeImage;
    [SerializeField] private Background background;

    [SerializeField] private Transform spawnListT;

    [Header("�Ö��̐ݒ�")]
    [SerializeField] private float fadeInAlpha;
    [SerializeField] private float fadeDuration;

    [Header("�e�X�e�[�g�ő��삷��UI��RectTransform")]
    [SerializeField] private RectTransform rectT_title;
    [SerializeField] private RectTransform rectT_ingame;
    [SerializeField] private RectTransform rectT_ranking;
    //�e�X�e�[�gUI���ގ��̊�_
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
            Debug.LogWarning("���V�[����GameStateSystem���������݂���");
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
                Debug.LogError("�s���ȃX�e�[�g���w�肳��Ă���");
                break;
        }
    }

    private void EnterTitleState()
    {
        RankingSystem.instance.InitRankings();

        //Ranking�X�e�[�g��UI����ʉE�ɗ���
        rectT_ranking.DOLocalMoveX(exitEndPos_x, 1)
            .OnComplete(() =>
            {
                rectT_ranking.localPosition = new(enterBeginPos_x, 0, 0);
            });

        //Title�X�e�[�g��UI����ʍ���������
        rectT_title.DOLocalMoveX(0, 1);

        soundSystem.BGM.CrossFade(soundDB.AddressBGMTitle, fadeTimeToTitleFromRanking).Forget();
    }

    private void EnterIngameState()
    {
        //Title�X�e�[�g��UI����ʉE����ޏo������
        rectT_title.DOLocalMoveX(exitEndPos_x, 1)
            .OnComplete(() =>
            {
                rectT_title.localPosition = new(enterBeginPos_x, 0, 0);
            });

        //�Ö�������
        fadeImage.raycastTarget = false;
        fadeImage.DOFade(0, fadeDuration);

        //Ingame�X�e�[�g��UI����ʍ���������
        rectT_ingame.DOLocalMoveX(0, 1);

        //�X�R�A,�z�u���ꂽ�G��A�C�e�������Z�b�g
        ScoreSystem.instance.InitScore();
        for (int i = 0; i < spawnListT.childCount; i++)
        {
            var obj = spawnListT.GetChild(i).gameObject;
            Destroy(obj);
        }

        //���@��z�u���A�G,�A�C�e���X�|�[���J�n
        rocket.Spawn();
        SpawnSystem.instance.StartSpawning();

        soundSystem.BGM.CrossFade(soundDB.AddressBGMIngameStandard, fadeTimeToIngameFromTitle, 0.25f).Forget();
    }

    private void EnterRankingState()
    {
        //�G,�A�C�e���X�|�[����~
        SpawnSystem.instance.StopSpawning();

        RankingSystem.instance.AdjustRankings();
        background.AnimateColor_Gameover();

        //Ingame�X�e�[�g��UI����ʉE����ޏo������
        rectT_ingame.DOLocalMoveX(exitEndPos_x, 1)
            .OnComplete(() =>
            {
                rectT_ingame.localPosition = new(enterBeginPos_x, 0, 0);
            });

        //�Ö��o����ARanking�X�e�[�g��UI��\��
        fadeImage.raycastTarget = true;
        fadeImage.DOFade(fadeInAlpha, fadeDuration);
        rectT_ranking.DOLocalMoveX(0, 1);

        soundSystem.BGM.CrossFade(soundDB.AddressBGMRanking, fadeTimeToRankingFromIngame).Forget();
    }
}