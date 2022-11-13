using GameSystem.SaveLoad;
using TMPro;
using UnityEngine;

namespace GameSystem.UI
{
    public class SaveSlot : MonoBehaviour
    {
        [SerializeField] private TMP_Text saveLabel;

        private SaveScreen saveScreen;

        public string SaveId { get; private set; } = "Save1";


        public void SetSaveId(string saveId, SaveScreen saveUi)
        {
            SaveId = saveId;
            saveLabel.text = saveId;
            this.saveScreen = saveUi;
        }

        public void OnDelete()
        {
            SaveManager.Instance.DeleteSave(SaveId);
            saveScreen.Refresh();
        }

        public void OnLoad()
        {
            GameManager.Instance.LoadGame(SaveId);
            saveScreen.Refresh();
        }

        public void OnSave()
        {
            SaveManager.Instance.SaveGame(SaveId);
            saveScreen.Refresh();
        }
    }
}