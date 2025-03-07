using UnityEngine;

[CreateAssetMenu(fileName = "NewSoundParam", menuName = "ParameterDataBase/SoundParameter")]
public class SoundParameter : ScriptableObject
{
    [Header("BGMのリソースアドレス")]
    [SerializeField] private string address_bgm_title            = "BGM_title";
    [SerializeField] private string address_bgm_ingame_standard  = "BGM_ingame_standard";
    [SerializeField] private string address_bgm_ingame_barrier   = "BGM_ingame_barrier";
    [SerializeField] private string address_bgm_ranking          = "BGM_ranking";
    public string AddressBGMTitle => address_bgm_title;
    public string AddressBGMIngameStandard => address_bgm_ingame_standard;
    public string AddressBGMIngameBarrier => address_bgm_ingame_barrier;
    public string AddressBGMRanking => address_bgm_ranking;

    [Header("SEのリソースアドレス")]
    [SerializeField] private string address_se_rocketDead     = "SE_rocketDead";
    [SerializeField] private string address_se_rocketRotate   = "SE_rocketRotate";
    [SerializeField] private string address_se_rocketMove     = "SE_rocketMove";
    [SerializeField] private string address_se_buttonSelect   = "SE_buttonSelect";
    [SerializeField] private string address_se_enemyDead      = "SE_enemyDead";
    [SerializeField] private string address_se_starGet        = "SE_starGet";
    [SerializeField] private string address_se_barrierGet     = "SE_barrierGet";
    public string AddressSERocketDead => address_se_rocketDead;
    public string AddressSERocketRotate => address_se_rocketRotate;
    public string AddressSERocketMove => address_se_rocketMove;
    public string AddressSEButtonSelected => address_se_buttonSelect;
    public string AddressSEEnemyDead => address_se_enemyDead;
    public string AddressSEStarGet => address_se_starGet;
    public string AddressSEBarrierGet => address_se_barrierGet;
}