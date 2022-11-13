using GameSystem.SaveLoad;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameSystem.UI
{
    public class SaveScreen : MonoBehaviour
    {
        [SerializeField] private SaveSlot slotPrefab;
        [SerializeField] private RectTransform slotParent;

        [SerializeField] private TMP_InputField inputField;

   
        private void Start()
        {
            Refresh();
        }
        
        private void OnEnable()
        {
            if (Application.isPlaying)
            {
                Refresh();
            }
        }

        public void Refresh()
        {
            for (var i = slotParent.childCount-1; i >=0; i--)
            {
                Destroy(slotParent.GetChild(i).gameObject);
            }

            foreach (var save in SaveManager.Instance.ListSaves())
            {
                var slot = Instantiate(slotPrefab, slotParent);
                slot.SetSaveId(save, this);
            }
        }

        public void NewSave()
        {
            Debug.Log("New Saved");
            SaveManager.Instance.SaveGame(inputField.text);
            Refresh();
        }
    }
}