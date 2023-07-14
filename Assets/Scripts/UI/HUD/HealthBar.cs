using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    // NOTE for later: Can change color of healthbar at certain percentage thresholds using a gradient (Brackeys tutorial)
    public Slider slider;

    public void SetMaxHealth(int health)
    {
        slider.maxValue = health;
        slider.value = health;
    }

    public void SetHealth(int health)
    {
        slider.value = health;
    }
}
