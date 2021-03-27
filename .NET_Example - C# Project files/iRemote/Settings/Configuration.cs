using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        public Dictionary<string, CodeAtributes> Storage { get; private set; }

    }

    /// <summary>
    /// Storage of information about the receiver device
    /// </summary>
    public class Device
    {
        public string Model { get; private set; }
        public string MAC { get; private set; }

        public Device(string _model, string _mac)
        {
            MAC = _mac;
            Model = _model;
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
        public int IRda_sleep;
        /// <summary>
        /// Path to the program configuration
        /// </summary>
        public string path_to_file = null;

        public Configuration()
        {
            Loader = new Loader();
            Codes = new Codes();
            if (iRemote.version == Loader.data["program"]["version"])
            {
                MessageBox.Show("");
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
