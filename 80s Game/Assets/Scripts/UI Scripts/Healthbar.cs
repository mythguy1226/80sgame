using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    Slider healthSlider;
    public void Config(int maxHP)
    {
        healthSlider = GetComponent<Slider>();
        healthSlider.maxValue = maxHP;
    }

    public void DecreaseValue(int amount)
    {
        healthSlider.value = healthSlider.value - amount;
    }
}