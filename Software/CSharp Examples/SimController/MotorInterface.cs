using sFndCLIWrapper;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
using System.Xml.Linq;

namespace SimController
{
    public class CommandedState
    {
        public double yawRateCountsPerSecond = 0;
        public double pitchRateCountsPerSecond = 0;
        public double rollRateCountsPerSecond = 0;

        public int yawPositionCounts = 0;
        public int pitchPositionCounts = 0;
        public int rollPositionCounts = 0;

        public bool isVelocityCommand = false;
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

        private int maxAcceleration = 3000; // Max acceleration in RPM / s for normal motion
        private int maxVelocity = 2000; // Max velocity in RPM / s for normal motion

        public event Action<string>? StatusChanged;
        public event Action<SimulatorState>? StateChanged;
        public readonly IProgress<string> StatusReporter;
        public readonly IProgress<SimulatorState> StateReporter;

        public const int CountsPerRevolution = 32000;

        private const int yawCountsPerOutputRotation = (int)((double)CountsPerRevolution * (19.099 / 2.228) * 15);

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

//                    myPort.BrakeControl.BrakeSetting(0, sFndCLIWrapper.cliIBrakeControl._BrakeControls.BRAKE_PREVENT_MOTION);

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

                    EnableMotors();
                    myMgr.Delay(500);

                    ProcessCommands();
                }
                catch (Exception e)
                {
                    StatusReporter.Report(e.Message);
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
            while (!_cts.Token.IsCancellationRequested)
            {
                List<SimulatorCommand> batch = new();

                try
                {
                    // Block until at least one item is available
                    var first = _commandQueue.Take(_cts.Token);
                    batch.Add(first);

                    // Drain the rest (non-blocking)
                    while (_commandQueue.TryTake(out var next))
                    {
                        batch.Add(next);
                    }
                }
                catch (OperationCanceledException)
                {
                    break;
                }

                // Keep only the latest "StartUnmonitoredMove" command or "GetState" command, and preserve other types
                SimulatorCommand? latestMove = null;
                SimulatorCommand? latestPoll = null;
                foreach (var cmd in batch)
                {
                    if (cmd.Name == "StartUnmonitoredMove")
                        latestMove = cmd;
                    else if (cmd.Name == "GetState")
                        latestPoll = cmd;
                    else
                    {
                        if (!_cts.Token.IsCancellationRequested)
                        {
                            HandleCommand(cmd);
                        }
                    }
                }

                if (!_cts.Token.IsCancellationRequested && latestMove != null)
                    HandleCommand(latestMove);

                if (!_cts.Token.IsCancellationRequested && latestPoll != null)
                    HandleCommand(latestPoll);
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
            } else if (cmd.Name == "ConfigureRateLimits")
            {
                ConfigureRateLimits();
            } 
            else if (cmd.Name == "RefreshPitchAndRollMotorPositions")
            {
                refreshPitchAndRollMotorPositions();
            } else if (cmd.Name == "StartUnmonitoredMove")
            {
                StartUnmonitoredMove(cmd);
            }
            else
            {
                throw new ArgumentException($"Unknown command: {cmd.Name}");
            }

            return $"Handled command: {cmd.Name}";
        }

        private void refreshPitchAndRollMotorPositions()
        {
            if (myNodes != null && myNodes.Length == 3)
            {
                var pitchNode = myNodes[pitchNodeIndex];
                var rollNode = myNodes[rollNodeIndex];

                pitchNode.Motion.PosnMeasured.Refresh();
                rollNode.Motion.PosnMeasured.Refresh();

                int pitchPosition = (int)myNodes[pitchNodeIndex].Motion.PosnMeasured.Value();
                int rollPosition = -(int)myNodes[rollNodeIndex].Motion.PosnMeasured.Value();
                simulatorState.pitchCounts = pitchPosition;
                simulatorState.rollCounts = rollPosition;
                
                StateReporter.Report(simulatorState);
            }
        }

        private void PollMotors()
        {
            if (myNodes != null && myNodes.Length == 3)
            {
                myNodes[yawNodeIndex].Motion.PosnMeasured.Refresh();
                myNodes[pitchNodeIndex].Motion.PosnMeasured.Refresh();
                myNodes[rollNodeIndex].Motion.PosnMeasured.Refresh();
                myNodes[yawNodeIndex].Motion.VelMeasured.Refresh();

                int yawPosition = -(int)myNodes[yawNodeIndex].Motion.PosnMeasured.Value();
                int pitchPosition = (int)myNodes[pitchNodeIndex].Motion.PosnMeasured.Value();
                int rollPosition = -(int)myNodes[rollNodeIndex].Motion.PosnMeasured.Value();
                int yawRate = -(int)myNodes[yawNodeIndex].Motion.VelMeasured.Value();

                simulatorState.yawCounts = yawPosition;
                simulatorState.pitchCounts = pitchPosition;
                simulatorState.rollCounts = rollPosition;
                simulatorState.yawRate = yawRate;

                // If our app state thinks we aren't homed, and we aren't homing at the moment, check to see if the motor controller
                // says we are homed. This allows us to restart the app without having to rehome the motors.
                if (!simulatorState.motorsHomed && !simulatorState.homingInProgress)
                {
                    bool pitchHomed = myNodes[pitchNodeIndex].Motion.Homing.WasHomed();
                    bool rollHomed = myNodes[rollNodeIndex].Motion.Homing.WasHomed();

                    simulatorState.motorsHomed = pitchHomed && rollHomed;
                }                    

                StateReporter.Report(simulatorState);
            }
        }
        private void ZeroAllMotors()
        {
            if (simulatorState.portConnected && myMgr != null && myPort != null && myNodes != null)
            {
                Console.WriteLine("Port {0}: state={1}, nodes={2}", myPort.NetNumber(), myPort.OpenState(), myPort.NodeCount());

//                EnableMotors();
//                myMgr.Delay(200);

                simulatorState.motorsHomed = false;
                simulatorState.homingInProgress = true;
                StateReporter.Report(simulatorState);

                //Once the code gets past this point, it can be assumed that the Port has been opened without issue
                //Now we can get a reference to our port object which we will use to access the node objects
                for (int n = 0; n < myNodes.Length; n++)
                {
                    myMgr.Delay(200);

                    // The following statements will attempt to enable the node.  First,
                    // any shutdowns or NodeStops are cleared, finally the node is enabled
                    myNodes[n].Status.AlertsClear();
                    myNodes[n].Motion.NodeStopClear();

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
                }

//                DisableMotors();
                simulatorState.motorsHomed = true;
                simulatorState.homingInProgress = false;
                StateReporter.Report(simulatorState);
            }
        }

        private void EnableMotors()
        {
            if (!simulatorState.portConnected || myNodes == null || myPort == null)
            {
                throw new Exception("Call Start() before attempting to enable motors.");
            }

            for (int n = 0; n < myNodes.Length; n++)
            {
                myNodes[n].EnableReq(true);
            }

//            myPort.BrakeControl.BrakeSetting(0, sFndCLIWrapper.cliIBrakeControl._BrakeControls.BRAKE_ALLOW_MOTION);

           // myMgr.Delay(200);

            simulatorState.motorsEnabled = true;
            StateReporter.Report(simulatorState);
        }

        private void DisableMotors()
        {
            if (!simulatorState.portConnected || myNodes == null || myPort == null)
            {
                throw new Exception("Call Start() before attempting to enable motors.");
            }

//            myPort.BrakeControl.BrakeSetting(0, sFndCLIWrapper.cliIBrakeControl._BrakeControls.BRAKE_PREVENT_MOTION);

          //  myMgr.Delay(200);

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
                throw new Exception("Motors must be homed first.");
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
            if (myNodes != null && myMgr != null)
            {
//                EnableMotors();

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
                    int targetPosition = 0;

                    if (n != yawNodeIndex)
                    {
                        myNodes[n].Motion.MovePosnStart(targetPosition, true, false);
                    }
                    else
                    {
                        targetPosition = (int)Math.Round((double)currentPosition / (double)yawCountsPerOutputRotation, MidpointRounding.AwayFromZero) * (int)yawCountsPerOutputRotation;
                        Console.WriteLine(targetPosition.ToString());
                        myNodes[n].Motion.MovePosnStart(targetPosition, true, false);
                    }

                    double timeout = myMgr.TimeStampMsec() + myNodes[n].Motion.MovePosnDurationMsec(Math.Abs(targetPosition - currentPosition), false) + 1000;
                    timeouts.Add(timeout);
                }

                // Poll until all moves are complete
                bool allDone = false;
                while (!allDone)
                {
                    for (int i = 0; i < timeouts.Count; i++)
                    {
                        allDone = true;

                        myMgr.Delay(200);

                        if (myMgr.TimeStampMsec() < timeouts[i])
                        {
                            allDone = false;
                        }
                    }
                }

//DisableMotors();

                simulatorState.movingToZero = false;
                StateReporter.Report(simulatorState);
            }
        }

        private void StartUnmonitoredMove(SimulatorCommand cmd)
        {
            var tasks = new List<Task>();

            if (myNodes != null)
            {
                for (int n = 0; n < myNodes.Length; n++)
                {
                    var node = myNodes[n];

                    // Issue these commands in parallel. Teknic support indicated this would
                    // roughly halve the time to executive all commands, since sFoundation will block and wait
                    // for a response on each command, but will send all commands sequentially without waiting
                    // for serial ACK, if the commands are all queued up before the first command is sent.
                    if (n == yawNodeIndex)
                    {
                        if (cmd.Data.isVelocityCommand)
                        {
                            tasks.Add(Task.Run(() => node.Motion.MoveVelStart(-cmd.Data.yawRateCountsPerSecond / CountsPerRevolution * 60)));
                        }
                        else
                        {
                            tasks.Add(Task.Run(() => node.Motion.MovePosnStart(-cmd.Data.yawPositionCounts, true, false)));
                        }
                    }
                    else if (n == pitchNodeIndex)
                    {
                        if (cmd.Data.isVelocityCommand)
                        {
                            tasks.Add(Task.Run(() => node.Motion.MoveVelStart(cmd.Data.pitchRateCountsPerSecond / CountsPerRevolution * 60)));
                        }
                        else
                        {
                            tasks.Add(Task.Run(() => node.Motion.MovePosnStart(cmd.Data.pitchPositionCounts, true, false)));
                        }
                    }
                    else if (n == rollNodeIndex)
                    {
                        if (cmd.Data.isVelocityCommand)
                        {
                            tasks.Add(Task.Run(() => node.Motion.MoveVelStart(-cmd.Data.rollRateCountsPerSecond / CountsPerRevolution * 60)));
                        }
                        else
                        {
                            tasks.Add(Task.Run(() => node.Motion.MovePosnStart(-cmd.Data.rollPositionCounts, true, false)));
                        }
                    }
                }

                Task.WaitAll(tasks.ToArray());

                refreshPitchAndRollMotorPositions();
            }
        }

        private void ConfigureRateLimits()
        {
            if (myNodes != null && simulatorState.portConnected)
            {
                for (int n = 0; n < myNodes.Length; n++)
                {
                    var node = myNodes[n];
                    node.AccUnit(cliINode._accUnits.RPM_PER_SEC);         // Set the units for Acceleration to RPM/SEC
                    node.VelUnit(cliINode._velUnits.RPM);                 // Set the units for Velocity to RPM
                    node.Motion.AccLimit.Value(maxAcceleration);           // Set Acceleration Limit (RPM/Sec)
                    node.Motion.VelLimit.Value(maxVelocity);              // Set Velocity Limit (RPM)
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
