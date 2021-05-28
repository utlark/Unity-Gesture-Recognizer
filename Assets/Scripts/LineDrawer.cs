using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(GameObject))]
public class LineDrawer : MonoBehaviour
{
    [Header("Components")]
    public GameObject linePrefab;

    [Header("Parameters")]
    [Tooltip("Tolerance for Line Simplify. TenserFlow only.")]
    public float tolerance = 0.01f;
    [Tooltip("The minimum distance to spawn new line point.")]
    public float minVertexDistance = 0.01f;

    private GameObject _currentLine;
    private LineRenderer _lineRenderer;
    private readonly List<Vector2> LinePoints = new List<Vector2>();
    private readonly List<Vector2> LineModifyPoints = new List<Vector2>();

    public void CreateLine(Vector3 startPosition)
    {
        _currentLine = Instantiate(linePrefab, Vector3.zero, Quaternion.identity);
        _lineRenderer = _currentLine.GetComponent<LineRenderer>();

        _lineRenderer.SetPosition(0, startPosition);

        LinePoints.Clear();
        LinePoints.Add(Camera.main.WorldToScreenPoint(startPosition));
    }

    public void UpdateLine(Vector3 newPosition)
    {
        if (Vector3.Distance(newPosition, _lineRenderer.GetPosition(_lineRenderer.positionCount - 1)) > minVertexDistance)
        {
            _lineRenderer.positionCount++;
            _lineRenderer.SetPosition(_lineRenderer.positionCount - 1, newPosition);

            LinePoints.Add(Camera.main.WorldToScreenPoint(newPosition));
        }
    }

    public void DeleteLine()
    {
        Destroy(_currentLine);
    }

    public async Task<Texture2D> GetLineTexture(int targetWidth, int targetHeight)
    {
        int width, height, border;
        float xMax = LinePoints[0].x, xMin = LinePoints[0].x;
        float yMax = LinePoints[0].y, yMin = LinePoints[0].y;

        foreach (var point in LinePoints)
        {
            if (xMax < point.x)
                xMax = point.x;
            if (xMin > point.x)
                xMin = point.x;

            if (yMax < point.y)
                yMax = point.y;
            if (yMin > point.y)
                yMin = point.y;
        }

        width = (int)(xMax - xMin);
        height = (int)(yMax - yMin);

        if (width > 0 || height > 0)
        {
            await Task.Run(() =>
            {
                if (width > height)
                    border = (int)(width * 0.2) + 10;
                else
                    border = (int)(height * 0.2) + 10;

                LineModifyPoints.Clear();
                for (int i = 0; i < LinePoints.Count; i++)
                    if (width > height)
                        LineModifyPoints.Add(new Vector2(LinePoints[i].x - xMin + (border / 2), LinePoints[i].y - yMin + (border / 2) + ((width - height) / 2)));
                    else
                        LineModifyPoints.Add(new Vector2(LinePoints[i].x - xMin + (border / 2) + ((height - width) / 2), LinePoints[i].y - yMin + (border / 2)));

                if (width > height)
                    height = width;
                else
                    width = height;

                width += border;
                height += border;

                LineUtility.Simplify(LineModifyPoints, tolerance, LineModifyPoints);
            });
            Texture2D texture = new Texture2D(width, height);

            for (int i = 0; i < LineModifyPoints.Count - 1; i++)
                texture.DrawLine(LineModifyPoints[i], LineModifyPoints[i + 1], Color.black);

            texture.Apply();

            texture = texture.Scale(targetWidth, targetHeight);
            return texture;
        }
        return null;
    }

    public List<Vector2> GetLinePoints()
    {
        LineModifyPoints.Clear();
        for (int i = 0; i < LinePoints.Count; i++)
            LineModifyPoints.Add(new Vector2(LinePoints[i].x, Screen.height - LinePoints[i].y));

        return LineModifyPoints;
    }
}
