using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using JulMar.Tapi3;

namespace FairManager.ClickToDial {
	public static class CallEventHandler {
		private static readonly log4net.ILog log = log4net.LogManager.GetLogger( System.Reflection.MethodBase.GetCurrentMethod().DeclaringType );
		private static readonly TTapi tapi = new TTapi();

		public static void CreateCall( string[] args ) {
			tapi.Initialize();

			if( args.Length < 1 ) {
				// Set application up as default tel: handler.
				RunSetup();
				return;
			}

			Configuration.Load();
			CheckForTapiLineErrors();

			// Convert input parameters to actual number we want to dial.
			string phoneNumber = NumberToCall( args );
			InitiateCall( phoneNumber );
		}

		private static void RunSetup() {
			log.Info( "Application runs without arguments. Starting setup…" );
			// Start setup.exe from the same folder we're running from.
			System.Diagnostics.Process process = new System.Diagnostics.Process {
				StartInfo = {
					FileName = "setup.exe",
					Verb = "runas",
					WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden,
					WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory
				}
			};
			process.Start();
			log.Info( "Setup completed." );
			CallTapiLineConfiguration();
		}

		private static string NumberToCall( IReadOnlyList<string> args ) {
			const string protocol = "tel:";

			if( args.Count < 1 ) {
				log.Error( "No arguments given." );
				MessageBox.Show( "No arguments given.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
				return string.Empty;
			}

			string phoneNumber = args[ 0 ];
			if( !phoneNumber.StartsWith( protocol ) ) {
				log.Error( $"Unexpected input. Expected argument to start with '{protocol}'." );
				MessageBox.Show( $"Unexpected input. Expected argument to start with '{protocol}'", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
				return string.Empty;
			}

			phoneNumber = phoneNumber.Substring( protocol.Length );
			// Replace + prefix with a 00
			if( phoneNumber.StartsWith( "+" ) ) {
				phoneNumber = "00" + phoneNumber.Substring( 1 );
			}

			return phoneNumber;
		}

		private static void CheckForTapiLineErrors() {
			// If no line is configured, start the configuration.
			if( string.IsNullOrEmpty( Configuration.Container.LineToUse ) ) {
				log.Warn( "No line configuration value set. Starting settings application…" );
				CallTapiLineConfiguration();
			}

			// If, after configuration, there's still no line, show an error and exit.
			if( string.IsNullOrEmpty( Configuration.Container.LineToUse ) ) {
				log.Error( "No TAPI line selected!" );
				MessageBox.Show( "No TAPI line configured or none available.", "Configuration error", MessageBoxButtons.OK, MessageBoxIcon.Error );
				Environment.Exit( 0 );
			}

			// Find the actual TAPI object for the line name.
			string lineToUse = Configuration.Container.LineToUse;
			TAddress line = tapi.Addresses.SingleOrDefault( a => a.AddressName == lineToUse );
			if( null != line ) {
				return;
			}

			log.Error( $"Unable to find TAPI line with name '{lineToUse}'!" );
			DialogResult reconfigure = MessageBox.Show(
				$"Unable to find TAPI line with name '{lineToUse}'!\nDo you wish to select another TAPI line?", "Error", MessageBoxButtons.YesNo, MessageBoxIcon.Error );
			if( reconfigure == DialogResult.Yes ) {
				CallTapiLineConfiguration();
			}
		}

		private static void CallTapiLineConfiguration() {
			SelectTapiForm tapiForm = new SelectTapiForm( Configuration.Container.LineToUse );
			Application.Run( tapiForm );

			// If the form wasn't cancelled, check selected line for errors.
			if( !tapiForm.WasCancelled ) {
				CheckForTapiLineErrors();
			}
		}

		private static void InitiateCall( string phoneNumber ) {
			if( string.IsNullOrEmpty( phoneNumber ) ) {
				return;
			}

			string lineToUse = Configuration.Container.LineToUse;
			log.Info( $"Creating call via line '{lineToUse}'." );
			TAddress line = tapi.Addresses.SingleOrDefault( a => a.AddressName == lineToUse );

			// Always assumes 0 prefix is needed to dial out.
			TCall call = line.CreateCall( "0" + phoneNumber, LINEADDRESSTYPES.PhoneNumber, TAPIMEDIATYPES.AUDIO );
			try {
				call.Connect( false );
			} catch( TapiException ex ) {
				log.Error( "TapiException: ", ex );
				return;
			}

			log.Info( $"Calling '{phoneNumber}'..." );
		}
	}
}
