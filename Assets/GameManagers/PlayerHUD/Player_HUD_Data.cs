using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Player_HUD_Data : MonoBehaviour
{
    //HUD
    public TextMeshProUGUI AmmoText;
    public GameObject Heart;
    public Sprite EmptyHeart;
    public Sprite FullHeart;
    public GameObject SortingLayerHearts;
    public GameObject Portrait;
    public GameObject[] Portraits = new GameObject[5];
    public Image StaminaImage;
    public Text ScoreText;

    //Gun 
    public Text LightAmmoText;
    public Text HeavyAmmoText;
    public Image GunIcon;

    private void Start()
    {
        GameManager.GameManagerInstance.SetScoreText(ScoreText);
    }
}
