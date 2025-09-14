using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerConfiner : MonoBehaviour
{
    int minX, maxX;

    void Awake(){
        minX = 0;
        maxX = 80;
    }

    public void SetMax(int x){
        maxX = x+1;
    }

    public void SetMin(int x)
    {
        minX = x;
    }

    void FixedUpdate(){
        Vector2 pos = transform.position;
        if(pos.x < minX)
            transform.position = new Vector2(minX,pos.y);
        else if(pos.x > maxX)
            transform.position = new Vector2(maxX,pos.y);
    }
}
