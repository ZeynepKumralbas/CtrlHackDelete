using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public GameObject txtInteractionButton;

    public Slider missionCompletePercentSlider;
    public TMP_Dropdown missionListDropdown;
    public Image imgMissionPointer;

    public GameObject pnlWatcherVisibilityBlocker;

    public Image[] humanSkills;

    void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        for(int i = 0; i < humanSkills.Length; i++)
        {
            Color color = humanSkills[i].GetComponent<Image>().color;
            color.a = 0.1f;
            humanSkills[i].GetComponent<Image>().color = color;
        }
    }
}

