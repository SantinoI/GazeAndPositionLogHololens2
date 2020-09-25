using UnityEngine;
using UnityEngine.Events;
using System;
using UnityEngine.Serialization;
using System.Collections;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Windows.WebCam;
using System.Linq;

namespace Microsoft.MixedReality.Toolkit.Input
{
    public class Log : MonoBehaviour
    {
        // Start is called before the first frame update


        /*
         public TextMeshPro Origin;
         public TextMeshPro Direction;
         public TextMeshPro X_2D;
         public TextMeshPro Y_2D;
         public TextMeshPro Ancor_pos;
         */

        static readonly float MaxRecordingTime = 15.0f;
        VideoCapture m_VideoCapture = null;
        float m_stopRecordingTimer = float.MaxValue;
        
        static string timestamp;
        private float distanza_hololens_ancora_x;
        private float distanza_hololens_ancora_y;
        private float distanza_hololens_ancora_z;
        public GameObject ancora_real;
        public Vector3 hololens;
        private Vector3 ancora_reale;
        private float time = 0.0f;
        float global_time = 0;
        int second;
        public Transform target;
        Camera cam;
        void Start()
        {
            /*
            Origin = GameObject.Find("Origin").GetComponent<TextMeshPro>();
            Direction = GameObject.Find("Direction").GetComponent<TextMeshPro>();
            X_2D = GameObject.Find("X_2D").GetComponent<TextMeshPro>();
            Y_2D = GameObject.Find("Y_2D").GetComponent<TextMeshPro>();
            Ancor_pos = GameObject.Find("Ancor_pos").GetComponent<TextMeshPro>();*/
            ancora_reale = ancora_real.transform.position;
            timestamp = Guid.NewGuid().ToString("N");

            cam = GameObject.Find("Main Camera").GetComponent<Camera>();
            System.IO.File.WriteAllText(LogNamePosition(), String.Empty);
            System.IO.File.WriteAllText(LogNameGaze(), String.Empty);
            StartVideoCaptureTest();

        }
            public static string LogNamePosition()
        {
            /*return string.Format("{0}/screenshots/screen_{1}x{2}_{3}.png",
                                 Application.dataPath,
                                 width, height,
                                 System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"));
                                 */

            string filename = string.Format(@"LogPosition.txt");
            string filePath = System.IO.Path.Combine(Application.persistentDataPath, filename);
            return filePath;

        }
        public static string LogNameGaze()
        {
            /*return string.Format("{0}/screenshots/screen_{1}x{2}_{3}.png",
                                 Application.dataPath,
                                 width, height,
                                 System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"));
                                 */
           
            string filename = string.Format(@"LogGaze.txt");
            string filePath = System.IO.Path.Combine(Application.persistentDataPath, filename);
            return filePath;

        }

        // Update is called once per frame
        void Update()
        {
            ancora_reale = ancora_real.transform.position;
            hololens = cam.transform.position;
            var eyeGazeProvider = CoreServices.InputSystem?.EyeGazeProvider;
            if (eyeGazeProvider != null && eyeGazeProvider.UseEyeTracking && eyeGazeProvider.IsEyeGazeValid)
            {

                //Debug.Log(eyeGazeProvider.GazeDirection.normalized);
                //Debug.Log("Gaze is looking in direction: " + eyeGazeProvider.GazeDirection);
                //Debug.Log("Gaze origin is: " + eyeGazeProvider.GazeOrigin);
            }
           
            /*Origin.text = "Gaze Origin: " + CoreServices.InputSystem.EyeGazeProvider.GazeOrigin;
            Direction.text = "Gaze Direction: " + CoreServices.InputSystem.EyeGazeProvider.GazeDirection.normalized;
            Ancor_pos.text = "Ancor_pos: " + ancora_real.transform.position;*/

            Vector3 viewPos = cam.WorldToScreenPoint(CoreServices.InputSystem.EyeGazeProvider.GazeOrigin +
                CoreServices.InputSystem.EyeGazeProvider.GazeDirection.normalized); // PASSO DALLE COORDINATE DEL GAZE(WORLD) AI PIXEL DELL'IMMAGINE

           // X_2D.text = "X (2D): " + (viewPos.x * 1);
            //Y_2D.text = "Y (2D): " + (viewPos.y * 1);


            time += Time.deltaTime; ;
            if (time > 1)
            {
                //Asse x 
                distanza_hololens_ancora_x = ancora_reale.x - hololens.x;
                
                // Asse z
                distanza_hololens_ancora_z = ancora_reale.z - hololens.z;
                // Asse y
                distanza_hololens_ancora_y = ancora_reale.y - hololens.y;

                //Debug.Log(distanza_hololens_ancora_x +" "+ distanza_hololens_ancora_y +" "+ distanza_hololens_ancora_z);

                System.IO.File.AppendAllText(LogNamePosition(), global_time.ToString() + "  " + "X: " + distanza_hololens_ancora_x + " Y: "+ distanza_hololens_ancora_y +
                    " Z: "+ distanza_hololens_ancora_z + Environment.NewLine);
                System.IO.File.AppendAllText(LogNameGaze(), global_time.ToString() + "  " + "X: " + viewPos.x + "   " + "Y: " + viewPos.y + Environment.NewLine);
                global_time += 1;
                time = 0;
            }


            if (m_VideoCapture == null || !m_VideoCapture.IsRecording)
            {
                return;
            }

            if (Time.time > m_stopRecordingTimer)
            {
                m_VideoCapture.StopRecordingAsync(OnStoppedRecordingVideo);
            }
        }

        void StartVideoCaptureTest()
        {
            Resolution cameraResolution = VideoCapture.SupportedResolutions.OrderByDescending((res) => res.width * res.height).First();
            Debug.Log(cameraResolution);

            float cameraFramerate = VideoCapture.GetSupportedFrameRatesForResolution(cameraResolution).OrderByDescending((fps) => fps).First();
            Debug.Log(cameraFramerate);

            VideoCapture.CreateAsync(true, delegate (VideoCapture videoCapture)
            {
                if (videoCapture != null)
                {
                    m_VideoCapture = videoCapture;
                    Debug.Log("Created VideoCapture Instance!");

                    CameraParameters cameraParameters = new CameraParameters();
                    cameraParameters.hologramOpacity = 1f;
                    cameraParameters.frameRate = cameraFramerate;
                    cameraParameters.cameraResolutionWidth = cameraResolution.width;
                    cameraParameters.cameraResolutionHeight = cameraResolution.height;
                    cameraParameters.pixelFormat = CapturePixelFormat.BGRA32;

                    m_VideoCapture.StartVideoModeAsync(cameraParameters,
                        VideoCapture.AudioState.ApplicationAndMicAudio,
                        OnStartedVideoCaptureMode);
                }
                else
                {
                    Debug.LogError("Failed to create VideoCapture Instance!");
                }
            });
        }

        void OnStartedVideoCaptureMode(VideoCapture.VideoCaptureResult result)
        {
            time = 0.0f;


            Debug.Log("Started Video Capture Mode!");
            //string timeStamp = Time.time.ToString().Replace(".", "").Replace(":", "");
            string filename = string.Format("Video.mp4");
            string filepath = System.IO.Path.Combine(Application.persistentDataPath, filename);
            filepath = filepath.Replace("/", @"\");
            m_VideoCapture.StartRecordingAsync(filepath, OnStartedRecordingVideo);
        }

        void OnStoppedVideoCaptureMode(VideoCapture.VideoCaptureResult result)
        {
            Debug.Log("Stopped Video Capture Mode!");
        }

        void OnStartedRecordingVideo(VideoCapture.VideoCaptureResult result)
        {
            Debug.Log("Started Recording Video!");
            m_stopRecordingTimer = Time.time + MaxRecordingTime;
        }

        void OnStoppedRecordingVideo(VideoCapture.VideoCaptureResult result)
        {
            Debug.Log("Stopped Recording Video!");
            m_VideoCapture.StopVideoModeAsync(OnStoppedVideoCaptureMode);
        }

    }
}