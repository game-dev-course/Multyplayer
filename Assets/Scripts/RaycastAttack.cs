
using Fusion;
using UnityEngine;
using UnityEngine.InputSystem;

// from Fusion tutorial: https://doc.photonengine.com/fusion/current/tutorials/shared-mode-basics/5-remote-procedure-calls
public class RaycastAttack : NetworkBehaviour {
    [SerializeField] int Damage;

    [SerializeField] InputAction attack;
    [SerializeField] InputAction attackLocation;
    [SerializeField] private int killBonus = 1;
    [SerializeField] float shootDistance = 10f;
    
    public bool HasShield { get; set; }
    
    private void OnEnable() { attack.Enable(); attackLocation.Enable();  }
    private void OnDisable() { attack.Disable(); attackLocation.Disable(); }
    void OnValidate() {
        // Provide default bindings for the input actions. Based on answer by DMGregory: https://gamedev.stackexchange.com/a/205345/18261
        if (attack == null)
            attack = new InputAction(type: InputActionType.Button);
        if (attack.bindings.Count == 0)
            attack.AddBinding("<Mouse>/leftButton");

        if (attackLocation == null)
            attackLocation = new InputAction(type: InputActionType.Value, expectedControlType: "Vector2");
        if (attackLocation.bindings.Count == 0)
            attackLocation.AddBinding("<Mouse>/position");
    }
    

    void Update() {
        if (!HasStateAuthority)  return;

        if (attack.WasPerformedThisFrame()) {
            Vector2 attackLocationInScreenCoordinates = attackLocation.ReadValue<Vector2>();

            var camera = Camera.main;
            // Camera camera = this.gameObject.GetComponentInChildren<Camera>();
            Ray ray = camera.ScreenPointToRay(attackLocationInScreenCoordinates);
            //ray.origin = this.gameObject.transform.position;
            float drawRayDuration = 10f;  // Time for the raw to disappear.
            Debug.DrawRay(ray.origin, ray.direction * shootDistance, Color.red, drawRayDuration);
            
            if (Runner.GetPhysicsScene().Raycast(ray.origin, ray.direction * shootDistance, out var hit)) {
                GameObject hitObject = hit.transform.gameObject;
                RaycastAttack hitObjectRayCastAttack = hitObject.GetComponent<RaycastAttack>();
                if (hitObject.GetInstanceID() != this.gameObject.GetInstanceID() && !hitObjectRayCastAttack.HasShield)
                {
                    Debug.Log("Raycast hit: name="+ hitObject.name+" tag="+hitObject.tag+" collider="+hit.collider);
                    
                    if (hitObject.TryGetComponent<Health>(out var health)) {
                        if (health.GetHealth() <= 0)
                        {
                            Debug.Log("Destroy hitObject!!");
                            Destroy(hitObject);
                        }
                        Debug.Log("Dealing damage");
                        health.DealDamageRpc(Damage);
                        this.gameObject.GetComponent<Health>().NetworkedHealth += killBonus;
                    }
                }
                
            }
        }
    }

}