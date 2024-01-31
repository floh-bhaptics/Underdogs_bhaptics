using MelonLoader;
using Bhaptics.SDK2;

namespace MyBhapticsTactsuit
{
    public class TactsuitVR
    {
        /* A class that contains the basic functions for the bhaptics Tactsuit, like:
         * - A Heartbeat function that can be turned on/off
         * - A function to read in and register all .tact patterns in the bHaptics subfolder
         * - A logging hook to output to the Melonloader log
         * - 
         * */
        public bool suitDisabled = true;
        public bool systemInitialized = false;
        // Event to start and stop the heartbeat thread
        private static ManualResetEvent HeartBeat_mrse = new ManualResetEvent(false);
        // dictionary of all feedback patterns found in the bHaptics directory
        public Dictionary<String, FileInfo> FeedbackMap = new Dictionary<String, FileInfo>();

        public int heartbeatCount = 0;

        public void HeartBeatFunc()
        {
            while (true)
            {
                // Check if reset event is active
                HeartBeat_mrse.WaitOne();
                PlaybackHaptics("HeartBeat");
                if (heartbeatCount > 15)
                {
                    StopHeartBeat();
                }
                heartbeatCount++;
                Thread.Sleep(600);
            }
        }

        public TactsuitVR()
        {
            LOG("Initializing suit");
            var res = BhapticsSDK2.Initialize("65b5fb3743994972a944c895", "GCqINqyyMOlZe73leTkI");

            if (res > 0)
            {
                LOG("Failed to do bhaptics initialization...");
            }
            LOG("Starting HeartBeat thread...");
            Thread HeartBeatThread = new Thread(HeartBeatFunc);
            HeartBeatThread.Start();
            PlaybackHaptics("HeartBeat");
        }

        public void LOG(string logStr)
        {
            MelonLogger.Msg(logStr);
        }



        public void PlaybackHaptics(String key, float intensity = 1.0f, float duration = 1.0f, float xzAngle = 0f, float yShift = 0f)
        {
            int res;
            res = BhapticsSDK2.Play(key.ToLower(), intensity, duration, xzAngle, yShift);
            //if (res > 0) LOG("Playback failed: " + key + " " + key.ToLower());
        }


        public void Recoil(string weaponName, bool isRightHand, bool twoHanded = false, float intensity = 1.0f)
        {
            // weaponName is a parameter that will go into the vest feedback pattern name
            // isRightHand is just which side the feedback is on
            // intensity should usually be between 0 and 1

            // make postfix according to parameter
            string postfix = "_L";
            if (isRightHand) { postfix = "_R"; }

            //string key = "Recoil" + weaponName + "Vest" + postfix;
            string key = "Recoil" + postfix;
            PlaybackHaptics(key);
        }

        public void FootStep(bool isRightFoot)
        {
            if (!BhapticsSDK2.IsDeviceConnected(PositionType.FootL)) { return; }
            string postfix = "_L";
            if (isRightFoot) { postfix = "_R"; }
            string key = "FootStep" + postfix;
            PlaybackHaptics(key);
        }

        public void PlayBackHit(String key, float xzAngle, float yShift)
        {
            // two parameters can be given to the pattern to move it on the vest:
            // 1. An angle in degrees [0, 360] to turn the pattern to the left
            // 2. A shift [-0.5, 0.5] in y-direction (up and down) to move it up or down
            BhapticsSDK2.Play(key.ToLower(), 1f, 1f, xzAngle, yShift);
        }

        public void StartHeartBeat()
        {
            HeartBeat_mrse.Set();
        }

        public void StopHeartBeat()
        {
            HeartBeat_mrse.Reset();
            heartbeatCount = 0;
        }

        public bool IsPlaying(String effect)
        {
            return BhapticsSDK2.IsPlaying(effect.ToLower());
        }

        public void StopHapticFeedback(String effect)
        {
            BhapticsSDK2.Stop(effect.ToLower());
        }

        public void StopAllHapticFeedback()
        {
            StopThreads();
            BhapticsSDK2.StopAll();
        }

        public void StopThreads()
        {
            // Yes, looks silly here, but if you have several threads like this, this is
            // very useful when the player dies or starts a new level
            StopHeartBeat();
        }


    }
}
