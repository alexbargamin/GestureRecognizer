using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DrawingControl : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    #region delegate and event fields

    public delegate void DrawAction(Vector3 drawingPosition);
    public delegate void DrawingFinish(List<Vector2> listPoints);
    public delegate void DrawOutsideSurface();

    // Fire when we draw something
    public event DrawAction OnDraw;

    // Fire when we finished drawing
    public event DrawingFinish OnFinishDraw;

    // Fire when we drawing outside the drawing surface
    public event DrawOutsideSurface OnOutsideSurfaceDraw;

    #endregion

    #region private fields

    [SerializeField]
    private Canvas m_Canvas;

    [SerializeField]
    private Image m_Image;

    private Texture2D m_Texture;

    private RectTransform m_RectCanvas;

    private float m_WidthCoef;
    private float m_HeightCoef;

    private Vector2 m_ImageOffset = Vector2.zero;

    private Vector2 m_PrevPixel = Vector2.zero;
    private Vector2 m_CurPixel = Vector2.zero;

    private Color[] m_Pixels;

    private bool m_IsEnableDrawing = false;

    private bool m_IsWhithinSurface = true;

    private List<Vector2> m_PointList = new List<Vector2>();

    #endregion

    #region public methods

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (m_IsEnableDrawing == true)
        {
            m_IsWhithinSurface = true;

            m_PointList.Clear();

            m_Pixels = m_Texture.GetPixels();

            Vector2 pixel = PointerToTexture(eventData.position);
            m_PrevPixel = pixel;
            m_CurPixel = pixel;
            DrawingUtil.Draw(m_Texture, m_Pixels, m_PrevPixel, m_CurPixel, Color.red);
            m_PointList.Add(pixel);

            if (OnDraw != null)
            {
                Vector3 canvasCoordinate = PointerToCanvas(eventData.position);
                OnDraw(canvasCoordinate);
            }
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (m_IsEnableDrawing == true)
        {
            Vector2 pixel = PointerToTexture(eventData.position);
            m_CurPixel = pixel;
            DrawingUtil.Draw(m_Texture, m_Pixels, m_PrevPixel, m_CurPixel, Color.red);
            m_PointList.Add(pixel);

            m_PrevPixel = m_CurPixel;

            if (m_IsWhithinSurface == true)
            {
                m_IsWhithinSurface = DrawingUtil.checkAvailablePixel(pixel);
            }

            if (OnDraw != null)
            {
                Vector3 canvasCoordinate = PointerToCanvas(eventData.position);
                OnDraw(canvasCoordinate);
            }
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (m_IsEnableDrawing == true)
        {
            Vector2 pixel = PointerToTexture(eventData.position);
            m_CurPixel = pixel;
            DrawingUtil.Draw(m_Texture, m_Pixels, m_PrevPixel, m_CurPixel, Color.red);
            m_PointList.Add(pixel);

            if (OnDraw != null)
            {
                Vector3 canvasCoordinate = PointerToCanvas(eventData.position);
                OnDraw(canvasCoordinate);
            }

            m_PointList.RemoveAt(0);
            m_PointList.RemoveAt(m_PointList.Count - 1);

            if (m_IsWhithinSurface == true)
            {
                if (OnFinishDraw != null)
                {
                    OnFinishDraw(m_PointList);
                }
            }
            else
            {
                if (OnOutsideSurfaceDraw != null)
                {
                    OnOutsideSurfaceDraw();
                }
            }
        }
    }

    public void ClearDrawingSurface()
    {
        DrawingUtil.ClearDrawingSurface(m_Texture, m_Pixels);
    }

    public void EnableDrawing()
    {
        m_IsEnableDrawing = true;
    }

    public void DisableDrawing()
    {
        m_IsEnableDrawing = false;
    }

    #endregion

    #region private methods

    private Vector3 PointerToCanvas(Vector2 screenPosition)
    {
        Vector3 canvasPos = new Vector3(screenPosition.x * m_WidthCoef - m_RectCanvas.sizeDelta.x / 2, screenPosition.y * m_HeightCoef - m_RectCanvas.sizeDelta.y / 2, 10f);
        return canvasPos;
    }

    private Vector3 PointerToTexture(Vector2 screenPosition)
    {
        Vector3 drawingPos = new Vector3(screenPosition.x * m_WidthCoef - m_ImageOffset.x, screenPosition.y * m_HeightCoef - m_ImageOffset.y);
        return drawingPos;
    }

    private void InitDrawingSurface()
    {
        m_Image.sprite = DrawingUtil.CreateSprite();

        m_Texture = m_Image.sprite.texture;
        m_Pixels = m_Texture.GetPixels();

        DrawingUtil.ClearDrawingSurface(m_Image.sprite.texture, m_Pixels);

        m_RectCanvas = m_Canvas.GetComponent<RectTransform>();

        float screenWidth = (float)Camera.main.pixelWidth;
        float screenHeight = (float)Camera.main.pixelHeight;

        float imageWidth = m_Image.rectTransform.sizeDelta.x;
        float imageHeight = m_Image.rectTransform.sizeDelta.y;

        float canvasWidth = m_RectCanvas.sizeDelta.x;
        float canvasHeigth = m_RectCanvas.sizeDelta.y;

        m_WidthCoef = canvasWidth / screenWidth;
        m_HeightCoef = canvasHeigth / screenHeight;

        m_ImageOffset = new Vector2((canvasWidth / 2 - imageWidth) / 2, (canvasHeigth - imageHeight) / 2);
    }

    // Use this for initialization
    private void Start()
    {
        InitDrawingSurface();
    }

    private void OnDestroy()
    {
        ClearDrawingSurface();
    }

    #endregion
}
