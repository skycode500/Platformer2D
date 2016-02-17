using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Controller2D))]

public class Player : MonoBehaviour {



	public float jumpHeight = 4;
	public float timeToJumpApex = .4f;


	float moveSpeed = 6;
	
	float gravity;
	Vector3 velocity;
	float jumpVelocity;



	Controller2D controller;	
		
	void Start() {
		controller = GetComponent<Controller2D>();
		
		gravity = -(2 * jumpHeight) / Mathf.Pow (timeToJumpApex, 2);
		jumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
		print ("Gravity: " + gravity + " Jump Velocity: " + jumpVelocity);
							
	}		
	
				
	void Update() {
	
	
	
		// Prevent player from accumulating gravity when on the ground
		if (controller.collisions.above || controller.collisions.below) {
			velocity.y = 0;
		}
	
	
		Vector2 input = new Vector2 (Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
		
		
		// jump on space press
		if (Input.GetKeyDown (KeyCode.Space) && controller.collisions.below) {
			velocity.y = jumpVelocity;
		}
		
		
		
	
		velocity.x = input.x * moveSpeed;
		
		velocity.y += gravity * Time.deltaTime;
		controller.Move (velocity * Time.deltaTime);
	
	}
					
						
							
								
									
										
											
												
													
														
															
																
																	
																		
																				
}
