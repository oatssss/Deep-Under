using UnityEngine;

namespace Extensions
{
    public static class ExtensionMethods {

        public static void EnforceLayerMembership(this MonoBehaviour script, string layer)
        {
            if (script.gameObject.layer != LayerMask.NameToLayer(layer))
            {
                Debug.LogWarningFormat("[{1}] does not belong to the \"{0}\" layer. Assigning now...", layer, script.gameObject);
                script.gameObject.layer = LayerMask.NameToLayer(layer);
            }
        }
    }
}