using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Windows.Forms;
using IniParser;
using IniParser.Model;

namespace Shmelevdi.iRemoteProject.Settings
{
    /// <summary>
    /// It is a set of attributes for the code
    /// </summary>
    public class CodeAtributes
    {
        /// <summary>
        /// The action of pressing the keyboard key. Contains the keyboard key code of the form: {ENTER}
        /// </summary>
        public string Keyaction { get; private set; }

        /// <summary>
        /// An action associated with the program itself or the program environment. Minimizes or expands the program window, shuts down or restarts the computer, etc.
        /// </summary>
        public string Action { get; private set; }

        /// <summary>
        /// Contains a human-readable name for the command
        /// </summary>
        public string Name { get; private set; }

        public CodeAtributes(string _keyaction, string _action, string _name)
        {
            Keyaction = _keyaction;
            Action = _action;
            Name = _name;
        }

    }

    /// <summary>
    /// It is a code store with an instance of the code attribute class
    /// </summary>
    public class Codes
    {
        /// <summary>
        /// Directly the code storage itself
        /// </summary>
        public Dictionary<string, CodeAtributes> Storage { get; set; }

        public Codes()
        {
            Storage = new Dictionary<string, CodeAtributes> { };
        }

    }

    /// <summary>
    /// Storage of information about the receiver device
    /// </summary>
    public class Device
    {
        public string Model { get; private set; }
        public string MAC { get; private set; }
        public bool ManualConnect { get; private set; }
        public IPAddress ManualIPAddress { get; private set; }
        public ushort ManualPort { get; private set; }

        public Device(string _model, string _mac, bool _manualcon, IPAddress _manualip, ushort _manualport)
        {
            MAC = _mac;
            Model = _model;
            ManualConnect = _manualcon;
            ManualIPAddress = _manualip;
            ManualPort = _manualport;
        }
    }
    /// <summary>
    /// Program settings
    /// </summary>
    public class ProgramSettings
    {
        /// <summary>
        /// Reconnect if the connection is broken? True or false
        /// </summary>
        public bool Reconnect { get; private set; }

        /// <summary>
        /// Reconnect timer in seconds
        /// </summary>
        public int ReconnectTimer { get; private set; }

        /// <summary>
        /// Program version on configuration
        /// </summary>
        public string Version { get; private set; }

        public ProgramSettings(string _version, bool _reconnect=false, int _reconTimer=10)
        {
            Reconnect = _reconnect;
            ReconnectTimer = _reconTimer;
            Version = _version;
        }
    }

    /// <summary>
    /// Loads the program configuration
    /// </summary>
    public class Loader
    {
        public IniData data;
        /// <summary>
        /// Path to the program configuration
        /// </summary>
        public string path_to_file;

        public Loader(string path = null)
        {
            try
            {
                var parser = new FileIniDataParser();
                if (path == null) path_to_file = Environment.CurrentDirectory + @"\config.ini";
                else path_to_file = path;

                if(!File.Exists(path_to_file))
                {

                }
                data = new IniData();
                data = parser.ReadFile(path_to_file);
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("iRemote :: An error occurred while loading the configuration.\r\n" +
                    "Error details: {0}\r\nError stack trace: {1}", ex.Message, ex.StackTrace));
            }
        }

    }

    /// <summary>
    /// Interaction class with the software configuration. Loads, processes, and saves the program configuration
    /// </summary>
    public class Configuration
    {
        private Loader Loader;
        public Codes Codes;
        public Device Device;
        public ProgramSettings ProgramSettings;
        public int IRda_sleep;
        /// <summary>
        /// Path to the program configuration
        /// </summary>
        public string path_to_file = null;

        public Configuration()
        {
            Loader = new Loader();
            Codes = new Codes();

            IPAddress manip = null;
            ushort manport = 8080;
            bool mancon = false;
            string model = null;
            string mac = null;

            bool recon = false;
            int recontimer = 10;

            if (iRemote.version == Loader.data["program"]["version"])
            {

                if (Loader.data["device"].ContainsKey("model")) model = Loader.data["device"]["model"];
                if (Loader.data["device"].ContainsKey("mac")) mac = Loader.data["device"]["mac"].Replace(":", "");
                if (Loader.data["device"].ContainsKey("manual_con")) mancon = bool.Parse(Loader.data["device"]["manual_con"]);
                if (Loader.data["device"].ContainsKey("manual_con_ip")) manip = IPAddress.Parse(Loader.data["device"]["manual_con_ip"]);
                if (Loader.data["device"].ContainsKey("manual_con_port")) manport = ushort.Parse(Loader.data["device"]["manual_con_port"]);
                Device = new Device(model, mac, mancon, manip, manport);

                if (Loader.data["program"].ContainsKey("reconnect")) recon = bool.Parse(Loader.data["program"]["reconnect"]);
                if (Loader.data["program"].ContainsKey("reconnect_timer")) recontimer = Int32.Parse(Loader.data["program"]["reconnect_timer"]);
                ProgramSettings = new ProgramSettings(Loader.data["program"]["version"], recon, recontimer);

                for (int i=1;i<= Loader.data["codes"].Count;i++)
                {
                    string code_num = "code" + i.ToString();               
                    Codes.Storage.Add(
                        Loader.data["codes"][code_num],
                        new CodeAtributes(
                            Loader.data[code_num]["keyaction"],
                            Loader.data[code_num]["action"],
                            Loader.data[code_num]["name"]
                        )
                    );                    
                }
            }
            else
            {
                throw new Exception(string.Format("iRemote :: The version of the configuration file differs from the version of the software product."));
            }

        }

        /// <summary>
        /// Assigns the path to the program configuration. Takes the full path.
        /// </summary>
        /// <param name="path"></param>
        public void SetConfigFile(string path)
        {
            path_to_file = path;
            Loader = new Loader(path);
        }
    }
}
