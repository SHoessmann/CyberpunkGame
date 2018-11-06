using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
	public class LeapZombie : PatrolZombie
	{

		public float airTime;

		private Transform player;
		private bool leaping = false;

		public override void Update()
		{

			base.Update();
			
		}

		public void Start() {
			player = GameObject.FindWithTag("Player").transform;
		}

		private void Leap() {
			// put stuff here
			Vector2 playerDistV2 = player.position.ToV2() - transform.position.ToV2() - new Vector2(0,(float)0.5*GetComponent<BoxCollider2D>().size.y);

			float yVel = (float)(playerDistV2.y - (Physics.gravity.y*0.5*airTime*airTime)) / airTime;
			float xVel = (float) playerDistV2.x / airTime;

			GetComponent<Rigidbody2D>().velocity = new Vector3(xVel, yVel, 0);
			leaping = true;
		}
	}
}