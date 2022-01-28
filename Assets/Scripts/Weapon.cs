using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{

	//  the information needed for the laser to fire from the correct place
	// public LineRenderer lineRenderer;
	public GameObject bullet;
	public float laserDistance = 5f;                      //  how far the spaceman can shoot and hit something
	float timeToFire = 0;                     //  time between bursts of fire for a multiple fire weapon
	public float fireRate = 5;

	public Vector3 mousePosition;
	public Vector3 firePointPosition;

	// Start is called before the first frame update
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{
		//  if it's a multiple fire weapon and the fire button is being HELD DOWN and the time is right for the next (or first) shot then shoot
		if (Input.GetButton("Fire1") && Time.time > timeToFire)
		{
			//    update the timeToFire so we wait until it's time to fire before firing the next bullet.
			timeToFire = Time.time + 1 / fireRate;
			Shoot();
		}
	}


	void Shoot()
	{

		// Debug.Log("SHOOT");
		GameObject b = Instantiate(bullet);
		b.transform.position = transform.position;
		b.GetComponent<BulletBehaviour>().StartShoot(true);

		// mousePosition = new Vector3(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y, 0);
		// firePointPosition = new Vector3(transform.position.x, transform.position.y, 0);

		// RaycastHit2D hit = Physics2D.Raycast(firePointPosition, mousePosition - firePointPosition, 100);
		// Debug.DrawLine(firePointPosition, (mousePosition - firePointPosition) * 100, Color.red);

		// Vector3 laserEndPoint = new Vector3(transform.position.x + 5, transform.position.y, 0);
		// RaycastHit2D hitInfo = Physics2D.Raycast(transform.position, laserEndPoint - transform.position, laserDistance);
		// // Debug.DrawLine(transform.position, (laserEndPoint - transform.position) * 100, Color.cyan);

		// if (hitInfo.collider != null)
		// {
		// 	Debug.Log("We hit something: " + hitInfo.transform.name);
		// 	lineRenderer.SetPosition(0, transform.position);
		// 	lineRenderer.SetPosition(1, hitInfo.point);

		// }
		// else
		// {
		// 	lineRenderer.SetPosition(0, transform.position);
		// 	lineRenderer.SetPosition(1, transform.position + transform.right * laserDistance);
		// }

		// lineRenderer.enabled = true;
		// yield return new WaitForSeconds(0.02f);
		// lineRenderer.enabled = false;




	}

}
