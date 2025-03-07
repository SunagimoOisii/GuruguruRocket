using TMPro;
using UnityEngine;

/// <summary>
/// �C���Q�[���I����̃X�R�A�����L���O�v�Z���s��
/// </summary>
public class RankingSystem : MonoBehaviour
{
    public static RankingSystem instance;

    [SerializeField] private TextMeshProUGUI[] best3ScoreTexts;

    [Header("�X�R�A�e�L�X�g�̐F�ݒ�")]
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
            Debug.LogError("RankingSystem���V�[����ɕ������݂��Ă���");
            Destroy(instance.gameObject);
        }        
    }

    public void InitRankings()
    {
        //�����L���O�̊e�X�R�A�e�L�X�g�̏�����(�F�ƒl)
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
    /// �v���C�X�R�A�̒l�����Ƀ����L���O���X�V����
    /// </summary>
    public void AdjustRankings()
    {
        //�X�R�A�x�X�g3��ǂݍ���
        int[] best3Scores = new int[3];
        for (int i = 0; i < 3; i++)
        {
            string key = best3ScoresKeyBase + "_" + i;
            best3Scores[i] = SerializedPlayerPrefs.GetObject<int>(key);
        }

        //�e�x�X�g�X�R�A�ƃv���C�X�R�A���r��
        //�K�v�ɉ����ăf�[�^�ƃe�L�X�g���Ƀ����L���O�X�V
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
        //�x�X�g3�X�R�A��S��0�ɂ���
        for (int i = 0; i < best3ScoreTexts.Length; i++)
        {
            int resetNum = 0;
            string key = best3ScoresKeyBase + "_" + i;
            SerializedPlayerPrefs.SetObject(key, resetNum);
            best3ScoreTexts[i].text = resetNum.ToString("D7");
        }
    }
}