
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GyroV2 : MonoBehaviour
{
    //public SerialController serialController;
    public Test test;
    public JengaController jenga;

    bool firstTimeReading = true;
    public Vector3 offset;
    public Vector3 error;
    Vector3 lastInput;
    // Start is called before the first frame update
    void Awake()
    {
    }
    // Update is called once per frame
    void Update()
    {
        ReadMessage();
        if (Input.GetKeyDown(KeyCode.R))
        {
            firstTimeReading = true;
        }
    }
    void ReadMessage()
    {
        //string message = serialController.ReadSerialMessage();
        string message = test.ReadSerialMessage();
        if (message == null)
            return;

            if (message[0] == '#')
                DecryptMessage(message);
            else
                Debug.Log("System message : " + message);
    }
    void DecryptMessage(string message)
    {
        if(jenga != null)
        {
            string[] s = message.Substring(1).Split('/');
            //Vector3 inputVector = new Vector3(-float.Parse(s[2]), float.Parse(s[0]), float.Parse(s[1]));
            Vector3 inputVector = new Vector3(0, float.Parse(s[0]), 0);

            if (firstTimeReading)
            {
                offset = inputVector;
                lastInput = inputVector;
                firstTimeReading = false;
                error = Vector3.zero;
            }
            if (Mathf.Abs(inputVector.y - lastInput.y) <= 0.1f)
                error.y += inputVector.y - lastInput.y;
            jenga.FeedVector(inputVector - offset - error);
            lastInput = inputVector;
        }
    }
}

