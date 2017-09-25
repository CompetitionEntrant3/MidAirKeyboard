using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap;
using Leap.Unity;
using System;


public class KeyHoverHandler : MonoBehaviour {

    public struct FD_curve
    {
        public float total_travel; // 20, 40 or 60 millimeters 
        public float initial_force; 
        public float switching_force; 
        public float final_force; 
        public float offset_travel;
        public float switching_travel; 
        public float trigger_travel;
    }

    public uint current_state; // 0 = nothing, 1 = slip on, 2 = over, 3 = slip off
    public uint previous_state;

    LeapProvider provider; //leap motion
    UHProvider array; //array

    //finger position
    private double finger_position_x;
    private double finger_position_z;
    private double fingertip_height;

    //key geometry data
    private double key_centre_x;
    private double key_centre_y;
    private double key_width;
    private double key_height;
    private Ultrahaptics.Vector3 key_position;

    //distance info between key and finger --> move to update?
    private double dist_x;
    private double dist_y;
    private double normalized_dist_x;
    private double normalized_dist_y;
    private double slip_dist_x;
    private double slip_dist_y;
	
    private double displacement;
    private double start_curve; //the height at which the tap_start was detected (reference for feedback & data)
    private double end_curve; //the height at which the tap_end was detected  (reference for data)

    //key functionnality data
    public uint slip_type = 0; //0: Continuous; 1: Discontinuous (not implemted yet)
    public uint hover_type = 2; //0: Pure Tone 170Hz; 1: Timbre 200Hz; (other might follow but not implemented yet)
    FD_curve fd_curve;


    //key functionnal info
    private string key_id;
    public string ID;
    public double edge_limit = 0.5; //Let's take x = edge_limit*100 --> x% of the key surface is "hover feedback" while only (1-x)% is about slip
    public double hover_intensity = 0.8; //intensity of the hover feedback; for continuous slip, intensity goes from 0 to hover_intensity 
    public float current_intensity = 0; //interpolation of the intensity for slip
    public float peakVelocityThreshold = 0.3f;

    private uint travel_sampling_rate = 1000;
    private float[] _force_intensities; //fd curve, used as envelop; vary on displacement

    public Leap.Finger.FingerType supportedFinger;
    public uint fingerness; //similar to above...
    public bool handiness; //0:left hand, 1: right hand



    // Use this for initialization
    void Start () {
        //initialisation states;
        current_state = 0;
        previous_state = 0;

        provider = FindObjectOfType<LeapProvider>() as LeapProvider; //leap motion
        array = FindObjectOfType<UHProvider>() as UHProvider;

        //initialisation key geometrical features
        key_centre_x = transform.position.x;
        key_centre_y = transform.position.z;
        key_width = transform.lossyScale.x;
        key_height = transform.lossyScale.y;
        key_position = new Ultrahaptics.Vector3((float)key_centre_x, (float)key_centre_y + 0.097f, 0.2f);

        //fdcurve initilisation
        fd_curve.total_travel = 0.04f;
        fd_curve.initial_force = 0.8f;
        fd_curve.switching_force = 3.0f;
        fd_curve.final_force = 0.0f;
        fd_curve.offset_travel = 0.2f;
        fd_curve.switching_travel = 0.7f;
        fd_curve.trigger_travel = 0.8f;

        //compute fingerness --> is that the same as FingerSupported?
        key_id = name;
        //Debug.Log("Initialise KEY: " + key_id);
        if (key_id.CompareTo("Q") == 0 || key_id.CompareTo("A") == 0 || key_id.CompareTo("Z") == 0) fingerness = 0;
        else if (key_id.CompareTo("W") == 0 || key_id.CompareTo("S") == 0 || key_id.CompareTo("X") == 0) fingerness = 1;
        else if (key_id.CompareTo("E") == 0 || key_id.CompareTo("D") == 0 || key_id.CompareTo("C") == 0) fingerness = 2;
        else if(key_id.CompareTo("R") == 0 || key_id.CompareTo("F") == 0 || key_id.CompareTo("V") == 0) fingerness = 3;
        else if (key_id.CompareTo("T") == 0 || key_id.CompareTo("G") == 0 || key_id.CompareTo("B") == 0) fingerness = 3;
        else if (key_id.CompareTo("Y") == 0 || key_id.CompareTo("H") == 0 || key_id.CompareTo("N") == 0) fingerness = 4;
        else if (key_id.CompareTo("U") == 0 || key_id.CompareTo("J") == 0 || key_id.CompareTo("M") == 0) fingerness = 4;
        else if (key_id.CompareTo("I") == 0 || key_id.CompareTo("K") == 0 ) fingerness = 5;
        else if (key_id.CompareTo("O") == 0 || key_id.CompareTo("L") == 0 ) fingerness = 6;
        else if (key_id.CompareTo("P") == 0 ) fingerness = 7;

        //compute handiness
        if (fingerness > 3) handiness = true;
        else handiness = false;

        //initiate fd_curve
        _force_intensities = new float[travel_sampling_rate];
        computeFDCurve_intensities();
    }
	
	// Update is called once per frame
	void Update () {
        /*
        key_id = name;
        ID = key_id;
        key_centre_x = transform.position.x;
        key_centre_y = transform.position.z;
        key_position = new Ultrahaptics.Vector3((float)key_centre_x, (float)key_centre_y + 0.097f, 0.2f);*/

        Frame frame = provider.CurrentFrame; //frame from leap
        foreach (Hand hand in frame.Hands)
        {
            foreach (Finger finger in hand.Fingers)
            {
                if ((finger.Type == supportedFinger) && (hand.IsRight == handiness))
                {
                    //deal with slip&hover, when key is not being pressed
                    if (current_state != 3)
                    {
                        //compute distances key-finger
                        finger_position_x = finger.TipPosition.x;
                        finger_position_z = finger.TipPosition.z;
                        dist_x = Math.Abs(finger_position_x - key_centre_x);
                        dist_y = Math.Abs(finger_position_z - key_centre_y);
                        normalized_dist_x = 2 * dist_x / key_width;
                        normalized_dist_y = 2 * dist_y / key_height;

                        //if finger is on the "key pillar"
                        if (normalized_dist_x <= 1 && normalized_dist_y <= 1)
                        {
                            //compute slip
                            slip_dist_x = (normalized_dist_x - edge_limit) / (1.0f - edge_limit);
                            slip_dist_y = (normalized_dist_y - edge_limit) / (1.0f - edge_limit);

                            //slip case
                            if (slip_dist_x >= 0)
                            {
                                current_intensity = (float)(-slip_dist_x * hover_intensity + hover_intensity);
                                current_state = 1;
                            }
                            else if (slip_dist_y >= 0)
                            {
                                current_intensity = (float)(-slip_dist_y * hover_intensity + hover_intensity);
                                current_state = 1;
                            }

                            //hover case
                            else
                            {
                                current_intensity = (float)(hover_intensity);
                                current_state = 2;
                            }
                        }
                        else
                        {
                            current_state = 0;
                            current_intensity = 0;
                        }
                    }

                    //if key is active, handle the "tap detection", "press" feedback and "release detection"
                    if(current_state != 0)
                    {
                        //look for a press or release motion and update button state
                        if (finger.TipVelocity.y < -peakVelocityThreshold && current_state != 3)
                        {
                            current_state = 3;
                            //Debug.Log("Press" + key_id);

                            start_curve = finger.TipPosition.y;
                        }
                        else if (finger.TipVelocity.y > 0 && finger.TipVelocity.y < 0.1 && current_state == 3)
                        {
                            current_intensity = (float)hover_intensity;
                            current_state = 2;
                            // Debug.Log("Release");
                            end_curve = finger.TipPosition.y;
                        }

                        //compute fd-curve if state == 3, otherwise it is done in hover
                        if (current_state == 3)
                        {
                            displacement = ((start_curve - finger.TipPosition.y)) / fd_curve.total_travel;
                            if (displacement >= 1) displacement = 1;
                            else if (displacement < 0) displacement = 0;

                            uint current_force_idx = (uint)(displacement * travel_sampling_rate);
                            if (current_force_idx >= travel_sampling_rate) current_force_idx = travel_sampling_rate - 1;
                            current_intensity = _force_intensities[current_force_idx];
                            //Debug.Log(current_intensity);
                        }
                    }

                    //finally, update the status
                    array.set_update(fingerness, key_id, current_state, 
                        new Ultrahaptics.Vector3(key_position.x, key_position.y, finger.TipPosition.y),
                        current_intensity, hover_type);
                    previous_state = current_state;
                }
            }
         
        }
    }

    void FixedUpdate() {
        
    }

    private void computeFDCurve_intensities()
    {
        for (int i = 0; i < travel_sampling_rate; i++)
        {
            float _temp_offset = fd_curve.offset_travel * travel_sampling_rate;
            float _temp_switching = fd_curve.switching_travel * travel_sampling_rate;
            float _temp_trigger = fd_curve.trigger_travel * travel_sampling_rate;

            if (i < _temp_offset)
            {
                _force_intensities[i] = fd_curve.initial_force;
            }
            else if (i < _temp_switching)
            {
                _force_intensities[i] = ((fd_curve.initial_force - fd_curve.switching_force) / 
                    (_temp_offset - _temp_switching)) * (i - _temp_offset) + fd_curve.initial_force;
            }
            else if (i < _temp_trigger)
            {
                _force_intensities[i] = ((fd_curve.final_force - fd_curve.switching_force) / 
                    (_temp_trigger - _temp_switching)) * (i - _temp_switching) + fd_curve.switching_force;
            }
            else
            {
                _force_intensities[i] = fd_curve.final_force;
            }
        }
    }
}
