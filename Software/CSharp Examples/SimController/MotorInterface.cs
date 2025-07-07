using sFndCLIWrapper;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;

namespace SimController
{
    public class CommandedState
    {
        public double yawRateCountsPerSecond;
        public int yawPositionCounts;
        public int pitchPositionCounts;
        public int rollPositionCounts;
    }
    public class SimulatorCommand
    {
        public string Name { get; set; } = "";
        public CommandedState Data { get; set; } = new CommandedState();
    }

    internal class MotorInterface
    {
        private readonly BlockingCollection<SimulatorCommand> _commandQueue = new();
        private readonly CancellationTokenSource _cts = new();

        const int HOMING_TIMEOUT_MS = 60000;

        private SimulatorState simulatorState;

        private cliSysMgr? myMgr;
        private string? comHubPort;
        private cliIPort? myPort;
        private cliINode[]? myNodes;

        private const int yawNodeIndex = 2;
        private const int pitchNodeIndex = 1;
        private const int rollNodeIndex = 0;

        private int maxAcceleration = 5000; // Max acceleration in RPM / s for normal motion
        private int maxVelocity = 2000; // Max velocity in RPM / s for normal motion

        public event Action<string>? StatusChanged;
        public event Action<SimulatorState>? StateChanged;
        public readonly IProgress<string> StatusReporter;
        public readonly IProgress<SimulatorState> StateReporter;

        public const int CountsPerRevolution = 32000;

        public MotorInterface() {
            StatusReporter = new Progress<string>(msg => StatusChanged?.Invoke(msg));
            StateReporter = new Progress<SimulatorState>(state => StateChanged?.Invoke(state));
            simulatorState = new SimulatorState();
        }

        public void Start()
        {
            List<String> comHubPorts = new List<String>();

            if (simulatorState.portConnected) {
                throw new Exception("Attempting to initialize the motor interface, but it has already been initialized.");
            }

            Task.Run(() =>
            {
                try
                {
                    //Create the SysManager object. This object will coordinate actions among various ports
                    // and within nodes. In this example we use this object to setup and open our port.
                    myMgr = new cliSysMgr();

                    StatusReporter.Report("Connecting to motion simulator.");
                    myMgr.FindComHubPorts(comHubPorts);

                    if (comHubPorts.Count != 1)
                    {
                        StatusReporter.Report("Error, expected 1 ComHub port, found " + comHubPorts.Count + ".");
                        return;
                    }

                    comHubPort = comHubPorts[0];
                    myMgr.ComPortHub(0, comHubPort, cliSysMgr._netRates.MN_BAUD_48X);
                    myMgr.PortsOpen(1);
                    myPort = myMgr.Ports(0);

                    if (myPort.NodeCount() != 3)
                    {
                        StatusReporter.Report("Error, expected 3 motors, found " + myPort.NodeCount() + ".");
                        myMgr.PortsClose();
                        myPort.Dispose();
                        myMgr.Dispose();
                        return;
                    }
                    myNodes = new cliINode[myPort.NodeCount()];
                    for (int n = 0; n < myNodes.Length; n++)
                    {
                        myNodes[n] = myPort.Nodes(n);
                        myNodes[n].Info.Ex.Parameter(98, 1);    // Configures the node to accept interuppting moves (instead of buffering).
                    }

                    StatusReporter.Report("Connected.");
                    simulatorState.portConnected = true;
                    StateReporter.Report(simulatorState);

                    ProcessCommands();
                }
                catch (Exception e)
                {
                    StatusReporter.Report("Failed to open simulator COM port. " + e.Message);
                    return;
                }
            });
        }

        public void EnqueueCommand(SimulatorCommand command)
        {
            _commandQueue.Add(command);
        }

        public void Stop()
        {
            _cts.Cancel();
            _commandQueue.CompleteAdding();
        }

        private void ProcessCommands()
        {
            try
            {
                foreach (SimulatorCommand cmd in _commandQueue.GetConsumingEnumerable(_cts.Token))
                {
                    string result = HandleCommand(cmd);
                }
            }
            catch (OperationCanceledException)
            {
                // TODO: Graceful shutdown
            }
        }

        private string HandleCommand(SimulatorCommand cmd)
        {
            if (cmd == null)
            {
                throw new ArgumentNullException(nameof(cmd), "Command cannot be null.");
            }

            if (cmd.Name == "GetState")
            {
                PollMotors();
            } else if (cmd.Name == "ZeroAllMotors")
            {
                ZeroAllMotors();
            } else if (cmd.Name == "EnableMotors")
            {
                EnableMotors();
            } else if (cmd.Name == "DisableMotors")
            {
                DisableMotors();
            } else if (cmd.Name == "ClearAlarms")
            {
                ClearAlarms();
            } else if (cmd.Name == "GotoZero")
            {
                GotoZero();
            } else if (cmd.Name == "StartUnmonitoredMove")
            {
                if (cmd.Data == null)
                {
                    throw new ArgumentException("Command data cannot be null for StartUnmonitoredMove.");
                }
                StartUnmonitoredMove(cmd.Data.yawRateCountsPerSecond, cmd.Data.pitchPositionCounts, cmd.Data.rollPositionCounts);
            } else
            {
                throw new ArgumentException($"Unknown command: {cmd.Name}");
            }

            return $"Handled command: {cmd.Name}";
        }

        private void PollMotors()
        {
            if (myNodes != null && myNodes.Length == 3)
            {
                myNodes[yawNodeIndex].Motion.PosnMeasured.Refresh();
                myNodes[pitchNodeIndex].Motion.PosnMeasured.Refresh();
                myNodes[rollNodeIndex].Motion.PosnMeasured.Refresh();
                myNodes[yawNodeIndex].Motion.VelMeasured.Refresh();

                int yawPosition = (int)myNodes[yawNodeIndex].Motion.PosnMeasured.Value();
                int pitchPosition = (int)myNodes[pitchNodeIndex].Motion.PosnMeasured.Value();
                int rollPosition = (int)myNodes[rollNodeIndex].Motion.PosnMeasured.Value();
                int yawRate = (int)myNodes[yawNodeIndex].Motion.VelMeasured.Value();

                simulatorState.yawCounts = yawPosition;
                simulatorState.pitchCounts = pitchPosition;
                simulatorState.rollCounts = rollPosition;
                simulatorState.yawRate = yawRate;

                StateReporter.Report(simulatorState);
            }
        }
        private void ZeroAllMotors()
        {
            if (simulatorState.portConnected && myMgr != null && myPort != null && myNodes != null)
            {
                Console.WriteLine("Port {0}: state={1}, nodes={2}", myPort.NetNumber(), myPort.OpenState(), myPort.NodeCount());

                simulatorState.motorsEnabled = false;
                simulatorState.motorsHomed = false;
                simulatorState.homingInProgress = true;
                StateReporter.Report(simulatorState);

                //Once the code gets past this point, it can be assumed that the Port has been opened without issue
                //Now we can get a reference to our port object which we will use to access the node objects
                for (int n = 0; n < myNodes.Length; n++)
                {                       
                    myNodes[n].EnableReq(false);
                    myMgr.Delay(200);

                    Console.WriteLine("   Node[{0}]: type={1}", n, myNodes[n].Info.NodeType());
                    Console.WriteLine("            userID: {0}", myNodes[n].Info.UserID);
                    Console.WriteLine("        FW version: {0}", myNodes[n].Info.FirmwareVersion.Value());
                    Console.WriteLine("          Serial #: {0}", myNodes[n].Info.SerialNumber.Value());
                    Console.WriteLine("             Model: {0}", myNodes[n].Info.Model.Value());

                    // The following statements will attempt to enable the node.  First,
                    // any shutdowns or NodeStops are cleared, finally the node is enabled
                    myNodes[n].Status.AlertsClear();
                    myNodes[n].Motion.NodeStopClear();
                    myNodes[n].EnableReq(true);
                    Console.WriteLine("Node {0} enabled.", n);
                    double timeout = myMgr.TimeStampMsec() + HOMING_TIMEOUT_MS;     // Define a timeout in case the node is unable to enable
                                                                                    // This will loop checking on the Real time values of the node's Ready status
                    while (!myNodes[n].Motion.IsReady())
                    {
                        if (myMgr.TimeStampMsec() > timeout)
                        {
                            Console.WriteLine("Error: Timed out waiting for Node {0} to enable.", n);
                        }
                    }

                    if (myNodes[n].Motion.Homing.HomingValid())
                    {
                        if (myNodes[n].Motion.Homing.WasHomed())
                        {
                            Console.WriteLine("Node {0} has already been homed, current position is: {1} ", n, myNodes[n].Motion.PosnMeasured.Value());
                            Console.WriteLine("Rehoming Node... \n");
                        }
                        else
                        {
                            Console.WriteLine("Node [{0}] has not been homed.  Homing Node now...", n);
                        }
                        // Now we will home the Node
                        myNodes[n].Motion.Homing.Initiate();

                        timeout = myMgr.TimeStampMsec() + HOMING_TIMEOUT_MS;    // Define a timeout in case the node is unable to enable
                                                                                // Basic mode - Poll until disabled
                        while (!myNodes[n].Motion.Homing.WasHomed())
                        {
                            if (myMgr.TimeStampMsec() > timeout)
                            {
                                Console.WriteLine("Node did not complete homing:  \n\t -Ensure Homing settings have been defined through ClearView. \n\t -Check for alerts/Shutdowns \n\t -Ensure timeout is longer than the longest possible homing move.");

                            }
                        }
                        myNodes[n].Motion.PosnMeasured.Refresh();      // Refresh our current measured position
                        Console.WriteLine("Node completed homing, current position: {0} ", myNodes[n].Motion.PosnMeasured.Value());
                        Console.WriteLine("Soft limits now active");
                    }
                    else
                    {
                        Console.WriteLine("Node[{0}] has not had homing setup through ClearView.  The node will not be homed.", n);
                    }

                    // Disable the node
                    Console.WriteLine("Disabling Node.");
                    myNodes[n].EnableReq(false);
                }

                simulatorState.motorsHomed = true;
                simulatorState.homingInProgress = false;
                StateReporter.Report(simulatorState);
            }
        }

        private void EnableMotors()
        {
            if (!simulatorState.portConnected || myNodes == null)
            {
                throw new Exception("Call Start() before attempting to enable motors.");
            }

            for (int n = 0; n < myNodes.Length; n++)
            {
                myNodes[n].EnableReq(true);
            }
            simulatorState.motorsEnabled = true;
            StateReporter.Report(simulatorState);
        }

        private void DisableMotors()
        {
            if (!simulatorState.portConnected || myNodes == null)
            {
                throw new Exception("Call Start() before attempting to enable motors.");
            }

            for (int n = 0; n < myNodes.Length; n++)
            {
                myNodes[n].EnableReq(false);
            }
            simulatorState.motorsEnabled = false;
            StateReporter.Report(simulatorState);
        }

        private void ClearAlarms()
        {
            if (!simulatorState.portConnected || myNodes == null)
            {
                throw new Exception("Cannot clear alarms. Not connected.");
            }

            for (int n = 0; n < myNodes.Length; n++)
            {
                myNodes[n].Status.AlertsClear();
                myNodes[n].Motion.NodeStopClear();
            }
        }

        private void AssertReadyToMove()
        {
            if (!simulatorState.portConnected || myNodes == null || myPort == null || myMgr == null)
            {
                throw new Exception("Call Start() before attempting to move motors.");
            }

            if (!simulatorState.motorsHomed)
            {
             //   throw new Exception("Motors must be homed first.");
            }

            if (!simulatorState.motorsEnabled)
            {
                throw new Exception("Motors must be enabled to move to home.");
            }

            if (simulatorState.movingToZero)
            {
                throw new Exception("Moving to zero.");
            }
        }

        private void GotoZero()
        {
            AssertReadyToMove();

            if (myNodes != null && myMgr != null)
            {
                simulatorState.movingToZero = true;
                StateReporter.Report(simulatorState);

                // Clear the rising edge Move done register on all axes and set movement units
                List<double> timeouts = new List<double>(3);
                for (int n = 0; n < myNodes.Length; n++)
                {
                    myNodes[n].Motion.MoveWentDone();
                    myNodes[n].AccUnit(cliINode._accUnits.RPM_PER_SEC);         // Set the units for Acceleration to RPM/SEC
                    myNodes[n].VelUnit(cliINode._velUnits.RPM);                 // Set the units for Velocity to RPM
                    myNodes[n].Motion.AccLimit.Value(1000);      // Set Acceleration Limit (RPM/Sec)
                    myNodes[n].Motion.VelLimit.Value(200);              // Set Velocity Limit (RPM)

                    myNodes[n].Motion.PosnMeasured.Refresh();      // Refresh our current measured position
                    int currentPosition = (int)Math.Round(myNodes[n].Motion.PosnMeasured.Value());
                    myNodes[n].Motion.MovePosnStart(0, true, false);
                    double timeout = myMgr.TimeStampMsec() + myNodes[n].Motion.MovePosnDurationMsec(Math.Abs(currentPosition), false) + 1000;
                    timeouts.Add(timeout);
                }

                simulatorState.movingToZero = false;
                StateReporter.Report(simulatorState);

                return;
                /*
                Boolean finishedOrTimedOut = false;
                while (!finishedOrTimedOut)
                {
                    for (int n = 0; n < myNodes.Length; n++)
                    {

                    }
                }
                */
            }
        }

        private void StartUnmonitoredMove(double yawRateCountsPerSecond, int pitchPositionCounts, int rollPositionCounts)
        {
            AssertReadyToMove();

            if (myNodes != null)
            {
                for (int n = 0; n < myNodes.Length; n++)
                {
                    myNodes[n].Motion.MoveWentDone();
                    myNodes[n].AccUnit(cliINode._accUnits.RPM_PER_SEC);         // Set the units for Acceleration to RPM/SEC
                    myNodes[n].VelUnit(cliINode._velUnits.RPM);                 // Set the units for Velocity to RPM
                    myNodes[n].Motion.AccLimit.Value(maxAcceleration);      // Set Acceleration Limit (RPM/Sec)
                    myNodes[n].Motion.VelLimit.Value(maxVelocity);              // Set Velocity Limit (RPM)

                    if (n == yawNodeIndex)
                    {
                        myNodes[n].Motion.MoveVelStart(yawRateCountsPerSecond / CountsPerRevolution * 60);
                    }
                    else if (n == pitchNodeIndex)
                    {
                        myNodes[n].Motion.MovePosnStart(pitchPositionCounts, true, false);
                    }
                    else if (n == rollNodeIndex)
                    {
                        myNodes[n].Motion.MovePosnStart(rollPositionCounts, true, false);
                    }
                }
            }
        }

        public void Dispose()
        {
            Task.Run(() =>
            {
                // Disable all nodes (motors) and close the ports
                if (myPort != null && myNodes != null)
                {
                    for (int n = 0; n < myPort.NodeCount(); n++)
                    {
                        // Create a shortcut reference for a node
                        myNodes[n] = myPort.Nodes(n);
                        myNodes[n].EnableReq(false);
                        myNodes[n].Dispose();
                    }
                }

                myMgr?.PortsClose();
                myPort?.Dispose();
                myMgr?.Dispose();

                simulatorState.portConnected = false;
                simulatorState.motorsEnabled = false;
                simulatorState.motorsHomed = false;
                StateReporter.Report(simulatorState);
            });            
        }
    }
}
