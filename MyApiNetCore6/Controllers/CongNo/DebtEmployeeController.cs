<?xml version="1.0" encoding="utf-8"?>
<Project ToolsVersion="12.0" DefaultTargets="Build" xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
  <PropertyGroup>
    <Configuration Condition=" '$(Configuration)' == '' ">Debug</Configuration>
    <Platform Condition=" '$(Platform)' == '' ">x86</Platform>
    <ProductVersion>8.0.30703</ProductVersion>
    <SchemaVersion>2.0</SchemaVersion>
    <ProjectGuid>{6DAB5C49-C9CE-41CB-93DE-3F2677516FD7}</ProjectGuid>
    <OutputType>Library</OutputType>
    <AppDesignerFolder>Properties</AppDesignerFolder>
    <RootNamespace>TS24.HD.BKupRestore</RootNamespace>
    <AssemblyName>TS24.HD.BKupRestore</AssemblyName>
    <TargetFrameworkVersion>v4.7.2</TargetFrameworkVersion>
    <FileAlignment>512</FileAlignment>
    <TargetFrameworkProfile />
  </PropertyGroup>
  <PropertyGroup Condition=" '$(Configuration)|$(Platform)' == 'Debug|x86' ">
    <PlatformTarget>AnyCPU</PlatformTarget>
    <DebugSymbols>true</DebugSymbols>
    <DebugType>full</DebugType>
    <Optimize>false</Optimize>
    <OutputPath>..\..\Lib\</OutputPath>
    <DefineConstants>DEBUG;TRACE</DefineConstants>
    <ErrorReport>prompt</ErrorReport>
    <WarningLevel>2</WarningLevel>
    <Prefer32Bit>false</Prefer32Bit>
  </PropertyGroup>
  <PropertyGroup Condition=" '$(Configuration)|$(Platform)' == 'Release|x86' ">
    <PlatformTarget>x86</PlatformTarget>
    <DebugType>pdbonly</DebugType>
    <Optimize>true</Optimize>
    <OutputPath>bin\Release\</OutputPath>
    <DefineConstants>TRACE</DefineConstants>
    <ErrorReport>prompt</ErrorReport>
    <WarningLevel>4</WarningLevel>
    <Prefer32Bit>false</Prefer32Bit>
  </PropertyGroup>
  <PropertyGroup>
    <StartupObject />
  </PropertyGroup>
  <ItemGroup>
    <Reference Include="Devart.Data, Version=5.0.1214.0, Culture=neutral, PublicKeyToken=09af7300eec23701, processorArchitecture=MSIL">
      <SpecificVersion>False</SpecificVersion>
      <HintPath>..\..\Lib\Devart.Data.dll</HintPath>
    </Reference>
    <Reference Include="Devart.Data.MySql, Version=8.3.422.0, Culture=neutral, PublicKeyToken=09af7300eec23701, processorArchitecture=MSIL">
      <SpecificVersion>False</SpecificVersion>
      <HintPath>..\..\Lib\Devart.Data.MySql.dll</HintPath>
    </Reference>
    <Reference Include="DevExpress.Printing.v25.1.Core, Version=25.1.3.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" />
    <Reference Include="DevExpress.Drawing.v25.1, Version=25.1.3.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" />
    <Reference Include="DevExpress.Data.v25.1, Version=25.1.3.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" />
    <Reference Include="DevExpress.Data.Desktop.v25.1, Version=25.1.3.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" />
    <Reference Include="DevExpress.Utils.v25.1, Version=25.1.3.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" />
    <Reference Include="DevExpress.XtraEditors.v25.1, Version=25.1.3.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" />
    <Reference Include="DevExpress.XtraWizard.v25.1, Version=25.1.3.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a, processorArchitecture=MSIL" />
    <Reference Include="Google.Core, Version=1.0.0.28, Culture=neutral, processorArchitecture=MSIL">
      <SpecificVersion>False</SpecificVersion>
      <HintPath>..\..\Lib\Google.Core.dll</HintPath>
    </Reference>
    <Reference Include="Ionic.Zip, Version=1.9.1.8, Culture=neutral, PublicKeyToken=edbe51ad942a3f5c, processorArchitecture=MSIL">
      <SpecificVersion>False</SpecificVersion>
      <HintPath>..\..\Lib\Ionic.Zip.dll</HintPath>
    </Reference>
    <Reference Include="Log, Version=1.0.0.9, Culture=neutral, processorArchitecture=x86">
      <SpecificVersion>False</SpecificVersion>
      <HintPath>..\..\Lib\Log.dll</HintPath>
    </Reference>
    <Reference Include="System" />
    <Reference Include="System.Core" />
    <Reference Include="System.Xml.Linq" />
    <Reference Include="System.Data.DataSetExtensions" />
    <Reference Include="System.Data" />
    <Reference Include="System.Deployment" />
    <Reference Include="System.Drawing" />
    <Reference Include="System.Windows.Forms" />
    <Reference Include="System.Xml" />
    <Reference Include="TS24.HD.BaseMethod, Version=1.0.0.17, Culture=neutral, processorArchitecture=x86">
      <SpecificVersion>False</SpecificVersion>
      <HintPath>..\..\Lib\TS24.HD.BaseMethod.dll</HintPath>
    </Reference>
    <Reference Include="TS24.MySQLLib, Version=1.0.0.92, Culture=neutral, processorArchitecture=x86">
      <SpecificVersion>False</SpecificVersion>
      <HintPath>..\..\Lib\TS24.MySQLLib.dll</HintPath>
    </Reference>
    <Reference Include="TS24.MySQLUtilities, Version=18.2.15.7, Culture=neutral, processorArchitecture=x86">
      <SpecificVersion>False</SpecificVersion>
      <HintPath>..\..\Lib\TS24.MySQLUtilities.dll</HintPath>
    </Reference>
    <Reference Include="TS24.TO.Commons, Version=2.0.0.128, Culture=neutral, processorArchitecture=x86">
      <SpecificVersion>False</SpecificVersion>
      <HintPath>..\..\Lib\TS24.TO.Commons.dll</HintPath>
    </Reference>
    <Reference Include="TS24.TO.HDDB, Version=1.0.0.37, Culture=neutral, processorArchitecture=x86">
      <SpecificVersion>False</SpecificVersion>
      <HintPath>..\..\Lib\TS24.TO.HDDB.dll</HintPath>
    </Reference>
  </ItemGroup>
  <ItemGroup>
    <Compile Include="AutoBackup.cs" />
    <Compile Include="Backup.cs">
      <SubType>Form</SubType>
    </Compile>
    <Compile Include="Backup.Designer.cs">
      <DependentUpon>Backup.cs</DependentUpon>
    </Compile>
    <Compile Include="BKupGoogleDrive.cs" />
    <Compile Include="GoogleSync.cs" />
    <Compile Include="Program.cs" />
    <Compile Include="Properties\AssemblyInfo.cs" />
    <Compile Include="Restore.cs">
      <SubType>Form</SubType>
    </Compile>
    <Compile Include="Restore.Designer.cs">
      <DependentUpon>Restore.cs</DependentUpon>
    </Compile>
    <Compile Include="SetingGSync.cs">
      <SubType>Form</SubType>
    </Compile>
    <Compile Include="SetingGSync.Designer.cs">
      <DependentUpon>SetingGSync.cs</DependentUpon>
    </Compile>
    <EmbeddedResource Include="Backup.resx">
      <DependentUpon>Backup.cs</DependentUpon>
      <SubType>Designer</SubType>
    </EmbeddedResource>
    <EmbeddedResource Include="Properties\Resources.resx">
      <Generator>ResXFileCodeGenerator</Generator>
      <LastGenOutput>Resources.Designer.cs</LastGenOutput>
      <SubType>Designer</SubType>
    </EmbeddedResource>
    <Compile Include="Properties\Resources.Designer.cs">
      <AutoGen>True</AutoGen>
      <DependentUpon>Resources.resx</DependentUpon>
      <DesignTime>True</DesignTime>
    </Compile>
    <EmbeddedResource Include="Restore.resx">
      <DependentUpon>Restore.cs</DependentUpon>
      <SubType>Designer</SubType>
    </EmbeddedResource>
    <EmbeddedResource Include="SetingGSync.resx">
      <DependentUpon>SetingGSync.cs</DependentUpon>
      <SubType>Designer</SubType>
    </EmbeddedResource>
    <None Include="Properties\Settings.settings">
      <Generator>SettingsSingleFileGenerator</Generator>
      <LastGenOutput>Settings.Designer.cs</LastGenOutput>
    </None>
    <Compile Include="Properties\Settings.Designer.cs">
      <AutoGen>True</AutoGen>
      <DependentUpon>Settings.settings</DependentUpon