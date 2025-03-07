using UnityEngine;
using Cysharp.Threading.Tasks;

public class Item_Star : MonoBehaviour, ICollide
{
    [SerializeField] private Item_StarParameter param;

    [SerializeField] private SoundParameter soundParam;
    private SoundSystem soundSystem;
    private string soundSystemTag = "SoundSystem";

    [SerializeField] private bool isBigStar = false;
    [SerializeField] private SpriteRenderer sprRenderer;

    private void Start()
    {
        soundSystem = GameObject.FindWithTag(soundSystemTag).GetComponent<SoundSystem>();
    }

    public void OnCollide(params object[] args)
    {
        foreach (var arg in args)
        {
            //Rocket(���@)�ƏՓ˂����ꍇ�AStar�擾�A�j���[�V�����ƃX�R�A���Z�����s
            if (arg is Rocket)
            {
                if (isBigStar)
                {
                    ScoreSystem.instance.AddScore(param.BonusSmall, ScoreSystem.TextAnimScale.Medium);
                }
                else
                {
                    ScoreSystem.instance.AddScore(param.BonusBig, ScoreSystem.TextAnimScale.Large);
                }
                GetComponent<Collider2D>().enabled = false;
                StartCollectedAnim().Forget();
                _ = soundSystem.SE.Play(soundParam.AddressSEStarGet);
            }
        }
    }

    private async UniTaskVoid StartCollectedAnim()
    {
        Color   initColor = sprRenderer.color;
        Vector3 initPos   = transform.position;
        Vector3 initScale = transform.localScale;
        Vector3 targetPos = initPos + new Vector3(0, param.CollectMoveUpDistance, 0);

        //�A�j���[�V�������s
        float elapsedTime = 0f;
        while (elapsedTime < param.CollectAnimDuration)
        {
            //�I�u�W�F�N�g���j������Ă�����I��
            if (this == null) return;

            elapsedTime += Time.deltaTime;
            float t = elapsedTime / param.CollectAnimDuration;

            //Y���W�㏸,Y����]
            transform.position = Vector3.Lerp(initPos, targetPos, t);
            transform.Rotate(Vector3.up, param.CollectRotationSpeed * Time.deltaTime);

            //�X�P�[����������
            transform.localScale = Vector3.Lerp(initScale, Vector3.zero, t);

            //�X�v���C�g�̓����x��������
            float alpha = Mathf.Lerp(1, 0, t);
            sprRenderer.color = new(initColor.r, initColor.g, initColor.b, alpha);

            await UniTask.Yield();
        }

        //�Ō�ɃI�u�W�F�N�g��j��
        Destroy(gameObject);
    }
}