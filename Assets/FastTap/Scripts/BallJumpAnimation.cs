using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallJumpAnimation : MonoBehaviour {

	
    private void OnCollisionEnter2D(Collision2D collision)
    {
		if (collision.gameObject.name.Contains("PodiumStart"))
            GetComponent<Rigidbody2D>().AddForce(Vector2.up*25,ForceMode2D.Impulse);
    }
    
}
