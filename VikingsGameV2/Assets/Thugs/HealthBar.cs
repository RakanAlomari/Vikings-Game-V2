using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour {
    private Slider slider;

    void Awake() {
        slider = GetComponent<Slider>();
    }

    public void SetHealth(int current, int max) {
        slider.maxValue = max;
        slider.value = current;
    }
}
