﻿using UnityEngine;
using System.Collections;

[RequireComponent (typeof (BoxCollider2D))]
public class Controller2D : MonoBehaviour {


	public LayerMask collisionMask;
	
	const float skinwidth = .015f;
	public int horizontalRayCount = 4;
	public int verticalRayCount = 4;
	
	float horizontalRaySpacing;
	float verticalRaySpacing;
	
	BoxCollider2D collider;
	RaycastOrigins raycastOrigins;
	
	void Start() {
		collider = GetComponent<BoxCollider2D>(); 
		
		CalculateRaySpacing ();
		
	}
	
	
	
	
	
	
	public void Move(Vector3 velocity) {
	
		
		UpdateRaycastOrigins();
		
		VerticalCollisions (ref velocity);
		
		transform.Translate (velocity);
	
	}
	
	
	
	void VerticalCollisions(ref Vector3 velocity) {
	
		// moving down is -1, up is +1
		float directionY = Mathf.Sign (velocity.y);
		float rayLength = Mathf.Abs (velocity.y) + skinwidth;
	
	
		for (int i =0; i < verticalRayCount; i++) {
		
			// want to see which direction we are moving if we are moving down we want our rays to start 
			// in the  bottom left corner, if we are moving up ray is top left
			Vector2 rayOrigin = (directionY == -1)?raycastOrigins.bottomLeft:raycastOrigins.topLeft;
			
			// e2 6:00
			rayOrigin += Vector2.right * (verticalRaySpacing * i + velocity.x);
			
			RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, collisionMask);
			
		
		
			Debug.DrawRay(raycastOrigins.bottomLeft + Vector2.right * verticalRaySpacing * i, Vector2.up * -2, Color.red);
			
			// e2 : 8.30
			if (hit) {
				velocity.y = (hit.distance - skinwidth) * directionY;
				rayLength = hit.distance;
			
			}
			
			
			
			
		}
	}
	
	
	
	void UpdateRaycastOrigins() {
		Bounds bounds = collider.bounds;
		bounds.Expand (skinwidth * -2);
		
		raycastOrigins.bottomLeft = new Vector2 (bounds.min.x, bounds.min.y);
		raycastOrigins.bottomRight = new Vector2 (bounds.max.x, bounds.min.y);
		raycastOrigins.topLeft = new Vector2 (bounds.min.x, bounds.max.y);
		raycastOrigins.topRight = new Vector2 (bounds.max.x, bounds.max.y);
		
		
	}
	
	void CalculateRaySpacing() {
		Bounds bounds = collider.bounds;
		bounds.Expand (skinwidth * -2);
		
		// make sure ray count is at least 2
		horizontalRayCount = Mathf.Clamp (horizontalRayCount, 2, int.MaxValue);
		verticalRayCount = Mathf.Clamp (verticalRayCount, 2, int.MaxValue);
		
	
		// 11:00 EP1
		horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
		verticalRaySpacing = bounds.size.x / (verticalRayCount -1);
	
	}
	
	
	
	struct RaycastOrigins {
		public Vector2 topLeft, topRight;
		public Vector2 bottomLeft, bottomRight;
	
	}
	
	
	
	
	
	
	
}

















