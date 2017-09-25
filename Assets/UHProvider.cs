using System;
using Ultrahaptics;
using UnityEngine;


public class UHProvider : MonoBehaviour {


    public struct Finger_data
    {
        public string key_id; //the key_id is hovering
        public Ultrahaptics.Vector3 position; //key_centre,finger_height
        public uint current_state; //0: none; 1:slip; 2:hover; 3:push
        public uint previous_state; //same as above
        public float current_intensity;//relative intensity for Hover/slip/press

        public uint slip_type; //0:continuous --> do nothing; 1: discontinuous --> Burst!!
        public uint modulation_type; //0: Pure tone 170Hz, 1:Timbre 170Hz, others not implemented;
    }
    private Finger_data[] finger_data = new Finger_data[8]; //8 fingers, (2hands excluding thumbs)

    public bool large_key = true;
    public float size = 0.01f;
    public bool pilar_constraint = true;


    static TimePointStreamingEmitter _emitter;
    public struct UH_point_data
    {
        public Ultrahaptics.Vector3 position;
        public float current_intensity;
        public uint modulation_type; //0:170; 1:timbre(160); 2: lissajous(3;2)(60Hz)
        public uint slip_type;
        public uint current_state;
        public uint current_intensity_idx; //cursor for intensities; goes from 0 to intensity_point_count, per step of 1 every time a path is executed
        public string key_id;
    }
    static UH_point_data[] uh_point_data;//2 points data in this project, 1 per hand.
    static float[][] _modulation_intensities;  //modulation signal; as many as there are modulation type
    static Ultrahaptics.Vector3[] _path_positions; //curve around the keysurface; vary with _current

    //sampling rates
    static uint _sample_rate; //same for the array
    static uint[] _intensity_point_count;//0: 170Hz tone, 1: 170Hz timbre
    static uint _position_point_count; 

    //indexes
    static uint _current_idx = 0; //cursor for sample rate; goes from 0 to sample_rate, per step of 1 every cycle 
    static uint _current_position_idx = 0;// cursor for positions; goes from 0 to position_point_count, per step of 1 every tick!

    //public copies
    public float copy_intensity_1;
    public string copy_key_id_1;
    public uint copy_current_state_1;
    public uint copy_idx_1;
    public float copy_x_1;
    public float copy_intensity_2;
    public string copy_key_id_2;
    public uint copy_current_state_2;
    public uint copy_idx_2;
    public float copy_x_2;





    static void callback(TimePointStreamingEmitter emitter, OutputInterval interval, TimePoint deadline, object user_obj)
    {
        // For each time point in this interval...
        foreach (var tpoint in interval)
        {
            // For each control point available at this time point...
            for (int i = 0; i < tpoint.count(); ++i)
            {
                // Set the relevant data for this control point
                var point = tpoint.persistentControlPoint(i);
                uint idx = uh_point_data[i].modulation_type;
                uint current = uh_point_data[i].current_intensity_idx;
                if (idx == 2)
                {
                    point.setPosition(_path_positions[current] + uh_point_data[i].position);//center + path point
                    point.setIntensity(uh_point_data[i].current_intensity);
                }
                else
                {
                    point.setPosition(uh_point_data[i].position);//center position
                    point.setIntensity(uh_point_data[i].current_intensity * _modulation_intensities[idx][current]);
                }
                point.enable();
            }
            // Increment the counter so that we get the next "step" next time
            //*
            _current_idx = (_current_idx + 1) % _sample_rate;
            _current_position_idx = _current_idx % _position_point_count;
            uh_point_data[0].current_intensity_idx = (uint)((_current_idx / _position_point_count) % _intensity_point_count[uh_point_data[0].modulation_type]);
            uh_point_data[1].current_intensity_idx = (uint)((_current_idx / _position_point_count) % _intensity_point_count[uh_point_data[1].modulation_type]);

            // Debug.Log("current: "+ _current+ " - current position: "+ _current_position +" - current intensity: "+ _current_intensity);
            //*/

            // _current_intensity = (_current_intensity + 1) % _intensity_point_count);
        }
    }

    // Use this for initialization
    void Start() {
        // Create a timepoint streaming emitter
        // Note that this automatically attempts to connect to the device, if present
        _emitter = new TimePointStreamingEmitter();
        // Inform the SDK how many control points you intend to use
        // This also calculates the resulting sample rate in Hz, which is returned to the user
        _sample_rate = _emitter.setMaximumControlPointCount(2);
        _intensity_point_count = new uint[3];
        _intensity_point_count[0] = (uint)(_sample_rate /170);
        //Debug.Log("Point count = " + _intensity_point_count[0]);
        _intensity_point_count[1] = _sample_rate;
        //Debug.Log("Point count = " + _intensity_point_count[1]);
        _intensity_point_count[2] = (uint)(_sample_rate / 30);
        //Debug.Log("Point count = " + _intensity_point_count[2]);

        //initialise haptic points to state 0, so there are no points at the moment
        uh_point_data = new UH_point_data[2];
        uh_point_data[0].current_state = 0;
        uh_point_data[1].current_state = 0;

        //side of a key is 2.5cm (quad is of scale 1, but "letter key" is scale 0.5 and keyboard is scale of "0.05")
        //length is 10cm, "tactile point" is 1cm diameter, so 20 points equally space shall do the job.
        _position_point_count = 1;
        computePath_positions(); //compute square path

        

        _modulation_intensities = new float[2][]; // 0: pure tone 170Hz, 1: Timbre 160Hz;
        computeModulation_intensities(0); //compute pure tone 170Hz waveform
        computeModulation_intensities(1); //compute Timbre 160Hz waveform
     
        //initialise haptic points to state 0, so there are no points at the moment
        
        uh_point_data = new UH_point_data[2];
        uh_point_data[0].current_state = 0;
        uh_point_data[0].current_intensity_idx = 0;
        uh_point_data[0].current_intensity = 1;
        uh_point_data[0].modulation_type = 0;
        uh_point_data[0].position = new Ultrahaptics.Vector3(0.0f, 0.05f, 0.2f);
        uh_point_data[0].slip_type = 0;
        uh_point_data[0].key_id = "0";

        uh_point_data[1].current_state = 0;
        uh_point_data[1].current_intensity_idx = 0;
        uh_point_data[1].current_intensity = 1;
        uh_point_data[1].modulation_type = 0;
        uh_point_data[1].position = new Ultrahaptics.Vector3(0.0f, -0.05f, 0.2f);
        uh_point_data[1].slip_type = 0;
        uh_point_data[1].key_id = "1";

        if (large_key)
        {
            uh_point_data[0].modulation_type = 2;
            uh_point_data[1].modulation_type = 2;
            computePath_positions();
        }
        


        // Set our callback to be called each time the device is ready for new points
        _emitter.setEmissionCallback(callback, null);
        // Instruct the device to call our callback and begin emitting
        bool isOK = _emitter.start();
        if (!isOK);
        else
        {
            //Debug.Log("Emitter started succesfully");
            _emitter.initialize();
        }
        //Debug.Log("sample rate = " + _sample_rate + " - position count = " + _position_point_count + " - intensity count = " + _intensity_point_count );

    }

    // Update is called once per frame
    void Update() {
        //Debug.Log("state = " + _state + " - output_force: " + _output_force);


        //update which uh_point_data, based on finger_data
        //first point
        bool isLeftPointInactive = true;
        for (int i = 0; i<4; i++)
        {
            //priority for press button
            if(finger_data[i].current_state == 3)
            {
                uh_point_data[0].position = finger_data[i].position;
                uh_point_data[0].modulation_type = finger_data[i].modulation_type;
                uh_point_data[0].current_intensity = finger_data[i].current_intensity;
                uh_point_data[0].key_id = finger_data[i].key_id;
                uh_point_data[0].current_state = finger_data[i].current_state;
                uh_point_data[0].current_intensity_idx = 0;
                isLeftPointInactive = false;
            }
        }
        if (isLeftPointInactive)
        {
            if (finger_data[3].key_id == "F")
            {
                uh_point_data[0].position = finger_data[3].position;
                uh_point_data[0].modulation_type = finger_data[3].modulation_type;
                uh_point_data[0].current_intensity = finger_data[3].current_intensity;
            }
            else
            {
                uh_point_data[0].current_intensity = 0;

            }
            uh_point_data[0].key_id = finger_data[3].key_id;
            uh_point_data[0].current_state = finger_data[3].current_state;
            uh_point_data[0].current_intensity_idx = 0;
        }


        //Exactly the same for second point
        bool isRightPointInactive = true;
        for (int i = 4; i<8; i++)
        {
            //priority for press button
            if (finger_data[i].current_state == 3)
            {
                uh_point_data[1].position = finger_data[i].position;
                uh_point_data[1].modulation_type = finger_data[i].modulation_type;
                uh_point_data[1].current_intensity = finger_data[i].current_intensity;
                uh_point_data[1].key_id = finger_data[i].key_id;
                uh_point_data[1].current_state = finger_data[i].current_state;
                uh_point_data[1].current_intensity_idx = 0;

                isRightPointInactive = false;
            }
        }
        if (isRightPointInactive)
        {
            if (finger_data[4].key_id == "J")
            {
                uh_point_data[1].position = finger_data[4].position;
                uh_point_data[1].modulation_type = finger_data[4].modulation_type;
                uh_point_data[1].current_intensity = finger_data[4].current_intensity;
            }
            else
            {
                uh_point_data[1].current_intensity = 0;
            }
            uh_point_data[1].key_id = finger_data[4].key_id;
            uh_point_data[1].current_state = finger_data[4].current_state;
            uh_point_data[1].current_intensity_idx = 0;
        }

    }

    private void FixedUpdate()
    {

        //uint idx = uh_point_data[0].modulation_type;
        //uint current = uh_point_data[0].current_intensity_idx;
        //Debug.Log("idx = " + idx);
        //Debug.Log("current = " + current);
        //Debug.Log("Intenisity output" + uh_point_data[0].current_intensity * _modulation_intensities[idx][current]);
        //Debug.Log("Position: x=" + uh_point_data[0].position.x + ";y=" + uh_point_data[0].position.y + ";z=" + uh_point_data[0].position.z);

        copy_intensity_1 = uh_point_data[0].current_intensity;
        copy_idx_1 = uh_point_data[0].current_intensity_idx;
        copy_current_state_1 = uh_point_data[0].current_state;
        copy_key_id_1 = uh_point_data[0].key_id;
        copy_x_1 = uh_point_data[0].position.x;

        copy_intensity_2 = uh_point_data[1].current_intensity;
        copy_idx_2 = uh_point_data[1].current_intensity_idx;
        copy_current_state_2 = uh_point_data[1].current_state;
        copy_key_id_2 = uh_point_data[1].key_id;
        copy_x_2 = uh_point_data[1].position.x;
    }

    public void set_update(uint finger_tag, string key_id, uint state, Ultrahaptics.Vector3 position, float output_force,  uint modulation_type)
    {
        //Debug.Log("Data received: finger_tag("+finger_tag+ ") key_id(" + key_id + ") state(" + state + ") outputforce(" + output_force + ")");
        if (finger_tag < 8)//0: left pinky; 1: left ring, ... 8: right ring, 9:right pinky
        {
            //Is emitter free
            if (finger_data[finger_tag].current_state == 0)
            {
                //is new state different from none
                if (state != 0)
                {
                    //update
                    finger_data[finger_tag].key_id = key_id;
                    finger_data[finger_tag].previous_state = finger_data[finger_tag].current_state;
                    finger_data[finger_tag].current_state = state;
                    finger_data[finger_tag].position = position;
                    finger_data[finger_tag].current_intensity = output_force;
                    finger_data[finger_tag].modulation_type = modulation_type;
                }
                //else don't update
            }
            else
            {
                //Is key_id same as current
                if (finger_data[finger_tag].key_id == key_id)
                {
                    //Is state as current
                    if (finger_data[finger_tag].current_state == state)
                    {
                        //update
                        finger_data[finger_tag].position = position;
                        finger_data[finger_tag].current_intensity = output_force;
                        finger_data[finger_tag].modulation_type = modulation_type;
                    }
                    else
                    {
                        //Is state transition allowed
                        if ((finger_data[finger_tag].current_state == 1 && (state == 0 || state == 2)) || //slip can go none or hover
                            (finger_data[finger_tag].current_state == 2 && (state == 0 || state == 1 || state == 3)) || //hover can go none, slip or push (none if finger is too quick) 
                            (finger_data[finger_tag].current_state == 3 && (state == 2))) //push can only gohover
                        {
                            //update
                            finger_data[finger_tag].position = position;
                            finger_data[finger_tag].current_intensity = output_force;
                            finger_data[finger_tag].modulation_type = modulation_type;
                            finger_data[finger_tag].previous_state = finger_data[finger_tag].current_state;
                            finger_data[finger_tag].current_state = state;
                            if(state == 0) finger_data[finger_tag].current_intensity = 0.0f;
                        }
                        //else don't update
                    }

                }
                //else if the state is a press but current state is different from press
                else if ( state == 3 && finger_data[finger_tag].current_state != 3)
                {
                    //update all
                    finger_data[finger_tag].key_id = key_id;
                    finger_data[finger_tag].previous_state = finger_data[finger_tag].current_state;
                    finger_data[finger_tag].current_state = state;
                    finger_data[finger_tag].position = position;
                    finger_data[finger_tag].current_intensity = output_force;
                    finger_data[finger_tag].modulation_type = modulation_type;
                }
                //else don't update
            }
        }
        //Debug.Log("Data  updated: finger_tag(" + finger_tag + ") key_id(" + finger_data[finger_tag].key_id + ") state(" + finger_data[finger_tag].current_state + ") outputforce(" + finger_data[finger_tag].current_intensity + ")");

    }

    public void get_active_button(ref string button_id, ref uint state, uint hand_tag)
    {
        button_id = finger_data[hand_tag].key_id;
        state = finger_data[hand_tag].current_state;
    }

    private void computeModulation_intensities(uint modulation_type)
    {
        _modulation_intensities[modulation_type] = new float[_intensity_point_count[modulation_type]];

        if(modulation_type == 0)
        {
            for (int i = 0; i < _intensity_point_count[modulation_type]; i++)
            {
                float intensity = 0.5f * (1.0f - (float)Math.Cos(2.0f * Math.PI * i / (float)_intensity_point_count[modulation_type]));
                _modulation_intensities[modulation_type][i] = (intensity);
                //Debug.Log(intensity + ";" + i);
            }
        }
        else if(modulation_type == 1)
        {
            uint freq = 160;

            System.IO.StreamReader csv_file = new System.IO.StreamReader("./Assets/Ressources/timbre_" + freq + "Hz.wav.csv");

            var line = csv_file.ReadLine();
            var values = line.Split(',');

            uint nb_element = (uint)Int32.Parse(values[0]);
            Debug.Log("nb element = " + nb_element);
            _intensity_point_count[modulation_type] = nb_element;

            line = csv_file.ReadLine();
            values = line.Split(',');

            _modulation_intensities[modulation_type] = new float[_intensity_point_count[modulation_type]];

            for (int i = 0; i < nb_element; i++)
            {
                _modulation_intensities[modulation_type][i] = (float)Convert.ToDouble(values[i]);
            }
        }
      
       
    }

    private void computePath_positions()
    {
        
        _path_positions = new Ultrahaptics.Vector3[_intensity_point_count[2]];

        for(int i = 0; i<_intensity_point_count[2]; i++)
        {
            float x = (float)Math.Sin(2.0f * Math.PI * 3.0f * i / _intensity_point_count[2]);
            float y = (float)Math.Sin(2.0f * Math.PI * 2.0f * i / _intensity_point_count[2]);

            _path_positions[i] = new Ultrahaptics.Vector3(x * size, y * size, 0.0f);
            //Debug.Log("X:" + x + " ; Y:" + y);
        }
        /*
        //compute a square pattern in the x,y plan; each step is 0.005m (half of the tactile point diameter).
        //The number of point (_position_point_count) is discussed in the start loop
        if (_position_point_count >= 4)
        {
            uint side_count = _position_point_count / 4;
            float step = 0.005f; //side in mm

            //first side
            for (int i = 0; i < side_count; i++)
            {
                _path_positions[i] = new Ultrahaptics.Vector3(i * step, 0.0f, 0.0f);
            }
            //second side
            for (int i = 0; i < side_count; i++)
            {
                _path_positions[side_count + i] = new Ultrahaptics.Vector3(step * side_count, -i * step, 0.0f);
            }
            //third side
            for (int i = 0; i < side_count; i++)
            {
                _path_positions[2 * side_count + i] = new Ultrahaptics.Vector3((step * side_count - i * step), -step * side_count, 0.0f);
            }
            //fourth side
            for (int i = 0; i < side_count; i++)
            {
                _path_positions[3 * side_count + i] = new Ultrahaptics.Vector3(0.0f, (-step * side_count + i * step), 0.0f);
            }
        }
        else if(_position_point_count == 2)
        {
            _path_positions[0] = new Ultrahaptics.Vector3(0.0f, 0.0f, 0.0f);
            _path_positions[1] = new Ultrahaptics.Vector3(0.012f, -0.012f, 0.0f);
        }
        else
        {
            for (int i = 0; i < _position_point_count; i++)
                _path_positions[i] = new Ultrahaptics.Vector3(0.0f, 0.0f, 0.0f);
        }
        /*
        for(int i = 0; i < _position_point_count; i++)
        {
            Debug.Log("Position " + i + " = " + _path_positions[i].x + ";" + _path_positions[i].y + ";" + _path_positions[i].z + ";");
        }
        */


    }

    // Ensure the emitter is stopped on exit
    private void OnDestroy()
    {
        _emitter.stop();
    }
}
