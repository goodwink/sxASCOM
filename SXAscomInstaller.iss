                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                ; Generated by Bret McKee on 5/7/2010 (UTC)
;

;#define BUILD_TYPE "Debug"
#define BUILD_TYPE "Release"
;#define EXPIRATION "2013/04/15"
#define EXPIRATION "0"
#define APP_VERSION "5.5.1.13082"
#define ASCOM_VERSION_REQUIRED  "5.5"
#define DRIVER_EXE_NAME "ASCOM.SXCamera.exe"

[Setup]
AppId=sxASCOM
AppName=ASCOM SX Camera Driver
AppVerName=ASCOM SX Camera Driver {#APP_VERSION}
AppPublisher=Bret McKee <bretm@daddog.com>
AppPublisherURL=http://www.daddog.com/ascom/sx/index.html
AppVersion={#APP_VERSION}
AppSupportURL=http://tech.groups.yahoo.com/group/ASCOM-Talk/
AppUpdatesURL=http://ascom-standards.org/
VersionInfoVersion="{#APP_VERSION}"
MinVersion=0,5.0.2195sp4
DefaultDirName="{cf}\ASCOM\Camera"
DisableDirPage=yes
DisableProgramGroupPage=yes
OutputDir="."
OutputBaseFilename="SXAscomInstaller-v{#APP_VERSION}"
Compression=lzma
SolidCompression=yes
; Put there by Platform if Driver Installer Support selected
WizardImageFile="C:\Program Files (x86)\ASCOM\InstallGen\Resources\WizardImage.bmp"
LicenseFile="CreativeCommons.txt"

; {cf}\ASCOM\Uninstall\Camera folder created by Platform, always
UninstallFilesDir="{cf}\ASCOM\Uninstall\Camera\sxASCOM"

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Dirs]
Name: "{cf}\ASCOM\Uninstall\Camera\sxASCOM"
Name: "{app}"

[Files]
Source: "SXCamera\bin\{#BUILD_TYPE}\{#DRIVER_EXE_NAME}"; DestDir: "{app}"
; Require a read-me HTML to appear after installation, maybe driver's Help doc
Source: "SXCamera.Readme.txt"; DestDir: "{app}"; Flags: isreadme
; TODO: Add other files needed by your driver here (add subfolders above)

; Only if driver is .NET
[Run]
; Only for .NET local-server drivers
Filename: "{app}\ASCOM.SXCamera.exe"; Parameters: "{code:RegistrationArgs}"

; Only if driver is .NET
[UninstallRun]
; Only for .NET local-server drivers
Filename: "{app}\ASCOM.SXCamera.exe"; Parameters: "/unregister"


[CODE]
// Global Variables
var
  StandaloneGuiderPage: TInputOptionWizardPage;
  LodeStarsPage: TInputOptionWizardPage;
  CoStarsPage: TInputOptionWizardPage;
  SuperStarsPage: TInputOptionWizardPage;
  ImagingCameraPage: TInputOptionWizardPage;
  GuideCameraPage: TInputOptionWizardPage;
  GuideCamerasPage: TInputOptionWizardPage;

/////////////////////////////////////////////////////////////////////
function GetUninstallString(): String;
var
  sUnInstPath: String;
  sUnInstallString: String;
begin
  sUnInstPath := ExpandConstant('Software\Microsoft\Windows\CurrentVersion\Uninstall\{#emit SetupSetting("AppId")}_is1');
  sUnInstallString := '';
  if not RegQueryStringValue(HKLM, sUnInstPath, 'UninstallString', sUnInstallString) then
    RegQueryStringValue(HKCU, sUnInstPath, 'UninstallString', sUnInstallString);
  Result := sUnInstallString;
end;


/////////////////////////////////////////////////////////////////////
function IsUpgrade(): Boolean;
begin
  Result := (GetUninstallString() <> '');
end;


/////////////////////////////////////////////////////////////////////
function UnInstallOldVersion(): Integer;
var
  sUnInstallString: String;
  iResultCode: Integer;
begin
// Return Values:
// 1 - uninstall string is empty
// 2 - error executing the UnInstallString
// 3 - successfully executed the UnInstallString

  // default return value
  Result := 0;

  // get the uninstall string of the old app
  sUnInstallString := GetUninstallString();
  if sUnInstallString <> '' then begin
    sUnInstallString := RemoveQuotes(sUnInstallString);
    if Exec(sUnInstallString, '/SILENT /NORESTART /SUPPRESSMSGBOXES','', SW_HIDE, ewWaitUntilTerminated, iResultCode) then
      Result := 3
    else
      Result := 2;
  end else
    Result := 1;
end;

// Convert the version number.  I have had problems with 
// internatioalization, so I had to write this function.  
// The problem was that this line:
// if (H2.PlatformVersion >= {#ASCOM_VERSION_REQUIRED})
// was raising an exception.  The Platform version is not
// internationlaized, and when used with the language set to
// one that requires a comma instead of a period as the 
// decimal point seperator (french was the language that 
// caught it...)

function MajorVersion(VersionString : String) : LongWord;
var 
    Accumulated : LongWord;
    Index : Integer;
    c : char;
begin
    Accumulated := 0;

    for Index := 1 to Length(VersionString)
    do
        begin
            c := VersionString[Index];

            if ((c >= '0') and ( c <= '9'))
            then
                begin
                    Accumulated := Accumulated * 10 + Ord(c) - Ord('0');
                end
            else
                break;
        end

    Result := Accumulated;
end;

function MinorVersion(VersionString : String) : LongWord;
var 
    Accumulated : LongWord;
    Index : Integer;
    c : char;
begin
    Accumulated := 0;

    for Index := 1 to Length(VersionString)
    do
        begin
            c := VersionString[Index];

            if ((c >= '0') and ( c <= '9'))
            then
                begin
                    Accumulated := Accumulated * 10 + Ord(c) - Ord('0');
                end
            else
                Accumulated := 0; // reset the accumulated when we hit a non-digit
        end

    Result := Accumulated;
end;

//
// Before the installer UI appears, verify that the (prerequisite)
// ASCOM Platform 5 or greater is installed, including both Helper
// components. Helper is required for all types (COM and .NET)!
//
function InitializeSetup(): Boolean;
var
   H : Variant;
   H2 : Variant;
   P  : Variant;
begin
    Result := FALSE;  // Assume failure

    try               // Will catch all errors including missing reg data
        try
            H := CreateOLEObject('DriverHelper.Util');  // Assure both are available
        except
            RaiseException('Unable to locate DriverHelper.Util - is the ASCOM Platform correctly installed?');
        end;

        try
            H2 := CreateOleObject('DriverHelper2.Util');
        except
            RaiseException('Unable to locate DriverHelper2.Util - is the ASCOM Platform correctly installed?');
        end;

        if ((MajorVersion(H2.PlatformVersion) < MajorVersion('{#ASCOM_VERSION_REQUIRED}')) or
            ((MajorVersion(H2.PlatformVersion) = MajorVersion('{#ASCOM_VERSION_REQUIRED}')) and 
             (MinorVersion(H2.PlatformVersion) < MinorVersion('{#ASCOM_VERSION_REQUIRED}'))))
        then
            begin
                MsgBox('The ASCOM Platform {#ASCOM_VERSION_REQUIRED} or greater is required for this driver.', mbInformation, MB_OK);
            end
        else
            begin

                try
                    P := CreateOLEObject('ASCOM.Utilities.Profile');
                except
                    RaiseException('Unable to create OLE object for ASCOM.Utilities.Profile - is the ASCOM Platform correctly installed?')
                end

                P.DeviceType := 'Camera';

                // see if there is a version we can auto remove
                if IsUpgrade() and 
                    (MsgBox('A previous version of this driver was detected and will be uninstalled before this installation proceeds.',
                        mbInformation, MB_OKCANCEL) = IDOK)
                then
                    UnInstallOldVersion();


                // see if there is still a version installed.  If so, it must require manual removal

                if P.IsRegistered('ASCOM.SXMain0.Camera') or 
                   P.IsRegistered('ASCOM.SXMain1.Camera') or 
                   P.IsRegistered('ASCOM.SXMain2.Camera') or 
                   P.IsRegistered('ASCOM.SXMain3.Camera') or 
                   P.IsRegistered('ASCOM.SXMain4.Camera') or 
                   P.IsRegistered('ASCOM.SXMain5.Camera') or 
                   P.IsRegistered('ASCOM.SXGuide0.Camera') or
                   P.IsRegistered('ASCOM.SXGuide1.Camera')
                then
                    begin
                        MsgBox('A previous version of this driver that cannot be automatically removed was detected. You must uninstall the previous version from the control panel before installation can proceed.', mbInformation, MB_OK);
                    end
                else
                    begin
                        // all the old versions are gone, we can proceed
                        Result := TRUE;
                    end
            end
    except
        ShowExceptionMessage;
    end
end;

function ShouldSkipPage(PageID: Integer): Boolean;
begin
    Result := FALSE;

    if PageID = LodeStarsPage.ID then
        Result := StandAloneGuiderPage.Values[0];

    if PageID = CoStarsPage.ID then
        Result := StandAloneGuiderPage.Values[0];

    if PageID = SuperStarsPage.ID then
        Result := StandAloneGuiderPage.Values[0];

    if PageID = GuideCamerasPage.ID then
        Result := not ImagingCameraPage.Values[2];

    if PageID = GuideCameraPage.ID then
        result := not ImagingCameraPage.Values[1];

end;

function NextButtonClick(CurPageID: Integer): Boolean;
begin
    Result := TRUE;
    if CurPageID = ImagingCameraPage.ID then
    begin
        if LodeStarsPage.Values[0] and CostarsPage.Values[0] and SuperstarsPage.Values[0] and ImagingCameraPage.Values[0] then
        begin
            MsgBox('No cameras specified for Installation, unable to proceed', mbError, MB_OK);
            Result := FALSE;
        end
    end;
end;

procedure InitializeWizard();
begin

    StandAloneGuiderPage := CreateInputOptionPage(wpLicense,
      'Standalone Guide Camera Configuration', 
      'Standalone Guide Cameras are small, eyepiece sized guide cameras that connect to your PC via USB.',
      'Do you have an Starlight Xpress Standalone Guide Cameras?',
      True, False);

    StandAloneGuiderPage.Add('No');
    StandAloneGuiderPage.Add('Yes');

    StandAloneGuiderPage.Values[0] := True;
    StandAloneGuiderPage.Values[1] := False;

    LodeStarsPage := CreateInputOptionPage(StandAloneGuiderPage.ID,
      'Lodestar Configuration', 
      'Lodestar is a small, eyepiece sized monochrome guide camera that connects to your PC via USB.',
      'Do you have a Starlight Xpress LodeStar USB Guide Camera?',
      True, False);
    LodeStarsPage.Add('No');
    LodeStarsPage.Add('Yes, I have 1 Lodestar Camera');
    LodeStarsPage.Add('Yes, I have 2 Lodestar Cameras');

    LodeStarsPage.Values[0] := True;
    LodeStarsPage.Values[1] := False;
    LodeStarsPage.Values[2] := False;

    CoStarsPage := CreateInputOptionPage(LodeStarsPage.ID,
      'CoStar Configuration', 
      'CoStar is a small, eyepiece sized color guide camera that connects to your PC via USB.',
      'Do you have a Starlight Xpress CoStar USB Guide Camera?',
      True, False);
    CoStarsPage.Add('No');
    CoStarsPage.Add('Yes, I have 1 CoStar Camera');
    CoStarsPage.Add('Yes, I have 2 CoStar Cameras');

    CoStarsPage.Values[0] := True;
    CoStarsPage.Values[1] := False;
    CoStarsPage.Values[2] := False;

    SuperStarsPage := CreateInputOptionPage(CoStarsPage.ID,
      'SuperStar Configuration', 
      'SuperStar is a small, eyepiece sized color guide camera that connects to your PC via USB.',
      'Do you have a Starlight Xpress SuperStar USB Guide Camera?',
      True, False);
    SuperStarsPage.Add('No');
    SuperStarsPage.Add('Yes, I have 1 SuperStar Camera');
    SuperStarsPage.Add('Yes, I have 2 SuperStar Cameras');

    SuperStarsPage.Values[0] := True;
    SuperStarsPage.Values[1] := False;
    SuperStarsPage.Values[2] := False;

    ImagingCameraPage := CreateInputOptionPage(SuperStarsPage.ID,
      'Imaging Camera Configuration', 
      'This is a camera which connects to to your PC via USB.',
      'Do you have a Starlight Xpress Imaging Camera?',
      True, False);
    ImagingCameraPage.Add('No');
    ImagingCameraPage.Add('Yes, I have 1 Imaging Camera');
    ImagingCameraPage.Add('Yes, I have 2 Imaging Cameras');

    ImagingCameraPage.Values[0] := True;
    ImagingCameraPage.Values[1] := False;
    ImagingCameraPage.Values[2] := False;

    GuideCamerasPage := CreateInputOptionPage(ImagingCameraPage.ID,
      'Autoguide Camera Configuration', 
      'This is a small, eyepiece sized guide camera that plugs into the back of an imaging camera (not into your computer).',
      'Do you have any Starlight Xpress SXV or ExView Autoguide Cameras?',
      True, False);
    GuideCamerasPage.Add('No');
    GuideCamerasPage.Add('Yes, I have 1 Autoguide Camera');
    GuideCamerasPage.Add('Yes, I have 2 Autoguide Cameras');

    GuideCamerasPage.Values[0] := True;
    GuideCamerasPage.Values[1] := False;
    GuideCamerasPage.Values[2] := False;

    GuideCameraPage := CreateInputOptionPage(GuideCamerasPage.ID,
      'Autoguide Camera Configuration', 
      'This is a small, eyepiece sized guide camera that plugs into the back of an imaging camera (not into your computer).',
      'Do you have a Starlight Xpress SXV or ExView Autoguide Camera?',
      True, False);
    GuideCameraPage.Add('No');
    GuideCameraPage.Add('Yes');

    GuideCameraPage.Values[0] := True;
    GuideCameraPage.Values[1] := False;

end;

function RegistrationArgs(Param : String) : String;
begin
    Result := '/register';

    if LodeStarsPage.Values[1] then
        Result := Result + ' /lodestar'
    else 
        if LodeStarsPage.Values[2] then
            Result := Result + ' /lodestar2';

    if CoStarsPage.Values[1] then
        Result := Result + ' /costar'
    else 
        if CoStarsPage.Values[2] then
            Result := Result + ' /costar2';

    if SuperStarsPage.Values[1] then
        Result := Result + ' /superstar'
    else 
        if SuperStarsPage.Values[2] then
            Result := Result + ' /superstar2';

    if ImagingCameraPage.Values[1] then
        Result := Result + ' /main'
    else
        if ImagingCameraPage.Values[2] then
            Result := Result + ' /main2'

    if GuideCameraPage.Values[1] or GuideCamerasPage.Values[1] then
        Result := Result + ' /autoguide'
    else
        if GuideCamerasPage.Values[2] then
            Result := Result + ' /autoguide2'

    Result := Result + ' /expiration={#EXPIRATION}'
end;
