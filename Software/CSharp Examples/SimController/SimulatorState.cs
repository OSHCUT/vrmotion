using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimController
{
    internal class SimulatorState
    {
        public Boolean portConnected = false;
        public Boolean motorsEnabled = false;
        public Boolean motorsHomed = false;
        public Boolean homingInProgress = false;

        public long yawCounts = 0;
        public long pitchCounts = 0;
        public long rollCounts = 0;
        public double yaw = 0;
        public double pitch = 0;
        public double roll = 0;

        public double yawRate = 0;
        public double pitchRate = 0;
        public double rollRate = 0;

        public double yawTorque = 0;
        public double pitchTorque = 0;
        public double rollTorque = 0;

        public double yawCommandedRate = 0;
        public double pitchCommandedRate = 0;
        public double rollCommandedRate = 0;

        public double yawCommandedPosition = 0;
        public double pitchCommandedPosition = 0;
        public double rollCommandedPosition = 0;

        public long yawCountsError = 0;
        public long pitchCountsError = 0;
        public long rollCountsError = 0;

        public SimulatorState()
        {

        }
        public SimulatorState(SimulatorState state)
        {
            portConnected = state.portConnected;
            motorsEnabled = state.motorsEnabled;
            motorsHomed = state.motorsHomed;
            homingInProgress = state.homingInProgress;

            yawCounts = state.yawCounts;
            pitchCounts = state.pitchCounts;
            rollCounts = state.rollCounts;

            yaw = state.yaw;
            pitch = state.pitch;
            roll = state.roll;

            yawRate = state.yawRate;
            pitchRate = state.pitchRate;
            rollRate = state.rollRate;

            yawTorque = state.yawTorque;
            pitchTorque = state.pitchTorque;
            rollTorque = state.rollTorque;

            yawCommandedRate = state.yawCommandedRate;
            pitchCommandedRate = state.pitchCommandedRate;
            rollCommandedRate = state.rollCommandedRate;

            yawCommandedPosition = state.yawCommandedPosition;
            pitchCommandedPosition = state.pitchCommandedPosition;
            rollCommandedPosition = state.rollCommandedPosition;

            yawCountsError = state.yawCountsError;
            pitchCountsError = state.pitchCountsError;
            rollCountsError = state.rollCountsError;
        }
    }
}
