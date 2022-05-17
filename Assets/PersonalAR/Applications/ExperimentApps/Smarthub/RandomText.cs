using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

[RequireComponent(typeof(TextMeshPro))]
public class RandomText : MonoBehaviour
{
    public enum RandomTextType { ModelNumber, SerialNumber, ManufactureDate, PurchaseDate }

    public RandomTextType textType;
    [ReadOnly] public string text;

    private System.Random rand = new System.Random();

    void OnValidate()
    {
        switch (textType)
        {
            case RandomTextType.ModelNumber:
                this.text = GenerateModelNumber();
                break;
            case RandomTextType.SerialNumber:
                this.text = GenerateSerialNumber();
                break;
            case RandomTextType.ManufactureDate:
                this.text = GenerateManufactureDate();
                break;
            case RandomTextType.PurchaseDate:
                this.text = GeneratePurchaseDate();
                break;
        }
        GetComponent<TextMeshPro>().text = this.text;    
    }

    // Start is called before the first frame update
    void Start()
    {
        switch (textType)
        {
            case RandomTextType.ModelNumber:
                this.text = GenerateModelNumber();
                break;
            case RandomTextType.SerialNumber:
                this.text = GenerateSerialNumber();
                break;
            case RandomTextType.ManufactureDate:
                this.text = GenerateManufactureDate();
                break;
            case RandomTextType.PurchaseDate:
                this.text = GeneratePurchaseDate();
                break;
        }
        GetComponent<TextMeshPro>().text = this.text;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private string GenerateModelNumber()
    {
        int randIdx = rand.Next(0, 9);
        List<string> randModels = new List<string>()
        {
            "DTD", "DKG", "LXT", "XGT", "XAM", "THX", "I0N", "ACG", "DHR", "JSD"
        };
        string randModelName = randModels[randIdx];
        return $"Model {randModelName}{rand.Next(100, 9999).ToString()}";
    }

    private string GenerateSerialNumber()
    {
        string rand1 = rand.Next(0, 2) != 0 ? "C" : "S";
        string rand2 = rand.Next(0, 2) != 0 ? "LW" : "PN";
        string rand3 = rand.Next(0, 2) != 0 ? "X" : "";
        return $"Serial {rand1}{rand.Next(10, 99).ToString()}{rand2}{rand.Next(100, 9999).ToString()}{rand3}";
    }

    private string GenerateManufactureDate()
    {
        DateTime start = new DateTime(2022, 1, 1);
        DateTime randDate = start.AddDays(-rand.Next(365, 1095));
        return $"Mfg. Date {randDate.ToString("MM/yy")}";
    }

    private string GeneratePurchaseDate()
    {
        DateTime start = new DateTime(2022, 1, 1);
        DateTime randDate = start.AddDays(-rand.Next(100, 364));
        return $"Date of Purchase {randDate.ToString("MM-dd-yy")}";  
    }
}
