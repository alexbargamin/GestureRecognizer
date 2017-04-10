using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    #region private fields

    [SerializeField]
    private float m_TimerValue = 20f;

    [SerializeField]
    private float m_MatchThreshold = 0.75f;

    [SerializeField]
    private float m_AnswerDuration = 1f;

    [SerializeField]
    private DrawingControl m_DrawingControl;

    [SerializeField]
    private DrawingTemplate m_DrawingTemplate;

    [SerializeField]
    private ShapeLoader m_ShapeLoader;

    [SerializeField]
    private Timer m_Timer;

    [SerializeField]
    private Text m_TimerText;

    [SerializeField]
    private Button m_ButtonStart;

    [SerializeField]
    private Button m_ButtonRestart;

    [SerializeField]
    private Text m_ScoreText;

    [SerializeField]
    private GameObject m_CorrectAnswer;

    [SerializeField]
    private GameObject m_WrongAnswer;

    [SerializeField]
    private GameObject m_ScoreArea;

    [SerializeField]
    private GameObject m_TimerArea;

    [SerializeField]
    private GameObject m_Trail;

    [SerializeField]
    private GameObject m_Blocker;

    [SerializeField]
    private GameObject m_ErrorConfigScreen;

    private int m_Score = 0;

    private float m_ExtraTime = 5f;

    private float m_DeltaTime = 0.5f;

    #endregion

    #region private methods

    private void ShowStartScreen()
    {
        m_ButtonRestart.gameObject.SetActive(false);
        m_ButtonStart.gameObject.SetActive(true);
        m_ScoreArea.SetActive(false);
        m_Blocker.SetActive(true);
    }

    private void ShowRestartScreen()
    {
        m_DrawingControl.DisableDrawing();
        m_DrawingControl.ClearDrawingSurface();

        m_DrawingTemplate.ClearSurface();

        m_ButtonRestart.gameObject.SetActive(true);
        m_ButtonStart.gameObject.SetActive(false);
        m_ScoreArea.SetActive(true);
        m_Blocker.SetActive(true);

        ShowScore();
    }

    private void ShowScore()
    {
        m_ScoreText.text = m_Score.ToString();
    }

    private void UpdateTimer()
    {
        float bonusTime = m_ExtraTime - m_Score * m_DeltaTime;
        if (bonusTime < 0)
        {
            bonusTime = 0;
        }

        m_Timer.Add(bonusTime);
    }

    private void CloseUI()
    {
        m_ButtonRestart.gameObject.SetActive(false);
        m_ButtonStart.gameObject.SetActive(false);
        m_ScoreArea.SetActive(false);
        m_Blocker.SetActive(false);
    }

    private void ShowAnswer(bool isCorrect)
    {
        GameObject answer = null;
        answer = (isCorrect == true) ? m_CorrectAnswer : m_WrongAnswer;
        StartCoroutine(ShowAnswer(answer, m_AnswerDuration, OnBeginAnswerShow, OnAnswerShowed));
    }

    private IEnumerator ShowAnswer(GameObject answer, float answerTime, UnityAction OnBeginAnswerShow, UnityAction OnAnswerShowed)
    {
        if (OnBeginAnswerShow != null)
        {
            OnBeginAnswerShow();
        }

        answer.SetActive(true);

        yield return new WaitForSeconds(answerTime);

        if (OnAnswerShowed != null)
        {
            OnAnswerShowed();
        }

        answer.SetActive(false);
    }

    private bool RecognizeGesture(List<Vector2> points, List<Vector2> pointsTemplate)
    {
        bool result = false;

        Vector2[] candidatePoints = points.ToArray();
        Vector2[] templatePoints = pointsTemplate.ToArray();

        Gesture candidate = new Gesture(candidatePoints);
        Gesture template = new Gesture(templatePoints);

        float matchCoef = GestureRecognizer.CheckMatchingShape(candidate, template);

        if (matchCoef > m_MatchThreshold)
        {
            result = true;
        }

        return result;
    }

    private void Initialization()
    {
        m_ButtonStart.onClick.AddListener(OnStartButtonClick);
        m_ButtonRestart.onClick.AddListener(OnRestartButtonClick);

        m_ShapeLoader.OnSuccessLoad += OnSuccessLoadConfig;
        m_ShapeLoader.OnFailLoad += OnFailLoadConfig;

        m_DrawingControl.OnDraw += OnDraw;
        m_DrawingControl.OnFinishDraw += OnFinishDraw;
        m_DrawingControl.OnOutsideSurfaceDraw += OnOutsideDraw;

        m_Timer.OnChangeValue += OnTimerTextChange;
        m_Timer.OnEnd += ShowRestartScreen;
        m_Timer.Set(m_TimerValue);

        m_CorrectAnswer.SetActive(false);
        m_WrongAnswer.SetActive(false);
        m_TimerArea.SetActive(false);

        m_ErrorConfigScreen.SetActive(false);
    }

    private void Awake()
    {
        Initialization();
    }

    #endregion

    #region handlers

    private void OnStartButtonClick()
    {
        CloseUI();
        m_TimerArea.SetActive(true);
        m_DrawingControl.EnableDrawing();
        m_DrawingTemplate.ShowFirstShapeTemplate();
        m_Timer.Run();
    }

    private void OnRestartButtonClick()
    {
        CloseUI();
        m_DrawingControl.EnableDrawing();
        m_DrawingTemplate.ShowFirstShapeTemplate();
        m_Score = 0;
        m_Timer.Set(m_TimerValue);
        m_Timer.Run();
    }

    private void OnTimerTextChange(string value)
    {
        m_TimerText.text = value;
    }

    private void OnDraw(Vector3 drawingPosition)
    {
        m_Trail.transform.position = drawingPosition;
    }

    private void OnFinishDraw(List<Vector2> points)
    {
        bool isRecognized = RecognizeGesture(points, m_DrawingTemplate.CurrentShape.Points);

        if (isRecognized == true)
        {
            m_Score++;

            UpdateTimer();
        }

        ShowAnswer(isRecognized);
    }

    private void OnOutsideDraw()
    {
        ShowAnswer(false);
    }

    private void OnAnswerShowed()
    {
        m_Timer.UnPause();
        m_DrawingControl.ClearDrawingSurface();
        m_DrawingTemplate.ShowNextShapeTemplate();
    }

    private void OnBeginAnswerShow()
    {
        m_Timer.Pause();
    }

    private void OnSuccessLoadConfig()
    {
        List<Shape> shapes = m_ShapeLoader.GetShapes();
        if (shapes != null && shapes.Count > 0)
        {

            m_DrawingTemplate.SetShapes(m_ShapeLoader.GetShapes());
            ShowStartScreen();
        }
        else
        {
            OnFailLoadConfig();
        }
    }

    private void OnFailLoadConfig()
    {
        m_ErrorConfigScreen.SetActive(true);
    }

    #endregion
}
