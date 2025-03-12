using UnityEngine;

public class OverlapDetector_2D
{
    public bool DetectSpriteUIOverlap(SpriteRenderer sr, RectTransform UIRectT, Canvas c)
    {
        Rect sprRect = ConvertToScreenRect(sr);
        Rect uiRect  = ConvertToScreenRect(UIRectT, c);

        return sprRect.Overlaps(uiRect);
    }

    private Rect ConvertToScreenRect(SpriteRenderer sr)
    {
        //Sprite�̃��[���h���W��̍���,�E��[�̓_�����߂�
        var bounds = sr.bounds;
        Vector3 bottomLeft = Camera.main.WorldToScreenPoint(bounds.min);
        Vector3 topRight   = Camera.main.WorldToScreenPoint(bounds.max);

        //�X�N���[�����W���Rect��Ԃ�
        float width  = topRight.x - bottomLeft.x;
        float height = topRight.y - bottomLeft.y;
        return new(bottomLeft.x, bottomLeft.y, width, height);
    }

    private Rect ConvertToScreenRect(RectTransform rt)
    {
        //UI�̃��[���h���W��̍���,�E��[�̓_�����߂�
        Vector3[] corners = new Vector3[4];
        rt.GetWorldCorners(corners);
        Vector3 bottomLeft = Camera.main.WorldToScreenPoint(corners[0]);
        Vector3 topRight   = Camera.main.WorldToScreenPoint(corners[2]);
        
        //�X�N���[�����W���Rect��Ԃ�
        float width  = topRight.x - bottomLeft.x;
        float height = topRight.y - bottomLeft.y;
        return new(bottomLeft.x, bottomLeft.y, width, height);
    }

    private Rect ConvertToScreenRect(RectTransform rt, Canvas c)
    {
        //UI�̃��[���h���W��̍���,�E��[�̓_�����߂�
        Vector3[] corners = new Vector3[4];
        rt.GetWorldCorners(corners);
        Vector3 bottomLeft, topRight;
        if (c.renderMode == RenderMode.ScreenSpaceOverlay)
        {
            bottomLeft = corners[0];
            topRight   = corners[2];
        }
        else //ScreenSpaceCamera,World Space�̏ꍇ
        {
            bottomLeft = Camera.main.WorldToScreenPoint(corners[0]);
            topRight   = Camera.main.WorldToScreenPoint(corners[2]);
        }

        //�X�N���[�����W���Rect��Ԃ�
        float width  = topRight.x - bottomLeft.x;
        float height = topRight.y - bottomLeft.y;
        return new(bottomLeft.x, bottomLeft.y, width, height);
    }
}