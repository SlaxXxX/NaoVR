﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Valve.VR;

public class UIManager : StateListener
{
    public Text InfoText, StatusText;
    public GameObject InfoCanvas, FloorMarker, LeftGripMarker, RightGripMarker, LeftHandMarker, RightHandMarker, ArmedContainer, Nao;
    public SteamVR_RenderModel LeftModel, RightModel;
    void Start()
    {
        Register();
    }

    public override void StateChanged(StateManager.State newState)
    {
        switch (newState)
        {
            case StateManager.State.init:
                break;
            case StateManager.State.positioned:
                InfoText.text = "Great!\nNow look straight forward and press your right grip button to calibrate your height.";
                RightGripMarker.SetActive(true);
                break;
            case StateManager.State.calibrated:
                InfoText.text = "Place your hands inside the marked areas and press your left grip button to begin synchronization.";
                StatusText.text = "Calibrated";
                StatusText.color = Color.green;

                Nao.SetActive(true);

                RightGripMarker.SetActive(false);
                LeftGripMarker.SetActive(true);
                RightHandMarker.SetActive(true);
                LeftHandMarker.SetActive(true);
                FloorMarker.SetActive(false);
                break;
            case StateManager.State.disarmed:
                StatusText.text = "Disarmed";
                StatusText.color = new Color(1, 0.5f, 0.2f);

                LeftModel.SetMeshRendererState(true);
                RightModel.SetMeshRendererState(true);

                ArmedContainer.SetActive(false);
                RightGripMarker.SetActive(false);
                LeftGripMarker.SetActive(true);
                RightHandMarker.SetActive(true);
                LeftHandMarker.SetActive(true);
                break;
            case StateManager.State.armed:
                InfoCanvas.SetActive(false);
                StatusText.text = "Armed";
                StatusText.color = Color.red;

                LeftModel.SetMeshRendererState(false);
                RightModel.SetMeshRendererState(false);

                ArmedContainer.SetActive(true);
                LeftGripMarker.SetActive(false);
                RightHandMarker.SetActive(false);
                LeftHandMarker.SetActive(false);
                break;
        }
    }

    private void ChangeLayerRecursive(GameObject obj, int layer)
    {
        obj.layer = layer;
        foreach (Transform child in obj.transform)
        {
            ChangeLayerRecursive(child.gameObject, layer);
        }
    }
}
