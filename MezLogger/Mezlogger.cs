using UnityEngine;
using UnityEngine.UI;
using MelonLoader;
using System.Collections;
using TMPro;

namespace MezLogger
{
    public static class MezLogger
    {
        internal static string _clientName = "MezLogger"; //Client Name or Mod Name, will be whatever text displays before the message.

        internal static string _primaryColour = "#6A329F"; //Colour of text "Client name"

        internal static string _secondaryColour = "#a1dcff"; //Text Colour of the text that comes after the mods title

        internal static Vector3 _uiPosition = new Vector3(-20, -300, 0); //UI Position on screen (by default its right beside the mute button on the HUD - bare in mind the text has a slight curve to it the further you get away from the center)

        internal static float _textSpacing = 25f; //Spacing between multiple notifications (Try not to set it too low to avoid text overlapping, I find this number works just fine)

        internal static GameObject LogGUI;
        internal static IEnumerator MakeUI() //You're going to want to call MelonCoroutines.Start(MezLogger.MakeUI()); or whatever corresponds in your own project to this coroutine on application start to init MezLogger
        {
            while (RoomManager.field_Internal_Static_ApiWorld_0 == null)
                yield return new WaitForSeconds(1f);

            LogGUI = UnityEngine.Object.Instantiate(GameObject.Find("/UserInterface").transform.Find("UnscaledUI/HudContent_Old/Hud/AlertTextParent/Capsule").gameObject, GameObject.Find("/UserInterface").transform.Find("UnscaledUI/HudContent_Old/Hud/AlertTextParent"));
            LogGUI.SetActive(true);
            LogGUI.name = "MezLogParent";
            GameObject text = LogGUI.transform.Find("Text").gameObject;

            yield return new WaitForEndOfFrame();

            LogGUI.transform.localPosition = _uiPosition;

            GameObject.DestroyImmediate(LogGUI.transform.GetComponent<HorizontalLayoutGroup>());
            GameObject.DestroyImmediate(LogGUI.transform.GetComponent<ContentSizeFitter>());
            GameObject.DestroyImmediate(LogGUI.transform.GetComponent<ContentSizeFitter>());
            //GameObject.DestroyImmediate(LogGUI.transform.GetComponent<ImageThreeSlice>());

            LogGUI.gameObject.AddComponent<ContentSizeFitter>().horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            LogGUI.gameObject.AddComponent<VerticalLayoutGroup>().spacing = _textSpacing;

            TextMeshProUGUI textMesh = text.GetComponent<TextMeshProUGUI>();
            textMesh.alignment = TextAlignmentOptions.Left;
            textMesh.text = $"<color={_primaryColour}>[{_clientName}]</color> ";

            yield return new WaitForEndOfFrame();

            text.SetActive(false);
        }

        //Works just like melonlogger.msg , error or warn. You would want to write MezLogger.Msg , error or warn. EX: MezLogger.Warn("Message Text", 2.5f); the float at the end being the time it displays on the HUD for.
        internal static void Msg(string text, float timer)
        {
            MelonCoroutines.Start(MezText(text, 1, timer));
        }

        internal static void Error(string text, float timer)
        {
            MelonCoroutines.Start(MezText(text, 2, timer));
        }

        internal static void Warn(string text, float timer)
        {
            MelonCoroutines.Start(MezText(text, 3, timer));
        }

        private static IEnumerator MezText(string text, int textType, float timeBeforeDeletion)
        {
            TextMeshProUGUI textMesh;
            try
            {
                GameObject textObj = Object.Instantiate(GameObject.Find("UserInterface/UnscaledUI/HudContent_Old/Hud/AlertTextParent/Capsule/Text"), LogGUI.transform);
                textMesh = textObj.GetComponent<TextMeshProUGUI>();
                MelonCoroutines.Start(FadeInFadeOut(textMesh, false));
                textMesh.text +=
                    textType switch
                    {
                        1 => "",
                        2 => "<color=red>[ERROR]</color> ",
                        3 => "<color=yellow>[Warning]</color> ",
                        _ => "Something broke whoops"
                    } + $"<color={_secondaryColour}>{text}</color>";

                textObj.SetActive(true);
            }
            catch
            {
                yield break;
            }

            yield return new WaitForSeconds(timeBeforeDeletion);
            MelonCoroutines.Start(FadeInFadeOut(textMesh, true));
        }
        //thanks https://github.com/MimicVirus
        private static IEnumerator FadeInFadeOut(TextMeshProUGUI textMesh, bool shouldDestroy)
        {
            if (shouldDestroy)
            {
                for (float index = 1; index >= 0; index -= Time.deltaTime)
                {
                    if (textMesh != null) textMesh.color = new Color(1f, 1f, 1f, index);
                    yield return null;
                }

                Object.Destroy(textMesh.gameObject);
            }

            else
            {
                for (float index = 0; index <= 1; index += Time.deltaTime)
                {
                    if (textMesh != null) textMesh.color = new Color(1f, 1f, 1f, index);
                    yield return null;
                }
            }
        }
    }
}