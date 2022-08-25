using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{

    public string coinPileType;

    public int randomize()
    {

        switch (coinPileType.ToLower())
        {
            default:
            case "single": return Mathf.CeilToInt(Random.Range(1, 3));
            case "small": return Mathf.CeilToInt(Random.Range(5, 15));
            case "medium": return Mathf.CeilToInt(Random.Range(15, 40));
            case "big": return Mathf.CeilToInt(Random.Range(50, 100));
        }

    }


}
