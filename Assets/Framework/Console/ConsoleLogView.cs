using UnityEngine;
using UnityEngine.UI;

namespace Framework
{
    [RequireComponent(typeof(Text))]
    public class ConsoleLogView : MonoBehaviour
    {
        float TimeToLive { get; set; }
        float TimeStamp { get; set; }
        CanvasGroup Canvas { get; set; }

        const float FadeTime = 1f;

        private void Awake()
        {
            Canvas = gameObject.GetOrAddComponent<CanvasGroup>();
        }

        public void SetText(string text, float timeToLive)
        {
            GetComponent<Text>().text = text;
            TimeToLive = timeToLive;
            TimeStamp = Time.time;

            gameObject.SetActive(true);
        }

        private void Update()
        {
            if (Time.time > TimeStamp + TimeToLive)
            {
                Canvas.alpha -= Time.deltaTime / FadeTime;

                if (Time.time > TimeStamp + TimeToLive + FadeTime)
                    Destroy(gameObject);
            }
        }
    }
}
