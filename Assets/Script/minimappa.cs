using UnityEngine;
using UnityEngine.Events;
using System;
using UnityEngine.Serialization;
using System.Collections;
using UnityEngine.UI;
using TMPro;

namespace Microsoft.MixedReality.Toolkit.Input
{
    public class minimappa : MonoBehaviour
    {

        public GameObject bottom_dx;



        public GameObject min_bottom_dx;


        private Vector3 ancora_reale;


        private Vector3 min_ancora_reale;

        public TextMeshPro Distanza_X;
        public TextMeshPro Distanza_Y;
        public GameObject c;

        public Vector3 hololens;
        private Quaternion rotationOrigin;
        public GameObject enviroment;

        private float scaled_distance;
        private float distanza_hololens_ancora;
        private float new_x;
        private float new_y;
        Camera cam;

        // Start is called before the first frame update
        void Start()
        {
            Invoke("Delayed_Start", 2);
        }
        void Delayed_Start()
        {
            Distanza_X = GameObject.Find("Distanza_X").GetComponent<TextMeshPro>();
            Distanza_Y= GameObject.Find("Distanza_Y").GetComponent<TextMeshPro>();
            ancora_reale = bottom_dx.transform.position;
            min_ancora_reale = min_bottom_dx.transform.position;
            cam = GameObject.Find("Main Camera").GetComponent<Camera>();
            hololens = cam.transform.position;
            rotationOrigin = cam.transform.rotation;



            //Asse x 
            distanza_hololens_ancora = ancora_reale.x - hololens.x;
            scaled_distance = distanza_hololens_ancora /65f;
            new_x = min_ancora_reale.x - scaled_distance;

            // Asse z
            distanza_hololens_ancora = ancora_reale.z - hololens.z;
            scaled_distance = distanza_hololens_ancora / 48f; // 
            new_y = min_ancora_reale.y - scaled_distance;

            c.transform.position = new Vector3(new_x, new_y, min_ancora_reale.z );
            Distanza_X.text = "Distanza_X: " + (ancora_reale.x - hololens.x).ToString();
            Distanza_Y.text = "DIistanza_Y: " + (ancora_reale.z - hololens.z).ToString();


        }

        void Update()
        {
            //Distanza_X.text = "Distanza_X: "+(ancora_reale.x - hololens.x).ToString();
            //Distanza_Y.text =  "DIistanza_Y: "+(ancora_reale.z - hololens.z).ToString();
            Vector3 diff = cam.transform.position - hololens;
            Quaternion diffRotation = cam.transform.rotation * Quaternion.Inverse(rotationOrigin);
            c.transform.position = c.transform.position + c.transform.TransformDirection(new Vector3(diff[0] / 53f, 0, diff[2] / 45f));

            hololens = cam.transform.position;

        }
    }
}
