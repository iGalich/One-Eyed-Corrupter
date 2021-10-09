using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomCounter : MonoBehaviour
{
    private int spawnerAmount = 29;
    public int count = 0;

    public void IncreaseCount()
    {
        count++;
        CheckCount();
    }
    private void CheckCount()
    {
        if (count == spawnerAmount)
            GameManager.instance.ShowText("I have explored this entire area.", 30, Color.white, GameManager.instance.player.transform.position + new Vector3(0, 0.16f, 0), Vector3.up * 15, 5f);
    }
}
