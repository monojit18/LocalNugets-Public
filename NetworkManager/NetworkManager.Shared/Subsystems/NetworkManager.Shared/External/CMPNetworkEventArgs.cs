﻿using System;

namespace Subsystems.NetworkManager.Shared.External
{

    public enum NetworkState
    {

        Unknown,
        NoNetwork,
        WifiNetwork,
        CarrierNetwork

    }

    public class CMPNetworkEventArgs : EventArgs
    {

        public NetworkState State { get; private set; }      

        public CMPNetworkEventArgs(NetworkState state)
        {

            State = state;

        }
    }
}
