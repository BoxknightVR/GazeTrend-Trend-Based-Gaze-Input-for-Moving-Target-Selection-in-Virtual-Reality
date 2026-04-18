private void Move_Target_Dwell(List<GameObject> items, Vector3 T_Pos, Vector3 T_Vec)
    {
        GameObject result = null;
        for(int i = 0; i < items.Count; i++)
        {
            float data = items[i].GetComponent<Angle_Distance>().Return_Dis_and_Ang(T_Pos, T_Vec);
            if (result == null)
            {
                if(data <= Gaze_Min_Angle)
                {
                    result = items[i];
                }
            }
            else
            {
                float data_Cur = result.GetComponent<Angle_Distance>().Return_Dis_and_Ang(T_Pos, T_Vec);
                if(data <= data_Cur)
                {
                    result = items[i];
                }
            }
        }

        if(result != null)
        {
            if(selectedObj != null && selectedObj != result.transform)
            {
                selectedObj.GetComponent<ETObject>().UnFocused();
                selectedObj = null;
            }
            else if(selectedObj == null)
            {
                selectedObj = result.transform;
                selectedObj.GetComponent<ETObject>().DwellFoused();
                Dwell_Begin_Audio.Play();
                DWELL_START_TIME = System.DateTime.Now;
            }

            if(selectedObj != null)
            {
                double time_length = GET_SUB_SECONDS(DWELL_START_TIME, System.DateTime.Now);
                //Debug.Log(time_length);
                if(time_length >= DWELL_Time_Length)
                {
                    Select_Audio.Play();
                    selectedObj.GetComponent<ETObject>().IsFocused();
                    DWELL_START_TIME = System.DateTime.Now;

                    if (TEST_BEGIN)
                    {
                        int idx = items.FindIndex(go => go && go.transform == selectedObj);
                        if (idx == TEST_Result_Index)
                        {
                            sw.WriteLine("Item Size: " + THREE_ObjectY_UserX_Object_Num[THREE_ObjectY_UserX_Object_Num_Index].ToString()
                                + " Speed: " + Object_Speed_Index.ToString()
                                + " DWELL Result T Time " + GET_SUB_SECONDS(TEST_BEGIN_TIME_RECORD, DateTime.Now).ToString("F2") + " HEADMOVE " + TEST_Head_Movement.ToString("F1") + "\n");
                            Clean_Once_Test_Round();
                        }
                        else
                        {
                            sw.WriteLine("Item Size: " + THREE_ObjectY_UserX_Object_Num[THREE_ObjectY_UserX_Object_Num_Index].ToString()
                                + " Speed: " + Object_Speed_Index.ToString()
                                + " DWELL Result F Time " + GET_SUB_SECONDS(TEST_BEGIN_TIME_RECORD, DateTime.Now).ToString("F2"));
                        }
                    }
                }
                else
                {
                    selectedObj.GetComponent<ETObject>().DwellFoused();
                }
            }
        }
        else
        {
            if (selectedObj != null)
            {
                selectedObj.GetComponent<ETObject>().UnFocused();
                selectedObj = null;
            }
        }
    }
