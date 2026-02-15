using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class Timer : MonoBehaviour
{
    const float secondsInADay = 36000f; //  cada día dura 10 horas del juego
    float time;
    int hours, minutes;
    public float daySpeed = 30f; // 2 segundos en tiempo real son 1 minuto en el juego

    [SerializeField] TMP_Text text;

    public static event Action OnDayPassed;

    void Update()
    {
        time += Time.deltaTime * daySpeed;
        toHoursMinutes(time);
        text.text = "Time: " + hours.ToString("00") + ":" + minutes.ToString("00");
        if (time >= secondsInADay)
        {
            time = 0f;
            OnDayPassed?.Invoke();
            Debug.Log("A day has passed in the game.");
        }
    }

    void toHoursMinutes(float totalSeconds)
    {
        hours = (int)(totalSeconds / 3600) % 10;
        minutes = (int)(totalSeconds / 60) % 60;
    }

    /*
    Este timepo influye a las plantas, que crecen cada cierto número de días.
    El personaje puede seguir jugando independientemente del tiempo y de los días que pasen.
    Para que el personaje tenga que irse a dormir 
    */
}
