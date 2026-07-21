using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenuManager : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] GameObject MainPanel;
    [SerializeField] GameObject SettingPanel;
    [SerializeField] GameObject CreditPanel;

    [Header("Buttons")]
    [SerializeField] Button ResumeBtn;
    [SerializeField] Button SettingBtn;
    [SerializeField] Button SettingReturnBtn;
    [SerializeField] Button CreditBtn;
    [SerializeField] Button ExitBtn;
    [SerializeField] Button CreditReturnBtn;

    [Header("VolumeSliders")]
    [SerializeField] Slider MasterSlider;
    [SerializeField] Slider BgmSlider;
    [SerializeField] Slider SfxSlider;

    // Start is called before the first frame update
    private void OnEnable()
    {
        OpenMainPanel();
        ButtonSetup();
        SliderSetup();
    }

    public void OpenSetting()
    {
        MainPanel.SetActive(false);
        SettingPanel.SetActive(true);
    }

    public void OpenMainPanel()
    {
        MainPanel.SetActive(true);
        SettingPanel.SetActive(false);
        CreditPanel.SetActive(false);
    }

    private void SliderSetup()
    {
        MasterSlider.SetValueWithoutNotify(PlayerPrefs.GetFloat("MasterVolume", 1f));
        BgmSlider.SetValueWithoutNotify(PlayerPrefs.GetFloat("BGMVolume", 1f));
        SfxSlider.SetValueWithoutNotify(PlayerPrefs.GetFloat("SFXVolume", 1f));
    }

    private void ButtonSetup()
    {
        SettingBtn.onClick.AddListener(OpenSetting);
        SettingReturnBtn.onClick.AddListener(OpenMainPanel);
        ExitBtn.onClick.AddListener(ReturnToMM);
        CreditBtn.onClick.AddListener(CreditOpen);
        CreditReturnBtn.onClick.AddListener(CreditReturn);
    }

    private void ReturnToMM()
    {
        SceneLoadingManager.Instance.LoadScene("MainMenu");
    }

    private void CreditOpen()
    {
        MainPanel.SetActive(false);
        CreditPanel.SetActive(true);
    }

    private void CreditReturn()
    {
        MainPanel.SetActive(true);
        CreditPanel.SetActive(false);
    }
}
