﻿using UnityEngine;
using System.Collections;

[RequireComponent (typeof (BoxCollider2D))]
public class Controller2D : MonoBehaviour {


	public LayerMask collisionMask;
	
	const float skinwidth = .015f;
	public int horizontalRayCount = 4;
	public int verticalRayCount = 4;
	
	float maxClimbAngle = 80;
	float maxDescendAngle = 75;
	
	float horizontalRaySpacing;
	float verticalRaySpacing;
	
	BoxCollider2D collider;
	RaycastOrigins raycastOrigins;
	public CollisionInfo collisions;
	
	
	void Start() {
		collider = GetComponent<BoxCollider2D>(); 
		
		CalculateRaySpacing ();
		
	}
	
	
	
	
	
	
	public void Move(Vector3 velocity) {
	
		UpdateRaycastOrigins();
		
		collisions.Reset();
		
		collisions.velocityOld = velocity;
		
		
		if (velocity.y < 0) {
			DescendSlope(ref velocity);
		}
		
		
		if (velocity.x != 0) {
			HorizontalCollisions (ref velocity);
		}
		
		if (velocity.y != 0) {
			VerticalCollisions (ref velocity);
		}
		
		
		transform.Translate (velocity);
	
	}
	
	void HorizontalCollisions(ref Vector3 velocity) {
		
		float directionX = Mathf.Sign (velocity.x);
		float rayLength = Mathf.Abs (velocity.x) + skinwidth;
		
		
		for (int i =0; i < horizontalRayCount; i++) {
			
			
			Vector2 rayOrigin = (directionX == -1)?raycastOrigins.bottomLeft:raycastOrigins.bottomRight;
			
			rayOrigin += Vector2.up * (horizontalRaySpacing * i);
			
			RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);
			
			
			
			Debug.DrawRay(rayOrigin, Vector2.right * directionX * rayLength, Color.red);
			
			if (hit) {
				
				float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
				
				if (i == 0 && slopeAngle <= maxClimbAngle) {
				
					// e5 13 - fix bug
					if (collisions.descendingSlope) {
						collisions.descendingSlope = false;
						velocity = collisions.velocityOld;
					}
				
				
					// E4 - 12
					float distanceToSlopeStart = 0;
					if (slopeAngle != collisions.slopeAngleOld) {
						distanceToSlopeStart = hit.distance-skinwidth;
						velocity.x -= distanceToSlopeStart * directionX;
					}
					ClimbSlope(ref velocity, slopeAngle);
					// add the velocity back after you climb slope
					velocity.x += distanceToSlopeStart * directionX;
				}
				
			
				// Don't check for collisions when you are climbing the slope
				if (!collisions.climbingSlope || slopeAngle > maxClimbAngle ) {
			
					velocity.x = (hit.distance - skinwidth) * directionX;
					rayLength = hit.distance;
					
					// E4- 15, Fix collisions when obstable is side by side with player on slope
					if (collisions.climbingSlope) {
						velocity.y = Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(velocity.x);
					}
					
					
					
					// if we hit something and collisions.left is true
					collisions.left = directionX == -1;
					collisions.right = directionX == 1;
				
				}
				
				
			}
			
			
			
			
		}
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
			
		
		
			Debug.DrawRay(rayOrigin, Vector2.up * directionY * rayLength, Color.red);
			
			// e2 : 8.30
			if (hit) {
				velocity.y = (hit.distance - skinwidth) * directionY;
				rayLength = hit.distance;
			
			
				// E4- 16.30 Fix collision above player
				if (collisions.climbingSlope) {
					velocity.x = velocity.y  / Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Sign(velocity.x);
				}
			
			
				collisions.below = directionY == -1;
				collisions.above = directionY == 1;
			
			}
		}
		
		// E5-0 - Fix bug that causes player to fall back a bit when you go through a tiny gap. Make it smooth
		if (collisions.climbingSlope) {
			float directionX = Mathf.Sign(velocity.x);
			rayLength = Mathf.Abs(velocity.x) + skinwidth;
			Vector2 rayOrigin = ((directionX == -1)?raycastOrigins.bottomLeft:raycastOrigins.bottomRight) + Vector2.up * velocity.y;
			RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);
			
			if (hit) {
				float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
				if (slopeAngle != collisions.slopeAngle) {
					velocity.x = (hit.distance - skinwidth) * directionX;
					collisions.slopeAngle = slopeAngle;
				}
			}
		}
		
		
	}
	
	
	void ClimbSlope(ref Vector3 velocity, float slopeAngle) {
		float moveDistance = Mathf.Abs (velocity.x);
		
		float climbVelocityY =  Mathf.Sin (slopeAngle * Mathf.Deg2Rad) * moveDistance;
		
		if (velocity.y <= climbVelocityY) {
			velocity.y = climbVelocityY;
			velocity.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(velocity.x);
			collisions.below = true;
			collisions.climbingSlope = true;
			collisions.slopeAngle = slopeAngle;
		}
	}
	
	// E5 - 5
	void DescendSlope(ref Vector3 velocity) {
		float directionX = Mathf.Sign (velocity.x);
		Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomRight : raycastOrigins.bottomLeft;
		RaycastHit2D hit = Physics2D.Raycast (rayOrigin, -Vector2.up, Mathf.Infinity, collisionMask);
		
		if (hit) {
			float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
			if (slopeAngle != 0 && slopeAngle <= maxDescendAngle) {
				if (Mathf.Sign(hit.normal.x) == directionX) {
					if (hit.distance - skinwidth <= Mathf.Tan(slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(velocity.x)) {
						float moveDistance = Mathf.Abs(velocity.x);
						float descendVelocityY = Mathf.Sin (slopeAngle * Mathf.Deg2Rad) * moveDistance;
						velocity.x = Mathf.Cos (slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign (velocity.x);
						velocity.y -= descendVelocityY;
						
						collisions.slopeAngle = slopeAngle;
						collisions.descendingSlope = true;
						collisions.below = true;
					}
				}
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
	
	public struct CollisionInfo {
		public bool above, below;
		public bool left, right;
		
		public bool climbingSlope;
		public bool descendingSlope;
		public float slopeAngle, slopeAngleOld;
		public Vector3 velocityOld;
		
		public void Reset() {
			above = below = false;
			left = right = false;
			climbingSlope = false;
			descendingSlope = false;
			
			slopeAngleOld = slopeAngle;
			slopeAngle = 0;
		}
	
	
	
	
	
	
	}
	
	
	
	
	
}

















