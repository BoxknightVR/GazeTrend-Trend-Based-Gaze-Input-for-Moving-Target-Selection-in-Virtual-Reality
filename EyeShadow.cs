private void Move_Target_EyeShadow(List<GameObject> items, Vector3 T_Pos, Vector3 T_Vec)
    {
        int Current = -1 ;

        for(int i = 0; i < items.Count; i++)
        {
            float data = items[i].GetComponent<Angle_Distance>().Return_Dis_and_Ang(T_Pos, T_Vec);
            
            if (data <= Gaze_Min_Angle)
            {
                if(THREE_Items_Is_Active[i] == 0)
                {
                    //Debug.Log("Am I here? " + items[i].name);
                    THREE_Items_Is_Active[i] = 1;
                    THREE_Items_Time_Record[i] = DateTime.Now;
                    EyeShadow_Active_Virtual_Clutch(i, T_Pos, T_Vec);
                }
                else if(THREE_Items_Is_Active[i] == 1)
                {
                    THREE_Items_Time_Record[i] = DateTime.Now;
                }
            }

            if(THREE_Items_Is_Active[i] == 1 || THREE_Items_Is_Active[i] == 2)
            {
                EyeShadow_Reset_Virtual_Clutch_Render_LineRender(i);
            }
        }

        GameObject result = null;
        for (int i = 0; i < THREE_Items_Virtual_Clutch.Count; i++)
        {
            if (THREE_Items_Virtual_Clutch[i].activeSelf)
            {
                float data_vc = THREE_Items_Virtual_Clutch[i].GetComponent<Angle_Distance>().Return_Dis_and_Ang(T_Pos, T_Vec); 

                if(result == null)
                {
                    if (data_vc <= Gaze_Min_Angle)
                    {
                        result = THREE_Items_Virtual_Clutch[i];
                        Current = i;
                    }
                }
                else
                {
                    float data_Cur = result.GetComponent<Angle_Distance>().Return_Dis_and_Ang(T_Pos, T_Vec);
                    if(data_vc <= data_Cur)
                    {
                        result = THREE_Items_Virtual_Clutch[i];
                        Current = i;
                    }
                }
            }
        }

        if(result != null)
        {
            if (EyeShadow_Focus_Same_Item == -1)
            {
                EyeShadow_Focus_Same_Item = Current;
                THREE_Items_Is_Active[Current] = 2;
                THREE_Items_Time_Record[Current] = DateTime.Now;
            }
            else if (EyeShadow_Focus_Same_Item != Current)
            {
                THREE_Items_Is_Active[EyeShadow_Focus_Same_Item] = 1;
                THREE_Items_Time_Record[EyeShadow_Focus_Same_Item] = DateTime.Now;
                THREE_Items[EyeShadow_Focus_Same_Item].GetComponent<ETObject>().UnFocused();
                EyeShadow_Focus_Same_Item = Current;
                THREE_Items_Is_Active[Current] = 2;
                THREE_Items_Time_Record[Current] = DateTime.Now;
            }
            else
            {
                if (GET_SUB_SECONDS(THREE_Items_Time_Record[EyeShadow_Focus_Same_Item], DateTime.Now) >= EyeShadow_Time_Length)
                {
                    Select_Audio.Play();
                    items[EyeShadow_Focus_Same_Item].GetComponent<ETObject>().IsFocused();
                    THREE_Items_Time_Record[EyeShadow_Focus_Same_Item] = DateTime.Now;

                    if (TEST_BEGIN)
                    {
                        if (EyeShadow_Focus_Same_Item == TEST_Result_Index)
                        {
                            sw.WriteLine("Item Size: " + THREE_ObjectY_UserX_Object_Num[THREE_ObjectY_UserX_Object_Num_Index].ToString()
                                + " Speed: " + Object_Speed_Index.ToString()
                                + " EYESHADOW Result T Time " + GET_SUB_SECONDS(TEST_BEGIN_TIME_RECORD, DateTime.Now).ToString("F2") + " HEADMOVE " + TEST_Head_Movement.ToString("F1") + "\n");
                            Clean_Once_Test_Round();
                        }
                        else
                        {
                            sw.WriteLine("Item Size: " + THREE_ObjectY_UserX_Object_Num[THREE_ObjectY_UserX_Object_Num_Index].ToString()
                                + " Speed: " + Object_Speed_Index.ToString()
                                + " EYESHADOW Result F Time " + GET_SUB_SECONDS(TEST_BEGIN_TIME_RECORD, DateTime.Now).ToString("F2"));
                        }
                    }

                    items[EyeShadow_Focus_Same_Item].GetComponent<ETObject>().UnFocused();
                    EyeShadow_Disactive_Virtual_Clutch(EyeShadow_Focus_Same_Item);
                    THREE_Items_Is_Active[EyeShadow_Focus_Same_Item] = 0;
                    EyeShadow_Focus_Same_Item = -1;
                }
                else
                {
                    items[EyeShadow_Focus_Same_Item].GetComponent<ETObject>().DwellFoused();
                }
            }
        }
        else
        {
            if(EyeShadow_Focus_Same_Item != -1)
            {
                THREE_Items_Is_Active[EyeShadow_Focus_Same_Item] = 1;
                THREE_Items_Time_Record[EyeShadow_Focus_Same_Item] = DateTime.Now;
                THREE_Items[EyeShadow_Focus_Same_Item].GetComponent<ETObject>().UnFocused();
            }

            EyeShadow_Focus_Same_Item = -1;
            Current = -1;
        }

        for(int i = 0; i < THREE_Items_Is_Active.Count; i++)
        {
            if (THREE_Items_Is_Active[i] == 1)
            {
                if (GET_SUB_SECONDS(THREE_Items_Time_Record[i], DateTime.Now) >= EyeShadow_Disappear_Time_Length)
                {
                    EyeShadow_Disactive_Virtual_Clutch(i);
                    THREE_Items_Is_Active[i] = 0;
                }
                else
                {
                    EyeShadow_Transparency_Virtual_Clutch(i);
                }
            }
        }

    }

    private void EyeShadow_Active_Virtual_Clutch(int i, Vector3 T_Pos, Vector3 T_Vec)
    {
        if (!THREE_Items_Virtual_Clutch[i].activeSelf)
        {
            THREE_Items_Virtual_Clutch[i].SetActive(true);
        }

        Vector3 Start_Pos = THREE_Items[i].transform.position;
        Vector3 End_Pos = THREE_Object_End_Pos[i];

        Vector2 Start_2D_Pos = new Vector2(Start_Pos.x, Start_Pos.y);
        Vector2 End_2D_Pos = new Vector2(End_Pos.x, End_Pos.y);

        float dis = Vector3.Distance(Start_Pos, T_Pos);
        float scale = MathF.Tan(EyeShadow_Item_Display_Angle / 2f * Mathf.Deg2Rad) * dis * 2f;

        Vector2 velocity = End_2D_Pos - Start_2D_Pos;
        if (velocity.magnitude < 0.001f)
        {
            Vector3 Direction = new Vector3(1, 1, 0).normalized;
            THREE_Items_Virtual_Clutch[i].transform.position = T_Pos + T_Vec * dis + Direction * scale;
        }
        else
        {
            Vector2 dir = velocity.normalized;
            Vector2 pos = Start_2D_Pos + dir * scale;
            THREE_Items_Virtual_Clutch[i].transform.position = new Vector3(pos.x, pos.y, Start_Pos.z);
        }

        EyeShadow_Reset_Virtual_Clutch_Render_LineRender(i);
    }

    private void EyeShadow_Reset_Virtual_Clutch_Render_LineRender(int i)
    {
        THREE_Items_Virtual_Clutch[i].GetComponent<Renderer>().material.color = THREE_Items[i].GetComponent<Renderer>().material.color;
        LineRenderer line = THREE_Items_Virtual_Clutch[i].GetComponent<LineRenderer>();
        line.positionCount = 2;
        line.SetPosition(0, THREE_Items_Virtual_Clutch[i].transform.position);
        line.SetPosition(1, THREE_Items[i].transform.position);
        line.startWidth = 0.005f;
        line.endWidth = 0.005f;
        line.material = LineMaterial;
    }

    private void EyeShadow_Transparency_Virtual_Clutch111(int i)
    {
        Renderer rend = THREE_Items_Virtual_Clutch[i].GetComponent<Renderer>();

        if (rend != null)
        {
            Material mat = rend.material;
            mat.SetFloat("_Mode", 3);
            mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            mat.SetInt("_ZWrite", 0);
            mat.DisableKeyword("_ALPHATEST_ON");
            mat.EnableKeyword("_ALPHABLEND_ON");
            mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            mat.renderQueue = 3000;

            Color color = THREE_Items[i].GetComponent<Renderer>().material.color;

            float precent = 1f - (float)(GET_SUB_SECONDS(THREE_Items_Time_Record[i], DateTime.Now) / EyeShadow_Disappear_Time_Length);
            color.a = precent;
            mat.color = color;
        }
    }

    private void EyeShadow_Transparency_Virtual_Clutch(int i)
    {
        float precent = 1f - (float)(GET_SUB_SECONDS(THREE_Items_Time_Record[i], DateTime.Now) / EyeShadow_Disappear_Time_Length);
        Material material = THREE_Items_Virtual_Clutch[i].GetComponent<Renderer>().material;
        material.color = new Color(material.color.r, material.color.g, material.color.b, precent);
    }

    private void EyeShadow_Disactive_Virtual_Clutch(int i)
    {
        LineRenderer line = THREE_Items_Virtual_Clutch[i].GetComponent<LineRenderer>();
        line.positionCount = 0;
        line = THREE_Items[i].GetComponent<LineRenderer>();
        line.positionCount = 0;
        THREE_Items_Virtual_Clutch[i].SetActive(false);
    }
