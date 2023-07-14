using UnityEngine;
using UnityEngine.UI;

public class ManaBar : MonoBehaviour
{
    // NOTE for later: Can change color of healthbar at certain percentage thresholds using a gradient (Brackeys tutorial)
    public Slider slider;

    public void SetMaxMana(int health)
    {
        slider.maxValue = health;
        slider.value = health;
    }

    public void SetMana(int health)
    {
        slider.value = health;
    }
}
