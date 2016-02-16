using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Controller2D))]

public class Player : MonoBehaviour {


	float gravity = -20;
	Vector3 velocity;



	Controller2D controller;	
		
	void Start() {
		controller = GetComponent<Controller2D>();
							
	}		
	
				
	void Update() {
		velocity.y += gravity * Time.deltaTime;
		controller.Move (velocity * Time.deltaTime);
	
	}
					
						
							
								
									
										
											
												
													
														
															
																
																	
																		
																				
}
