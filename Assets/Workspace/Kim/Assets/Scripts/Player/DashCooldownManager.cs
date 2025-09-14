using UnityEngine;
using UnityEngine.UI;

public class DashCooldownManager : MonoBehaviour
{
    public Slider cooldownSlider;
    public float cooldownTime = 2.5f;

    private float timer = 0f;
    private bool isCooldown = false;

    void Start()
    {
        cooldownSlider.value = 1f;
    }

    void Update()
    {
        if (isCooldown)
        {
            timer += Time.deltaTime;

            if (timer >= cooldownTime)
            {
                isCooldown = false;
                timer = cooldownTime;
                cooldownSlider.value = 1f;
            }
            else
            {
                cooldownSlider.value = Mathf.Clamp01(timer / cooldownTime);
            }
        }
    }

    public bool CanDash()
    {
        return !isCooldown;
    }

    public void StartCooldown()
    {
        isCooldown = true;
        timer = 0f;
        cooldownSlider.value = 0f;
    }
}