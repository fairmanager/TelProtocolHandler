using System;
using Microsoft.Win32;

namespace TelProtocolHandlerSetup {
	internal static class Program {
		private static void Main( string[] args ) {
			Console.WriteLine( "Installing Windows tel prototcol handler..." );

			if( System.Environment.Is64BitOperatingSystem ) {
				Console.WriteLine( "Operating system is: 64 bit (dual-registry)" );

			} else {
				Console.WriteLine( "Operating system is: 32 bit" );
			}

			if( System.Environment.Is64BitProcess ) {
				Console.WriteLine( "Current process is : 64 bit" );

			} else {
				Console.WriteLine( "Current process is : 32 bit" );
			}

			if( 0 < args.Length && args[ 0 ] == "--uninstall" ) {
				Console.WriteLine( "Removing application registration..." );
				Program.Uninstall();

			} else {
				Console.WriteLine( "Registering application..." );
				Program.Install();
			}

			Console.WriteLine( "The process completed." );
		}

		private static void Install() {
			// Register as the default handler for the tel: protocol.
			const string protocolValue = "TEL:Telephone Invocation";
			WriteClassesRoot( @"tel", string.Empty, protocolValue );
			WriteClassesRoot( @"tel", "URL Protocol", string.Empty );

			string command = $"\"{AppDomain.CurrentDomain.BaseDirectory}dial.exe\" \"%1\"";
			WriteClassesRoot( @"tel\shell\open\command", string.Empty, command );

			// For Windows 8+, register as a choosable protocol handler.

			// Version detection from http://stackoverflow.com/a/17796139/259953
			Version win8Version = new Version( 6, 2, 9200, 0 );

			if( Environment.OSVersion.Platform == PlatformID.Win32NT &&
			    Environment.OSVersion.Version >= win8Version ) {
				WriteLocalMachine(
					@"SOFTWARE\Classes\TelProtocolHandler",
					string.Empty,
					protocolValue );
				WriteLocalMachine(
					@"SOFTWARE\Classes\TelProtocolHandler\shell\open\command",
					string.Empty,
					command );

				WriteLocalMachine(
					@"SOFTWARE\TelProtocolHandler\Capabilities",
					"ApplicationName",
					"TelProtocolHandler" );
				WriteLocalMachine(
					@"SOFTWARE\TelProtocolHandler\Capabilities",
					"ApplicationDescription",
					"Invokes a TAPI line with the supplied phone number." );
				WriteLocalMachine(
					@"SOFTWARE\TelProtocolHandler\Capabilities\URLAssociations",
					"tel",
					"TelProtocolHandler" );

				WriteLocalMachine(
					@"SOFTWARE\RegisteredApplications",
					"TelProtocolHandler",
					@"SOFTWARE\TelProtocolHandler\Capabilities" );
			}
		}

		private static void Uninstall() {
			// Remove the default handler for the tel: protocol.
			DeleteClassesRoot( @"tel\shell\open\command", string.Empty );

			DeleteClassesRoot( @"tel", "URL Protocol" );
			DeleteClassesRoot( @"tel", string.Empty );

			// For Windows 8+, remove the choosable protocol handler.

			// Version detection from http://stackoverflow.com/a/17796139/259953
			Version win8Version = new Version( 6, 2, 9200, 0 );

			if( Environment.OSVersion.Platform == PlatformID.Win32NT &&
			    Environment.OSVersion.Version >= win8Version ) {
				DeleteLocalMachine(
					@"SOFTWARE\Classes\TelProtocolHandler\shell\open\command",
					string.Empty );
				DeleteLocalMachine(
					@"SOFTWARE\Classes\TelProtocolHandler",
					string.Empty );
				DeleteLocalMachine(
					@"SOFTWARE\Classes",
					"TelProtocolHandler" );

				DeleteLocalMachine(
					@"SOFTWARE\TelProtocolHandler\Capabilities\URLAssociations",
					"tel" );
				DeleteLocalMachine(
					@"SOFTWARE\TelProtocolHandler\Capabilities",
					"ApplicationDescription" );
				DeleteLocalMachine(
					@"SOFTWARE\TelProtocolHandler\Capabilities",
					"ApplicationName" );
				DeleteLocalMachine(
					@"SOFTWARE",
					"TelProtocolHandler" );

				DeleteLocalMachine(
					@"SOFTWARE\RegisteredApplications",
					"TelProtocolHandler" );
			}
		}

		private static void WriteClassesRoot( string where, string name, string value, RegistryView view = RegistryView.Registry32 ) {
			try {
				if( view == RegistryView.Registry64 ) {
					Console.WriteLine( "   x64: {0}\\{1} = {2}", @where, string.IsNullOrEmpty( name ) ? "(Default)" : name, value );
				} else {
					Console.WriteLine( "COMMIT: {0}\\{1} = {2}", @where, string.IsNullOrEmpty( name ) ? "(Default)" : name, value );
				}

				RegistryKey classRoot = RegistryKey.OpenBaseKey( RegistryHive.ClassesRoot, view );
				RegistryKey key = classRoot.CreateSubKey( @where );
				key.SetValue( name, value, RegistryValueKind.String );

			} catch( Exception e ) {
				Console.WriteLine( "\t" + e.Message );
			}

			if( view == RegistryView.Registry32 && System.Environment.Is64BitOperatingSystem ) {
				WriteClassesRoot( where, name, value, RegistryView.Registry64 );
			}
		}

		private static void DeleteClassesRoot( string where, string name, RegistryView view = RegistryView.Registry32 ) {
			try {
				if( view == RegistryView.Registry64 ) {
					Console.WriteLine( "   x64: {0}\\{1}", @where, string.IsNullOrEmpty( name ) ? "(Default)" : name );
				} else {
					Console.WriteLine( "COMMIT: {0}\\{1}", @where, string.IsNullOrEmpty( name ) ? "(Default)" : name );
				}

				RegistryKey classRoot = RegistryKey.OpenBaseKey( RegistryHive.ClassesRoot, view );
				classRoot.DeleteSubKeyTree( @where );

			} catch( Exception e ) {
				Console.WriteLine( "\t" + e.Message );
			}

			if( view == RegistryView.Registry32 && System.Environment.Is64BitOperatingSystem ) {
				DeleteClassesRoot( where, name, RegistryView.Registry64 );
			}
		}

		private static void WriteLocalMachine( string where, string name, string value, RegistryView view = RegistryView.Registry32 ) {
			try {
				if( view == RegistryView.Registry64 ) {
					Console.WriteLine( "   x64: {0}\\{1} = {2}", @where, string.IsNullOrEmpty( name ) ? "(Default)" : name, value );
				} else {
					Console.WriteLine( "COMMIT: {0}\\{1} = {2}", @where, string.IsNullOrEmpty( name ) ? "(Default)" : name, value );
				}

				RegistryKey localMachine = RegistryKey.OpenBaseKey( RegistryHive.LocalMachine, view );
				RegistryKey key = localMachine.CreateSubKey( @where );
				key.SetValue( name, value, RegistryValueKind.String );

			} catch( Exception e ) {
				Console.WriteLine( "\t" + e.Message );
			}

			if( view == RegistryView.Registry32 && System.Environment.Is64BitOperatingSystem ) {
				WriteLocalMachine( where, name, value, RegistryView.Registry64 );
			}
		}

		private static void DeleteLocalMachine( string where, string name, RegistryView view = RegistryView.Registry32 ) {
			try {
				if( view == RegistryView.Registry64 ) {
					Console.WriteLine( "   x64: {0}\\{1}", @where, string.IsNullOrEmpty( name ) ? "(Default)" : name );
				} else {
					Console.WriteLine( "COMMIT: {0}\\{1}", @where, string.IsNullOrEmpty( name ) ? "(Default)" : name );
				}

				RegistryKey localMachine = RegistryKey.OpenBaseKey( RegistryHive.LocalMachine, view );
				localMachine.DeleteSubKeyTree( @where );

			} catch( Exception e ) {
				Console.WriteLine( "\t" + e.Message );
			}

			if( view == RegistryView.Registry32 && System.Environment.Is64BitOperatingSystem ) {
				DeleteLocalMachine( where, name, RegistryView.Registry64 );
			}
		}
	}
}
