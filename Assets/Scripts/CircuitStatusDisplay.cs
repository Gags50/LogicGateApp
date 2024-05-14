using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CircuitStatusDisplay : MonoBehaviour
{
    public Color32 on, off;
    public Image in1, in2, in3, in4, out1, out2, out3, out4;

    CircuitState state = new();

    public void Awake() {
        CircuitManager.StateChange += (CircuitState newState) => state = newState;
        ShowCircuitState(new CircuitState());
    }

    public void OnDestroy() {
        CircuitManager.StateChange -= ShowCircuitState;
    }

    void Update() {
        ShowCircuitState(state);
    }
    public void ShowCircuitState(CircuitState state) {
        in1.color = state.in1 ? on : off;
        in2.color = state.in2 ? on : off;
        in3.color = state.in3 ? on : off;
        in4.color = state.in4 ? on : off;

        out1.color = state.out1 ? on : off;
        out2.color = state.out2 ? on : off;
        out3.color = state.out3 ? on : off;
        out4.color = state.out4 ? on : off;
    }
}
