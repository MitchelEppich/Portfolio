using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

[System.Serializable]
public class Dot_Truck : System.Object
{
	public WheelCollider leftWheel;
	public GameObject leftWheelMesh;
	public WheelCollider rightWheel;
	public GameObject rightWheelMesh;
	public bool motor;
	public bool steering;
	public bool reverseTurn; 
}

public class Dot_Truck_Controller : NetworkBehaviour {

	public float maxMotorTorque;
	public float maxSteeringAngle;
	public List<Dot_Truck> truck_Infos;
    public Vector3 com;

    private float motor = 0;
    private float steering = 0;
    private float brakeTorque = 0;

    public bool occupied = false;

    /* 
     * This method is taking the position and rotation of the wheel collider and setting those of
     * the wheel mesh to exactly those.
    */
    public void RpcVisualizeWheel(Dot_Truck wheelPair)
	{
		Quaternion rot;
		Vector3 pos;
		wheelPair.leftWheel.GetWorldPose ( out pos, out rot);
		wheelPair.leftWheelMesh.transform.position = pos;
		wheelPair.leftWheelMesh.transform.rotation = rot;
		wheelPair.rightWheel.GetWorldPose ( out pos, out rot);
		wheelPair.rightWheelMesh.transform.position = pos;
		wheelPair.rightWheelMesh.transform.rotation = rot;
	}

    public void Start()
    {
        GetComponent<Rigidbody>().centerOfMass = com;
    }

    public void Move(float dir)
    {
        motor = -maxMotorTorque * dir;
    }

    [ClientRpc]
    public void RpcTurn(float dir)
    {
        steering = maxSteeringAngle * dir;
    }

    public void Turn(float dir)
    {
        RpcTurn(dir);
        steering = maxSteeringAngle * dir;
    }

    public void Brake(float val)
    {
        if (val > 0.001)
        {
            brakeTorque = maxMotorTorque * 1000;
            motor = 0;
        }
        else
        {
            brakeTorque = 0;
        }
    }

    public void Update()
	{

		foreach (Dot_Truck truck_Info in truck_Infos)
		{
			if (truck_Info.steering == true) {
				truck_Info.leftWheel.steerAngle = truck_Info.rightWheel.steerAngle = ((truck_Info.reverseTurn)?-1:1)*steering;
			}

			if (truck_Info.motor == true)
			{
				truck_Info.leftWheel.motorTorque = motor;
				truck_Info.rightWheel.motorTorque = motor;
			}

			truck_Info.leftWheel.brakeTorque = brakeTorque;
			truck_Info.rightWheel.brakeTorque = brakeTorque;

            RpcVisualizeWheel(truck_Info);

		}

	}

    private void OnTriggerStay(Collider other)
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (other.gameObject.tag == "Player" && other.GetComponent<PlayerController>().currentCar == null)
            {
                other.GetComponent<PlayerController>().currentCar = gameObject;
                other.gameObject.transform.parent = gameObject.transform;
                other.GetComponent<Renderer>().enabled = false;
                this.occupied = true;
            }
        }
    }


}