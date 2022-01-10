using UnityEngine;

public class VehicleController : MonoBehaviour {

    private void Awake() {
        carRigidbody = GetComponent<Rigidbody>();
    }

    private void Update() {
        //// Subtract from y position if isGrounded = false;
        if (forwardAmount > 0) // Accelerating
            speed += forwardAmount * acceleration * Time.deltaTime;
        if (forwardAmount < 0) {
            if (speed > 0) // Braking
                speed += forwardAmount * brakeSpeed * Time.deltaTime;
            else // Reversing
                speed += forwardAmount * reverseSpeed * Time.deltaTime;
        }

        if (forwardAmount == 0) {
            // Not accelerating or braking
            if (speed > 0) speed -= idleSlowdown * Time.deltaTime;
            if (speed < 0) speed += idleSlowdown * Time.deltaTime;
        }

        speed = Mathf.Clamp(speed, speedMin, speedMax);

        carRigidbody.velocity = transform.forward * speed;

        if (speed < 0) // Going backwards, invert wheels
            turnAmount = turnAmount * -1f;

        if (turnAmount > 0 || turnAmount < 0) {
            // Turning
            if (turnSpeed > 0 && turnAmount < 0 || turnSpeed < 0 && turnAmount > 0) {
                // Changing turn direction
                var minTurnAmount = 20f;
                turnSpeed = turnAmount * minTurnAmount;
            }

            turnSpeed += turnAmount * turnSpeedAcceleration * Time.deltaTime;
        }
        else {
            // Not turning
            if (turnSpeed > 0) turnSpeed -= turnIdleSlowdown * Time.deltaTime;
            if (turnSpeed < 0) turnSpeed += turnIdleSlowdown * Time.deltaTime;
            if (turnSpeed > -1f && turnSpeed < +1f) // Stop rotating
                turnSpeed = 0f;
        }

        var speedNormalized = speed / speedMax;
        var invertSpeedNormalized = Mathf.Clamp(1 - speedNormalized, .75f, 1f);

        turnSpeed = Mathf.Clamp(turnSpeed, -turnSpeedMax, turnSpeedMax);

        carRigidbody.angularVelocity = new Vector3(0, turnSpeed * (invertSpeedNormalized * 1f) * Mathf.Deg2Rad, 0);

        if (transform.eulerAngles.x > 2 || transform.eulerAngles.x < -2 || transform.eulerAngles.z > 2 ||
            transform.eulerAngles.z < -2) transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
    }

    private void FixedUpdate() {
        GroundCheckAndSetEnabled();
    }

    public void GroundCheckAndSetEnabled() {
        if (gameObject.GetComponent<VehiclePointsFollower>()) {
            if (Physics.Raycast(transform.position, Vector3.down, distToGround)) {
                isGrounded = true;
                gameObject.GetComponent<VehiclePointsFollower>().enabled = true;
            }
            else {
                isGrounded = false;
                // this.gameObject.GetComponent<Rigidbody>().AddForce(transform.up * -1000);
                gameObject.transform.position = new Vector3(transform.position.x,
                    transform.position.y - Time.deltaTime * fallSpeed, transform.position.z);
                gameObject.GetComponent<VehiclePointsFollower>().enabled = false;
            }
        }
    }

    // private void OnCollisionEnter(Collision collision) {
    //     if (collision.gameObject.layer == GameHandler.SOLID_OBJECTS_LAYER) {
    //         speed = Mathf.Clamp(speed, 0f, 20f);
    //     }
    // }

    public void SetInputs(float forwardAmount, float turnAmount) {
        this.forwardAmount = forwardAmount;
        this.turnAmount = turnAmount;
    }

    public void ClearTurnSpeed() {
        turnSpeed = 0f;
    }

    public float GetSpeed() {
        return speed;
    }

    public void SetSpeedMax(float speedMax) {
        this.speedMax = speedMax;
    }

    public void SetTurnSpeedMax(float turnSpeedMax) {
        this.turnSpeedMax = turnSpeedMax;
    }

    public void SetTurnSpeedAcceleration(float turnSpeedAcceleration) {
        this.turnSpeedAcceleration = turnSpeedAcceleration;
    }

    public void StopCompletely() {
        speed = 0f;
        turnSpeed = 0f;
    }
    
    private readonly float distToGround = 0.05f;
    public bool isGrounded;
    public float fallSpeed = 10;
    public float speed;
    public float speedMax = 70f;
    public float speedMin; // -50f;
    public float acceleration = 30f;
    public float brakeSpeed = 100f;
    public float reverseSpeed = 30f;
    public float idleSlowdown = 10f;
    public float turnSpeed;
    public float turnSpeedMax = 900f; // 300f;
    public float turnSpeedAcceleration = 900f; // = 300f;
    public float turnIdleSlowdown = 500f;
    public float forwardAmount;
    public float turnAmount;
    private Rigidbody carRigidbody;
}

public class VehicleController2 : MonoBehaviour {

    private void Awake() {
        carRigidbody = GetComponent<Rigidbody>();
    }

    private void Update() {
        if (forwardAmount > 0) // Accelerating
            speed += forwardAmount * acceleration * Time.deltaTime;
        if (forwardAmount < 0) {
            if (speed > 0) // Braking
                speed += forwardAmount * brakeSpeed * Time.deltaTime;
            else // Reversing
                speed += forwardAmount * reverseSpeed * Time.deltaTime;
        }

        if (forwardAmount == 0) {
            // Not accelerating or braking
            if (speed > 0) speed -= idleSlowdown * Time.deltaTime;
            if (speed < 0) speed += idleSlowdown * Time.deltaTime;
        }

        speed = Mathf.Clamp(speed, speedMin, speedMax);

        carRigidbody.velocity = transform.forward * speed;

        if (speed < 0) // Going backwards, invert wheels
            turnAmount = turnAmount * -1f;

        if (turnAmount > 0 || turnAmount < 0) {
            // Turning
            if (turnSpeed > 0 && turnAmount < 0 || turnSpeed < 0 && turnAmount > 0) {
                // Changing turn direction
                var minTurnAmount = 20f;
                turnSpeed = turnAmount * minTurnAmount;
            }

            turnSpeed += turnAmount * turnSpeedAcceleration * Time.deltaTime;
        }
        else {
            // Not turning
            if (turnSpeed > 0) turnSpeed -= turnIdleSlowdown * Time.deltaTime;
            if (turnSpeed < 0) turnSpeed += turnIdleSlowdown * Time.deltaTime;
            if (turnSpeed > -1f && turnSpeed < +1f) // Stop rotating
                turnSpeed = 0f;
        }

        var speedNormalized = speed / speedMax;
        var invertSpeedNormalized = Mathf.Clamp(1 - speedNormalized, .75f, 1f);

        turnSpeed = Mathf.Clamp(turnSpeed, -turnSpeedMax, turnSpeedMax);

        carRigidbody.angularVelocity = new Vector3(0, turnSpeed * (invertSpeedNormalized * 1f) * Mathf.Deg2Rad, 0);

        if (transform.eulerAngles.x > 2 || transform.eulerAngles.x < -2 || transform.eulerAngles.z > 2 ||
            transform.eulerAngles.z < -2) transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
    }

    private void FixedUpdate() {
        GroundCheckAndSetEnabled();
    }

    public void GroundCheckAndSetEnabled() {
        if (gameObject.GetComponent<VehiclePointsFollower>()) {
            if (Physics.Raycast(transform.position, Vector3.down, distToGround)) {
                isGrounded = true;
                gameObject.GetComponent<VehiclePointsFollower>().enabled = true;
            }
            else {
                isGrounded = false;
                gameObject.transform.position = new Vector3(transform.position.x,
                    transform.position.y - Time.deltaTime * fallSpeed, transform.position.z);
                gameObject.GetComponent<VehiclePointsFollower>().enabled = false;
            }
        }
    }

    // private void OnCollisionEnter(Collision collision) {
    //     if (collision.gameObject.layer == GameHandler.SOLID_OBJECTS_LAYER) {
    //         speed = Mathf.Clamp(speed, 0f, 20f);
    //     }
    // }

    public void SetInputs(float forwardAmount, float turnAmount) {
        this.forwardAmount = forwardAmount;
        this.turnAmount = turnAmount;
    }

    public void ClearTurnSpeed() {
        turnSpeed = 0f;
    }

    public float GetSpeed() {
        return speed;
    }

    public void SetSpeedMax(float speedMax) {
        this.speedMax = speedMax;
    }

    public void SetTurnSpeedMax(float turnSpeedMax) {
        this.turnSpeedMax = turnSpeedMax;
    }

    public void SetTurnSpeedAcceleration(float turnSpeedAcceleration) {
        this.turnSpeedAcceleration = turnSpeedAcceleration;
    }

    public void StopCompletely() {
        speed = 0f;
        turnSpeed = 0f;
    }

    private readonly float distToGround = 0.05f;
    public bool isGrounded;
    public float fallSpeed = 10;
    public float speed;
    public float speedMax = 70f;
    public float speedMin; // -50f;
    public float acceleration = 30f;
    public float brakeSpeed = 100f;
    public float reverseSpeed = 30f;
    public float idleSlowdown = 10f;
    public float turnSpeed;
    public float turnSpeedMax = 900f; // 300f;
    public float turnSpeedAcceleration = 900f; // = 300f;
    public float turnIdleSlowdown = 500f;
    public float forwardAmount;
    public float turnAmount;
    private Rigidbody carRigidbody;

}