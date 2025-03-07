using UnityEngine;
using Cysharp.Threading.Tasks;

public class Enemy_Alien : MonoBehaviour
{
    [SerializeField] private Enemy_AlienParameter param;

    private SpriteRenderer sprRenderer;
    private Collider2D col;
    private Vector2 moveDirection;
    private Vector2 screenBounds;
    private bool canMove = false;

    private void Awake()
    {
        col = GetComponent<Collider2D>();
        sprRenderer = GetComponent<SpriteRenderer>();
        screenBounds = Camera.main.ScreenToWorldPoint(new(Screen.width, Screen.height, 0));

        //移動フラグ,当たり判定の無効化
        canMove     = false;
        col.enabled = false;

        //移動方向を決定
        Vector2 spawnPos = transform.position;
        if (Mathf.Abs(spawnPos.x) > Mathf.Abs(spawnPos.y))
        {
            moveDirection = spawnPos.x > 0 ? Vector2.left : Vector2.right;
        }
        else
        {
            moveDirection = spawnPos.y > 0 ? Vector2.down : Vector2.up;
        }

        //スポーン直後のフェードイン処理のために
        //予めアルファ値を下げる
        var c = sprRenderer.color;
        c.a = 0f;
        sprRenderer.color = c;
        FadeIn().Forget();
    }

    private async UniTask FadeIn()
    {
        float elapsedTime = 0f;
        while (gameObject != null &&
               elapsedTime < param.FadeDuration)
        {
            elapsedTime += Time.deltaTime;

            //段々スプライトのアルファ値を上げる
            var color = sprRenderer.color;
            float a = Mathf.Lerp(0f, 1f, elapsedTime / param.FadeDuration);
            color.a = a;
            sprRenderer.color = color;

            await UniTask.Yield(PlayerLoopTiming.Update);
        }
        var c = sprRenderer.color;
        c.a = 1f;
        sprRenderer.color = c;

        //完全に表示されたら移動と当たり判定を有効化
        canMove     = true;
        col.enabled = true;
    }

    private void FixedUpdate()
    {
        if (!canMove) return;

        //弧を描くように移動させる
        float waveOffset = Mathf.Sin(Time.time * param.MoveWaveFrequency) * param.MoveWaveAmplitude;
        Vector2 waveMovement = new(waveOffset, waveOffset);
        transform.Translate((moveDirection * param.MoveSpeed + waveMovement) * Time.deltaTime);

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