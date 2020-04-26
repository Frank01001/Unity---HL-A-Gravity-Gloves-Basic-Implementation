using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows.Speech;
using System.Linq;
using System;
using Valve.VR;
using Valve.VR.InteractionSystem;

/** ************************************************* */
/** Gravity Gloves Basic Implementation by Frank01001 */
/** ************************************************* */

public class GravityGloves : MonoBehaviour
{
    //Enum used to select action (I named it spell because I initially intended this to be one of many actions)
    public enum Spell { NONE, ATTRACT};

    [Header("Links")]
    //Left Hand's child "ObjectAttachmentPoint"
    [SerializeField]
    Transform left_hand_triplet;
    //Right Hand's child "ObjectAttachmentPoint"
    [SerializeField]
    Transform right_hand_triplet;
    //LayerMask excluding player colliders from Raycasting
    [SerializeField]
    LayerMask player_complement_mask;
    //Particle System to use on targeted objects
    [SerializeField]
    Transform grabbity_tracker;

    /* SteamVR Input */

    //Grip button (I don't own a Valve Index, knuckles may work differently)
    [SerializeField]
    SteamVR_Action_Boolean stvr_grip;
    //Controller tracking data (position, velocity, ...)
    [SerializeField]
    SteamVR_Action_Pose stvr_controller_pose;

    [Header("Status")]
    /*Current data*/
    public Spell current_spell;
    [Header("Parameters")]
    [Range(0.0f, 1.0f)]
    //How much hand movement is needed to trigger the object's leap?
    [SerializeField]
    float attraction_spell_sensitivity;

    //Attached SteamVR Player Component Components
    Player stvr_player;

    //Buffers
    private RaycastHit left_hand_raycast_hit, right_hand_raycast_hit;
    private SteamVR_Input_Sources in_source_buffer;

    /* ATTRACTION BUFFERS*/
    private Transform attr_left_target, attr_right_target;

    // Start is called before the first frame update
    void Start()
    {
        stvr_player = GetComponent<Player>();
    }

    // Update is called once per frame
    void Update()
    {

        //What is each hand pointing to?
        Physics.Raycast(left_hand_triplet.transform.position, left_hand_triplet.forward, out left_hand_raycast_hit, 50, player_complement_mask);
        Physics.Raycast(right_hand_triplet.transform.position, right_hand_triplet.forward, out right_hand_raycast_hit, 50, player_complement_mask);

        /* Spell code */
        switch (current_spell)
        {
            /* --------------------------------- */
            /* Similar to HL:A's Grabbity Gloves */
            /* --------------------------------- */
            case Spell.ATTRACT:
            {
                    in_source_buffer = stvr_player.leftHand.handType;

                    //If grip is pressed, change attraction target
                    if (stvr_grip[in_source_buffer].stateDown && left_hand_raycast_hit.transform.tag == "ALLOW_MANIPULATION")
                    {
                        attr_left_target = left_hand_raycast_hit.transform;
                        //Create and start particle system
                        Transform tracker = Instantiate(grabbity_tracker, attr_left_target.position, Quaternion.Euler(0.0f, 0.0f, 0.0f));
                        tracker.SetParent(attr_left_target);

                    }

                    //Check for flick of left hand
                    if (attr_left_target != null && Vector3.Dot(stvr_controller_pose[in_source_buffer].velocity, (Camera.main.transform.forward - Camera.main.transform.up)) < -attraction_spell_sensitivity)
                    {
                        //Calculate velocity and apply it to the target
                        Vector3 calculated_velocity = ComplementarCalculations.CalculateParabola(attr_left_target.transform.position, left_hand_triplet.position);
                        attr_left_target.GetComponent<Rigidbody>().velocity = calculated_velocity;
                        //Destroy particle system
                        Destroy(attr_left_target.GetChild(1).gameObject);
                        attr_left_target = null;
                    }

                    in_source_buffer = stvr_player.rightHand.handType;

                    //If grip is pressed, change attraction target
                    if (stvr_grip[in_source_buffer].stateDown && right_hand_raycast_hit.transform.tag == "ALLOW_MANIPULATION")
                    {
                        attr_right_target = right_hand_raycast_hit.transform;
                        //Create particle system
                        Transform tracker = Instantiate(grabbity_tracker, attr_right_target.position, Quaternion.Euler(0.0f, 0.0f, 0.0f));
                        tracker.SetParent(attr_right_target);

                    }

                    //Check for flick of left hand
                    if (attr_right_target != null && Vector3.Dot(stvr_controller_pose[in_source_buffer].velocity, (Camera.main.transform.forward - Camera.main.transform.up)) < -attraction_spell_sensitivity)
                    {
                        //Calculate velocity and apply it to the target
                        Vector3 calculated_velocity = ComplementarCalculations.CalculateParabola(attr_right_target.transform.position, right_hand_triplet.position);
                        attr_right_target.GetComponent<Rigidbody>().velocity = calculated_velocity;
                        //Destroy particle system
                        Destroy(attr_right_target.GetChild(1).gameObject);
                        attr_right_target = null;
                    }

                    break;
            }
            default:
                break;
        }
    }

    
}
