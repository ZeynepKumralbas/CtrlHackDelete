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

    void Awake()
    {
        Instance = this;
    }
}

