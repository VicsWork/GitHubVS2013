﻿#pragma warning disable 1591
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace PowerCalibration
{
	using System.Data.Linq;
	using System.Data.Linq.Mapping;
	using System.Data;
	using System.Collections.Generic;
	using System.Reflection;
	using System.Linq;
	using System.Linq.Expressions;
	using System.ComponentModel;
	using System;
	
	
	[global::System.Data.Linq.Mapping.DatabaseAttribute(Name="ManufacturingStore_v2")]
	public partial class ManufacturingStore_DataContext : System.Data.Linq.DataContext
	{
		
		private static System.Data.Linq.Mapping.MappingSource mappingSource = new AttributeMappingSource();
		
    #region Extensibility Method Definitions
    partial void OnCreated();
    partial void InsertStationSite(StationSite instance);
    partial void UpdateStationSite(StationSite instance);
    partial void DeleteStationSite(StationSite instance);
    partial void InsertTestStationMachine(TestStationMachine instance);
    partial void UpdateTestStationMachine(TestStationMachine instance);
    partial void DeleteTestStationMachine(TestStationMachine instance);
    partial void InsertProductionSite(ProductionSite instance);
    partial void UpdateProductionSite(ProductionSite instance);
    partial void DeleteProductionSite(ProductionSite instance);
    #endregion
		
		public ManufacturingStore_DataContext() : 
				base(global::PowerCalibration.Properties.Settings.Default.ManufacturingStore_v2ConnectionString, mappingSource)
		{
			OnCreated();
		}
		
		public ManufacturingStore_DataContext(string connection) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public ManufacturingStore_DataContext(System.Data.IDbConnection connection) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public ManufacturingStore_DataContext(string connection, System.Data.Linq.Mapping.MappingSource mappingSource) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public ManufacturingStore_DataContext(System.Data.IDbConnection connection, System.Data.Linq.Mapping.MappingSource mappingSource) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public System.Data.Linq.Table<StationSite> StationSites
		{
			get
			{
				return this.GetTable<StationSite>();
			}
		}
		
		public System.Data.Linq.Table<TestStationMachine> TestStationMachines
		{
			get
			{
				return this.GetTable<TestStationMachine>();
			}
		}
		
		public System.Data.Linq.Table<ProductionSite> ProductionSites
		{
			get
			{
				return this.GetTable<ProductionSite>();
			}
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.StationSite")]
	public partial class StationSite : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		private string _StationMac;
		
		private int _ProductionSiteId;
		
		private EntityRef<ProductionSite> _ProductionSite;
		
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnStationMacChanging(string value);
    partial void OnStationMacChanged();
    partial void OnProductionSiteIdChanging(int value);
    partial void OnProductionSiteIdChanged();
    #endregion
		
		public StationSite()
		{
			this._ProductionSite = default(EntityRef<ProductionSite>);
			OnCreated();
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_StationMac", DbType="Char(12) NOT NULL", CanBeNull=false, IsPrimaryKey=true)]
		public string StationMac
		{
			get
			{
				return this._StationMac;
			}
			set
			{
				if ((this._StationMac != value))
				{
					this.OnStationMacChanging(value);
					this.SendPropertyChanging();
					this._StationMac = value;
					this.SendPropertyChanged("StationMac");
					this.OnStationMacChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_ProductionSiteId", DbType="Int NOT NULL")]
		public int ProductionSiteId
		{
			get
			{
				return this._ProductionSiteId;
			}
			set
			{
				if ((this._ProductionSiteId != value))
				{
					if (this._ProductionSite.HasLoadedOrAssignedValue)
					{
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
					}
					this.OnProductionSiteIdChanging(value);
					this.SendPropertyChanging();
					this._ProductionSiteId = value;
					this.SendPropertyChanged("ProductionSiteId");
					this.OnProductionSiteIdChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.AssociationAttribute(Name="ProductionSite_StationSite", Storage="_ProductionSite", ThisKey="ProductionSiteId", OtherKey="Id", IsForeignKey=true)]
		public ProductionSite ProductionSite
		{
			get
			{
				return this._ProductionSite.Entity;
			}
			set
			{
				ProductionSite previousValue = this._ProductionSite.Entity;
				if (((previousValue != value) 
							|| (this._ProductionSite.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if ((previousValue != null))
					{
						this._ProductionSite.Entity = null;
						previousValue.StationSites.Remove(this);
					}
					this._ProductionSite.Entity = value;
					if ((value != null))
					{
						value.StationSites.Add(this);
						this._ProductionSiteId = value.Id;
					}
					else
					{
						this._ProductionSiteId = default(int);
					}
					this.SendPropertyChanged("ProductionSite");
				}
			}
		}
		
		public event PropertyChangingEventHandler PropertyChanging;
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
			{
				this.PropertyChanging(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.TestStationMachines")]
	public partial class TestStationMachine : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		private int _Id;
		
		private string _Name;
		
		private string _Description;
		
		private string _MacAddress;
		
		private string _IpAddress;
		
		private string _MachineGuid;
		
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnIdChanging(int value);
    partial void OnIdChanged();
    partial void OnNameChanging(string value);
    partial void OnNameChanged();
    partial void OnDescriptionChanging(string value);
    partial void OnDescriptionChanged();
    partial void OnMacAddressChanging(string value);
    partial void OnMacAddressChanged();
    partial void OnIpAddressChanging(string value);
    partial void OnIpAddressChanged();
    partial void OnMachineGuidChanging(string value);
    partial void OnMachineGuidChanged();
    #endregion
		
		public TestStationMachine()
		{
			OnCreated();
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Id", AutoSync=AutoSync.OnInsert, DbType="Int NOT NULL IDENTITY", IsPrimaryKey=true, IsDbGenerated=true)]
		public int Id
		{
			get
			{
				return this._Id;
			}
			set
			{
				if ((this._Id != value))
				{
					this.OnIdChanging(value);
					this.SendPropertyChanging();
					this._Id = value;
					this.SendPropertyChanged("Id");
					this.OnIdChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Name", DbType="VarChar(50) NOT NULL", CanBeNull=false)]
		public string Name
		{
			get
			{
				return this._Name;
			}
			set
			{
				if ((this._Name != value))
				{
					this.OnNameChanging(value);
					this.SendPropertyChanging();
					this._Name = value;
					this.SendPropertyChanged("Name");
					this.OnNameChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Description", DbType="VarChar(100) NOT NULL", CanBeNull=false)]
		public string Description
		{
			get
			{
				return this._Description;
			}
			set
			{
				if ((this._Description != value))
				{
					this.OnDescriptionChanging(value);
					this.SendPropertyChanging();
					this._Description = value;
					this.SendPropertyChanged("Description");
					this.OnDescriptionChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_MacAddress", DbType="VarChar(16) NOT NULL", CanBeNull=false)]
		public string MacAddress
		{
			get
			{
				return this._MacAddress;
			}
			set
			{
				if ((this._MacAddress != value))
				{
					this.OnMacAddressChanging(value);
					this.SendPropertyChanging();
					this._MacAddress = value;
					this.SendPropertyChanged("MacAddress");
					this.OnMacAddressChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_IpAddress", DbType="VarChar(15) NOT NULL", CanBeNull=false)]
		public string IpAddress
		{
			get
			{
				return this._IpAddress;
			}
			set
			{
				if ((this._IpAddress != value))
				{
					this.OnIpAddressChanging(value);
					this.SendPropertyChanging();
					this._IpAddress = value;
					this.SendPropertyChanged("IpAddress");
					this.OnIpAddressChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_MachineGuid", DbType="NChar(36)")]
		public string MachineGuid
		{
			get
			{
				return this._MachineGuid;
			}
			set
			{
				if ((this._MachineGuid != value))
				{
					this.OnMachineGuidChanging(value);
					this.SendPropertyChanging();
					this._MachineGuid = value;
					this.SendPropertyChanged("MachineGuid");
					this.OnMachineGuidChanged();
				}
			}
		}
		
		public event PropertyChangingEventHandler PropertyChanging;
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
			{
				this.PropertyChanging(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.ProductionSite")]
	public partial class ProductionSite : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		private int _Id;
		
		private string _Name;
		
		private bool _LoadRangeTest;
		
		private bool _RunIct;
		
		private bool _RunRangeTest;
		
		private bool _LoadApplication;
		
		private bool _ForceChannel;
		
		private bool _Erase;
		
		private bool _EnableFirmwareChange;
		
		private EntitySet<StationSite> _StationSites;
		
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnIdChanging(int value);
    partial void OnIdChanged();
    partial void OnNameChanging(string value);
    partial void OnNameChanged();
    partial void OnLoadRangeTestChanging(bool value);
    partial void OnLoadRangeTestChanged();
    partial void OnRunIctChanging(bool value);
    partial void OnRunIctChanged();
    partial void OnRunRangeTestChanging(bool value);
    partial void OnRunRangeTestChanged();
    partial void OnLoadApplicationChanging(bool value);
    partial void OnLoadApplicationChanged();
    partial void OnForceChannelChanging(bool value);
    partial void OnForceChannelChanged();
    partial void OnEraseChanging(bool value);
    partial void OnEraseChanged();
    partial void OnEnableFirmwareChangeChanging(bool value);
    partial void OnEnableFirmwareChangeChanged();
    #endregion
		
		public ProductionSite()
		{
			this._StationSites = new EntitySet<StationSite>(new Action<StationSite>(this.attach_StationSites), new Action<StationSite>(this.detach_StationSites));
			OnCreated();
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Id", DbType="Int NOT NULL", IsPrimaryKey=true)]
		public int Id
		{
			get
			{
				return this._Id;
			}
			set
			{
				if ((this._Id != value))
				{
					this.OnIdChanging(value);
					this.SendPropertyChanging();
					this._Id = value;
					this.SendPropertyChanged("Id");
					this.OnIdChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Name", DbType="VarChar(50) NOT NULL", CanBeNull=false)]
		public string Name
		{
			get
			{
				return this._Name;
			}
			set
			{
				if ((this._Name != value))
				{
					this.OnNameChanging(value);
					this.SendPropertyChanging();
					this._Name = value;
					this.SendPropertyChanged("Name");
					this.OnNameChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_LoadRangeTest", DbType="Bit NOT NULL")]
		public bool LoadRangeTest
		{
			get
			{
				return this._LoadRangeTest;
			}
			set
			{
				if ((this._LoadRangeTest != value))
				{
					this.OnLoadRangeTestChanging(value);
					this.SendPropertyChanging();
					this._LoadRangeTest = value;
					this.SendPropertyChanged("LoadRangeTest");
					this.OnLoadRangeTestChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_RunIct", DbType="Bit NOT NULL")]
		public bool RunIct
		{
			get
			{
				return this._RunIct;
			}
			set
			{
				if ((this._RunIct != value))
				{
					this.OnRunIctChanging(value);
					this.SendPropertyChanging();
					this._RunIct = value;
					this.SendPropertyChanged("RunIct");
					this.OnRunIctChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_RunRangeTest", DbType="Bit NOT NULL")]
		public bool RunRangeTest
		{
			get
			{
				return this._RunRangeTest;
			}
			set
			{
				if ((this._RunRangeTest != value))
				{
					this.OnRunRangeTestChanging(value);
					this.SendPropertyChanging();
					this._RunRangeTest = value;
					this.SendPropertyChanged("RunRangeTest");
					this.OnRunRangeTestChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_LoadApplication", DbType="Bit NOT NULL")]
		public bool LoadApplication
		{
			get
			{
				return this._LoadApplication;
			}
			set
			{
				if ((this._LoadApplication != value))
				{
					this.OnLoadApplicationChanging(value);
					this.SendPropertyChanging();
					this._LoadApplication = value;
					this.SendPropertyChanged("LoadApplication");
					this.OnLoadApplicationChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_ForceChannel", DbType="Bit NOT NULL")]
		public bool ForceChannel
		{
			get
			{
				return this._ForceChannel;
			}
			set
			{
				if ((this._ForceChannel != value))
				{
					this.OnForceChannelChanging(value);
					this.SendPropertyChanging();
					this._ForceChannel = value;
					this.SendPropertyChanged("ForceChannel");
					this.OnForceChannelChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Erase", DbType="Bit NOT NULL")]
		public bool Erase
		{
			get
			{
				return this._Erase;
			}
			set
			{
				if ((this._Erase != value))
				{
					this.OnEraseChanging(value);
					this.SendPropertyChanging();
					this._Erase = value;
					this.SendPropertyChanged("Erase");
					this.OnEraseChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_EnableFirmwareChange", DbType="Bit NOT NULL")]
		public bool EnableFirmwareChange
		{
			get
			{
				return this._EnableFirmwareChange;
			}
			set
			{
				if ((this._EnableFirmwareChange != value))
				{
					this.OnEnableFirmwareChangeChanging(value);
					this.SendPropertyChanging();
					this._EnableFirmwareChange = value;
					this.SendPropertyChanged("EnableFirmwareChange");
					this.OnEnableFirmwareChangeChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.AssociationAttribute(Name="ProductionSite_StationSite", Storage="_StationSites", ThisKey="Id", OtherKey="ProductionSiteId")]
		public EntitySet<StationSite> StationSites
		{
			get
			{
				return this._StationSites;
			}
			set
			{
				this._StationSites.Assign(value);
			}
		}
		
		public event PropertyChangingEventHandler PropertyChanging;
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
			{
				this.PropertyChanging(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
		
		private void attach_StationSites(StationSite entity)
		{
			this.SendPropertyChanging();
			entity.ProductionSite = this;
		}
		
		private void detach_StationSites(StationSite entity)
		{
			this.SendPropertyChanging();
			entity.ProductionSite = null;
		}
	}
}
#pragma warning restore 1591
