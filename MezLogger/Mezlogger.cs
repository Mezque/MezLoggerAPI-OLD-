using UnityEngine;
using UnityEngine.UI;
using MelonLoader;
using System.Collections;
using TMPro;
using Obj = UnityEngine.GameObject;

/*
 * CREDITS: Full Credits for the original code go to Mezque
 * SOURCE: https://github.com/Mezque/MezLoggerAPI
 */

namespace MezLogger
{
    public static class MezLogger
    {
        //Just a notice to other developers planning on using this in their mods or clients, you will need to be using at least C# V8 to use this. I use V9 in my own projects. 
        //If you run into issue its not too hard to fix it you will just want to go to your " csproj " file and add this line " <LangVersion>9</LangVersion> " below the <ProjectGUID> blablabla </ProjectGuid> line ( so that LangVer becomes line 8 )
        //Please feel free to report any problems you run into well using :)

        private static string _clientName = "MezLogger"; //Client Name or Mod Name, will be whatever text displays before the message.

        private static string _primaryColour = "#6A329F"; //Colour of text "Client name"

        private static string _secondaryColour = "#a1dcff"; //Text Colour of the text that comes after the mods title

        private static Vector3 _uiPosition = new Vector3(-20, -300, 0); //UI Position on screen (by default its right beside the mute button on the HUD - bare in mind the text has a slight curve to it the further you get away from the center)

        private static float _textSpacing = 25f; //Spacing between multiple notifications (Try not to set it too low to avoid text overlapping, I find this number works just fine)

        public static IEnumerator MakeUI() //You're going to want to call MelonCoroutines.Start(MezLogger.MakeUI()); or whatever corresponds in your own project to this coroutine on application start to init MezLogger
        {
            while (RoomManager.field_Internal_Static_ApiWorld_0 == null)
                yield return new WaitForSeconds(1f);

            GameObject gui;

            (gui = Obj.Find("/UserInterface").transform.Find("UnscaledUI/HudContent_Old/Hud/AlertTextParent/Capsule").gameObject).SetActive(true);
            GameObject text = gui.transform.Find("Text").gameObject;

            yield return new WaitForEndOfFrame();

            gui.transform.localPosition = _uiPosition;

            Obj.DestroyImmediate(gui.transform.GetComponent<HorizontalLayoutGroup>());
            Obj.DestroyImmediate(gui.transform.GetComponent<ContentSizeFitter>());
            Obj.DestroyImmediate(gui.transform.GetComponent<ContentSizeFitter>());
            Obj.DestroyImmediate(gui.transform.GetComponent<ImageThreeSlice>());

            gui.gameObject.AddComponent<ContentSizeFitter>().horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            gui.gameObject.AddComponent<VerticalLayoutGroup>().spacing = _textSpacing;

            TextMeshProUGUI textMesh = text.GetComponent<TextMeshProUGUI>();
            textMesh.alignment = TextAlignmentOptions.Left;
            textMesh.text = $"<color={_primaryColour}>[{_clientName}]</color> ";

            yield return new WaitForEndOfFrame();

            text.SetActive(false);
        }

        //Works just like melonlogger.msg , error or warn. You would want to write MezLogger.Msg , error or warn. EX: MezLogger.Warn("Message Text", 2.5f); the float at the end being the time it displays on the HUD for.
        public static void Msg(string text, float timer)
        {
            MelonCoroutines.Start(MezText(text, 1, timer));
        }

        public static void Error(string text, float timer)
        {
            MelonCoroutines.Start(MezText(text, 2, timer));
        }

        public static void Warn(string text, float timer)
        {
            MelonCoroutines.Start(MezText(text, 3, timer));
        }

        private static IEnumerator MezText(string text, int textType, float timeBeforeDeletion)
        {
            TextMeshProUGUI textMesh;
            try
            {
                GameObject textObj = Object.Instantiate(
                    GameObject.Find("UserInterface/UnscaledUI/HudContent_Old/Hud/AlertTextParent/Capsule/Text"),
                    GameObject.Find("UserInterface/UnscaledUI/HudContent_Old/Hud/AlertTextParent/Capsule").transform);
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