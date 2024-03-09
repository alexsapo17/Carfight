/*
MESSAGE FROM CREATOR: The physics was coded by Mena. You can use it in your games either these are commercial or
personal projects.


*/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class PrometeoCarController : MonoBehaviour,IPunObservable
{

    //CAR SETUP

      [Space(20)]
      //[Header("CAR SETUP")]
      [Space(10)]
      [Range(20, 190)]
      public int maxSpeed = 90; //The maximum speed that the car can reach in km/h. 
      [Range(10, 120)]
      public int maxReverseSpeed = 45; //The maximum speed that the car can reach while going on reverse in km/h.
      [Range(1, 10)]
      public int accelerationMultiplier = 2; // How fast the car can accelerate. 1 is a slow acceleration and 10 is the fastest.
      [Space(10)]
      [Range(10, 45)]
      public int maxSteeringAngle = 27; // The maximum angle that the tires can reach while rotating the steering wheel.
      [Range(0.1f, 1f)]
      public float steeringSpeed = 0.5f; // How fast the steering wheel turns.
      public float TurnLeftXEditor= 1f;
      public float TurnRightXEditor= -1f;
      [Space(10)]
      [Range(100, 600)]
      public int brakeForce = 350; // The strength of the wheel brakes.
      [Range(1, 10)]
      public int decelerationMultiplier = 2; // How fast the car decelerates when the user is not using the throttle.
      [Range(1, 10)]
      public int handbrakeDriftMultiplier = 5; // How much grip the car loses when the user hit the handbrake.
      [Space(10)]
      public Vector3 bodyMassCenter; // This is a vector that contains the center of mass of the car. I recommend to set this value
                                    // in the points x = 0 and z = 0 of your car. You can select the value that you want in the y axis,
                                    // however, you must notice that the higher this value is, the more unstable the car becomes.
                                    // Usually the y value goes from 0 to 1.5.

    //WHEELS

      //[Header("WHEELS")]

      /*
      The following variables are used to store the wheels' data of the car. We need both the mesh-only game objects and wheel
      collider components of the wheels. The wheel collider components and 3D meshes of the wheels cannot come from the same
      game object; they must be separate game objects.
      */
      public GameObject frontLeftMesh;
      public WheelCollider frontLeftCollider;
      [Space(10)]
      public GameObject frontRightMesh;
      public WheelCollider frontRightCollider;
      [Space(10)]
      public GameObject rearLeftMesh;
      public WheelCollider rearLeftCollider;
      [Space(10)]
      public GameObject rearRightMesh;
      public WheelCollider rearRightCollider;
    
public GameObject cameraPrefab;
 


    //PARTICLE SYSTEMS

      [Space(20)]
      //[Header("EFFECTS")]
      [Space(10)]
      //The following variable lets you to set up particle systems in your car
      public bool useEffects = false;

      // The following particle systems are used as tire smoke when the car drifts.
      public ParticleSystem RLWParticleSystem;
      public ParticleSystem RRWParticleSystem;

      [Space(10)]
      // The following trail renderers are used as tire skids when the car loses traction.
      public TrailRenderer RLWTireSkid;
      public TrailRenderer RRWTireSkid;

    //SPEED TEXT (UI)

      [Space(20)]
      //[Header("UI")]
      [Space(10)]
      //The following variable lets you to set up a UI text to display the speed of your car.
      public bool useUI = false;
      public Text carSpeedText; // Used to store the UI object that is going to show the speed of the car.

    //SOUNDS

      [Space(20)]
      //[Header("Sounds")]
      [Space(10)]
      //The following variable lets you to set up sounds for your car such as the car engine or tire screech sounds.
      public bool useSounds = false;
      public AudioSource carEngineSound; // This variable stores the sound of the car engine.
      public AudioSource tireScreechSound; // This variable stores the sound of the tire screech (when the car is drifting).
      float initialCarEngineSoundPitch; // Used to store the initial pitch of the car engine sound.

    //CONTROLS

      [Space(20)]
      //[Header("CONTROLS")]
      [Space(10)]
      //The following variables lets you to set up touch controls for mobile devices.
      public bool useTouchControls = false;
      PrometeoTouchInput throttlePTI;
      PrometeoTouchInput reversePTI;
      PrometeoTouchInput turnRightPTI;
      PrometeoTouchInput turnLeftPTI;
      PrometeoTouchInput handbrakePTI;
      private PhotonView photonView;
      private bool isHandbrakeActive = false;
       private bool isFrictionIncreased = false;
    private WheelFrictionCurve originalFrictionCurve;
   [SerializeField]
private HorizontalJoystick horizontalJoystick;
    private Rigidbody rb;  // Dichiarazione di rb come variabile di classe

    private Vector3 targetPosition;
    private Quaternion targetRotation;

public float positionLerpRate = 15;
public float rotationLerpRate = 10;
public bool applyVelocityPrediction = true;

 public bool controlsEnabled = true; // Variabile per controllare se i controlli sono abilitati

    //CAR DATA

      [HideInInspector]
      public float carSpeed; // Used to store the speed of the car.
      [HideInInspector]
      public bool isDrifting; // Used to know whether the car is drifting or not.
      [HideInInspector]
      public bool isTractionLocked; // Used to know whether the traction of the car is locked or not.

    //PRIVATE VARIABLES

      /*
      IMPORTANT: The following variables should not be modified manually since their values are automatically given via script.
      */
      Rigidbody carRigidbody; // Stores the car's rigidbody.
      float steeringAxis; // Used to know whether the steering wheel has reached the maximum value. It goes from -1 to 1.
      float throttleAxis; // Used to know whether the throttle has reached the maximum value. It goes from -1 to 1.
      float driftingAxis;
      float localVelocityZ;
      float localVelocityX;
      bool deceleratingCar;
      bool touchControlsSetup = false;
      public GameObject cameraInstance;

        private Camera cam;
      /*
      The following variables are used to store information about sideways friction of the wheels (such as
      extremumSlip,extremumValue, asymptoteSlip, asymptoteValue and stiffness). We change this values to
      make the car to start drifting.
      */
      WheelFrictionCurve FLwheelFriction;
      float FLWextremumSlip;
      WheelFrictionCurve FRwheelFriction;
      float FRWextremumSlip;
      WheelFrictionCurve RLwheelFriction;
      float RLWextremumSlip;
      WheelFrictionCurve RRwheelFriction;
      float RRWextremumSlip;




    // Start is called before the first frame update
    void Start()
    {
 controlsEnabled = false;


horizontalJoystick = FindObjectOfType<HorizontalJoystick>();
   
      photonView = GetComponent<PhotonView>();
      //In this part, we set the 'carRigidbody' value with the Rigidbody attached to this
      //gameObject. Also, we define the center of mass of the car with the Vector3 given
      //in the inspector.
      carRigidbody = gameObject.GetComponent<Rigidbody>();
      carRigidbody.centerOfMass = bodyMassCenter;
   rb = GetComponent<Rigidbody>();
        targetPosition = transform.position;
        targetRotation = transform.rotation;
      //Initial setup to calculate the drift value of the car. This part could look a bit
      //complicated, but do not be afraid, the only thing we're doing here is to save the default
      //friction values of the car wheels so we can set an appropiate drifting value later.
      FLwheelFriction = new WheelFrictionCurve ();
        FLwheelFriction.extremumSlip = frontLeftCollider.sidewaysFriction.extremumSlip;
        FLWextremumSlip = frontLeftCollider.sidewaysFriction.extremumSlip;
        FLwheelFriction.extremumValue = frontLeftCollider.sidewaysFriction.extremumValue;
        FLwheelFriction.asymptoteSlip = frontLeftCollider.sidewaysFriction.asymptoteSlip;
        FLwheelFriction.asymptoteValue = frontLeftCollider.sidewaysFriction.asymptoteValue;
        FLwheelFriction.stiffness = frontLeftCollider.sidewaysFriction.stiffness;
      FRwheelFriction = new WheelFrictionCurve ();
        FRwheelFriction.extremumSlip = frontRightCollider.sidewaysFriction.extremumSlip;
        FRWextremumSlip = frontRightCollider.sidewaysFriction.extremumSlip;
        FRwheelFriction.extremumValue = frontRightCollider.sidewaysFriction.extremumValue;
        FRwheelFriction.asymptoteSlip = frontRightCollider.sidewaysFriction.asymptoteSlip;
        FRwheelFriction.asymptoteValue = frontRightCollider.sidewaysFriction.asymptoteValue;
        FRwheelFriction.stiffness = frontRightCollider.sidewaysFriction.stiffness;
      RLwheelFriction = new WheelFrictionCurve ();
        RLwheelFriction.extremumSlip = rearLeftCollider.sidewaysFriction.extremumSlip;
        RLWextremumSlip = rearLeftCollider.sidewaysFriction.extremumSlip;
        RLwheelFriction.extremumValue = rearLeftCollider.sidewaysFriction.extremumValue;
        RLwheelFriction.asymptoteSlip = rearLeftCollider.sidewaysFriction.asymptoteSlip;
        RLwheelFriction.asymptoteValue = rearLeftCollider.sidewaysFriction.asymptoteValue;
        RLwheelFriction.stiffness = rearLeftCollider.sidewaysFriction.stiffness;
      RRwheelFriction = new WheelFrictionCurve ();
        RRwheelFriction.extremumSlip = rearRightCollider.sidewaysFriction.extremumSlip;
        RRWextremumSlip = rearRightCollider.sidewaysFriction.extremumSlip;
        RRwheelFriction.extremumValue = rearRightCollider.sidewaysFriction.extremumValue;
        RRwheelFriction.asymptoteSlip = rearRightCollider.sidewaysFriction.asymptoteSlip;
        RRwheelFriction.asymptoteValue = rearRightCollider.sidewaysFriction.asymptoteValue;
        RRwheelFriction.stiffness = rearRightCollider.sidewaysFriction.stiffness;

        // We save the initial pitch of the car engine sound.
        if(carEngineSound != null){
          initialCarEngineSoundPitch = carEngineSound.pitch;
        }




        // We invoke 2 methods inside this script. CarSpeedUI() changes the text of the UI object that stores
        // the speed of the car and CarSounds() controls the engine and drifting sounds. Both methods are invoked
        // in 0 seconds, and repeatedly called every 0.1 seconds.
    

        if(useSounds){
          InvokeRepeating("CarSounds", 0f, 0.1f);
        }else if(!useSounds){
          if(carEngineSound != null){
            carEngineSound.Stop();
          }
          if(tireScreechSound != null){
            tireScreechSound.Stop();
          }
        }

        if(!useEffects){
          if(RLWParticleSystem != null){
            RLWParticleSystem.Stop();
          }
          if(RRWParticleSystem != null){
            RRWParticleSystem.Stop(); 
          }
          if(RLWTireSkid != null){
            RLWTireSkid.emitting = false;
          }
          if(RRWTireSkid != null){
            RRWTireSkid.emitting = false;
          }
        }

if (useTouchControls)
    {
        // Trova i GameObject dei pulsanti e ottieni i componenti PrometeoTouchInput
        GameObject throttleButton = GameObject.Find("ThrottleButton");
        if (throttleButton != null)
            throttlePTI = throttleButton.GetComponent<PrometeoTouchInput>();

        GameObject reverseButton = GameObject.Find("ReverseButton");
        if (reverseButton != null)
            reversePTI = reverseButton.GetComponent<PrometeoTouchInput>();

        GameObject turnRightButton = GameObject.Find("TurnRightButton");
        if (turnRightButton != null)
            turnRightPTI = turnRightButton.GetComponent<PrometeoTouchInput>();

        GameObject turnLeftButton = GameObject.Find("TurnLeftButton");
        if (turnLeftButton != null)
            turnLeftPTI = turnLeftButton.GetComponent<PrometeoTouchInput>();

        GameObject handbrakeButton = GameObject.Find("HandbrakeButton");
        if (handbrakeButton != null)
            handbrakePTI = handbrakeButton.GetComponent<PrometeoTouchInput>();

        touchControlsSetup = true;
    }
    if (photonView.IsMine)
    {
        // Setup specifico per il proprietario del veicolo
        GameObject cameraInstance = Instantiate(cameraPrefab);
        cameraInstance.GetComponent<CameraFollow>().SetTarget(this.gameObject);
        cam = cameraInstance.GetComponent<Camera>();

        // Abilita l'UI per la velocità solo per il proprietario del veicolo
        if (useUI && carSpeedText != null)
        {
            carSpeedText.gameObject.SetActive(true);
            InvokeRepeating("CarSpeedUI", 0f, 0.1f);
        }

        // Altre configurazioni specifiche per il proprietario...
    }
    else
    {
        // Disabilita l'UI per i giocatori non proprietari
        if (carSpeedText != null)
        {
            carSpeedText.gameObject.SetActive(false);
        }

        // Altre configurazioni per giocatori non proprietari...
    }
    }
public void DestroyCameraInstance() {
    if (cameraInstance != null) {
        Debug.Log("Destroying camera instance");
        Destroy(cameraInstance);
        cameraInstance = null;
    }
    else {
        Debug.Log("Camera instance is null");
    }
}


  void FixedUpdate()
{
   if (!controlsEnabled)
        return;

    if (photonView.IsMine)
    {
          int controlSetup = PlayerPrefs.GetInt("ControlSetup", 1);
      //CAR DATA

      // We determine the speed of the car.
      carSpeed = (2 * Mathf.PI * frontLeftCollider.radius * frontLeftCollider.rpm * 60) / 1000;
   
      // Save the local velocity of the car in the z axis. Used to know if the car is going forward or backwards.
      localVelocityZ = transform.InverseTransformDirection(carRigidbody.velocity).z;
       // Save the local velocity of the car in the x axis. Used to know if the car is drifting.
      localVelocityX = transform.InverseTransformDirection(carRigidbody.velocity).x;
      //CAR PHYSICS


      /*
      The next part is regarding to the car controller. First, it checks if the user wants to use touch controls (for
      mobile devices) or analog input controls (WASD + Space).

      The following methods are called whenever a certain key is pressed. For example, in the first 'if' we call the
      method GoForward() if the user has pressed W.

      In this part of the code we specify what the car needs to do if the user presses W (throttle), S (reverse),
      A (turn left), D (turn right) or Space bar (handbrake).
      */
      if (useTouchControls && touchControlsSetup){


float turnValue = horizontalJoystick.GetHorizontal();
  
 float moveValue = horizontalJoystick.GetVertical();

            

if (turnValue < TurnLeftXEditor) 
{
    TurnLeft(turnValue); // Passa il valore del joystick
}
else if (turnValue > TurnRightXEditor)
{
    TurnRight(turnValue); // Passa il valore del joystick
}
else  
{
    ResetSteeringAngle();
}


            // Se è stato selezionato il Setup 1, disattiva certi controlli
            if (controlSetup == 1)
            {
                throttlePTI.buttonPressed = moveValue > 0.3f;
                reversePTI.buttonPressed = moveValue < -0.3f;
            }

    if (throttlePTI.buttonPressed)
    {
        CancelInvoke("DecelerateCar");
        deceleratingCar = false;
        GoForward();
    }
    else if (reversePTI.buttonPressed)
    {
        CancelInvoke("DecelerateCar");
        deceleratingCar = false;
        GoReverse();
    }
    else if (!deceleratingCar)
    {
        InvokeRepeating("DecelerateCar", 0f, 0.1f);
        deceleratingCar = true;
    }

        if(handbrakePTI.buttonPressed){
          CancelInvoke("DecelerateCar");
          deceleratingCar = false;
          Handbrake();
        }
        if(!handbrakePTI.buttonPressed){
          RecoverTraction();
        }
        if((!throttlePTI.buttonPressed && !reversePTI.buttonPressed)){
          ThrottleOff();
        }
        if((!reversePTI.buttonPressed && !throttlePTI.buttonPressed) && !handbrakePTI.buttonPressed && !deceleratingCar){
          InvokeRepeating("DecelerateCar", 0f, 0.1f);
          deceleratingCar = true;
        }


      }else{

        if(Input.GetKey(KeyCode.W)){
          CancelInvoke("DecelerateCar");
          deceleratingCar = false;
          GoForward();
        }
        if(Input.GetKey(KeyCode.S)){
          CancelInvoke("DecelerateCar");
          deceleratingCar = false;
          GoReverse();
        }

  if(Input.GetKey(KeyCode.A)){
  TurnLeft(-1f); // Passa un valore fisso
}
if(Input.GetKey(KeyCode.D)){
  TurnRight(1f); // Passa un valore fisso
}


        if((!Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.W))){
          ThrottleOff();
        }
        if((!Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.W)) && !Input.GetKey(KeyCode.Space) && !deceleratingCar){
          InvokeRepeating("DecelerateCar", 0f, 0.1f);
          deceleratingCar = true;
        }
        if(!Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D) && steeringAxis != 0f){
          ResetSteeringAngle();
        }

      }


      // We call the method AnimateWheelMeshes() in order to match the wheel collider movements with the 3D meshes of the wheels.
      AnimateWheelMeshes();

    }
        if (!photonView.IsMine) {
        // Interpola verso la posizione e rotazione target ricevute dalla rete
        rb.position = Vector3.Lerp(rb.position, targetPosition, Time.fixedDeltaTime * positionLerpRate);
        rb.rotation = Quaternion.Lerp(rb.rotation, targetRotation, Time.fixedDeltaTime * rotationLerpRate);
    }

}
void Update()
{
    if (photonView.IsMine)
    {
        HandleHandbrakeInput();
    }

        if (!photonView.IsMine)
    {
        Debug.DrawLine(transform.position, targetPosition, Color.blue); // Disegna una linea blu
                if (Time.frameCount % 60 == 0) 
        {
            Debug.Log("Target Position: " + targetPosition + ", Current Position: " + transform.position);
        }
    }
}

    // Se vuoi, puoi anche aggiungere un metodo per verificare lo stato dei controlli
    public bool AreControlsEnabled()
    {
        return controlsEnabled;
    }

public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
    if (stream.IsWriting) {
        // Invia i dati locali agli altri giocatori
        stream.SendNext(rb.position);
        stream.SendNext(rb.rotation);
        stream.SendNext(rb.velocity);
        stream.SendNext(rb.angularVelocity);
    } else {
        // Log per mostrare la targetPosition e la current position prima di ricevere l'aggiornamento
        Debug.Log($"[Ricezione] Prima dell'aggiornamento - Target Position: {targetPosition}, Current Position: {rb.position}");

        // Riceve i dati dagli altri giocatori
        Vector3 receivedTargetPosition = (Vector3)stream.ReceiveNext();
        Quaternion receivedTargetRotation = (Quaternion)stream.ReceiveNext();
        Vector3 receivedVelocity = (Vector3)stream.ReceiveNext();
        Vector3 receivedAngularVelocity = (Vector3)stream.ReceiveNext();

        // Aggiustamento per la latenza di rete
        float lag = Mathf.Abs((float)(PhotonNetwork.Time - info.SentServerTime));
        Vector3 adjustedPosition = receivedTargetPosition + (receivedVelocity * lag);

        // Log per mostrare la targetPosition ricevuta e dopo l'aggiustamento per la latenza
        Debug.Log($"[Ricezione] Dati ricevuti - Received Target Position: {receivedTargetPosition}, Adjusted Position with lag: {adjustedPosition}");

        // Aggiorna targetPosition e targetRotation con i nuovi valori ricevuti e aggiustati
        targetPosition = adjustedPosition;
        targetRotation = receivedTargetRotation;

        // Applica la velocità e la velocità angolare per predire il movimento, se necessario
        if (applyVelocityPrediction) {
            rb.velocity = receivedVelocity;
            rb.angularVelocity = receivedAngularVelocity;
        }

        // Log finale per confermare l'aggiornamento della targetPosition
        Debug.Log($"[Ricezione] Dopo l'aggiornamento - Target Position aggiornata: {targetPosition}, Current Position: {rb.position}");
    }
}





    public void SetKinematic(bool isKinematic)
    {
        carRigidbody.isKinematic = isKinematic;
    }
void HandleHandbrakeInput()
{
    if(Input.GetKey(KeyCode.Space) && !isHandbrakeActive)
    {
        isHandbrakeActive = true;
        Handbrake();
    }
    else if(Input.GetKeyUp(KeyCode.Space))
    {
        isHandbrakeActive = false;
        RecoverTraction();
    }
}
// Metodo per abilitare i controlli del veicolo
public void EnableControls()
{
    controlsEnabled = true;
            Debug.Log("Controlli abilitati nel PrometeoCarController.");

}

// Metodo per disabilitare i controlli del veicolo
public void DisableControls()
{
    controlsEnabled = false;
    StopCar();
            Debug.Log("Controlli disabilitati nel PrometeoCarController."); 

}

private void StopCar()
{
    frontLeftCollider.motorTorque = 0;
    frontRightCollider.motorTorque = 0;
    rearLeftCollider.motorTorque = 0;
    rearRightCollider.motorTorque = 0;

    frontLeftCollider.brakeTorque = brakeForce;
    frontRightCollider.brakeTorque = brakeForce;
    rearLeftCollider.brakeTorque = brakeForce;
    rearRightCollider.brakeTorque = brakeForce;
}
    // This method converts the car speed data from float to string, and then set the text of the UI carSpeedText with this value.
    public void CarSpeedUI(){

      if(useUI){
          try{
            float absoluteCarSpeed = Mathf.Abs(carSpeed);
            carSpeedText.text = Mathf.RoundToInt(absoluteCarSpeed).ToString();
          }catch(Exception ex){
            Debug.LogWarning(ex);
          }
      }

    }

    // This method controls the car sounds. For example, the car engine will sound slow when the car speed is low because the
    // pitch of the sound will be at its lowest point. On the other hand, it will sound fast when the car speed is high because
    // the pitch of the sound will be the sum of the initial pitch + the car speed divided by 100f.
    // Apart from that, the tireScreechSound will play whenever the car starts drifting or losing traction.
    public void CarSounds(){

      if(useSounds){
        try{
          if(carEngineSound != null){
            float engineSoundPitch = initialCarEngineSoundPitch + (Mathf.Abs(carRigidbody.velocity.magnitude) / 25f);
            carEngineSound.pitch = engineSoundPitch;
          }
          if((isDrifting) || (isTractionLocked && Mathf.Abs(carSpeed) > 12f)){
            if(!tireScreechSound.isPlaying){
              tireScreechSound.Play();
            }
          }else if((!isDrifting) && (!isTractionLocked || Mathf.Abs(carSpeed) < 12f)){
            tireScreechSound.Stop();
          }
        }catch(Exception ex){
          Debug.LogWarning(ex);
        }
      }else if(!useSounds){
        if(carEngineSound != null && carEngineSound.isPlaying){
          carEngineSound.Stop();
        }
        if(tireScreechSound != null && tireScreechSound.isPlaying){
          tireScreechSound.Stop();
        }
      }

    }

    //
    //STEERING METHODS
    //

    //The following method turns the front car wheels to the left. The speed of this movement will depend on the steeringSpeed variable.
    public void TurnLeft(float joystickValue){
    steeringAxis = Mathf.Max(steeringAxis - (Time.deltaTime * 10f * steeringSpeed), joystickValue);
    ApplySteering();
}

public void TurnRight(float joystickValue){
    steeringAxis = Mathf.Min(steeringAxis + (Time.deltaTime * 10f * steeringSpeed), joystickValue);
    ApplySteering();
}
private void ApplySteering() {
    if(steeringAxis < -1f){
        steeringAxis = -1f;
    } else if(steeringAxis > 1f){
        steeringAxis = 1f;
    }

    var steeringAngle = steeringAxis * maxSteeringAngle;
    frontLeftCollider.steerAngle = Mathf.Lerp(frontLeftCollider.steerAngle, steeringAngle, steeringSpeed);
    frontRightCollider.steerAngle = Mathf.Lerp(frontRightCollider.steerAngle, steeringAngle, steeringSpeed);
}


 public void ResetSteeringAngle(){
    
        steeringAxis = Mathf.MoveTowards(steeringAxis, 0f, Time.deltaTime * 10f * steeringSpeed);
        ApplySteering();
    
}




    // This method matches both the position and rotation of the WheelColliders with the WheelMeshes.
    void AnimateWheelMeshes(){
      try{
        Quaternion FLWRotation;
        Vector3 FLWPosition;
        frontLeftCollider.GetWorldPose(out FLWPosition, out FLWRotation);
        frontLeftMesh.transform.position = FLWPosition;
        frontLeftMesh.transform.rotation = FLWRotation;

        Quaternion FRWRotation;
        Vector3 FRWPosition;
        frontRightCollider.GetWorldPose(out FRWPosition, out FRWRotation);
        frontRightMesh.transform.position = FRWPosition;
        frontRightMesh.transform.rotation = FRWRotation;

        Quaternion RLWRotation;
        Vector3 RLWPosition;
        rearLeftCollider.GetWorldPose(out RLWPosition, out RLWRotation);
        rearLeftMesh.transform.position = RLWPosition;
        rearLeftMesh.transform.rotation = RLWRotation;

        Quaternion RRWRotation;
        Vector3 RRWPosition;
        rearRightCollider.GetWorldPose(out RRWPosition, out RRWRotation);
        rearRightMesh.transform.position = RRWPosition;
        rearRightMesh.transform.rotation = RRWRotation;
      }catch(Exception ex){
        Debug.LogWarning(ex);
      }
    }

    //
    //ENGINE AND BRAKING METHODS
    //

    // This method apply positive torque to the wheels in order to go forward.
    public void GoForward(){
      //If the forces aplied to the rigidbody in the 'x' asis are greater than
      //3f, it means that the car is losing traction, then the car will start emitting particle systems.
      if(Mathf.Abs(localVelocityX) > 2.5f){
        isDrifting = true;
        DriftCarPS();
      }else{
        isDrifting = false;
        DriftCarPS();
      }
      // The following part sets the throttle power to 1 smoothly.
      throttleAxis = throttleAxis + (Time.deltaTime * 3f);
      if(throttleAxis > 1f){
        throttleAxis = 1f;
      }
      //If the car is going backwards, then apply brakes in order to avoid strange
      //behaviours. If the local velocity in the 'z' axis is less than -1f, then it
      //is safe to apply positive torque to go forward.
      if(localVelocityZ < -1f){
        Brakes();
      }else{
        if(Mathf.RoundToInt(carSpeed) < maxSpeed){
          //Apply positive torque in all wheels to go forward if maxSpeed has not been reached.
          frontLeftCollider.brakeTorque = 0;
          frontLeftCollider.motorTorque = (accelerationMultiplier * 50f) * throttleAxis;
          frontRightCollider.brakeTorque = 0;
          frontRightCollider.motorTorque = (accelerationMultiplier * 50f) * throttleAxis;
          rearLeftCollider.brakeTorque = 0;
          rearLeftCollider.motorTorque = (accelerationMultiplier * 50f) * throttleAxis;
          rearRightCollider.brakeTorque = 0;
          rearRightCollider.motorTorque = (accelerationMultiplier * 50f) * throttleAxis;
        }else {
          // If the maxSpeed has been reached, then stop applying torque to the wheels.
          // IMPORTANT: The maxSpeed variable should be considered as an approximation; the speed of the car
          // could be a bit higher than expected.
    			frontLeftCollider.motorTorque = 0;
    			frontRightCollider.motorTorque = 0;
          rearLeftCollider.motorTorque = 0;
    			rearRightCollider.motorTorque = 0;
    		}
      }
    }

    // This method apply negative torque to the wheels in order to go backwards.
    public void GoReverse(){
      //If the forces aplied to the rigidbody in the 'x' asis are greater than
      //3f, it means that the car is losing traction, then the car will start emitting particle systems.
      if(Mathf.Abs(localVelocityX) > 2.5f){
        isDrifting = true;
        DriftCarPS();
      }else{
        isDrifting = false;
        DriftCarPS();
      }
      // The following part sets the throttle power to -1 smoothly.
      throttleAxis = throttleAxis - (Time.deltaTime * 3f);
      if(throttleAxis < -1f){
        throttleAxis = -1f;
      }
      //If the car is still going forward, then apply brakes in order to avoid strange
      //behaviours. If the local velocity in the 'z' axis is greater than 1f, then it
      //is safe to apply negative torque to go reverse.
      if(localVelocityZ > 1f){
        Brakes();
      }else{
        if(Mathf.Abs(Mathf.RoundToInt(carSpeed)) < maxReverseSpeed){
          //Apply negative torque in all wheels to go in reverse if maxReverseSpeed has not been reached.
          frontLeftCollider.brakeTorque = 0;
          frontLeftCollider.motorTorque = (accelerationMultiplier * 50f) * throttleAxis;
          frontRightCollider.brakeTorque = 0;
          frontRightCollider.motorTorque = (accelerationMultiplier * 50f) * throttleAxis;
          rearLeftCollider.brakeTorque = 0;
          rearLeftCollider.motorTorque = (accelerationMultiplier * 50f) * throttleAxis;
          rearRightCollider.brakeTorque = 0;
          rearRightCollider.motorTorque = (accelerationMultiplier * 50f) * throttleAxis;
        }else {
          //If the maxReverseSpeed has been reached, then stop applying torque to the wheels.
          // IMPORTANT: The maxReverseSpeed variable should be considered as an approximation; the speed of the car
          // could be a bit higher than expected.
    			frontLeftCollider.motorTorque = 0;
    			frontRightCollider.motorTorque = 0;
          rearLeftCollider.motorTorque = 0;
    			rearRightCollider.motorTorque = 0;
    		}
      }
    }
public void IncreaseTireFriction(float multiplier)
{
    // Assicurati di aver memorizzato l'attrito originale prima di modificarlo
    if (!isFrictionIncreased)
    {
        originalFrictionCurve = frontLeftCollider.sidewaysFriction;
        isFrictionIncreased = true;
    }

    // Applica l'aumento dell'attrito a ogni collider della ruota
    IncreaseFriction(frontLeftCollider, multiplier);
    IncreaseFriction(frontRightCollider, multiplier);
    IncreaseFriction(rearLeftCollider, multiplier);
    IncreaseFriction(rearRightCollider, multiplier);
}

private void IncreaseFriction(WheelCollider collider, float multiplier)
{
    var frictionCurve = collider.sidewaysFriction;
    frictionCurve.stiffness *= multiplier;
    collider.sidewaysFriction = frictionCurve;
}
public void ResetTireFriction()
{
    if (isFrictionIncreased)
    {
        // Ripristina l'attrito a quello originale
        ResetFriction(frontLeftCollider);
        ResetFriction(frontRightCollider);
        ResetFriction(rearLeftCollider);
        ResetFriction(rearRightCollider);
        isFrictionIncreased = false;
    }
}

private void ResetFriction(WheelCollider collider)
{
    collider.sidewaysFriction = originalFrictionCurve;
}


    //The following function set the motor torque to 0 (in case the user is not pressing either W or S).
    public void ThrottleOff(){
      frontLeftCollider.motorTorque = 0;
      frontRightCollider.motorTorque = 0;
      rearLeftCollider.motorTorque = 0;
      rearRightCollider.motorTorque = 0;
    }

    // The following method decelerates the speed of the car according to the decelerationMultiplier variable, where
    // 1 is the slowest and 10 is the fastest deceleration. This method is called by the function InvokeRepeating,
    // usually every 0.1f when the user is not pressing W (throttle), S (reverse) or Space bar (handbrake).
    public void DecelerateCar(){
      if(Mathf.Abs(localVelocityX) > 2.5f){
        isDrifting = true;
        DriftCarPS();
      }else{
        isDrifting = false;
        DriftCarPS();
      }
      // The following part resets the throttle power to 0 smoothly.
      if(throttleAxis != 0f){
        if(throttleAxis > 0f){
          throttleAxis = throttleAxis - (Time.deltaTime * 10f);
        }else if(throttleAxis < 0f){
            throttleAxis = throttleAxis + (Time.deltaTime * 10f);
        }
        if(Mathf.Abs(throttleAxis) < 0.15f){
          throttleAxis = 0f;
        }
      }
      carRigidbody.velocity = carRigidbody.velocity * (1f / (1f + (0.025f * decelerationMultiplier)));
      // Since we want to decelerate the car, we are going to remove the torque from the wheels of the car.
      frontLeftCollider.motorTorque = 0;
      frontRightCollider.motorTorque = 0;
      rearLeftCollider.motorTorque = 0;
      rearRightCollider.motorTorque = 0;
      // If the magnitude of the car's velocity is less than 0.25f (very slow velocity), then stop the car completely and
      // also cancel the invoke of this method.
      if(carRigidbody.velocity.magnitude < 0.25f){
        carRigidbody.velocity = Vector3.zero;
        CancelInvoke("DecelerateCar");
      }
    }
public void GoForwardJoystick(float joystickValue){
      float sensitivityMultiplier = 5f; // Regola questo valore per aumentare la sensibilità

    if(Mathf.Abs(localVelocityX) > 2.5f){
        isDrifting = true;
        DriftCarPS();
    } else {
        isDrifting = false;
        DriftCarPS();
    }

    // Regola la potenza dell'acceleratore in base al valore del joystick
    throttleAxis = Mathf.Clamp(throttleAxis + (Time.deltaTime * sensitivityMultiplier * joystickValue), 0f, 1f);

    if(localVelocityZ < -1f){
        Brakes();
    } else {
        if(Mathf.RoundToInt(carSpeed) < maxSpeed){
            frontLeftCollider.brakeTorque = 0;
            frontLeftCollider.motorTorque = (accelerationMultiplier * 50f) * throttleAxis;
            frontRightCollider.brakeTorque = 0;
            frontRightCollider.motorTorque = (accelerationMultiplier * 50f) * throttleAxis;
            rearLeftCollider.brakeTorque = 0;
            rearLeftCollider.motorTorque = (accelerationMultiplier * 50f) * throttleAxis;
            rearRightCollider.brakeTorque = 0;
            rearRightCollider.motorTorque = (accelerationMultiplier * 50f) * throttleAxis;
        } else {
            frontLeftCollider.motorTorque = 0;
            frontRightCollider.motorTorque = 0;
            rearLeftCollider.motorTorque = 0;
            rearRightCollider.motorTorque = 0;
        }
    }
}
public void GoReverseJoystick(float joystickValue){
      float sensitivityMultiplier = 5f; // Regola questo valore per aumentare la sensibilità

    if(Mathf.Abs(localVelocityX) > 2.5f){
        isDrifting = true;
        DriftCarPS();
    } else {
        isDrifting = false;
        DriftCarPS();
    }

    // Regola la potenza dell'acceleratore in base al valore del joystick
    throttleAxis = Mathf.Clamp(throttleAxis - (Time.deltaTime * sensitivityMultiplier * Mathf.Abs(joystickValue)), -1f, 0f);

    if(localVelocityZ > 1f){
        Brakes();
    } else {
        if(Mathf.Abs(Mathf.RoundToInt(carSpeed)) < maxReverseSpeed){
            frontLeftCollider.brakeTorque = 0;
            frontLeftCollider.motorTorque = (accelerationMultiplier * 50f) * throttleAxis;
            frontRightCollider.brakeTorque = 0;
            frontRightCollider.motorTorque = (accelerationMultiplier * 50f) * throttleAxis;
            rearLeftCollider.brakeTorque = 0;
            rearLeftCollider.motorTorque = (accelerationMultiplier * 50f) * throttleAxis;
            rearRightCollider.brakeTorque = 0;
            rearRightCollider.motorTorque = (accelerationMultiplier * 50f) * throttleAxis;
        } else {
            frontLeftCollider.motorTorque = 0;
            frontRightCollider.motorTorque = 0;
            rearLeftCollider.motorTorque = 0;
            rearRightCollider.motorTorque = 0;
        }
    }
}

    // This function applies brake torque to the wheels according to the brake force given by the user.
    public void Brakes(){
      frontLeftCollider.brakeTorque = brakeForce;
      frontRightCollider.brakeTorque = brakeForce;
      rearLeftCollider.brakeTorque = brakeForce;
      rearRightCollider.brakeTorque = brakeForce;
    }

    // This function is used to make the car lose traction. By using this, the car will start drifting. The amount of traction lost
    // will depend on the handbrakeDriftMultiplier variable. If this value is small, then the car will not drift too much, but if
    // it is high, then you could make the car to feel like going on ice.
    public void Handbrake(){
      CancelInvoke("RecoverTraction");
      // We are going to start losing traction smoothly, there is were our 'driftingAxis' variable takes
      // place. This variable will start from 0 and will reach a top value of 1, which means that the maximum
      // drifting value has been reached. It will increase smoothly by using the variable Time.deltaTime.
      driftingAxis = driftingAxis + (Time.deltaTime);
      float secureStartingPoint = driftingAxis * FLWextremumSlip * handbrakeDriftMultiplier;

      if(secureStartingPoint < FLWextremumSlip){
        driftingAxis = FLWextremumSlip / (FLWextremumSlip * handbrakeDriftMultiplier);
      }
      if(driftingAxis > 1f){
        driftingAxis = 1f;
      }
      //If the forces aplied to the rigidbody in the 'x' asis are greater than
      //3f, it means that the car lost its traction, then the car will start emitting particle systems.
      if(Mathf.Abs(localVelocityX) > 2.5f){
        isDrifting = true;
      }else{
        isDrifting = false;
      }
      //If the 'driftingAxis' value is not 1f, it means that the wheels have not reach their maximum drifting
      //value, so, we are going to continue increasing the sideways friction of the wheels until driftingAxis
      // = 1f.
      if(driftingAxis < 1f){
        FLwheelFriction.extremumSlip = FLWextremumSlip * handbrakeDriftMultiplier * driftingAxis;
        frontLeftCollider.sidewaysFriction = FLwheelFriction;

        FRwheelFriction.extremumSlip = FRWextremumSlip * handbrakeDriftMultiplier * driftingAxis;
        frontRightCollider.sidewaysFriction = FRwheelFriction;

        RLwheelFriction.extremumSlip = RLWextremumSlip * handbrakeDriftMultiplier * driftingAxis;
        rearLeftCollider.sidewaysFriction = RLwheelFriction;

        RRwheelFriction.extremumSlip = RRWextremumSlip * handbrakeDriftMultiplier * driftingAxis;
        rearRightCollider.sidewaysFriction = RRwheelFriction;
      }


      // Whenever the player uses the handbrake, it means that the wheels are locked, so we set 'isTractionLocked = true'
      // and, as a consequense, the car starts to emit trails to simulate the wheel skids.
      isTractionLocked = true;
      DriftCarPS();


    }

    // This function is used to emit both the particle systems of the tires' smoke and the trail renderers of the tire skids
    // depending on the value of the bool variables 'isDrifting' and 'isTractionLocked'.
    public void DriftCarPS(){

      if(useEffects){
        try{
          if(isDrifting){
            RLWParticleSystem.Play();
            RRWParticleSystem.Play();
          }else if(!isDrifting){
            RLWParticleSystem.Stop();
            RRWParticleSystem.Stop();
          }
        }catch(Exception ex){
          Debug.LogWarning(ex);
        }

        try{
          if((isTractionLocked || Mathf.Abs(localVelocityX) > 5f) && Mathf.Abs(carSpeed) > 12f){
            RLWTireSkid.emitting = true;
            RRWTireSkid.emitting = true;
          }else {
            RLWTireSkid.emitting = false;
            RRWTireSkid.emitting = false;
          }
        }catch(Exception ex){
          Debug.LogWarning(ex);
        }
      }else if(!useEffects){
        if(RLWParticleSystem != null){
          RLWParticleSystem.Stop();
        }
        if(RRWParticleSystem != null){
          RRWParticleSystem.Stop();
        }
        if(RLWTireSkid != null){
          RLWTireSkid.emitting = false;
        }
        if(RRWTireSkid != null){
          RRWTireSkid.emitting = false;
        }
      }

    }

    // This function is used to recover the traction of the car when the user has stopped using the car's handbrake.
    public void RecoverTraction(){

      isTractionLocked = false;
      isDrifting = false;
      driftingAxis = driftingAxis - (Time.deltaTime / 1.5f);
      if(driftingAxis < 0f){
        driftingAxis = 0f;
      }

      //If the 'driftingAxis' value is not 0f, it means that the wheels have not recovered their traction.
      //We are going to continue decreasing the sideways friction of the wheels until we reach the initial
      // car's grip.
      if(FLwheelFriction.extremumSlip > FLWextremumSlip){
        FLwheelFriction.extremumSlip = FLWextremumSlip * handbrakeDriftMultiplier * driftingAxis;
        frontLeftCollider.sidewaysFriction = FLwheelFriction;

        FRwheelFriction.extremumSlip = FRWextremumSlip * handbrakeDriftMultiplier * driftingAxis;
        frontRightCollider.sidewaysFriction = FRwheelFriction;

        RLwheelFriction.extremumSlip = RLWextremumSlip * handbrakeDriftMultiplier * driftingAxis;
        rearLeftCollider.sidewaysFriction = RLwheelFriction;

        RRwheelFriction.extremumSlip = RRWextremumSlip * handbrakeDriftMultiplier * driftingAxis;
        rearRightCollider.sidewaysFriction = RRwheelFriction;

        Invoke("RecoverTraction", Time.deltaTime);

      }else if (FLwheelFriction.extremumSlip < FLWextremumSlip){
        FLwheelFriction.extremumSlip = FLWextremumSlip;
        frontLeftCollider.sidewaysFriction = FLwheelFriction;

        FRwheelFriction.extremumSlip = FRWextremumSlip;
        frontRightCollider.sidewaysFriction = FRwheelFriction;

        RLwheelFriction.extremumSlip = RLWextremumSlip;
        rearLeftCollider.sidewaysFriction = RLwheelFriction;

        RRwheelFriction.extremumSlip = RRWextremumSlip;
        rearRightCollider.sidewaysFriction = RRwheelFriction;

        driftingAxis = 0f;
      }


    }

}
