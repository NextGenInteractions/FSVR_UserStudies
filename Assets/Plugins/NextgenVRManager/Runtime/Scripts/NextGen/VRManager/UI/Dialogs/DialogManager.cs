using NextGen.VrManager.Devices;
using System.Collections.Generic;
using UnityEngine;

namespace NextGen.VrManager.Ui
{
    public class DialogManager : MonoBehaviour
    {
        public static DialogManager Instance;

        public GameObject lightdim;
        public Transform dialoguesGameobjectMount;
        public GameObject simpleDialogPrefab;
        public GameObject setMetadataDialogPrefab;

        public List<Dialog> dialogs = new List<Dialog>();

        private void Awake()
        {
            Instance = this;
        }

        public static void AddDialog(Dialog d)
        {
            Instance.dialogs.Add(d);
            d.transform.SetParent(Instance.dialoguesGameobjectMount);
            Instance.RefreshUi();
        }

        public static void RemoveDialog(Dialog d)
        {
            if(Instance.dialogs.Contains(d))
                Instance.dialogs.Remove(d);
            Destroy(d.gameObject);
            Instance.RefreshUi();
        }

        public static void DisplayMessage(string message)
        {
            SimpleDialog d = Instantiate(Instance.simpleDialogPrefab).GetComponent<SimpleDialog>();
            d.SetMessage(message);
        }

        public static void SetMetadataDialog(Device d)
        {
            Instantiate(Instance.setMetadataDialogPrefab).GetComponent<SetMetadataDialog>().Init(d);
        }

        public void RefreshUi()
        {
            lightdim.SetActive(dialogs.Count > 0);

            if (dialogs.Count > 0)
                dialogs[0].gameObject.SetActive(true);

            for(int i = 1; i < dialogs.Count; i++)
            {
                dialogs[i].gameObject.SetActive(false);
            }
        }

        private void Update()
        {
        }
    }
}
