using UnityEngine;

public static class DrawingUtil
{
    #region constant fields

    public const int WIDTH = 300;
    public const int HEIGHT = 300;

    #endregion

    #region private static methods

    private static void DrawPixel(Texture2D tex, Vector2 pixel, Color[] pixels, Color color)
    {
        DrawThickPoint(tex, pixel, pixels, color);
    }

    private static void DrawThinPoint(Texture2D tex, Vector2 pixel, Color[] pixels, Color color)
    {
        CheckAndAddPixel(pixel, pixels, color);
    }

    private static void DrawThickPoint(Texture2D tex, Vector2 pixel, Color[] pixels, Color color)
    {
        Vector2 centralPixel = new Vector2(pixel.x, pixel.y);

        Vector2 topPixel = new Vector2(pixel.x, pixel.y + 1);
        Vector2 bottomPixel = new Vector2(pixel.x, pixel.y - 1);

        Vector2 rightPixel = new Vector2(pixel.x + 1, pixel.y);
        Vector2 leftPixel = new Vector2(pixel.x - 1, pixel.y);

        Vector2 leftTopPixel = new Vector2(pixel.x - 1, pixel.y + 1);
        Vector2 rightTopPixel = new Vector2(pixel.x + 1, pixel.y + 1);

        Vector2 leftBottomPixel = new Vector2(pixel.x - 1, pixel.y - 1);
        Vector2 rightBottomPixel = new Vector2(pixel.x + 1, pixel.y - 1);

        CheckAndAddPixel(centralPixel, pixels, color);
        CheckAndAddPixel(topPixel, pixels, color);
        CheckAndAddPixel(bottomPixel, pixels, color);
        CheckAndAddPixel(rightPixel, pixels, color);
        CheckAndAddPixel(leftPixel, pixels, color);
        CheckAndAddPixel(leftTopPixel, pixels, color);
        CheckAndAddPixel(rightTopPixel, pixels, color);
        CheckAndAddPixel(leftBottomPixel, pixels, color);
        CheckAndAddPixel(rightBottomPixel, pixels, color);
    }

    private static void DrawLine(Texture2D tex, Color[] pixels, Vector2 p1, Vector2 p2, Color color)
    {
        Vector2 t = p1;
        float frac = 1 / Mathf.Sqrt(Mathf.Pow(p2.x - p1.x, 2) + Mathf.Pow(p2.y - p1.y, 2));
        float ctr = 0;

        while ((int)t.x != (int)p2.x || (int)t.y != (int)p2.y)
        {
            t = Vector2.Lerp(p1, p2, ctr);
            ctr += frac;
            DrawPixel(tex, t, pixels, color);
        }
    }

    private static void DrawEmptySpace(Texture2D tex, Color[] pixels, Vector2 p1, Vector2 p2, Color color)
    {
        float emptyDistance = (p2 - p1).magnitude;
        if (emptyDistance > 1)
        {
            DrawLine(tex, pixels, p1, p2, color);
        }
    }

    private static bool CheckAndAddPixel(Vector2 pixel, Color[] pixels, Color color)
    {
        bool isAdded = false;
        if (checkAvailablePixel(pixel) == true)
        {
            isAdded = AddPixel(pixel, pixels, color);
        }

        return isAdded;
    }

    private static bool AddPixel(Vector2 pixel, Color[] pixels, Color color)
    {
        bool isAdded = false;
        int numPixel = (int)pixel.x + (int)pixel.y * HEIGHT;
        if (pixels[numPixel].r == 1 && pixels[numPixel].g == 1 && pixels[numPixel].b == 1)
        {
            pixels[numPixel] = color;
            isAdded = true;
        }

        return isAdded;
    }

    private static void SetAndApplyTexture(Texture2D tex, Color[] pixels)
    {
        tex.SetPixels(pixels);
        tex.Apply();
    }

    #endregion

    #region public static methods

    public static bool checkAvailablePixel(Vector2 pixel)
    {
        return (pixel.x >= 0 && pixel.x < WIDTH && pixel.y >= 0 && pixel.y < HEIGHT);
    }

    public static void ClearDrawingSurface(Texture2D tex, Color[] pixels)
    {
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = Color.white;
        }

        SetAndApplyTexture(tex, pixels);
    }

    public static void Draw(Texture2D tex, Color[] pixels, Vector2 prevPixel, Vector2 currPixel, Color colorLine)
    {
        DrawEmptySpace(tex, pixels, prevPixel, currPixel, colorLine);
        SetAndApplyTexture(tex, pixels);
    }

    public static Sprite CreateSprite()
    {
        Texture2D texture = new Texture2D(WIDTH, HEIGHT, TextureFormat.RGB24, false);
        return Sprite.Create(texture, new Rect(0, 0, WIDTH, HEIGHT), Vector2.zero);
    }

    #endregion
}
