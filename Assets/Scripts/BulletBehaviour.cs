using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBehaviour : MonoBehaviour
{
	[SerializeField] float speed;

	public void StartShoot(bool facingRight)
	{
		if (!facingRight) speed *= -1;
		GetComponent<Rigidbody2D>().velocity = new Vector2(speed, 0);
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		if (other.CompareTag("Ground") || other.CompareTag("Enemy"))
			Destroy(gameObject);
	}
}
