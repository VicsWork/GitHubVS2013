﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace PowerCalibration.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "15.1.0.0")]
    internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
        
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("COM1")]
        public string Meter_COM_Port_Name {
            get {
                return ((string)(this["Meter_COM_Port_Name"]));
            }
            set {
                this["Meter_COM_Port_Name"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("C:\\Program Files (x86)\\Ember\\ISA3 Utilities\\bin")]
        public string Ember_BinPath {
            get {
                return ((string)(this["Ember_BinPath"]));
            }
            set {
                this["Ember_BinPath"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("FT232H")]
        public string Relay_Controller_Type {
            get {
                return ((string)(this["Relay_Controller_Type"]));
            }
            set {
                this["Relay_Controller_Type"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string Last_Used_Board {
            get {
                return ((string)(this["Last_Used_Board"]));
            }
            set {
                this["Last_Used_Board"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool Meter_Manual_Measurement {
            get {
                return ((bool)(this["Meter_Manual_Measurement"]));
            }
            set {
                this["Meter_Manual_Measurement"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("IP")]
        public string Ember_Interface {
            get {
                return ((string)(this["Ember_Interface"]));
            }
            set {
                this["Ember_Interface"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("0.0.0.0")]
        public string Ember_Interface_IP_Address {
            get {
                return ((string)(this["Ember_Interface_IP_Address"]));
            }
            set {
                this["Ember_Interface_IP_Address"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("0")]
        public string Ember_Interface_USB_Address {
            get {
                return ((string)(this["Ember_Interface_USB_Address"]));
            }
            set {
                this["Ember_Interface_USB_Address"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool PrePost_Test_Enabled {
            get {
                return ((bool)(this["PrePost_Test_Enabled"]));
            }
            set {
                this["PrePost_Test_Enabled"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool DB_Loging_Enabled {
            get {
                return ((bool)(this["DB_Loging_Enabled"]));
            }
            set {
                this["DB_Loging_Enabled"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Centralite")]
        public string ProductionSiteName {
            get {
                return ((string)(this["ProductionSiteName"]));
            }
            set {
                this["ProductionSiteName"] = value;
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.SpecialSettingAttribute(global::System.Configuration.SpecialSetting.ConnectionString)]
        [global::System.Configuration.DefaultSettingValueAttribute("Data Source=rs01.centralite.com;Initial Catalog=ManufacturingStore_v2;Integrated " +
            "Security=True")]
        public string DBConnectionString {
            get {
                return ((string)(this["DBConnectionString"]));
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("120")]
        public double CalibrationLoadVoltageValue {
            get {
                return ((double)(this["CalibrationLoadVoltageValue"]));
            }
            set {
                this["CalibrationLoadVoltageValue"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("20")]
        public double CalibrationLoadVoltageTolarance {
            get {
                return ((double)(this["CalibrationLoadVoltageTolarance"]));
            }
            set {
                this["CalibrationLoadVoltageTolarance"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("500")]
        public double CalibrationLoadResistorValue {
            get {
                return ((double)(this["CalibrationLoadResistorValue"]));
            }
            set {
                this["CalibrationLoadResistorValue"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("20")]
        public double CalibrationLoadResistorTolarance {
            get {
                return ((double)(this["CalibrationLoadResistorTolarance"]));
            }
            set {
                this["CalibrationLoadResistorTolarance"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool CodeMinimizedOnPASS {
            get {
                return ((bool)(this["CodeMinimizedOnPASS"]));
            }
            set {
                this["CodeMinimizedOnPASS"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("41461589")]
        public int HoneycombSensorID {
            get {
                return ((int)(this["HoneycombSensorID"]));
            }
            set {
                this["HoneycombSensorID"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool Ember_ReadProtect_Enabled {
            get {
                return ((bool)(this["Ember_ReadProtect_Enabled"]));
            }
            set {
                this["Ember_ReadProtect_Enabled"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool Play_Sounds {
            get {
                return ((bool)(this["Play_Sounds"]));
            }
            set {
                this["Play_Sounds"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool Disable_ReadProtection_BeforeCoding {
            get {
                return ((bool)(this["Disable_ReadProtection_BeforeCoding"]));
            }
            set {
                this["Disable_ReadProtection_BeforeCoding"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("1")]
        public double Gain_Current_Max {
            get {
                return ((double)(this["Gain_Current_Max"]));
            }
            set {
                this["Gain_Current_Max"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("0.6")]
        public double Gain_Current_Min {
            get {
                return ((double)(this["Gain_Current_Min"]));
            }
            set {
                this["Gain_Current_Min"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("1.1")]
        public double Gain_Voltage_Max {
            get {
                return ((double)(this["Gain_Voltage_Max"]));
            }
            set {
                this["Gain_Voltage_Max"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("0.8")]
        public double Gain_Voltage_Min {
            get {
                return ((double)(this["Gain_Voltage_Min"]));
            }
            set {
                this["Gain_Voltage_Min"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("ISA_UTIL")]
        public string Coding_Method {
            get {
                return ((string)(this["Coding_Method"]));
            }
            set {
                this["Coding_Method"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("C:\\Users\\victormartin\\Documents\\Jigs\\Mahi_HALIBUT\\mahi3.hex")]
        public string Coding_File {
            get {
                return ((string)(this["Coding_File"]));
            }
            set {
                this["Coding_File"] = value;
            }
        }
    }
}
