using TMPro;
using UnityEngine;

public class Points : MonoBehaviour {

    [SerializeField] TextMeshProUGUI p;
    [SerializeField] TextMeshProUGUI f;
    [SerializeField] TextMeshProUGUI e;    

    void Start() {
        int flowers = 0; // PlayerPrefs.GetInt("Flower");

        int eyes = 0; //PlayerPrefs.GetInt("Eye");

        int points = 1000;

        for(int i = 0; i < PlayerPrefs.GetInt("Flower"); i++) {
            int less = -points/100;
            flowers += less;
            points += less;
        }

        for(int i = 0; i < PlayerPrefs.GetInt("Eye"); i++) {
            int less = -points/100;
            eyes += less;
            points += less;
        }

        f.text = flowers.ToString();
        e.text = eyes.ToString();
        p.text = points.ToString();
    }
}
