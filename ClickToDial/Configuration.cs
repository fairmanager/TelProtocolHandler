using System;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;

namespace FairManager.ClickToDial {
	public static class Configuration {
		private static readonly log4net.ILog log = log4net.LogManager.GetLogger( System.Reflection.MethodBase.GetCurrentMethod().DeclaringType );

		private static ConfigContainer _container;

		private static string ConfigFileName {
			get {
				string basePath = Environment.GetFolderPath( Environment.SpecialFolder.LocalApplicationData );
				string path = Path.Combine( basePath, "TelProtocolHandler" );
				return Path.Combine( path, "config.xml" );
			}
		}

		public static ConfigContainer Container {
			get => _container ?? ( _container = new ConfigContainer() );
			private set => _container = value;
		}

		public static void Load() {
			XmlSerializer xmlSerializer = new XmlSerializer( typeof( ConfigContainer ) );
			try {
				using( FileStream stream = File.OpenRead( ConfigFileName ) ) {
					Container = (ConfigContainer)xmlSerializer.Deserialize( stream );
				}
			} catch( FileNotFoundException ) {
				log.Warn( "Configuration file not found. Using standard settings." );
			} catch( DirectoryNotFoundException ) {
				log.Warn( "Configuration file not found. Using standard settings." );
			} catch( InvalidOperationException ) {
				log.Warn( "Could not read configuration file. Using standard settings. (check XML format?)" );
			}
		}

		public static void Save() {
			XmlSerializer xSerializer = new XmlSerializer( typeof( ConfigContainer ) );
			try {
				// Create the directory for the configuration file.
				Directory.CreateDirectory( Path.GetDirectoryName( ConfigFileName ) );

				using( XmlWriter writer = XmlWriter.Create( ConfigFileName ) ) {
					xSerializer.Serialize( writer, Container );
				}
			} catch( UnauthorizedAccessException ) {
				log.Error( "Could not write file 'Config.xml'. Settings will not be saved." );
				MessageBox.Show( "Could not write file 'Config.xml'. Settings will not be saved.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
			}
		}

		public class ConfigContainer {
			public string LineToUse;

			public ConfigContainer() {
				LineToUse = string.Empty;
			}
		}
	}
}
