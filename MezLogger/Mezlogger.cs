using UnityEngine;
using UnityEngine.UI;
using MelonLoader;
using System.Collections;
using TMPro;
using Obj = UnityEngine.GameObject;

[assembly: MelonInfo(typeof(MezLogger.MezLogger), "MezLogger", "0.1.0", "Mezque", "github.com/Mezque/MezLogger")]

namespace MezLogger
{
    public class MezLogger : MelonMod
    {
        public override void OnApplicationStart() 
        {
            MelonCoroutines.Start(MakeUI());

            MelonLogger.MsgCallbackHandler += (_, __, a, b) => Msg($"[{a}] {b}");
            MelonLogger.WarningCallbackHandler += (a, b) => Msg($"[{a}] {b}");
            MelonLogger.ErrorCallbackHandler += (a, b) => Msg($"[{a}] {b}");
        }

        // Just a notice to other developers planning on using this in their mods or clients, you will need to be using at least C# V8 to use this. I use V9 in my own projects. 
        // If you run into issue its not too hard to fix it you will just want to go to your " csproj " file and add this line " <LangVersion>9</LangVersion> " below the <ProjectGUID> blablabla </ProjectGuid> line ( so that LangVer becomes line 8 )
        // Please feel free to report any problems you run into well using :)

        private static string ClientName = "MezLogger"; // Client Name or Mod Name, will be whatever text displays before the message.
        private static string PrimaryColour = "#6A329F"; // Colour of text "Client name"
        private static Color SecondaryColour = new Color(0f, 1f, 1f, 1f); // Text Colour of the text that comes after the mods title
        private static Vector3 UIPosition = new Vector3(-20, -300, 0); // UI Position on screen (by default its right beside the mute button on the HUD - bare in mind the text has a slight curve to it the further you get away from the center)
        private static float TextSpacing = 25f; // Spacing between multiple notifications (Try not to set it too low to avoid text overlapping, I find this number works just fine)

        public static IEnumerator MakeUI() // You're going to want to call MelonCoroutines.Start(MezLogger.MakeUI()); or whatever corresponds in your own project to this coroutine on application start to init MezLogger
        {
            while (RoomManager.field_Internal_Static_ApiWorld_0 == null)
                yield return new WaitForSeconds(1f);

            GameObject GUI, text;

            (GUI = Obj.Find("/UserInterface").transform.Find("UnscaledUI/HudContent/Hud/AlertTextParent/Capsule").gameObject).SetActive(true);
            text = GUI.transform.Find("Text").gameObject;

            yield return new WaitForEndOfFrame();

            GUI.transform.localPosition = UIPosition;

            Obj.DestroyImmediate(GUI.transform.GetComponent<HorizontalLayoutGroup>());
            Obj.DestroyImmediate(GUI.transform.GetComponent<ContentSizeFitter>());
            Obj.DestroyImmediate(GUI.transform.GetComponent<ContentSizeFitter>());
            Obj.DestroyImmediate(GUI.transform.GetComponent<ImageThreeSlice>());

            GUI.gameObject.AddComponent<ContentSizeFitter>().horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            GUI.gameObject.AddComponent<VerticalLayoutGroup>().spacing = TextSpacing;

            var textMesh = text.GetComponent<TextMeshProUGUI>();
            textMesh.color = SecondaryColour;
            textMesh.alignment = TextAlignmentOptions.Left;
            textMesh.text = $"<color={PrimaryColour}>[{ClientName}]</color> ";

            yield return new WaitForEndOfFrame();

            text.SetActive(false);
        }

        // Works just like melonlogger.msg , error or warn. You would want to write MezLogger.Msg , error or warn. EX: MezLogger.Warn("Message Text", 2.5f); the float at the end being the time it displays on the HUD for.
        public static void Msg(string Text, float Timer) => MelonCoroutines.Start(MezText(Text, 1, Timer));
        public static void Error(string Text, float Timer) => MelonCoroutines.Start(MezText(Text, 2, Timer));
        public static void Warn(string Text, float Timer) => MelonCoroutines.Start(MezText(Text, 3, Timer));

        private static IEnumerator MezText(string Text, int TextType, float timeBeforeDeletion)
        {
            try {
                var textObj = Object.Instantiate(Obj.Find("UserInterface/UnscaledUI/HudContent/Hud/AlertTextParent/Capsule/Text").gameObject, Obj.Find("UserInterface/UnscaledUI/HudContent/Hud/AlertTextParent/Capsule").transform);
                var textMesh = textObj.GetComponent<TextMeshProUGUI>();

                textMesh.text += TextType switch { 1 => "", 2 => "<color=red>[ERROR]</color> ", 3 => "<color=yellow>[Warning]</color> ", _ => "Something broke whoops" } + Text;
                textObj.SetActive(true);
                yield return new WaitForSeconds(timeBeforeDeletion);
                GameObject.Destroy(textObj);
            } catch {} 
        }
    }
}