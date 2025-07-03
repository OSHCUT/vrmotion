using sFndCLIWrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SimController
{
    internal class MotorInterface
    {
        const int HOMING_TIMEOUT_MS = 60000;

        private SimulatorState simulatorState;

        private cliSysMgr? myMgr;
        private string? comHubPort;
        private cliIPort? myPort;
        private cliINode[]? myNodes;

        public event Action<string>? StatusChanged;
        public event Action<SimulatorState>? StateChanged;
        public readonly IProgress<string> StatusReporter;
        public readonly IProgress<SimulatorState> StateReporter;

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
                    myMgr.ComPortHub(0, comHubPort, cliSysMgr._netRates.MN_BAUD_24X);
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
                    }

                    StatusReporter.Report("Connected.");
                    simulatorState.portConnected = true;
                    StateReporter.Report(simulatorState);
                }
                catch (Exception e)
                {
                    StatusReporter.Report("Failed to open simulator COM port. " + e.Message);
                    return;
                }
            });
        }

        public void ZeroAllMotors()
        {
            Task.Run(() =>
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
                }
            });
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
