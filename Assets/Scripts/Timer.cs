using UnityEngine;

public class Timer : MonoBehaviour
{
    #region delegate and event fields

    public delegate void TimerAction();
    public delegate void ChangeValue(string value);

    // Fire when timer is end
    public event TimerAction OnEnd;

    // Fire when timer changes value
    public event ChangeValue OnChangeValue;

    #endregion

    #region private fields

    private bool m_IsEnabled = false;
    private float m_Time = 20f;

    #endregion

    #region public methods

    public void Run()
    {
        m_IsEnabled = true;
    }

    public void Set(float seconds)
    {
        m_Time = seconds;
    }

    public void Add(float seconds)
    {
        m_Time += seconds;
    }

    public void Pause()
    {
        m_IsEnabled = false;
    }

    public void UnPause()
    {
        m_IsEnabled = true;
    }

    #endregion

    #region private methods

    private string ValueToString()
    {
        int minutes = (int)m_Time / 60;
        int seconds = (int)m_Time % 60;
        string strMin = (minutes >= 10) ? minutes.ToString() : ("0" + minutes);
        string strSec = (seconds >= 10) ? seconds.ToString() : ("0" + seconds);
        return strMin + " : " + strSec;
    }

    // Update is called once per frame
    private void Update()
    {
        if (m_IsEnabled == true)
        {
            m_Time -= Time.deltaTime;

            if (m_Time <= 0)
            {
                m_Time = 0;
                m_IsEnabled = false;
            }

            if (OnChangeValue != null)
            {
                OnChangeValue(ValueToString());
            }

            if (m_IsEnabled == false && OnEnd != null)
            {
                OnEnd();
            }
        }
    }

    #endregion
}
