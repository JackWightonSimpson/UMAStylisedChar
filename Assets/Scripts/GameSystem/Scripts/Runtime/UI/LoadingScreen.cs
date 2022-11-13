using TMPro;
using UnityEngine;

namespace GameSystem.UI
{
    public class LoadingScreen : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text mainText;
        [SerializeField]
        private TMP_Text progressText;

        [SerializeField] private TMP_Text messageText;


        public void SetProgress(string stage, float progress, string message)
        {
            mainText.SetText(stage);
            progressText.SetText($"{progress * 100}%");
            messageText.SetText(message);
        }
        
    }
}