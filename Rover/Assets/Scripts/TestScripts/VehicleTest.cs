using UnityEngine;
using System.Collections;

public class VehicleTest : MonoBehaviour
{
	Rigidbody body;
	float deadZone = 0f;
	public float groundedDrag = 3f;
	public float maxVelocity = 50;

	public float forwardAcceleration = 8000f;
	public float reverseAcceleration = 4000f;
	[SerializeField] float thrust = 0f;

	public float turnStrength = 1000f;
	float turnValue = 0f;

	public ParticleSystem[] dustTrails = new ParticleSystem[2];

	[SerializeField] LayerMask layerMask;

	void Start()
	{
		body = GetComponent<Rigidbody>();
		body.centerOfMass = Vector3.down;

		layerMask = ~layerMask;
	}

	// Uncomment this to see a visual indication of the raycast hit points in the editor window
	//  void OnDrawGizmos()
	//  {
	//
	//    RaycastHit hit;
	//    for (int i = 0; i < hoverPoints.Length; i++)
	//    {
	//      var hoverPoint = hoverPoints [i];
	//      if (Physics.Raycast(hoverPoint.transform.position, 
	//                          -Vector3.up, out hit,
	//                          hoverHeight, 
	//                          layerMask))
	//      {
	//        Gizmos.color = Color.blue;
	//        Gizmos.DrawLine(hoverPoint.transform.position, hit.point);
	//        Gizmos.DrawSphere(hit.point, 0.5f);
	//      } else
	//      {
	//        Gizmos.color = Color.red;
	//        Gizmos.DrawLine(hoverPoint.transform.position, 
	//                       hoverPoint.transform.position - Vector3.up * hoverHeight);
	//      }
	//    }
	//  }

	void Update()
	{
		// Get thrust input
		thrust = 0.0f;
		float acceleration = Input.GetAxis("Vertical");
		if (acceleration > deadZone)
			thrust = acceleration * forwardAcceleration;
		else if (acceleration < -deadZone)
			thrust = acceleration * reverseAcceleration;

		// Get turning input
		turnValue = 0.0f;
		float turnAxis = Input.GetAxis("Horizontal");
		if (Mathf.Abs(turnAxis) > deadZone)
			turnValue = turnAxis;

		Debug.Log(turnAxis);
		Debug.Log(acceleration);
	}

	void FixedUpdate()
	{
		//add something to make emmission stop if not moving
		var emissionRate = 10;
		body.drag = groundedDrag;

		for (int i = 0; i < dustTrails.Length; i++)
		{
			var emission = dustTrails[i].emission;
			emission.rateOverTime = new ParticleSystem.MinMaxCurve(emissionRate);
		}

		// Handle Forward and Reverse forces
		if (Mathf.Abs(thrust) > 0)
			body.AddForce(transform.forward * thrust);

		// Handle Turn forces
		if (turnValue > 0)
		{
			body.AddRelativeTorque(Vector3.up * turnValue * turnStrength);
		}
		else if (turnValue < 0)
		{
			body.AddRelativeTorque(Vector3.up * turnValue * turnStrength);
		}


		// Limit max velocity
		if (body.velocity.sqrMagnitude > (body.velocity.normalized * maxVelocity).sqrMagnitude)
		{
			body.velocity = body.velocity.normalized * maxVelocity;
		}
	}
}
