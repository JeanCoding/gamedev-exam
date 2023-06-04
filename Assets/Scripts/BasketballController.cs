using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasketballController : MonoBehaviour {
    
    // Create References

    public float MoveSpeed = 10;
    public Transform Ball;
    public Transform Arms;
    public Transform PosOverHead;
    public Transform PosDribble;
    public Transform Target;

    // Create Variables

    private bool IsBallInHands = true;

    // Update is called once per frame
    void Update() {

        // Walking
        Vector3 direction = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        transform.position += direction * MoveSpeed * Time.deltaTime;
        transform.LookAt(transform.position + direction);

        // Ball is in your hands
        if(IsBallInHands) {

            if (Input.GetKey(KeyCode.Space)) {
                Ball.position = PosOverHead.position;
                Arms.localEulerAngles = Vector3.right * 180;

                // Looks towards the basket 
                transform.LookAt(Target.parent.position);
            } else {
                Ball.position = PosDribble.position + Vector3.up * Mathf.Abs(Mathf.Sin(Time.time * 5));
                Arms.localEulerAngles = Vector3.right * 0;
            }
        }
    }
}