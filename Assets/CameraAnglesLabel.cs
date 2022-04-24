using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class CameraAnglesLabel : MonoBehaviour
{
    [SerializeField] private Transform arCamera;
    private TMP_Text _text;


    private void Awake()
    {
        _text ??= GetComponent<TMP_Text>();
    }

    private void Update()
    {
        var eulerAngles = arCamera.eulerAngles;
        _text.text =
            $"Camera angles:\nx:{eulerAngles.x.ToString(CultureInfo.InvariantCulture)} y:{eulerAngles.y.ToString(CultureInfo.InvariantCulture)} z:{eulerAngles.z.ToString(CultureInfo.InvariantCulture)}";
    }
}