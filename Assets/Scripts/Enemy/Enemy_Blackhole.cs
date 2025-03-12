using UnityEngine;
using Cysharp.Threading.Tasks;

/// <summary>
/// �X�|�[������Ɏ��g�̃X�P�[���A�b�v���s������A�㉺���E���Âꂩ�̒����ړ����s��<para></para>
/// �X�P�[���A�b�v���I������܂œ����蔻��͂Ȃ�
/// </summary>
public class Enemy_Blackhole : MonoBehaviour
{
    [SerializeField] private Enemy_BlackholeParameter param;

    private Collider2D col;
    private Vector2 moveDirection;
    private Vector2 screenBounds;   

    private void Awake()
    {
        transform.localScale = Vector3.one * param.ScaleUpInit;

        screenBounds = Camera.main.ScreenToWorldPoint(new(Screen.width, Screen.height, 0));

        //�ړ��t���O,�����蔻��̖�����
        param.CanMove = false;
        col = GetComponent<Collider2D>();
        col.enabled = false;

        //�X�|�[�����W�����ɐi�s����������
        Vector2 spawnPos = transform.position;
        if (Mathf.Abs(spawnPos.x) > Mathf.Abs(spawnPos.y))
        {
            moveDirection = (spawnPos.x > 0) ? Vector2.left : Vector2.right;
        }
        else
        {
            moveDirection = (spawnPos.y > 0) ? Vector2.down : Vector2.up;
        }
    }

    private void Start()
    {
        ScaleUp().Forget();
    }

    private async UniTask ScaleUp()
    {
        float elapsedTime = 0f;
        while (gameObject != null &&
               elapsedTime < param.ScaleUpDuration)
        {
            elapsedTime += Time.deltaTime;

            //�X�P�[����i�X�傫������
            float scale = Mathf.Lerp(param.ScaleUpInit, param.ScaleUpMax, elapsedTime / param.ScaleUpDuration);
            transform.localScale = Vector3.one * scale;

            await UniTask.Yield(PlayerLoopTiming.Update);
        }
        transform.localScale = Vector3.one * param.ScaleUpMax;

        //�X�P�[���A�b�v�I����Ɉړ��\,�����蔻��L���Ƃ���
        param.CanMove = true;
        col.enabled   = true;
    }

    private void FixedUpdate()
    {
        if (!param.CanMove) return;

        transform.Translate(param.MoveSpeed * Time.deltaTime * moveDirection);

        //��ʊO + �]�T�����𒴂����ꍇ�́@�폜
        float bound_x = screenBounds.x;
        float bound_y = screenBounds.y;
        float margin_x = param.OffScreenMarginX;
        float margin_y = param.OffScreenMarginY;
        Vector2 nowPos = transform.position;
        if (nowPos.x < -bound_x - margin_x || 
            nowPos.x > bound_x + margin_x  ||
            nowPos.y < -bound_y - margin_y || 
            nowPos.y > bound_y + margin_y)
        {
            Destroy(GetComponent<TrailRenderer>());
            Destroy(gameObject);
        }
    }
}