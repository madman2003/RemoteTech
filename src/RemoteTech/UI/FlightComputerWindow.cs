﻿using System;
using UnityEngine;

namespace RemoteTech.UI
{
    public class FlightComputerWindow : AbstractWindow
    {
        private enum FragmentTab
        {
            Attitude = 0,
            Rover = 1,
            Power = 2,
        }

        private FragmentTab mTab = FragmentTab.Attitude;
        private readonly AttitudeFragment mAttitude;
        private readonly RoverFragment mRover;
        private readonly QueueFragment mQueue;
        private readonly PowerFragment mPower;
        private bool mQueueEnabled;
        private FlightComputer.FlightComputer mFlightComputer;
        private readonly String tabModeDescString = "Switch to Attitude, Rover or Power mode.";
        private static readonly String appTitle = "Flight Computer";

        private FragmentTab Tab
        {
            get
            {
                return mTab;
            }
            set
            {
                int NumberOfTabs = 3;
                if ((int)value >= NumberOfTabs) {
                    mTab = (FragmentTab)0;
                } else if ((int)value < 0) {
                    mTab = (FragmentTab)(NumberOfTabs - 1);
                } else {
                    mTab = value;
                }
            }
        }

        public FlightComputerWindow(FlightComputer.FlightComputer fc)
            : base(Guid.NewGuid(), appTitle, new Rect(100, 100, 0, 0), WindowAlign.Floating)
        {
            mSavePosition = true;
            mFlightComputer = fc;
            mAttitude = new AttitudeFragment(fc, () => OnQueue());
            mRover = new RoverFragment(fc, () => OnQueue());
            mPower = new PowerFragment(fc, () => OnQueue());
            mQueue = new QueueFragment(fc);
            mQueueEnabled = false;
        }

        public override void Show()
        {
            base.Show();
            mFlightComputer.OnActiveCommandAbort += mAttitude.Reset;
            mFlightComputer.OnNewCommandPop += mAttitude.getActiveFlightMode;

            mAttitude.getActiveFlightMode();
            mPower.getActivePowerMode();
        }

        public override void Hide()
        {
            mFlightComputer.OnActiveCommandAbort -= mAttitude.Reset;
            mFlightComputer.OnNewCommandPop -= mAttitude.getActiveFlightMode;
            base.Hide();
        }

        public override void Window(int id)
        {
            GUI.skin = null;
            GUILayout.BeginHorizontal();
            {
                GUILayout.BeginHorizontal();
                {
                    switch (mTab) {
                        case FragmentTab.Attitude:
                            mAttitude.Draw();
                            break;
                        case FragmentTab.Rover:
                            mRover.Draw();
                            break;
                        case FragmentTab.Power:
                            mPower.Draw();
                            break;
                    }
                }
                GUILayout.EndHorizontal();

                // RoverComputer
                if (GUI.Button(new Rect(2, 2, 16, 16), new GUIContent("<", tabModeDescString))) {
                    Tab--;
                }
                if (GUI.Button(new Rect(16, 2, 16, 16), new GUIContent(">", tabModeDescString))) {
                    Tab++;
                }

                if (mQueueEnabled) {
                    mQueue.Draw();
                } else {
                    GUILayout.BeginVertical();
                    {
                        GUILayout.BeginScrollView(Vector2.zero, GUILayout.ExpandHeight(true));
                        GUILayout.EndScrollView();
                    }
                    GUILayout.EndVertical();
                }
            }
            GUILayout.EndHorizontal();
            base.Window(id);
        }

        private void OnQueue()
        {
            mQueueEnabled = !mQueueEnabled;
            if(mQueueEnabled)
            {
                this.Title = appTitle + ": " + mFlightComputer.Vessel.vesselName.Substring(0, Math.Min(25, mFlightComputer.Vessel.vesselName.Length));
            }
            else
            {
                this.Title = appTitle;
            }

        }
    }
}
