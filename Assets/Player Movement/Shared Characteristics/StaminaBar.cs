using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StaminaBar : MonoBehaviour
{
    [SerializeField] private Image fill;

    private Stamina playerStamina;

    // Start is called before the first frame update
    void Start()
    {
        playerStamina = FindObjectOfType<FirstPersonController>().GetComponent<Stamina>();
    }

    // Update is called once per frame
    void Update()
    {
        fill.fillAmount = playerStamina.CurrentStamina / playerStamina.MaxStamina;
    }
}
