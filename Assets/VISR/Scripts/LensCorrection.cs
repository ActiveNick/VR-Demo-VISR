using UnityEngine;
using System.Collections;

namespace VisrSdk
{

    [ExecuteInEditMode]
    public class LensCorrection : MonoBehaviour
    {
        private Material material;
        VisrCamera visrCamera;

        // Creates a private material used to the effect
        void Awake()
        {
            if(!Application.isEditor)
                Configuration.CheckConfiguration();

            visrCamera = GetComponentInParent<VisrCamera>();

            Shader shader = visrCamera.DistortionShader;

            if (material != null)
                DestroyImmediate(material);

            if (shader == null)
                Debug.LogError("Shader was not found");
            else
                material = new Material(shader);
        }

        void Start()
        {
            if (!Application.isEditor)
                Configuration.CheckConfiguration();
            setShaderParams();
        }

        Vector2 getViewSize()
        {
#if UNITY_EDITOR
            System.Type T = System.Type.GetType("UnityEditor.GameView,UnityEditor");
            System.Reflection.MethodInfo GetSizeOfMainGameView = T.GetMethod("GetSizeOfMainGameView", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            System.Object Res = GetSizeOfMainGameView.Invoke(null, null);
            return (Vector2)Res;
#else
            return new Vector2(Screen.width, Screen.height);
#endif
        }

        void setShaderParams()
        {
            Camera camera = GetComponent<Camera>();
            var config = GetComponentInParent<VisrCamera>();

            setRectForTTL(camera, config);

            float pixelsToDegrees = 1.0f / camera.fieldOfView;


            if(transform.localPosition.x > 0)
            {
                transform.localPosition = new Vector3(config.IPD * 0.0005f, 0, 0);
            }
            else
            {
                transform.localPosition = new Vector3(config.IPD * -0.0005f, 0, 0);
            }

            float xOffset = config.ILD - 60;

            material.SetFloat("_XOffset", transform.localPosition.x > 0 ? -xOffset : xOffset);

            material.SetFloat("_K1", config.K1 * config.DistortionScale);
            material.SetFloat("_K2", config.K2 * config.DistortionScale);
            material.SetFloat("_K3", config.K3 * config.DistortionScale);
            material.SetFloat("_K4", config.K4 * config.DistortionScale);
        }

        void setRectForTTL(Camera camera, VisrCamera config)
        {
            Vector2 screenSizePixels = getViewSize(); //we cache this cause it's actually kinda expensive
            float inchesToMm = 25.4f;
            float pixPerMm = Screen.dpi / inchesToMm;

            if (pixPerMm == 0)
                Debug.LogError("Screen DPI value not valid, skipping lens config");

            //the vertical position of the middle of the screen in mm
            float centerHeight = (screenSizePixels.y / 2) / pixPerMm;
            float heightOffset = centerHeight - config.TTL;
            float heightOffsetClipSpace = heightOffset / screenSizePixels.y;

            //float lensHeight = () * config.TTL;
            //float ttl = lensHeight / getViewSize().y;

            camera.rect = new Rect(camera.rect.x, -heightOffsetClipSpace, camera.rect.width, camera.rect.height);
        }

        // Postprocess the image
        [ImageEffectOpaque]
        void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            if (material == null)
                return;

            if (Application.isEditor)
                setShaderParams();

            Graphics.Blit(source, destination, material);
        }

    }

}