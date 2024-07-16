using System;
using BepInEx;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using Utilla;

namespace GorillaChat
{
	/// <summary>
	/// This is your mod's main class.
	/// </summary>

	/* This attribute tells Utilla to look for [ModdedGameJoin] and [ModdedGameLeave] */
	[ModdedGamemode]
	[BepInDependency("org.legoandmars.gorillatag.utilla", "1.5.0")]
	[BepInPlugin(PluginInfo.GUID, PluginInfo.Name, PluginInfo.Version)]
	public class Plugin : BaseUnityPlugin, IOnEventCallback
    {
		bool inRoom;
        private string currentMessage = "";
        private string chatLog = "";
        private Vector2 scrollPosition;

        private void Start()
        {
            PhotonNetwork.AddCallbackTarget(this);
        }

        private void OnDestroy()
        {
            PhotonNetwork.RemoveCallbackTarget(this);
        }

        private void OnGUI()
        {
            GUILayout.BeginArea(new Rect(10, 10, 300, 500));

            // Display chat log
            scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Width(300), GUILayout.Height(400));
            GUILayout.Label(chatLog);
            GUILayout.EndScrollView();

            // Input field for typing messages
            GUILayout.BeginHorizontal();
            currentMessage = GUILayout.TextField(currentMessage, GUILayout.Width(200));

            // Send button
            if (GUILayout.Button("Send", GUILayout.Width(80)))
            {
                SendMessage();
            }
            GUILayout.EndHorizontal();

            GUILayout.EndArea();
        }

        public void SendMessage()
        {
            if (!string.IsNullOrEmpty(currentMessage))
            {
                object[] content = new object[] { PhotonNetwork.NickName, currentMessage };
                RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
                PhotonNetwork.RaiseEvent(ChatEventCodes.ChatMessage, content, raiseEventOptions, SendOptions.SendReliable);
                currentMessage = "";
            }
        }

        public void OnEvent(EventData photonEvent)
        {
            if (photonEvent.Code == ChatEventCodes.ChatMessage)
            {
                object[] data = (object[])photonEvent.CustomData;
                string sender = (string)data[0];
                string message = (string)data[1];
                chatLog += $"{sender}: {message}\n";
            }
        }
        void OnEnable()
		{
			/* Set up your mod here */
			/* Code here runs at the start and whenever your mod is enabled*/

			HarmonyPatches.ApplyHarmonyPatches();
		}

		void OnDisable()
		{
			/* Undo mod setup here */
			/* This provides support for toggling mods with ComputerInterface, please implement it :) */
			/* Code here runs whenever your mod is disabled (including if it disabled on startup)*/

			HarmonyPatches.RemoveHarmonyPatches();
		}

		void OnGameInitialized(object sender, EventArgs e)
		{
			/* Code here runs after the game initializes (i.e. GorillaLocomotion.Player.Instance != null) */
		}

		/* This attribute tells Utilla to call this method when a modded room is joined */
		[ModdedGamemodeJoin]
		public void OnJoin(string gamemode)
		{
			/* Activate your mod here */
			/* This code will run regardless of if the mod is enabled*/

			inRoom = true;
		}

		/* This attribute tells Utilla to call this method when a modded room is left */
		[ModdedGamemodeLeave]
		public void OnLeave(string gamemode)
		{
			/* Deactivate your mod here */
			/* This code will run regardless of if the mod is enabled*/

			inRoom = false;
		}
	}
    public static class ChatEventCodes
    {
        public const byte ChatMessage = 198;
    }
}
