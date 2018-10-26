using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
	public class MamaZombie : EnemyBehaviour
	{

		[Tooltip("This is the baby. For throwing purposes.")]
		public GameObject babyObject;
		[Tooltip("How long the baby is in the air before landing. Higher number = higher throw.")]
		public float airTime;
		[Tooltip("Aggro Distance.")]
		public float aggroDist;
		[Tooltip("Max number of babies that can spawn.")]
		public int maxBabies;
    	//private const float throwInterval = 2f;
		public float throwInterval;

		private Transform player;
		private float playerDist;
		private Vector2 playerDistV2;
		//private Vector2 playerPos;
		private float xVel;
		private float yVel;
		private int babyCount;
		private bool throwCooldown = false;
		

		private bool canAttack = true;
		 public float attackSlowAmount = 2f;
        [Tooltip("how much the zombie is teleported back after attacking")]
        public float displacementAmount = 0.9f;
        [Tooltip("how much time is allowed for attack animation, careful here")]
        public float attackRecoilTime = 0.35f;
        [Tooltip("BLOOD FOR THE BLOOD GOD")]
        public GameObject bloodEffect;
		public float walkSpeed = 1;
        [Tooltip("for when it sees the player")]
        public float runSpeed = 3;


		public void Start() {
			//SetAnimParameter(AnimParams.moveState, 1);
            player = GameObject.FindWithTag("Player").transform;
			//player = Instantiate(publicPlayer, new Vector3(0, 0, 0), Quaternion.identity);
			babyCount = 0;
		}

		public override void Update() {
			base.Update();
            if (IsPlayerInRange()){
				FacePlayer();
				AttackPattern();
			}
		}

		private bool IsPlayerInRange() {
            playerDist = Vector2.Distance(transform.position.ToV2(), player.position.ToV2());
            if (playerDist < aggroDist) return true;
            return false;
        }

        private void AttackPattern() {
			if (!throwCooldown){
            	throwCooldown = true;

            	Invoke("ThrowCooldown", throwInterval);
				StartCoroutine(AttackAnimation());
				//throwBaby();
        	}
        }

		private void FacePlayer(){
			if ((transform.position.x - player.position.x) < 0){
				FacingDirection = Direction.Right; // backwards?
			} else { 
				FacingDirection = Direction.Left;
			}
		}

		private void throwBaby(){
			//playerPos = player.position.ToV2();
			
			if (babyCount < maxBabies){
				babyCount++;
				Debug.Log("Throwing baby.");

				playerDistV2 = player.position.ToV2() - transform.position.ToV2() - new Vector2(0,(float)0.5*GetComponent<BoxCollider2D>().size.y);

				yVel = (float)(playerDistV2.y - (Physics.gravity.y*0.5*airTime*airTime)) / airTime;
				xVel = (float) playerDistV2.x / airTime;			

				EnemyBehaviour thrownBaby = EnemyManager.SpawnEnemy("baby_zombie", (transform.position.ToV2() + new Vector2(0,(float)0.5*GetComponent<BoxCollider2D>().size.y)), FacingDirection);
				
				//GameObject thrownBaby = Instantiate(babyObject, new Vector3(0, 0, 0), Quaternion.identity);
				thrownBaby.GetComponent<Rigidbody2D>().velocity = new Vector3(xVel, yVel, 0);
				//thrownBaby.GetComponent<Rigidbody2D>().velocity = new Vector3(100.0f, 100.0f, 0);
			}

		}

		private void ThrowCooldown(){
			throwCooldown = false;
		}

		private IEnumerator AttackAnimation() {
            SetAnimParameter(AnimParams.moveState, 0);
            canAttack = false;
            runSpeed /= attackSlowAmount;

            float timer = 0;
            SetAnimParameter(AnimParams.attackState, 0);//so animation starts from same position every time
            while (timer <= attackRecoilTime){
                timer += Time.deltaTime;
                SetAnimParameter(AnimParams.attackState, (GetAnimParameter<int>(AnimParams.attackState) + 1) % 3);
                yield return null;
            }

            SetAnimParameter(AnimParams.attackState, 0);
            canAttack = true;
            runSpeed *= attackSlowAmount;
			throwBaby();
            //if (useBloodEffect){
                //temporary effect,replace with better alternative later
            //    Instantiate(bloodEffect, player.transform.position, player.transform.rotation);
            //}

            //transform.position += GetPlayerDirection(true) * displacementAmount;
            //teleports enemy away from player, prevents bugs from never leaving
            //contact with player, should be replaced with better solution or deleted if not needed
        }
		
	}
}