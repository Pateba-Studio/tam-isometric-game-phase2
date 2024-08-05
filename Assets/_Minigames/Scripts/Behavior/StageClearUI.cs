using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StageClearUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _stageClearHeader;
    [SerializeField] private TextMeshProUGUI _stageClearNotes;

    [SerializeField] private Transform _root;
    [SerializeField] private List<Sprite> _imageList;

    [SerializeField] private Transform _continueButton;


    private bool _showed = false;

    private void Start()
    {
        _root.GetComponent<Image>().sprite = _imageList[SequenceManager.Instance.HallBId % _imageList.Count];
        _stageClearHeader.text = "CASE " + (SequenceManager.Instance.HallBId + 1) + "/" + SequenceManager.Instance._hallBList.Count + " Completed!";

        if ((SequenceManager.Instance.HallBId + 1) == SequenceManager.Instance._hallBList.Count)
        {
            _stageClearNotes.text = "kamu luar biasa, karena telah bermain \n sampai pada case ini ...";
        }
        else
        {
            _stageClearNotes.text = "Kamu telah menyelesaikan CASE " + (SequenceManager.Instance.HallBId + 1) + "\n" + "Siapkah melanjutkan ke CASE " + (SequenceManager.Instance.HallBId + 2) + "?";
        }

        _root.gameObject.SetActive(false);

        if ((SequenceManager.Instance.HallBId + 1) == SequenceManager.Instance._hallBList.Count)
        {
            _continueButton.transform.localPosition = new Vector3(
                0f,
                _continueButton.localPosition.y,
                _continueButton.localPosition.z
                );
            _continueButton.gameObject.SetActive(false);
        }
    }

    public void PressContinue()
    {
        SequenceManager.Instance.InterfaceContinue(true);
    }

    public void PressExit()
    {
        SequenceManager.Instance.InterfaceContinue(false);
    }

    private void LateUpdate()
    {
        if (!_showed)
        {
            if (SequenceManager.Instance)
            {
                if (SequenceManager.Instance.ShowConfirmNext)
                {
                    SequenceManager.Instance.ShowConfirmNext = false;
                    _showed = true;
                    _root.gameObject.SetActive(true);
                }
            }
        }
    }
}
