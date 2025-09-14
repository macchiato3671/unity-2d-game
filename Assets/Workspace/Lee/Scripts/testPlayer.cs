using Unity.VisualScripting;
using UnityEngine;

public class testPlayer : MonoBehaviour
{

    public Rigidbody2D rigid;
    public float horizontal_input;
    public float vertical_input;
    public float move_speed = 10f;
    public float climb_speed = 5f;
    public float jump_power = 10f;
    public float gravS = 5f;

    void Update()
    {
        horizontal_input = Input.GetAxisRaw("Horizontal");
        vertical_input = Input.GetAxisRaw("Vertical");
    }


    void FixedUpdate()
    {


        //Move Speed

        rigid.linearVelocity = new Vector2(horizontal_input * move_speed, rigid.linearVelocity.y);

    }
}
