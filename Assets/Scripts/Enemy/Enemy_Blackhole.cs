using UnityEngine;
using Cysharp.Threading.Tasks;

/// <summary>
/// スポーン直後に自身のスケールアップを行った後、上下左右いづれかの直線移動を行う<para></para>
/// スケールアップが終了するまで当たり判定はない
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

        //移動フラグ,当たり判定の無効化
        param.CanMove = false;
        col = GetComponent<Collider2D>();
        col.enabled = false;

        //スポーン座標を元に進行方向を決定
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

            //スケールを段々大きくする
            float scale = Mathf.Lerp(param.ScaleUpInit, param.ScaleUpMax, elapsedTime / param.ScaleUpDuration);
            transform.localScale = Vector3.one * scale;

            await UniTask.Yield(PlayerLoopTiming.Update);
        }
        transform.localScale = Vector3.one * param.ScaleUpMax;

        //スケールアップ終了後に移動可能,当たり判定有効とする
        param.CanMove = true;
        col.enabled   = true;
    }

    private void FixedUpdate()
    {
        if (!param.CanMove) return;

        transform.Translate(param.MoveSpeed * Time.deltaTime * moveDirection);

        //画面外 + 余裕距離を超えた場合は　削除
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