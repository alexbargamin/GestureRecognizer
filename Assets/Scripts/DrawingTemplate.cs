using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DrawingTemplate : MonoBehaviour
{
    #region private fields

    [SerializeField]
    private Image m_ImageTexture;

    private List<Shape> m_ShapeList;

    private int m_ShapeIndex = 0;

    private Texture2D m_TemplateTexture;

    private Color[] m_Pixels;

    private Shape m_CurrentShape;

    #endregion

    #region property fields

    public Shape CurrentShape { get { return m_CurrentShape; } }

    #endregion

    #region public methods

    public void ShowFirstShapeTemplate()
    {
        m_ShapeIndex = 0;
        ShowNextShapeTemplate();
    }

    public void ShowNextShapeTemplate()
    {
        if (m_ShapeIndex >= m_ShapeList.Count)
        {
            m_ShapeIndex = 0;
        }

        m_CurrentShape = m_ShapeList[m_ShapeIndex];

        DrawShapeTemplate(m_CurrentShape);

        m_ShapeIndex++;
    }

    public void ClearSurface()
    {
        DrawingUtil.ClearDrawingSurface(m_TemplateTexture, m_Pixels);
    }

    #endregion

    #region private methods

    private void DrawShapeTemplate(Shape shape)
    {
        DrawingUtil.ClearDrawingSurface(m_TemplateTexture, m_Pixels);

        Vector2 prevPixel = shape.Points[0];
        Vector2 curPixel = shape.Points[0];

        foreach (Vector2 point in shape.Points)
        {
            curPixel = point;
            DrawingUtil.Draw(m_TemplateTexture, m_Pixels, curPixel, prevPixel, Color.black);
            prevPixel = curPixel;
        }

        DrawingUtil.Draw(m_TemplateTexture, m_Pixels, prevPixel, shape.Points[0], Color.black);
    }

    private void InitializeTemplate()
    {
        m_ImageTexture.sprite = DrawingUtil.CreateSprite();

        m_ShapeList = ShapeParser.GetShapes();
        m_TemplateTexture = m_ImageTexture.sprite.texture;
        m_Pixels = m_TemplateTexture.GetPixels();

        ClearSurface();
    }

    // Use this for initialization
    private void Start()
    {
        InitializeTemplate();
    }

    private void OnDestroy()
    {
        ClearSurface();
    }

    #endregion
}
