using System;
using UnityEngine;

public static class TextureDrawer
{
    public static Texture2D Scale(this Texture2D texture, int width, int height)
    {
        RenderTexture render = new RenderTexture(width, height, 24);
        RenderTexture.active = render;
        Graphics.Blit(texture, render);

        Texture2D scaled = new Texture2D(width, height);
        scaled.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        scaled.Apply();
        return scaled;
    }

    public static void DrawPixel(this Texture2D texture, int x, int y, int width, int height, Color color)
    {
        if (x < 0 || x > width || y < 0 || y > height)
            return;
        texture.SetPixel(x, y, color);
    }

    public static void DrawLine(this Texture2D texture, Vector3 start, Vector3 end, Color color)
    {
        Line(texture, (int)start.x, (int)start.y, (int)end.x, (int)end.y, color);
    }

    private static void Line(Texture2D texture, int x0, int y0, int x1, int y1, Color color)
    {
        int width = texture.width;
        int height = texture.height;

        bool isSteep = Math.Abs(y1 - y0) > Math.Abs(x1 - x0);
        if (isSteep)
        {
            Swap(ref x0, ref y0);
            Swap(ref x1, ref y1);
        }
        if (x0 > x1)
        {
            Swap(ref x0, ref x1);
            Swap(ref y0, ref y1);
        }

        int deltaX = x1 - x0;
        int deltaY = Math.Abs(y1 - y0);

        int error = deltaX / 2;
        int yStep;
        int y = y0;

        if (y0 < y1)
            yStep = 1;
        else
            yStep = -1;

        for (int x = x0; x < x1; x++)
        {
            if (isSteep)
                texture.DrawPixel(y, x, width, height, color);
            else
                texture.DrawPixel(x, y, width, height, color);

            error -= deltaY;
            if (error < 0)
            {
                y += yStep;
                error += deltaX;
            }
        }
    }

    private static void Swap(ref int x, ref int y)
    {
        int temp = x;
        x = y;
        y = temp;
    }
}